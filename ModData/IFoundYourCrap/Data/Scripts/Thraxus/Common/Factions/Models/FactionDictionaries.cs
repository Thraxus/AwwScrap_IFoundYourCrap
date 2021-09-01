using System.Collections.Generic;
using System.Linq;
using System.Text;
using AwwScrap_IFoundYourCrap.Thraxus.Common.Factions.DataTypes.Enums;
using Sandbox.Definitions;
using Sandbox.ModAPI;
using VRage.Game.ModAPI;

namespace AwwScrap_IFoundYourCrap.Thraxus.Common.Factions.Models
{
	public static class FactionDictionaries
	{
		/// <summary>
		/// Normal rep controlled player factions
		/// </summary>
		public static readonly Dictionary<long, IMyFaction> PlayerFactions = new Dictionary<long, IMyFaction>();

		/// <summary>
		/// Players who have decided to opt out of the rep system (always hostile to NPCs)
		/// </summary>
		public static readonly Dictionary<long, IMyFaction> PlayerPirateFactions = new Dictionary<long, IMyFaction>();

		/// <summary>
		/// NPC factions who hate everyone
		/// </summary>
		public static readonly Dictionary<long, IMyFaction> PirateFactions = new Dictionary<long, IMyFaction>();

		/// <summary>
		/// NPC factions who hate people who hate other people
		/// </summary>
		public static readonly Dictionary<long, IMyFaction> EnforcementFactions = new Dictionary<long, IMyFaction>();

		/// <summary>
		/// NPC factions who like to be nice to everyone
		/// </summary>
		public static readonly Dictionary<long, IMyFaction> LawfulFactions = new Dictionary<long, IMyFaction>();

		/// <summary>
		/// All EEM NPC factions; doesn't discriminate if they are an asshole or angel
		/// </summary>
		public static readonly Dictionary<long, IMyFaction> AllEemNpcFactions = new Dictionary<long, IMyFaction>();

		/// <summary>
		/// All NPC factions that aren't controlled by EEM
		/// </summary>
		public static readonly Dictionary<long, IMyFaction> AllNonEemNpcFactions = new Dictionary<long, IMyFaction>();

		/// <summary>
		/// All Vanilla Trade factions 
		/// </summary>
		public static readonly Dictionary<long, IMyFaction> VanillaTradeFactions = new Dictionary<long, IMyFaction>();

		/// <summary>
		/// All NPC Factions that aren't considered a trader (cheaty stations are cheaty)
		/// </summary>
		public static readonly Dictionary<long, IMyFaction> NonTraderNpcFactions = new Dictionary<long, IMyFaction>();

		private static bool _setupComplete;

		public static void Initialize()
		{
			if (_setupComplete) return;
			_setupComplete = true;

			foreach (var faction in MyAPIGateway.Session.Factions.Factions)
			{
				MyFactionDefinition def = MyDefinitionManager.Static.TryGetFactionDefinition(faction.Value.Tag);
				if (def == null)
				{   // Player faction, Vanilla Trader, or some other mods faction that creates everything in code and nothing in the .sbc
					if (MyAPIGateway.Players.TryGetSteamId(faction.Value.FounderId) > 0)
					{   // Player faction of some sort
						if (Settings.PlayerFactionExclusionList.Any(x => faction.Value.Description.StartsWith(x)))
						{ // Player pirate
							PlayerPirateFactions.Add(faction.Key, faction.Value);
							continue;
						}
						// Regular player faction
						PlayerFactions.Add(faction.Key, faction.Value);
						continue;
					}
					// Vanilla trader faction
					VanillaTradeFactions.Add(faction.Key, faction.Value);
					continue;
				}

				if (Settings.FactionTags[FactionTypes.Neutral].Contains(def.Tag))
				{
					LawfulFactions.Add(faction.Key, faction.Value);
					NonTraderNpcFactions.Add(faction.Key, faction.Value);
					continue;
				}

				if (Settings.FactionTags[FactionTypes.Enforcement].Contains(def.Tag))
				{
					EnforcementFactions.Add(faction.Key, faction.Value);
					NonTraderNpcFactions.Add(faction.Key, faction.Value);
					continue;
				}

				if (Settings.FactionTags[FactionTypes.Hostile].Contains(def.Tag))
				{
					PirateFactions.Add(faction.Key, faction.Value);
					NonTraderNpcFactions.Add(faction.Key, faction.Value);
					continue;
				}

				if (Settings.FactionTags[FactionTypes.Player].Contains(def.Tag))
				{
					PlayerFactions.Add(faction.Key, faction.Value);
					continue;
				}

				// I'm guessing this is a NPC faction and it's not mine.
				AllNonEemNpcFactions.Add(faction.Key, faction.Value);
				NonTraderNpcFactions.Add(faction.Key, faction.Value);
			}
		}

		public static string Report()
		{
			StringBuilder report = new StringBuilder();
			const string x = "    ";
			report.AppendLine();
			report.AppendLine();
			report.AppendLine("Factions Report - Begin");
			report.AppendLine("═══════════════════════════════════════════");


			report.AppendLine("Lawful Factions");
			foreach (var faction in LawfulFactions)
			{
				report.AppendLine($"{x}{faction.Value.Tag}");
			}

			report.AppendLine();


			report.AppendLine("Enforcement Factions");
			foreach (var faction in EnforcementFactions)
			{
				report.AppendLine($"{x}{faction.Value.Tag}");
			}

			report.AppendLine();

			report.AppendLine("Pirate Factions");
			foreach (var faction in PirateFactions)
			{
				report.AppendLine($"{x}{faction.Value.Tag}");
			}

			report.AppendLine();

			report.AppendLine("Vanilla Trader Factions");
			if (VanillaTradeFactions.Count == 0) report.AppendLine($"{x}None");
			foreach (var faction in VanillaTradeFactions)
			{
				report.AppendLine($"{x}{faction.Value.Tag}");
			}

			report.AppendLine();

			report.AppendLine("Non-EEM NPC Factions");
			if (AllNonEemNpcFactions.Count == 0) report.AppendLine($"{x}None");
			foreach (var faction in AllNonEemNpcFactions)
			{
				report.AppendLine($"{x}{faction.Value.Tag}");
			}

			report.AppendLine();

			report.AppendLine("All NPC Factions");
			foreach (var faction in NonTraderNpcFactions)
			{
				report.AppendLine($"{x}{faction.Value.Tag}");
			}

			report.AppendLine();

			report.AppendLine("Player Factions");
			if (PlayerFactions.Count == 0) report.AppendLine($"{x}None");
			foreach (var faction in PlayerFactions)
			{
				report.AppendLine($"{x}{faction.Value.Tag}");
			}

			report.AppendLine();

			report.AppendLine("Player Pirate Factions");
			if (PlayerPirateFactions.Count == 0) report.AppendLine($"{x}None");
			foreach (var faction in PlayerPirateFactions)
			{
				report.AppendLine($"{x}{faction.Value.Tag}");
			}

			report.AppendLine("═══════════════════════════════════════════");
			report.AppendLine("Factions Report - End");

			return report.ToString();
		}
	}
}
