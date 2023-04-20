using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using System.Configuration;

namespace DevelopmentHell.Hubba.ListingProfile.Test.DAL
{
    [TestClass]
    public class ListingsDataAccessUnitTest
    {
        //Arrange
        private static string _listingsConnectionString = ConfigurationManager.AppSettings["ListingProfilesConnectionString"]!;
        private static string _tableName = ConfigurationManager.AppSettings["ListingsTable"]!;
        private readonly IListingsDataAccess _listingsDAO;

        public ListingsDataAccessUnitTest()
        {
            _listingsDAO = new ListingsDataAccess(_listingsConnectionString, _tableName);
        }

        [TestMethod]
        public async Task GetListing_ByListingId_Successful()
        {
            //Arrange
            Listing expected = new Listing()
            {
                OwnerId = 11,
                Title = "Test GetListing by ListingId",
                Published = true
            };
            await _listingsDAO.CreateListing(expected.OwnerId, expected.Title).ConfigureAwait(false);
            var listingId = await _listingsDAO.GetListingId(expected.OwnerId, expected.Title).ConfigureAwait(false);
            expected.ListingId = listingId.Payload;

            //Act
            var getListing = await _listingsDAO.GetListing(listingId.Payload).ConfigureAwait(false);
            var actual = getListing.Payload;

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(getListing.IsSuccessful);
            Assert.AreEqual(expected.GetType(), actual.GetType());
        }
    }
}