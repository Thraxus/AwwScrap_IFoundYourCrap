using System.Xml.Serialization;

namespace AwwScrap_IFoundYourCrap.Thraxus.Support
{
	[XmlRoot("UserSettings", IsNullable = false)]
	public class UserSettings
	{

		//[DefaultValue(Constants.DefaultTier4GrinderRefund)] 
		[XmlAttribute("BasicGrinderReturnRate")]
		public int BasicGrinderReturnRate = 90;

		//[DefaultValue(Constants.DefaultTier3GrinderRefund)] 
		[XmlAttribute("ProficientGrinderReturnRate")]
		public int ProficientGrinderReturnRate = 60;

		//[DefaultValue(Constants.DefaultTier0GrinderRefund)] 
		[XmlAttribute("EliteGrinderReturnRate")]
		public int EliteGrinderReturnRate = 30;

		//[DefaultValue(Constants.DefaultBodyBagDecayInMinutes)]
		[XmlAttribute("ScrapBodyBagDecayInMinutes")]
		public int ScrapBodyBagDecayInMinutes = 5;

		public void ParseLoadedUserSettings(UserSettings settings)
		{
			BasicGrinderReturnRate = settings.BasicGrinderReturnRate >= 0 && settings.BasicGrinderReturnRate < 100 ? settings.BasicGrinderReturnRate : Constants.DefaultBasicGrinderRefund;
			ProficientGrinderReturnRate = settings.ProficientGrinderReturnRate >= 0 && settings.ProficientGrinderReturnRate < 100 ? settings.ProficientGrinderReturnRate : Constants.DefaultProficientGrinderRefund;
			EliteGrinderReturnRate = settings.EliteGrinderReturnRate >= 0 && settings.EliteGrinderReturnRate < 100 ? settings.EliteGrinderReturnRate : Constants.DefaultEliteGrinderRefund;
			ScrapBodyBagDecayInMinutes = settings.ScrapBodyBagDecayInMinutes >= 0 && settings.ScrapBodyBagDecayInMinutes < 10 ? settings.ScrapBodyBagDecayInMinutes : Constants.DefaultBodyBagDecayInMinutes;
		}
	}
}