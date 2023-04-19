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
        Task<Result> GetListing(int listingId);
        Task<Result> GetListingAvailabilityByMonth(int listingId);
        
    }
}
