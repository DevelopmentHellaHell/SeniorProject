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
        Task<Result> GetListingId(int ownerId);
        Task<Result> GetTitle(int listingId);
        Task<Result> GetDescription(int listingId);
        Task<Result> GetLocation(int listingId);
        Task<Result> GetPrice(int listingId);
        Task<Result> GetLastEdited(int listingId);
        Task<Result> GetPublished(int listingId);
        Task<Result> CreateListing(ListingModel listing);
        Task<Result> UpdateListing(ListingModel listing);
        Task<Result> DeleteListing(int listingId);
    }
}
