using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.Models
{
    public class BookedTimeFrame
    {
        public int BookingId { get; set; }
        public int ListingId { get; set; }
        public int? AvailabilityId { get; }
        public DateTime StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
    }
}
