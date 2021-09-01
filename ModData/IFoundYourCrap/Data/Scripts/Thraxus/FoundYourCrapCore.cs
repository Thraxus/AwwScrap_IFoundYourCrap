using System;
using System.Collections.Generic;
using AwwScrap_IFoundYourCrap.Thraxus.Common.BaseClasses;
using AwwScrap_IFoundYourCrap.Thraxus.Common.Enums;
using AwwScrap_IFoundYourCrap.Thraxus.Common.Utilities.Statics;
using AwwScrap_IFoundYourCrap.Thraxus.Models;
using Sandbox.Game;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Weapons;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.ModAPI;
using VRage.ModAPI;

namespace AwwScrap_IFoundYourCrap.Thraxus
{
	[MySessionComponentDescriptor(MyUpdateOrder.BeforeSimulation, priority: int.MinValue + 2)]
	public class FoundYourCrapCore: BaseSessionComp
	{
		protected override string CompName { get; } = "ReactiveGrindingCore";
		protected override CompType Type { get; } = CompType.Server;
		protected override MyUpdateOrder Schedule { get; } = MyUpdateOrder.BeforeSimulation;
		
		private readonly GenericObjectPool<GrindOperationInformation> _grindOperations = new GenericObjectPool<GrindOperationInformation>(() => new GrindOperationInformation());
		private readonly ConcurrentCachingList<GrindOperationInformation> _pooledGrindOperations = new ConcurrentCachingList<GrindOperationInformation>();
		private readonly HashSet<long> _trackedGrinders = new HashSet<long>();

		protected override void EarlySetup()
		{
			MyAPIGateway.Session.DamageSystem.RegisterBeforeDamageHandler(Priority, BeforeDamageHandler);
			MyAPIGateway.Session.DamageSystem.RegisterAfterDamageHandler(Priority, AfterDamageHandler);
			MyAPIGateway.Entities.OnEntityAdd += EntityAdded;
			
			base.EarlySetup();
		}

		protected override void Unload()
		{
			MyAPIGateway.Entities.OnEntityAdd -= EntityAdded;
			base.Unload();
		}

		protected override void BeforeSimUpdate()
		{
			for (int i = _pooledGrindOperations.Count - 1; i >= 0; i--)
			{
				if (_pooledGrindOperations[i].Tick == TickCounter) continue;
				GrindOperationInformation op = _pooledGrindOperations[i];
				_pooledGrindOperations.RemoveAtImmediately(i);
				op.OnWriteToLog -= WriteToLog;
				op.Reset();
				_grindOperations.Return(op);
			}
			_pooledGrindOperations.ApplyRemovals();
			base.BeforeSimUpdate();
		}

		private void AfterDamageHandler(object target, MyDamageInformation info)
		{
			if (!_trackedGrinders.Contains(info.AttackerId)) return;
			CastSpellsToFindNewScrap(info.AttackerId);
		}

		private void CastSpellsToFindNewScrap(long grinderId)
		{
			GrindOperationInformation op = null;
			foreach (var pool in _pooledGrindOperations)
			{
				if (pool.GrinderId != grinderId && pool.Tick != TickCounter) continue;
				op = pool;
				break;
			}
			op?.ProcessAfterSim();
		}

		private void EntityAdded(IMyEntity entity)
		{
			var grinder = entity as IMyAngleGrinder;
			if (grinder == null) return;
			if (_trackedGrinders.Contains(entity.EntityId)) return;
			_trackedGrinders.Add(entity.EntityId);
			
			WriteToLog("EntityAdded", $"Found a grinder! {grinder.DefinitionId.SubtypeId}", LogType.General);
		}
		
		private void BeforeDamageHandler(object target, ref MyDamageInformation info)
		{
			try
			{
				if (!_trackedGrinders.Contains(info.AttackerId)) return;
				var grinder = (IMyAngleGrinder)MyAPIGateway.Entities.GetEntityById(info.AttackerId);
				IMyPlayer player = MyAPIGateway.Players.GetPlayerById(grinder.OwnerIdentityId);// MyAPIGateway.Players.GetPlayerControllingEntity(grinder);
				var myInventory = (MyInventory)player?.Character.GetInventory();
				if (myInventory == null)
				{
					WriteToLog("BeforeDamageHandler", $"Inventory was null! -- {player?.DisplayName} -- {grinder.OwnerId} -- {grinder.OwnerIdentityId}", LogType.General);
					return;
				}
				var block = target as IMySlimBlock;
				if (block == null)
				{
					WriteToLog("BeforeDamageHandler", $"Block was null!", LogType.General);
					return;
				}

				GrindOperationInformation op =  _grindOperations.Get();
				if (op == null)
				{
					WriteToLog("BeforeDamageHandler", $"Op was null!", LogType.General);
					return;
				}

				op.Grinder = grinder;
				op.Tick = TickCounter;
				op.GrinderId = grinder.EntityId;
				op.PlayerInventory = myInventory;
				op.DamagedBlock = block;
				op.OnWriteToLog += WriteToLog;
				op.ProcessBeforeSim();
				_pooledGrindOperations.Add(op);
				_pooledGrindOperations.ApplyAdditions();
				//WriteToLog("BeforeDamageHandler", $"Op added to pool!", LogType.General);
			}
			catch (Exception e)
			{
				WriteToLog("BeforeDamageHandler", $"I took a shit! \n\n{e}", LogType.Exception);
			}
		}
	}
}