using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using System.Configuration;

namespace DevelopmentHell.Hubba.Listing.Test
{
    [TestClass]
    public class ListingDataAccessUnitTest
    {
        //Arrange
        private static string _ListingsConnectionString = ConfigurationManager.AppSettings["ListingsConnectionString"]!;
        private static string _tableName = ConfigurationManager.AppSettings["ListingsTable"]!;
        private IListingDataAccess _listingDAO;

        public ListingDataAccessUnitTest()
        {
            _listingDAO = new ListingDataAccess(_ListingsConnectionString, _tableName);
        }

        [TestMethod]
        public async Task CreateListing()
        {
            //Arrange
            ListingModel _listing = new ListingModel()
            {
                OwnerId = 1,
                Title = "test Title",
                Description = "test Description",
                Location = "test Location",
                Price = 35,
                Published = true
            };
            var expected = true;

            //Act
            
            Result actual = await _listingDAO.CreateListing(_listing).ConfigureAwait(false);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsSuccessful == expected);
        }
    }
}