namespace api.Common
{
    public class ApiError
    {
        /// <summary>
        /// Error code that can be used on the client side to identify error type (e.g., 404, custom app errors).
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// Human-readable description of the error.
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }
}
