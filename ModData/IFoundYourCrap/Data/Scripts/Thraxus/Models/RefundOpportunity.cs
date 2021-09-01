using VRage.Utils;

namespace AwwScrap_IFoundYourCrap.Thraxus.Models
{
	public class RefundOpportunity
	{
		public string CompSubtype;
		public MyStringHash ScrapSubtype;
		public int Count;

		public void Copy(RefundOpportunity ro)
		{
			CompSubtype = ro.CompSubtype;
			ScrapSubtype = ro.ScrapSubtype;
			Count = ro.Count;
		}

		public void Reset()
		{
			CompSubtype = "";
			ScrapSubtype = MyStringHash.NullOrEmpty;
			Count = 0;
		}
	}
}