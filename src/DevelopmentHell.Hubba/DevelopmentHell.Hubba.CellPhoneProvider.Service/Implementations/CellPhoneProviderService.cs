using DevelopmentHell.Hubba.CellPhoneProvider.Service.Abstractions;
using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.CellPhoneProvider.Service.Implementations
{
	public class CellPhoneProviderService : ICellPhoneProviderService
	{
		public CellPhoneProviderService() { }

		public string GetProviderEmail(CellPhoneProviders provider)
		{
			string providerEmail = "";
			switch (provider)
			{
				case CellPhoneProviders.T_MOBILE:
					providerEmail = "@tmomail.net";
					break;
				case CellPhoneProviders.VERIZON:
					providerEmail = "@vtext.com";
					break;
				case CellPhoneProviders.ATAT:
					providerEmail = "@txt.att.net";
					break;
				case CellPhoneProviders.SPRINT_PCS:
					providerEmail = "@messaging.sprintpcs.com";
					break;
				case CellPhoneProviders.VIRGIN_MOBILE:
					providerEmail = "@vmobl.com";
					break;
			}

			return providerEmail;
		}
	}
}