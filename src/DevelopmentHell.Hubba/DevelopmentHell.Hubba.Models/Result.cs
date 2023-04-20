namespace DevelopmentHell.Hubba.Models
{
    public class Result
    {
        public bool IsSuccessful { get; set; }
        public string? ErrorMessage { get; set; }
        public int? StatusCode { get; set; }
        static public Result Success() => new Result { IsSuccessful = true, StatusCode = 200 };
        static public Result Failure(string errorMessage, int? statusCode = 500) => new Result { IsSuccessful = false, ErrorMessage = errorMessage, StatusCode = statusCode };
        public Result(Result result) { IsSuccessful = result.IsSuccessful; ErrorMessage = result.ErrorMessage; StatusCode = result.StatusCode; }
        public Result() { }
    }

    public class Result<T> : Result
    {
        public T? Payload { get; set; }
        static public Result<T> Success(T payload) => new Result<T> { IsSuccessful = true, StatusCode = 200, Payload = payload };
        static public new Result<T> Failure(string errorMessage, int? statusCode = 500) => new Result<T> { IsSuccessful = false, ErrorMessage = errorMessage, StatusCode = statusCode };
        public Result(Result result) { IsSuccessful = result.IsSuccessful; ErrorMessage = result.ErrorMessage; StatusCode = result.StatusCode; }
        public Result() { }
    }
}