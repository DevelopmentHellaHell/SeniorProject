using DevelopmentHell.Hubba.Scheduling.Manager;
using DevelopmentHell.Hubba.Scheduling.Service.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.Scheduling.Test.Manager
{
    [TestClass]
    public class SchedulingManagerTest
    {
        private readonly ISchedulingManager _schedulingManager;
        private readonly IBookingService _bookingService;
        private readonly IAvailabilityService _availabilityService;

        private readonly IListingDataAccess _listingDAO;
        private readonly IBookingDataAccess _bookingDAO;
        private readonly IBookedTimeFrameDataAccess _bookedTimeFrameDAO;

        private readonly string _bookingsConnectionString = ConfigurationManager.AppSettings["BookingsConnectionString"]!;
        private readonly string _bookingsTable = ConfigurationManager.AppSettings["BookingsTable"]!;
        private readonly string _bookedTimeFramesTable = ConfigurationManager.AppSettings["BookedTimeFramesTable"]!;
        private readonly string _listingsConnectionString = ConfigurationManager.AppSettings["ListingsConnectionString"]!;
        private readonly string _listingsTable = ConfigurationManager.AppSettings["ListingsTable"]!;

        public SchedulingManagerTest() 
        { 

        }
    }
}
