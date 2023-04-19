using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.SqlDataAccess.Abstractions
{
    public interface IListingsDataAccess
    {
        Task<Result> CreateListing(int ownerId, string title);

        Task<Result<List<Listing>>> GetUserListings(int ownerId);

        Task<Result<Listing>> GetListing(int listingId);

        Task<Result> UpdateListing(ListingEditorDTO listing);

        Task<Result<int>> GetListingOwnerId(int listingId);

        Task<Result> DeleteListing(int listingId);
        Task<Result> PublishListing(int listingId);

        Task<Result<int>> GetListingId(int ownerId, string title);
    }
}
