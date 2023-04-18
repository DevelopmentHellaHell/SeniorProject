using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using System.Configuration;

namespace DevelopmentHell.Hubba.Scheduling.Test
{
    [TestClass]
    public class ListingDataAccessUnitTest
    {
        //Arrange
        private static string _ListingsConnectionString = ConfigurationManager.AppSettings["ListingsConnectionString"]!;
        private static string _tableName = "Listings";
        private IListingDataAccess _listingDAO = new ListingDataAccess("ListingsConnectionString", "Listings");
        private int _ownerId = 1;
        private string _title = "test Title";
        private string _description = "test Description";
        private string _location = "test Location";
        private float _price = 35;
        private bool _published = true;

        [TestMethod]
        public void OverloadConstructor_ConnectionString_TableName_DatabaseConnected()
        {
            //Arrange
            ListingDataAccess expected = new ListingDataAccess("ListingsConnectionString", "Listings");

            //Act
            var actual = new ListingDataAccess(_ListingsConnectionString, _tableName);
            
            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.GetType(), actual.GetType());
        }

        [TestMethod]
        public async Task CreateListing()
        {
            //Arrange
            ListingModel expected = new ListingModel();
            expected.OwnerId = _ownerId;
            expected.Title = _title;
            expected.Description = _description;
            expected.Location = _location;
            expected.Price = _price;
            expected.Published = _published;

            //Act
            
            Result actual = await _listingDAO.CreateListing(expected).ConfigureAwait(false);

            //Assert
            Assert.IsNotNull(actual);
        }
    }
}