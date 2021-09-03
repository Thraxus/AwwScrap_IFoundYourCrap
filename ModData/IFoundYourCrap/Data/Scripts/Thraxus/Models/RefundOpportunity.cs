using VRage.Utils;

namespace AwwScrap_IFoundYourCrap.Thraxus.Models
{
	public class RefundOpportunity
	{
		public string CompSubtype;
		public MyStringHash ScrapSubtype;
		public int Count;
		
		public void Reset()
		{
			CompSubtype = "";
			ScrapSubtype = MyStringHash.NullOrEmpty;
			Count = 0;
		}
	}
}