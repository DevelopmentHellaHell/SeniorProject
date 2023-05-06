using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.Models
{
    public class Reservations
    {
        public int OwnerId { get; set; }
        public int UserId { get; set; }
        public int ListingId { get; set; }
        public string? Title { get; set; }

    }
}
