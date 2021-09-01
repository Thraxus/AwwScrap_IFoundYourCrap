using System.Collections.Generic;
using AwwScrap_IFoundYourCrap.Thraxus.Common.Factions.DataTypes.Enums;

namespace AwwScrap_IFoundYourCrap.Thraxus.Common.Factions.Models
{
	public static class Settings
	{
		public static readonly Dictionary<FactionTypes, List<string>> FactionTags = new Dictionary<FactionTypes, List<string>>
		{
			// Protectors of the neutral factions
			{ FactionTypes.Enforcement, new List<string> {
				"SEPD", "UCMF" }},

			// Neutral factions that just want to hang out and be friends
			{ FactionTypes.Neutral, new List<string> {
				"CIVL", "ISTG", "MA-I", "EXMC", "KUSS", "HS", "AMPH", "IMDC" }},

			// Asshat pirates! 
			{ FactionTypes.Hostile, new List<string> {
				"SPRT", "SPID", "MMEC" }},

			// May be unused, we'll see...  However, these are the EEM factions who all have at least 1 trade station
			{ FactionTypes.Trader, new List<string> {
				"IMDC", "ISTG", "MA-I" }},

			// I hate this faction, but I have to account for it somehow
			{ FactionTypes.Player, new List<string> {
				"FSTC" }},
		};

		public static IEnumerable<string> PlayerFactionExclusionList { get; } = new List<string>
		{
			"Pirate", "Rogue", "Outlaw", "Bandit"
		};
	}
}