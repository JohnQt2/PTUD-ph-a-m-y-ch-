using System.Net;
using System.Text.Json;
using MaterialApi.Exceptions;
using MaterialApi.Models.Common;
using Microsoft.AspNetCore.Http;

namespace MaterialApi.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var statusCode = (int)HttpStatusCode.InternalServerError;
        var message = "An internal server error occurred.";

        switch (exception)
        {
            case NotFoundException notFoundException:
                statusCode = (int)HttpStatusCode.NotFound;
                message = notFoundException.Message;
                break;

            case ValidationException validationException:
                statusCode = (int)HttpStatusCode.UnprocessableEntity; // 422
                message = validationException.Message;
                break;

            default:
                message = exception.Message; // Cảnh báo: Ở môi trường Production nên ẩn chi tiết lỗi
                break;
        }

        context.Response.StatusCode = statusCode;

        var response = ApiResponse<object>.FailureResponse(message);
        
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var jsonResponse = JsonSerializer.Serialize(response, options);

        return context.Response.WriteAsync(jsonResponse);
    }
}
