using AwwScrap_IFoundYourCrap.Thraxus.Common.Utilities;
using VRage.Utils;

namespace AwwScrap_IFoundYourCrap.Thraxus.Support
{
	public static class Constants
	{
		public const string ScrapSuffix = "Scrap";

		public const int DefaultRefundLow = 30;
		public const int DefaultRefundMid = 60;
		public const int DefaultRefundHigh = 90;

		public static MyStringHash AngleGrinderMid = MyStringHash.GetOrCompute("AngleGrinder3");
		public static MyStringHash AngleGrinderHigh = MyStringHash.GetOrCompute("AngleGrinder4");

		public static int Chance => CommonSettings.Random.Next(1, 100);
	}
}
