using System;
using System.Collections.Generic;
using AwwScrap_IFoundYourCrap.Thraxus.Common.BaseClasses;
using AwwScrap_IFoundYourCrap.Thraxus.Common.Enums;
using AwwScrap_IFoundYourCrap.Thraxus.Common.Utilities.FileHandlers;
using AwwScrap_IFoundYourCrap.Thraxus.Common.Utilities.Statics;
using AwwScrap_IFoundYourCrap.Thraxus.Models;
using AwwScrap_IFoundYourCrap.Thraxus.Support;
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
		protected override string CompName { get; } = "FoundYourCrapCore";
		protected override CompType Type { get; } = CompType.Server;
		protected override MyUpdateOrder Schedule { get; } = MyUpdateOrder.BeforeSimulation;

		private GenericObjectPool<GrindOperation> _grindOperations;
		private readonly ConcurrentCachingList<GrindOperation> _pooledGrindOperations = new ConcurrentCachingList<GrindOperation>();
		private readonly HashSet<long> _trackedGrinders = new HashSet<long>();

		private UserSettings _userSettings = new UserSettings();

		protected override void SuperEarlySetup()
		{
			if (!MyAPIGateway.Utilities.FileExistsInWorldStorage(Constants.SettingsFileName, typeof(UserSettings)))
				Save.WriteXmlFileToWorldStorage(Constants.SettingsFileName, _userSettings, typeof(UserSettings));
			_userSettings = Load.ReadXmlFileInWorldStorage<UserSettings>(Constants.SettingsFileName, typeof(UserSettings));
			_grindOperations = new GenericObjectPool<GrindOperation>(() => new GrindOperation(_userSettings));

			base.SuperEarlySetup();
		}

		public override void BeforeStart()
		{
			WriteToLog("EarlySetup", $"{_userSettings}", LogType.General);
			base.BeforeStart();
		}

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
			ClearGrindOperationsPool();
			_trackedGrinders.Clear();
			
			base.Unload();
		}

		protected override void BeforeSimUpdate()
		{
			ClearGrindOperationsPool();
			base.BeforeSimUpdate();
		}

		private void ClearGrindOperationsPool()
		{
			for (int i = _pooledGrindOperations.Count - 1; i >= 0; i--)
			{
				if (_pooledGrindOperations[i].Tick == TickCounter) continue;
				GrindOperation op = _pooledGrindOperations[i];
				_pooledGrindOperations.RemoveAtImmediately(i);
				op.OnWriteToLog -= WriteToLog;
				op.Reset();
				_grindOperations.Return(op);
			}
			_pooledGrindOperations.ApplyRemovals();
		}
		
		private void AfterDamageHandler(object target, MyDamageInformation info)
		{
			if (!_trackedGrinders.Contains(info.AttackerId)) return;
			CastSpellsToFindNewScrap(info.AttackerId);
		}

		private void CastSpellsToFindNewScrap(long grinderId)
		{
			GrindOperation op = null;
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
		}
		
		private void BeforeDamageHandler(object target, ref MyDamageInformation info)
		{
			try
			{
				if (!_trackedGrinders.Contains(info.AttackerId)) return;
				var grinder = (IMyAngleGrinder)MyAPIGateway.Entities.GetEntityById(info.AttackerId);
				IMyPlayer player = MyAPIGateway.Players.GetPlayerById(grinder.OwnerIdentityId);
				
				var myInventory = (MyInventory)player?.Character.GetInventory();
				if (myInventory == null)
				{
					return;
				}
				
				var block = target as IMySlimBlock;
				if (block == null)
				{
					return;
				}

				GrindOperation op =  _grindOperations.Get();
				op.Grinder = grinder;
				op.Tick = TickCounter;
				op.GrinderId = grinder.EntityId;
				op.PlayerInventory = myInventory;
				op.DamagedBlock = block;
				op.OnWriteToLog += WriteToLog;
				op.ProcessBeforeSim();
				_pooledGrindOperations.Add(op);
				_pooledGrindOperations.ApplyAdditions();
			}
			catch (Exception e)
			{
				WriteToLog("BeforeDamageHandler", $"I took a shit! \n\n{e}", LogType.Exception);
			}
		}
	}
}