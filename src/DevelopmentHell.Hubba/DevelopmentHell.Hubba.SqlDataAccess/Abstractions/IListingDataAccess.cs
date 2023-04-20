using DevelopmentHell.Hubba.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.SqlDataAccess.Abstractions
{
    public interface IListingDataAccess
    {
        Task<Result<int>> CreateListing(ListingModel listing);
        Task<Result<ListingModel>> GetListingByListingId(int listingId);
        Task<Result<List<ListingAvailability>>> GetListingAvailabilityByMonth(int listingId);
        
    }
}
