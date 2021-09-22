using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using AwwScrap_IFoundYourCrap.Thraxus.Common.BaseClasses;
using AwwScrap_IFoundYourCrap.Thraxus.Common.Utilities.Statics;
using AwwScrap_IFoundYourCrap.Thraxus.Support;
using Sandbox.Game;
using Sandbox.Game.Components;
using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Weapons;
using VRage;
using VRage.Game;
using VRage.Game.Entity;
using VRage.Game.ModAPI;
using VRage.Game.ObjectBuilders;
using VRage.ObjectBuilders;
using VRage.Utils;
using VRageMath;

namespace AwwScrap_IFoundYourCrap.Thraxus.Models
{
	public class GrindOperation : BaseLoggingClass
	{
		public GrindOperation(UserSettings userSettings)
		{
			_userSettings = userSettings ?? new UserSettings();
		}

		protected override string Id { get; } = nameof(GrindOperation);

		private readonly GenericObjectPool<RefundOpportunity> _refundPool = new GenericObjectPool<RefundOpportunity>(() => new RefundOpportunity());

		public Dictionary<string, int> BeforeGrindMissingBlockComponents = new Dictionary<string, int>();
		public Dictionary<string, int> AfterGrindMissingBlockComponents = new Dictionary<string, int>();
		
		public ConcurrentDictionary<string, int> BlockComponentDeltas = new ConcurrentDictionary<string, int>();
		public ConcurrentDictionary<MyStringHash, MyFixedPoint> BeforeGrindPlayerInventoryItems = new ConcurrentDictionary<MyStringHash, MyFixedPoint>();
		public ConcurrentDictionary<MyStringHash, MyFixedPoint> AfterGrindPlayerInventoryItems = new ConcurrentDictionary<MyStringHash, MyFixedPoint>();
		public ConcurrentDictionary<MyStringHash, MyFixedPoint> PlayerInventoryDeltas = new ConcurrentDictionary<MyStringHash, MyFixedPoint>();
		public ConcurrentDictionary<MyStringHash, MyFixedPoint> BodyBagContents = new ConcurrentDictionary<MyStringHash, MyFixedPoint>();

		private readonly List<RefundOpportunity> _refundOpportunities = new List<RefundOpportunity>();
		private readonly List<RefundOpportunity> _excessRefundOpportunities = new List<RefundOpportunity>();
		private readonly List<RefundOpportunity> _refunds = new List<RefundOpportunity>();
		private readonly List<RefundOpportunity> _excessRefunds = new List<RefundOpportunity>();

		public MyInventory PlayerInventory;
		public IMyAngleGrinder Grinder;
		public IMySlimBlock DamagedBlock;

		private readonly UserSettings _userSettings;

		public long GrinderId;
		public long Tick;
		private int _refundChance;

		public void Reset()
		{
			BeforeGrindMissingBlockComponents.Clear();
			AfterGrindMissingBlockComponents.Clear();

			BlockComponentDeltas.Clear();
			BeforeGrindPlayerInventoryItems.Clear();
			AfterGrindPlayerInventoryItems.Clear();
			PlayerInventoryDeltas.Clear();
			BodyBagContents.Clear();

			_refundOpportunities.Clear();
			_excessRefundOpportunities.Clear();
			_refunds.Clear();
			_excessRefunds.Clear();

			PlayerInventory = null;
			DamagedBlock = null;
			Grinder = null;

			GrinderId = 0;
			Tick = 0;
			_refundChance = 0;
		}

		private static bool ValidateScrap(string compName)
		{
			return compName.EndsWith(Constants.ScrapSuffix, StringComparison.OrdinalIgnoreCase) && !compName.Equals(Constants.ScrapSuffix, StringComparison.OrdinalIgnoreCase);
		}

