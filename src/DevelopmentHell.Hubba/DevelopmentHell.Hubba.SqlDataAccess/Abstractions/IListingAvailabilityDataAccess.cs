using DevelopmentHell.Hubba.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.SqlDataAccess.Abstractions
{
    public  interface IListingAvailabilityDataAccess
    {
        Task<Result> GetListingAvailabilityId(int listingId);
        Task<Result> GetListingAvailabilityStartDateTime(int listingId);
        Task<Result> GetListingAvailabilityEndDateTime(int listingId);
        Task<Result> GetListingAvailabilityTimeFrames(int listingId);
        Task<Result> CreateListingAvailability(int listingId, DateTime startDateTime, DateTime endDateTime);
        Task<Result> UpdateListingAvailability(ListingAvailability listingAvailability);
        Task<Result> DeleteListingAvailability(int listingId, int availabilityId);

    }
}
