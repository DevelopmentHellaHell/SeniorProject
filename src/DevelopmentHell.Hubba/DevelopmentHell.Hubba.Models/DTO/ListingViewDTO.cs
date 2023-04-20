using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.Models.DTO
{
    public class ListingViewDTO
    {
        public int OwnerId { get; set; }
        public string Title { get; set; }
        public string OwnerUsername { get; set; }
        public int ListingId { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public double? Price { get; set; }
        public DateTime LastEdited { get; set; }
        public bool Published { get; set; }
        public double? AverageRating { get; set; }

    }
}
