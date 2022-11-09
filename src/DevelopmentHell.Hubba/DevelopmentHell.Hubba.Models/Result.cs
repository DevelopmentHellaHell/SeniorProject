﻿namespace DevelopmentHell.Hubba.Models
{
    public class Result
    {
        public bool IsSuccessful { get; set; }
        public string ErrorMessage { get; set; }
        public List<List<object>>? Payload { get; set; }
        public Result(bool IsSuccessful = true, string ErrorMessage = "", List<List<object>>? Payload = null)
        {
            this.IsSuccessful = IsSuccessful;
            this.ErrorMessage = ErrorMessage;
            this.Payload = Payload;
        }

    }
}