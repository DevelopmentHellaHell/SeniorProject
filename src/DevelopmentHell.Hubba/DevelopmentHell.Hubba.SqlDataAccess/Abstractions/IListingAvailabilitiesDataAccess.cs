using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.SqlDataAccess.Abstractions
{
    public interface IListingAvailabilitiesDataAccess
    {
        Task<Result> AddListingAvailabilities(List<ListingAvailabilityDTO> listingAvailabilities);
        Task<Result> DeleteListingAvailabilities(List<ListingAvailabilityDTO> listingAvailabilities);
        Task<Result<List<ListingAvailability>>> GetListingAvailabilities(int listingId);
        Task<Result> UpdateListingAvailability(ListingAvailabilityDTO listingAvailability);
    }
}

