using System;
using System.Collections.Generic;

namespace MVCTemplete.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }

        public int StatusCode { get; set; }

        public string Message { get; set; }

        public T Data { get; set; }

        public List<string> Errors { get; set; }

        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;

        public string TraceId { get; set; } = Guid.NewGuid().ToString();

        public object Meta { get; set; }

        public ApiResponse()
        {
            Errors = new List<string>();
        }

        public static ApiResponse<T> SuccessResponse(
            T data,
            string message = "Success",
            int statusCode = 200)
        {
            return new ApiResponse<T>
            {
                Success = true,
                StatusCode = statusCode,
                Message = message,
                Data = data
            };
        }

        public static ApiResponse<T> FailureResponse(
            string message,
            List<string> errors = null,
            int statusCode = 400)
        {
            return new ApiResponse<T>
            {
                Success = false,
                StatusCode = statusCode,
                Message = message,
                Errors = errors ?? new List<string>()
            };
        }
    }
}