using System.Text;
using System.Xml.Serialization;

namespace AwwScrap_IFoundYourCrap.Thraxus.Support
{
	[XmlRoot("UserSettings", IsNullable = false)]
	public class UserSettings
	{
		//[DefaultValue(Constants.DefaultBasicGrinderRefund)]
		[XmlElement("BasicGrinderReturnRate", typeof(int))]
		public int BasicGrinderReturnRate = 80;

		//[DefaultValue(Constants.DefaultProficientGrinderRefund)]
		[XmlElement("EnhancedGrinderReturnRate", typeof(int))]
		public int EnhancedGrinderReturnRate = 60;

		//[DefaultValue(Constants.DefaultProficientGrinderRefund)]
		[XmlElement("ProficientGrinderReturnRate", typeof(int))]
		public int ProficientGrinderReturnRate = 40;

		//[DefaultValue(Constants.DefaultEliteGrinderRefund)]
		[XmlElement("EliteGrinderReturnRate", typeof(int))]
		public int EliteGrinderReturnRate = 20;

		//[DefaultValue(Constants.DefaultBodyBagDecayInMinutes)]
		[XmlElement("ScrapBodyBagDecayInMinutes", typeof(int))]
		public int ScrapBodyBagDecayInMinutes = 5;

		//[DefaultValue(Constants.ReturnComponentsFromUnownedGrids)]
		[XmlElement("ReturnComponentsFromUnownedGrids", typeof(bool))]
		public bool ReturnComponentsFromUnownedGrids;

		//[DefaultValue(Constants.ReturnRateForUnownedGrids)]
		[XmlElement("ReturnRateForUnownedGrids", typeof(int))]
		public int ReturnRateForUnownedGrids = 10;

		public void ParseLoadedUserSettings(UserSettings settings)
		{
			BasicGrinderReturnRate = settings.BasicGrinderReturnRate >= 0 && settings.BasicGrinderReturnRate < 100 ? settings.BasicGrinderReturnRate : Constants.DefaultBasicGrinderRefund;
			EnhancedGrinderReturnRate = settings.EnhancedGrinderReturnRate >= 0 && settings.EnhancedGrinderReturnRate < 100 ? settings.EnhancedGrinderReturnRate : Constants.DefaultEnhancedGrinderRefund;
			ProficientGrinderReturnRate = settings.ProficientGrinderReturnRate >= 0 && settings.ProficientGrinderReturnRate < 100 ? settings.ProficientGrinderReturnRate : Constants.DefaultProficientGrinderRefund;
			EliteGrinderReturnRate = settings.EliteGrinderReturnRate >= 0 && settings.EliteGrinderReturnRate < 100 ? settings.EliteGrinderReturnRate : Constants.DefaultEliteGrinderRefund;
			ScrapBodyBagDecayInMinutes = settings.ScrapBodyBagDecayInMinutes >= 0 && settings.ScrapBodyBagDecayInMinutes < 10 ? settings.ScrapBodyBagDecayInMinutes : Constants.DefaultBodyBagDecayInMinutes;
			ReturnComponentsFromUnownedGrids = settings.ReturnComponentsFromUnownedGrids;
			ReturnRateForUnownedGrids = settings.ReturnRateForUnownedGrids >= 0 && settings.ReturnRateForUnownedGrids < 100 ? settings.ReturnRateForUnownedGrids : Constants.ReturnRateForUnownedGrids;
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine();
			sb.AppendLine();
			sb.AppendLine("---------- IFoundYourScrap Settings ----------");
			sb.AppendLine();
			sb.AppendFormat("{0, -4}BasicGrinderReturnRate: {1, -6}\n", " ", BasicGrinderReturnRate);
			sb.AppendFormat("{0, -4}EnhancedGrinderReturnRate: {1, -6}\n", " ", EnhancedGrinderReturnRate);
			sb.AppendFormat("{0, -4}ProficientGrinderReturnRate: {1, -6}\n", " ", ProficientGrinderReturnRate);
			sb.AppendFormat("{0, -4}EliteGrinderReturnRate: {1, -6}\n", " ", EliteGrinderReturnRate);
			sb.AppendFormat("{0, -4}ScrapBodyBagDecayInMinutes: {1, -6}\n", " ", ScrapBodyBagDecayInMinutes);
			sb.AppendFormat("{0, -4}ReturnComponentsFromUnownedGrids: {1, -6}\n", " ", ReturnComponentsFromUnownedGrids);
			sb.AppendFormat("{0, -4}ReturnRateForUnownedGrids: {1, -6}\n", " ", ReturnRateForUnownedGrids);
			sb.AppendLine();
			sb.AppendLine("------------- End Settings Report -------------");
			sb.AppendLine();
			sb.AppendLine();
			return sb.ToString();
		}
	}
}