		private bool ValidateReturnEligibility()
		{
			// Get Block Grid
			// Get BigOwner List
			// Validate if Grinder Owner is on Big Owner list
			// If yes: 
			//	Calculate return rate (full)
			//	Return true;
			if (DamagedBlock.CubeGrid.BigOwners.Contains(Grinder.OwnerIdentityId))
			{
				_refundChance = 
					Grinder.DefinitionId.SubtypeId == Constants.EliteAngleGrinder ? _userSettings.EliteGrinderReturnRate :
					Grinder.DefinitionId.SubtypeId == Constants.ProficientAngleGrinder ? _userSettings.ProficientGrinderReturnRate :
					Grinder.DefinitionId.SubtypeId == Constants.EnhancedAngleGrinder ? _userSettings.EnhancedGrinderReturnRate :
					_userSettings.BasicGrinderReturnRate;
				return true;
			}

			// If no:
			//	Get Big Owner faction
			//	Get Grinder faction
			IMyFaction lf = MyAPIGateway.Session.Factions.TryGetPlayerFaction(Grinder.OwnerIdentityId);
			IMyFaction rf = MyAPIGateway.Session.Factions.TryGetPlayerFaction(DamagedBlock.CubeGrid.BigOwners.FirstOrDefault());

			// If Factions do not match (null or different)
			//	Option A) 
			//		Set Tick = 0 (discards refund opportunity)
			//	Option B) 
			//		Calculate Return Rate (full)
			//		Multiply Return Rate by some scalar (maybe 10%)
			if (lf == null || rf == null || rf.FactionId != lf.FactionId)
			{
				if (_userSettings.ReturnComponentsFromUnownedGrids)
				{
					_refundChance =
						Grinder.DefinitionId.SubtypeId == Constants.EliteAngleGrinder ? _userSettings.EliteGrinderReturnRate :
						Grinder.DefinitionId.SubtypeId == Constants.ProficientAngleGrinder ? _userSettings.ProficientGrinderReturnRate :
						Grinder.DefinitionId.SubtypeId == Constants.EnhancedAngleGrinder ? _userSettings.EnhancedGrinderReturnRate :
						_userSettings.BasicGrinderReturnRate;
					_refundChance = (int)(_refundChance * (float)_userSettings.ReturnRateForUnownedGrids/100);
					return true;
				}
				Tick = 0;
				return false;
			}

			// If Factions match: 
			//	Calculate return rate (full)
			//	Return true;
			_refundChance =
				Grinder.DefinitionId.SubtypeId == Constants.EliteAngleGrinder ? _userSettings.EliteGrinderReturnRate :
				Grinder.DefinitionId.SubtypeId == Constants.ProficientAngleGrinder ? _userSettings.ProficientGrinderReturnRate :
				Grinder.DefinitionId.SubtypeId == Constants.EnhancedAngleGrinder ? _userSettings.EnhancedGrinderReturnRate :
				_userSettings.BasicGrinderReturnRate;
			return true;
		}

		private void GetBeforeItems()
		{
			foreach (MyPhysicalInventoryItem item in PlayerInventory.GetItems())
			{
				
				if (!ValidateScrap(item.Content.SubtypeId.ToString())) continue;
				if (!BeforeGrindPlayerInventoryItems.TryAdd(item.Content.SubtypeId, item.Amount))
					BeforeGrindPlayerInventoryItems[item.Content.SubtypeId] += item.Amount;
			}
			DamagedBlock.GetMissingComponents(BeforeGrindMissingBlockComponents);
		}

		private void GetAfterItems()
		{
			foreach (MyPhysicalInventoryItem item in PlayerInventory.GetItems())
			{
				if (!ValidateScrap(item.Content.SubtypeId.ToString())) continue;
				if (!BeforeGrindPlayerInventoryItems.ContainsKey(item.Content.SubtypeId))
				{
					if (!PlayerInventoryDeltas.TryAdd(item.Content.SubtypeId, item.Amount))
						PlayerInventoryDeltas[item.Content.SubtypeId] += item.Amount;
					continue;
				}

				MyFixedPoint delta = item.Amount - BeforeGrindPlayerInventoryItems[item.Content.SubtypeId];
				
				if (delta <= 0) continue;
				if (!PlayerInventoryDeltas.TryAdd(item.Content.SubtypeId, delta))
					PlayerInventoryDeltas[item.Content.SubtypeId] = delta;
			}

			DamagedBlock.GetMissingComponents(AfterGrindMissingBlockComponents);

			foreach (KeyValuePair<string, int> comps in AfterGrindMissingBlockComponents)
			{
				if (!BeforeGrindMissingBlockComponents.ContainsKey(comps.Key))
				{
					if (BlockComponentDeltas.TryAdd(comps.Key, comps.Value))
						BlockComponentDeltas[comps.Key] = comps.Value;
					continue;
				}

				int delta = comps.Value - BeforeGrindMissingBlockComponents[comps.Key];
				if (delta <= 0) continue;
				if (!BlockComponentDeltas.TryAdd(comps.Key, delta))
					BlockComponentDeltas[comps.Key] = delta;
			}
		}

