using System.Collections.Generic;
using System.Text;
using Sandbox.ModAPI;
using VRage.Game;
using VRage.Game.ModAPI;

namespace AwwScrap_IFoundYourCrap.Thraxus.Common.Reporting
{
	public static class BasicGameInformation
	{
		public static StringBuilder Report()
		{
			StringBuilder sb = new StringBuilder();
			const string x = "    ";
			sb.AppendLine();
			sb.AppendLine();
			sb.AppendLine("Information Export Begin");
			sb.AppendLine("═══════════════════════════════════════════");
			sb.AppendLine();
			sb.AppendLine("Game Settings");
			sb.AppendLine("═══════════════════════════════════════════");
			sb.AppendLine($"{x}Adaptive Sim Enabled: {MyAPIGateway.Session.SessionSettings.AdaptiveSimulationQuality}");
			sb.AppendLine($"{x}Cargo Ships Enabled: {MyAPIGateway.Session.SessionSettings.CargoShipsEnabled}");
			sb.AppendLine($"{x}Stop Grids Period (minutes): {MyAPIGateway.Session.SessionSettings.StopGridsPeriodMin}");
			sb.AppendLine($"{x}Encounters Enabled: {MyAPIGateway.Session.SessionSettings.EnableEncounters}");
			sb.AppendLine($"{x}Economy Enabled: {MyAPIGateway.Session.SessionSettings.EnableEconomy}");
			sb.AppendLine($"{x}Economy Ticks (seconds): {MyAPIGateway.Session.SessionSettings.EconomyTickInSeconds}");
			sb.AppendLine($"{x}Bounty Contracts Enabled: {MyAPIGateway.Session.SessionSettings.EnableBountyContracts}");
			sb.AppendLine($"{x}Drones Enabled: {MyAPIGateway.Session.SessionSettings.EnableDrones}");
			sb.AppendLine($"{x}Scripts Enabled: {MyAPIGateway.Session.SessionSettings.EnableIngameScripts}");
			sb.AppendLine($"{x}Asteroid Density: {MyAPIGateway.Session.SessionSettings.ProceduralDensity}");
			sb.AppendLine($"{x}Weather Enabled: {MyAPIGateway.Session.SessionSettings.WeatherSystem}");
			sb.AppendLine($"{x}Online Mode: {MyAPIGateway.Session.SessionSettings.OnlineMode}");
			sb.AppendLine($"{x}Game Mode: {MyAPIGateway.Session.SessionSettings.GameMode}");
			sb.AppendLine($"{x}Spiders Enabled: {MyAPIGateway.Session.SessionSettings.EnableSpiders}");
			sb.AppendLine($"{x}Wolves Enabled: {MyAPIGateway.Session.SessionSettings.EnableWolfs}");
			sb.AppendLine($"{x}Sync Distance: {MyAPIGateway.Session.SessionSettings.SyncDistance}");
			sb.AppendLine($"{x}View Distance: {MyAPIGateway.Session.SessionSettings.ViewDistance}");
			sb.AppendLine($"{x}Player Inventory Size Multiplier: {MyAPIGateway.Session.SessionSettings.InventorySizeMultiplier}");
			sb.AppendLine($"{x}Grid Inventory Size Multiplier: {MyAPIGateway.Session.SessionSettings.BlocksInventorySizeMultiplier}");
			sb.AppendLine($"{x}Total Pirate PCU: {MyAPIGateway.Session.SessionSettings.PiratePCU}");
			sb.AppendLine($"{x}Total Player PCU: {MyAPIGateway.Session.SessionSettings.TotalPCU}");

			sb.AppendLine();
			sb.AppendLine("Installed Mods");
			sb.AppendLine("═══════════════════════════════════════════");
			foreach (MyObjectBuilder_Checkpoint.ModItem mod in MyAPIGateway.Session.Mods)
				sb.AppendLine($"{x}[Name (Id)] {mod} [Is a dependency] {mod.IsDependency}");

			sb.AppendLine();
			sb.AppendLine("Stored Identities");
			sb.AppendLine("═══════════════════════════════════════════");
			List<IMyIdentity> identityList = new List<IMyIdentity>();
			MyAPIGateway.Players.GetAllIdentites(identityList);
			foreach (IMyIdentity identity in identityList)
				sb.AppendFormat("{0,-4}[Id] {1,-18} [Display Name] {2,-35} [Is Dead] {3,-4} [SteamId > 0] {4,-4}\n", " ",identity.IdentityId, identity.DisplayName, identity.IsDead, MyAPIGateway.Players.TryGetSteamId(identity.IdentityId) > 0); // SteamId > 0 denotes player; no reason to see / save their ID though

			sb.AppendLine();
			sb.AppendLine("Factions");
			sb.AppendLine("═══════════════════════════════════════════");
			foreach (var faction in MyAPIGateway.Session.Factions.Factions)
				sb.AppendFormat("{0,-4}[Tag] {1, -7} [ID] {2, -18} [Name] {3,-30}\n", " ", faction.Value.Tag, faction.Value.FactionId, faction.Value.Name);

			sb.AppendLine();
			sb.AppendLine("Information Export End");
			sb.AppendLine("═══════════════════════════════════════════");
			return sb;
		}
	}
}