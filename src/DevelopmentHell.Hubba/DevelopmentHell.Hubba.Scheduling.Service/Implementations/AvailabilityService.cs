using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Scheduling.Service.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.Scheduling.Service.Implementations
{
    public class AvailabilityService : IAvailabilityService
    {
        public Task<Result> GetBookedTimeFrames(int id)
        {
            //TODO
            throw new NotImplementedException();
        }

        public Task<Result> GetListingAvailabilityByMonth(int listingId, int month, int year)
        {
            //TODO
            throw new NotImplementedException();
        }

        public Task<Result> GetOwnerId(int listingId)
        {
            //TODO
            throw new NotImplementedException();
        }
    }
}