		public void ProcessBeforeSim()
		{
			if (!ValidateReturnEligibility()) return;
			GetBeforeItems();
		}

		public void ProcessAfterSim()
		{
			if (Tick == 0) return;
			GetAfterItems();
			DetermineEligibleScrap();
			RollSomeDice(_refundOpportunities, _refunds);
			RefundComponents();
		}
		
		private void DetermineEligibleScrap()
		{
			foreach (KeyValuePair<string, int> component in BlockComponentDeltas)
			{
				MyStringHash lookFor = MyStringHash.GetOrCompute(component.Key + Constants.ScrapSuffix);
				MyFixedPoint count;
				
				if (!PlayerInventoryDeltas.TryGetValue(lookFor, out count) || PlayerInventoryDeltas.Count == 0)
					continue;
				
				RefundOpportunity ro = _refundPool.Get();
				ro.Count = (int)(count >= component.Value ? component.Value : count);
				ro.CompSubtype = component.Key;
				ro.ScrapSubtype = lookFor;
				_refundOpportunities.Add(ro);
			}
		}

		private void RollSomeDice(List<RefundOpportunity> opportunities, List<RefundOpportunity> refunds)
		{
			if (opportunities.Count == 0) return;
			foreach (RefundOpportunity ro in opportunities)
			{
				var refund = 0;
				for (var i = 0; i < ro.Count; i++)
				{
					int chance = Constants.Chance;
					if (chance > _refundChance) continue;
					refund++;
				}
				if (refund <= 0) continue;
				RefundOpportunity op = _refundPool.Get();
				op.Count = refund;
				op.CompSubtype = ro.CompSubtype;
				op.ScrapSubtype = ro.ScrapSubtype;
				refunds.Add(op);
			}
			ReturnToPool(opportunities);
		}
		
		public void RefundComponents()
		{
			foreach (RefundOpportunity refund in _refunds)
			{
				var id = new MyDefinitionId(typeof(MyObjectBuilder_Ore), refund.ScrapSubtype);
				int val = PlayerInventory.ComputeAmountThatFits(id).ToIntSafe();
				val = Math.Min(refund.Count, val);
				
				if (val < refund.Count) break;
				IMyInventoryItem invItem = new MyPhysicalInventoryItem()
				{
					Amount = val,
					Content = MyObjectBuilderSerializer.CreateNewObject<MyObjectBuilder_Component>(refund.CompSubtype)
				};

				RemoveFromInventory(PlayerInventory, refund.Count, refund.ScrapSubtype);
				AddToInventory(PlayerInventory, refund.Count, invItem);
			}

			ReturnToPool(_refunds);
			if (DamagedBlock.StockpileEmpty) return;
			RefundRemainingStockpile();
		}

		private void RefundRemainingStockpile()
		{
			MyInventory bodyBag = null;

			foreach (MyEntity ent in Statics.DetectAllEntitiesInSphere(PlayerInventory.Owner.PositionComp.GetPosition(), 5))
			{
				var bag = ent as MyInventoryBagEntity;
				if (bag == null) continue;
				bodyBag = bag.GetInventory();
			}
			
			if (bodyBag == null)
				bodyBag = SpawnBodyBag();

			if (bodyBag == null) return;

			// Snapshot InventoryBag Contents
			foreach (MyPhysicalInventoryItem item in bodyBag.GetItems())
			{
				if (!ValidateScrap(item.Content.SubtypeId.ToString())) continue;
				if (!BodyBagContents.TryAdd(item.Content.SubtypeId, item.Amount))
					BodyBagContents[item.Content.SubtypeId] += item.Amount;
			}

			// Move Stockpile to Body Bag
			DamagedBlock.MoveItemsFromConstructionStockpile(bodyBag);

			// Diff Body Bag Contents
			foreach (MyPhysicalInventoryItem item in bodyBag.GetItems())
			{
				if (!ValidateScrap(item.Content.SubtypeId.ToString())) continue;
				MyFixedPoint amount;
				if (!BodyBagContents.TryGetValue(item.Content.SubtypeId, out amount))
					amount = item.Amount;
				else amount = item.Amount - amount;

				RefundOpportunity ro = _refundPool.Get();
				ro.Count = (int)amount;
				ro.CompSubtype = item.Content.SubtypeName.Substring(0, item.Content.SubtypeName.Length - Constants.ScrapSuffix.Length);
				ro.ScrapSubtype = item.Content.SubtypeId;

				_excessRefundOpportunities.Add(ro);
			}

			// Roll some dice on refunds for the contents
			RollSomeDice(_excessRefundOpportunities, _excessRefunds);

			// Refund the rolls
			
			foreach (RefundOpportunity refund in _excessRefunds)
			{
				IMyInventoryItem invItem = new MyPhysicalInventoryItem()
				{
					Amount = refund.Count,
					Content = MyObjectBuilderSerializer.CreateNewObject<MyObjectBuilder_Component>(refund.CompSubtype)
				};

				RemoveFromInventory(bodyBag, refund.Count, refund.ScrapSubtype);
				AddToInventory(bodyBag, refund.Count, invItem);
			}

			ReturnToPool(_excessRefunds);
		}

