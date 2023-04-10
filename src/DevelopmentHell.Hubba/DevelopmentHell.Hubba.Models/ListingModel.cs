using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.Models
{
    public class ListingModel
    {
        public int ListingId { get; }
        public int OwnerId { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public double? Price { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? LastEdited { get; set; }
        public int? LastModifyUser { get; set; }
        public bool? Published { get; set; }
        
    }
}
