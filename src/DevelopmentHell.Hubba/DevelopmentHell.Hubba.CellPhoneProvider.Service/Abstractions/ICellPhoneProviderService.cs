using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.CellPhoneProvider.Service.Abstractions
{
    public interface ICellPhoneProviderService
    {
        string GetProviderEmail(CellPhoneProviders provider);
    }
}
