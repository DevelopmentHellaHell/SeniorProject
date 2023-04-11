namespace DevelopmentHell.Hubba.Models
{
    public class Result
    {
        public bool IsSuccessful { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class Result<T> : Result
    {
        public T? Payload { get; set; }
    }
}