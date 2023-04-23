using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.Models.DTO
{
    public class BookingViewDTO
    {
        public int BookingId { get; set; }
        public int? UserId { get; set; }
        public int? OwnerId { get; set; }
        public int? ListingId { get; set; }
        public string? ListingTitle { get; set; }
        public string? ListingLocation { get; set; }
        public double? FullPrice { get; set; }
        public List<BookedTimeFrame>? BookedTimeFrames { get; set; }
    }
}
