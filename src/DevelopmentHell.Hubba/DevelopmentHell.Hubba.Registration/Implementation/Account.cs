﻿namespace DevelopmentHell.Hubba.Registration
{
    public class Account
    {
        public int ID { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Passphrase { get; set; } = string.Empty;
        public string HashedPassphrase { get; }
        public string Username { get; set; } = string.Empty;
        public bool adminAccount { get; set; }
    }
}