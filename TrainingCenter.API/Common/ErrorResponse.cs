namespace TrainingCenter.API.Common
{
    public class ErrorResponse
    {
        public int StatusCode { get; set; }

        public string Message { get; set; } = null!;

        public DateTime Timestamp { get; set; }
    }
}