		private static bool RemoveFromInventory(MyInventory inventory, int count,  MyStringHash scrap)
		{
			return inventory.RemoveItemsOfType(count, MyObjectBuilderSerializer.CreateNewObject<MyObjectBuilder_Ore>(scrap.String));
		}

		private static bool AddToInventory(MyInventory inventory, int count, IMyInventoryItem item)
		{
			return inventory.Add(item, count);
		}

		private MyInventory SpawnBodyBag()
		{   //MyEntityExtensions

			// Spawn below Char
			MatrixD worldMatrix = PlayerInventory.Owner.WorldMatrix;
			worldMatrix.Translation += worldMatrix.Down + worldMatrix.Forward;

			var bagDefinition = new MyDefinitionId(typeof(MyObjectBuilder_InventoryBagEntity), "AstronautBackpack");
			//var bagDefinition = new MyDefinitionId(typeof(MyObjectBuilder_InventoryBagEntity), "InventoryBag");
			MyContainerDefinition definition;
			
			if (!MyComponentContainerExtension.TryGetContainerDefinition(bagDefinition.TypeId, bagDefinition.SubtypeId, out definition))
				return null;

			MyEntity myEntity = MyEntities.CreateFromComponentContainerDefinitionAndAdd(definition.Id, fadeIn: false);

			var myInventoryBagEntity = myEntity as MyInventoryBagEntity;
			if (myInventoryBagEntity == null) return null;
			
			myInventoryBagEntity.OwnerIdentityId = 0;
			MyTimerComponent component;
			if (myInventoryBagEntity.Components.TryGet(out component))
			{
				component.ChangeTimerTick((uint)(_userSettings.ScrapBodyBagDecayInMinutes * Common.Utilities.CommonSettings.TicksPerMinute));
			}

			myInventoryBagEntity.DisplayNameText = "AwwScrap: I Found Your Crap!";
				
			myEntity.PositionComp.SetWorldMatrix(ref worldMatrix);
			myEntity.Physics.LinearVelocity = Vector3.Zero;
			myEntity.Physics.AngularVelocity = Vector3.Zero;
			myEntity.Render.EnableColorMaskHsv = true;
			//myEntity.Render.ColorMaskHsv = PlayerInventory.Owner.Render.ColorMaskHsv;
			myEntity.Render.ColorMaskHsv = new Vector3(1,0,1);

			var backpackInventory = new MyInventory((MyFixedPoint)100,100000,new Vector3(5,5,5),MyInventoryFlags.CanSend)
			{
				RemoveEntityOnEmpty = true
			};
			
			myEntity.Components.Add((MyInventoryBase)backpackInventory);

			return myInventoryBagEntity.GetInventory();
		}

		private void ReturnToPool(List<RefundOpportunity> list)
		{
			if (list.Count == 0) return;
			
			for (int i = list.Count - 1; i >= 0; i--)
			{
				RefundOpportunity ro = list[i];
				list.RemoveAtFast(i);
				ro.Reset();
				_refundPool.Return(ro);
			}
			list.Clear();
		}
	}
}