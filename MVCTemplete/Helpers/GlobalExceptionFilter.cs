using MVCTemplete.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

public class GlobalExceptionFilter : ExceptionFilterAttribute
{
    public override void OnException(HttpActionExecutedContext context)
    {
        HttpStatusCode status = HttpStatusCode.InternalServerError;
        string message = "An unexpected error occurred.";

        if (context.Exception is ArgumentException)
        {
            status = HttpStatusCode.BadRequest;
            message = context.Exception.Message;
        }
        else if (context.Exception is UnauthorizedAccessException)
        {
            status = HttpStatusCode.Unauthorized;
            message = "Unauthorized access.";
        }
        else if (context.Exception is KeyNotFoundException)
        {
            status = HttpStatusCode.NotFound;
            message = context.Exception.Message;
        }

        context.Response = context.Request.CreateResponse(
            status,
            ApiResponse<object>.FailureResponse(message)
        );
    }
}