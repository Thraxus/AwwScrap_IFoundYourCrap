using System.ComponentModel;
using System.Xml.Serialization;

namespace AwwScrap_IFoundYourCrap.Thraxus.Support
{
	[XmlRoot("UserSettings", IsNullable = false)]
	public static class UserSettings
	{
		[XmlAttribute("RefundChanceLow")] 
		[DefaultValue(Constants.DefaultRefundLow)] 
		public static int RefundChanceLow = 30;
		
		[XmlAttribute("RefundChanceMid")] 
		[DefaultValue(Constants.DefaultRefundMid)] 
		public static int RefundChanceMid = 60;
		
		[XmlAttribute("RefundChanceHigh")] 
		[DefaultValue(Constants.DefaultRefundHigh)] 
		public static int RefundChanceHigh = 90;
	}
}
