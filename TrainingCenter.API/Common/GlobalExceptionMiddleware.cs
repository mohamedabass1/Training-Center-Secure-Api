using FluentValidation;
using System.Text.Json;
using TrainingCenter.Application.Exceptions;

namespace TrainingCenter.API.Common
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionMiddleware(RequestDelegate next)
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


        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            int statusCode;
            string message;


            switch (exception)
            {
                case NotFoundException:
                    statusCode = StatusCodes.Status404NotFound;
                    message = exception.Message;
                    break;


                case ConflictException:
                    statusCode = StatusCodes.Status409Conflict;
                    message = exception.Message;
                    break;


                case BadRequestException:
                    statusCode = StatusCodes.Status400BadRequest;
                    message = exception.Message;
                    break;

                case ValidationException validationException:
                    statusCode = StatusCodes.Status400BadRequest;

                    message = validationException.Message;

                    break;

                default:
                    statusCode = StatusCodes.Status500InternalServerError;
                    message = exception.GetType().FullName!;
                    break;
            }


            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;


            var response = new ErrorResponse
            {
                StatusCode = statusCode,
                Message = message,
                Timestamp = DateTime.UtcNow
            };


            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response));
        }
    }
}
