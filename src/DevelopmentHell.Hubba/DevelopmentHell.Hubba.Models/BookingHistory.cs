using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.Models
{
    public class BookingHistory
    {
        public int BookingId { get; set; }
        public int ListingId { get; set; }
        public int? FullPrice { get; set; }
        public int BookingStatusId { get; set; }
        public string? Title { get; set; }
        public string? Location { get; set; }
    }
}
