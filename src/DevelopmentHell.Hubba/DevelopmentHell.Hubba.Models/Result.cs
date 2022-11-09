namespace DevelopmentHell.Hubba.Models
{
    public class Result
    {
        public bool IsSuccessful { get; set; }
        public string ErrorMessage { get; set; }
        public object? Payload { get; set; }
        public Result(bool IsSuccessful = true, string ErrorMessage = "", object? Payload = null)
        {
            this.IsSuccessful = IsSuccessful;
            this.ErrorMessage = ErrorMessage;
            this.Payload = Payload;
        }

    }
}