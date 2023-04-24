using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.Models.DTO
{
    public class ListingAvailabilityViewDTO
    {
        public int ListingId { get; set; }
        public int AvailabilityId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
