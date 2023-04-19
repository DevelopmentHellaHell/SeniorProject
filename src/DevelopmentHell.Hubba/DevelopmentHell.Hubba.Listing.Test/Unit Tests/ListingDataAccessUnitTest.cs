using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using System.Configuration;

namespace DevelopmentHell.Hubba.Scheduling.Test.Unit_Tests
{
    [TestClass]
    public class ListingDataAccessUnitTest
    {
        //Arrange
        private static string _listingsConnectionString = ConfigurationManager.AppSettings["ListingsConnectionString"]!;
        private static string _tableName = ConfigurationManager.AppSettings["ListingsTable"]!;
        private readonly IListingDataAccess _listingsDAO;

        public ListingDataAccessUnitTest()
        {
            _listingsDAO = new ListingDataAccess(_listingsConnectionString, _tableName);
        }

        [TestMethod]
        public async Task GetListing_ByListingId_Successful()
        {
            //Arrange
            int listingId = 2;
            ListingModel expected = new ListingModel()
            {
                ListingId = listingId,
                OwnerId = 11,
                Published = true
            };
            
            //Act
            var getListing = await _listingsDAO.GetListing(listingId).ConfigureAwait(false);
            var actual = ((Result<List<ListingModel>>)getListing).Payload[0];

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(getListing.IsSuccessful);
            Assert.AreEqual(expected.GetType(), actual.GetType());
        }
    }
}