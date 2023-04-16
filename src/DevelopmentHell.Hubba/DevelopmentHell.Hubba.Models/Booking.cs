using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.Models
{
    public class Booking
    {
        public int BookingId { get; }
        public int UserId { get; set; }
        public int ListingId { get; set; }
        public float FullPrice { get; set; }
        public int BookingStatusId { get; set; }
        public DateTime CreateDate { get; set; }
        public int LastModifyUser { get; set; }

        public IEnumerable<BookedTimeFrame>? TimeFrames { get; set; }
    }
}
