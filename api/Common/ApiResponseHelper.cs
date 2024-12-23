using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace api.Common
{
    public static class ApiResponseHelper
    {
        /// <summary>
        /// Creates a standardized success response.
        /// </summary>
        public static ActionResult<ApiResponse<T>> Success<T>(T payload, int statusCode = StatusCodes.Status200OK)
        {
            var response = new ApiResponse<T>(
                success: true,
                payload: payload,
                error: null,
                statusCode: statusCode);

            return new ObjectResult(response) { StatusCode = statusCode };
        }

        /// <summary>
        /// Creates a standardized error response.
        /// </summary>
        public static ActionResult<ApiResponse<T>> Error<T>(
            int code,
            string message,
            int statusCode = StatusCodes.Status400BadRequest,
            T? payload = default)
        {
            var response = new ApiResponse<T>(
                success: false,
                payload: payload,
                error: new ApiError
                {
                    Code = code,
                    Message = message
                },
                statusCode: statusCode);

            return new ObjectResult(response) { StatusCode = statusCode };
        }
    }
}
