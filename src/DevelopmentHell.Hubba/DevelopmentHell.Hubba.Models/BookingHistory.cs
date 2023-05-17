﻿namespace DevelopmentHell.Hubba.Models
{
    public class BookingHistory
    {
        public int BookingId { get; set; }
        public int ListingId { get; set; }
        public double FullPrice { get; set; }
        public int BookingStatusId { get; set; }
        public string? Title { get; set; }
        public string? Location { get; set; }
        public List<BookedTimeFrame>? BookedTimeFrames { get; set; }
    }
}
