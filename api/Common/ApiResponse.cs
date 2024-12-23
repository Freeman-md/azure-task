namespace api.Common
{
    public class ApiResponse<T>
    {
        /// <summary>
        /// Indicates whether the request was successful.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Contains the application-specific data when the request is successful.
        /// </summary>
        public T? Payload { get; set; }

        /// <summary>
        /// Contains error information when the request fails.
        /// </summary>
        public ApiError? Error { get; set; }

        /// <summary>
        /// The HTTP status code of the response (optional).
        /// </summary>
        public int? StatusCode { get; set; }

        public ApiResponse(bool success, T? payload, ApiError? error, int? statusCode)
        {
            Success = success;
            Payload = payload;
            Error = error;
            StatusCode = statusCode;
        }
    }
}
