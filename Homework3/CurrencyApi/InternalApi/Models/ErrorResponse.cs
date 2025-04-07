namespace InternalApi.Models
{
    /// <summary>
    /// Returns if anything went wrong
    /// </summary>
    public class ErrorResponse
    {
        /// <summary>
        /// What went wrong
        /// </summary>
        public string Message { get; set; }
        public ErrorResponse(string message)
        {
            Message = message;
        }
    }
}
