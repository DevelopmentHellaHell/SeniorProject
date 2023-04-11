﻿namespace DevelopmentHell.Hubba.Models
{
    public class UserAccount
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        public string? PasswordSalt { get; set; }
        public int? LoginAttempts { get; set; }
        public object? FailureTime { get; set; }
        public bool? Disabled { get; set; }
        public string? Role { get; set; }
        public string? CellPhoneNumber { get; set; }
        public CellPhoneProviders? CellPhoneProvider { get; set; }
    }
}