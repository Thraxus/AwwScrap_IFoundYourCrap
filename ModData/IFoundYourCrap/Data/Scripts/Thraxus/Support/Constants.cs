﻿using AwwScrap_IFoundYourCrap.Thraxus.Common.Utilities;
using VRage.Utils;

namespace AwwScrap_IFoundYourCrap.Thraxus.Support
{
	public static class Constants
	{
		public const string SettingsFileName = "AwwScrap_IFoundYourCrapSettings.xml";

		public const string ScrapSuffix = "Scrap";

		public const int DefaultBasicGrinderRefund = 90;
		public const int DefaultProficientGrinderRefund = 60;
		public const int DefaultEliteGrinderRefund = 30;
		public const int DefaultBodyBagDecayInMinutes = 5;
		public const int ReturnRateForUnownedGrids = 10;
		
		public const bool ReturnComponentsFromUnownedGrids = false;

		public static MyStringHash ProficientAngleGrinder = MyStringHash.GetOrCompute("AngleGrinder3");
		public static MyStringHash EliteAngleGrinder = MyStringHash.GetOrCompute("AngleGrinder4");

		public static int Chance => CommonSettings.Random.Next(1, 100);
	}
}
