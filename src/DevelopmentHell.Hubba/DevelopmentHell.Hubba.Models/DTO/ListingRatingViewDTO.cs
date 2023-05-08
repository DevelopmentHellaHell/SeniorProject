﻿namespace DevelopmentHell.Hubba.Models.DTO
{
    public class ListingRatingViewDTO
    {
        public int ListingId { get; set; }
        public int UserId { get; set; }
        public string? Username { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public bool? Anonymous { get; set; }
        public DateTime? LastEdited { get; set; }
    }
}
