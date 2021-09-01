using System;
using Sandbox.Definitions;
using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using VRage.ModAPI;
using VRageMath;

namespace AwwScrap_IFoundYourCrap.Thraxus.Common.Utilities.Statics
{
	public static class ThrusterCalculations
	{
		public const float GravityConstant = 9.81f;

		public static float CalculateRequiredLift(MyCubeBlock block)
		{
			BoundingBoxD box = block.PositionComp.WorldAABB;
			MyPlanet closestPlanet = MyGamePruningStructure.GetClosestPlanet(ref box);
			if (closestPlanet == null) return 0;
			MyGravityProviderComponent myGravityProviderComponent = closestPlanet.Components.Get<MyGravityProviderComponent>();
			MatrixD worldMatrixRef = block.PositionComp.WorldMatrixRef;
			return block.CubeGrid.Mass * myGravityProviderComponent.GetGravityMultiplier(worldMatrixRef.Translation) * GravityConstant;
		}

		public static float PlanetaryInfluence(IMyEntity block)
		{
			BoundingBoxD box = block.PositionComp.WorldAABB;
			MyPlanet closestPlanet = MyGamePruningStructure.GetClosestPlanet(ref box);
			if (closestPlanet == null) return 0;
			MyGravityProviderComponent myGravityProviderComponent = closestPlanet.Components.Get<MyGravityProviderComponent>();
			return closestPlanet.GetAirDensity(box.Center) * myGravityProviderComponent.GetGravityMultiplier(box.Center);
			//MySimpleObjectDraw
			//billboa
		}

		public static float CalculatedMaxThrust(MyThrust thruster, bool inAtmosphere)
		{
			return thruster.BlockDefinition.ForceMagnitude * ((IMyThrust)thruster).ThrustMultiplier * CalculatedThrustScalar(thruster, inAtmosphere);
		}

		public static float AdjustedMaxPower(MyThrust thruster, bool inAtmosphere)
		{   // Atmospheric thrusters get scaled based on where in the atmospheric layer they are
			return thruster.MaxPowerConsumption * CalculatedThrustScalar(thruster, inAtmosphere);
		}

		public static float CalculatedCurrentPower(MyThrust thruster)
		{
			return (((IMyThrust)thruster).CurrentThrust / ((IMyThrust)thruster).MaxThrust) * thruster.MaxPowerConsumption;
		}

		public static float CalculatedThrustScalar(MyThrust thruster, bool inAtmosphere)
		{
			float result = 1f;
			MyThrustDefinition definition = thruster.BlockDefinition;
			if (definition.NeedsAtmosphereForInfluence && !inAtmosphere)
			{
				result = definition.EffectivenessAtMinInfluence;
			}
			else if (Math.Abs(definition.MaxPlanetaryInfluence - definition.MinPlanetaryInfluence) > 0)
			{
				float value = (PlanetaryInfluence(thruster) - definition.MinPlanetaryInfluence) * definition.InvDiffMinMaxPlanetaryInfluence;
				result = MathHelper.Lerp(definition.EffectivenessAtMinInfluence, definition.EffectivenessAtMaxInfluence, MathHelper.Clamp(value, 0f, 1f));
			}
			return result;
		}

		public static float CalculatedThrustScalar(MyThrust thruster, bool inAtmosphere, float planetaryInfluence)
		{
			float result = 1f;
			MyThrustDefinition definition = thruster.BlockDefinition;
			if (definition.NeedsAtmosphereForInfluence && !inAtmosphere)
			{
				result = definition.EffectivenessAtMinInfluence;
			}
			else if (Math.Abs(definition.MaxPlanetaryInfluence - definition.MinPlanetaryInfluence) > 0)
			{
				float value = (planetaryInfluence - definition.MinPlanetaryInfluence) * definition.InvDiffMinMaxPlanetaryInfluence;
				result = MathHelper.Lerp(definition.EffectivenessAtMinInfluence, definition.EffectivenessAtMaxInfluence, MathHelper.Clamp(value, 0f, 1f));
			}
			return result;
		}
	}
}
