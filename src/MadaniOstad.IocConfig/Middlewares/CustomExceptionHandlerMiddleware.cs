﻿using MadaniOstad.Common.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace MadaniOstad.IocConfig.Middlewares
{
    public static class CustomExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomExceptionHandlerMiddleware>();
        }
    }

    public class CustomExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<CustomExceptionHandlerMiddleware> _logger;

        public CustomExceptionHandlerMiddleware(RequestDelegate next,
            IWebHostEnvironment env,
            ILogger<CustomExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _env = env;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            string message = null;

            HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError;

            try
            {
                await _next(context);
            }
            catch (AppException exception)
            {

                _logger.LogError(exception, exception.Message);
                httpStatusCode = exception.StatusCode;

                if (_env.EnvironmentName == "Development")
                {
                    Dictionary<string, string> dic = new Dictionary<string, string>
                    {
                        ["Exception"] = exception.Message,
                        ["StackTrace"] = exception.StackTrace,
                    };
                    if (exception.InnerException != null)
                    {
                        dic.Add("InnerException.Exception", exception.InnerException.Message);
                        dic.Add("InnerException.StackTrace", exception.InnerException.StackTrace);
                    }

                    message = JsonSerializer.Serialize(dic);
                }
                else
                {
                    message = exception.Message;
                }
                await WriteToResponseAsync(exception.Data);
            }
            catch (SecurityTokenExpiredException exception)
            {
                _logger.LogError(exception, exception.Message);
                SetUnAuthorizeResponse(exception);
                await WriteToResponseAsync(exception.Data);
            }
            catch (UnauthorizedAccessException exception)
            {
                _logger.LogError(exception, exception.Message);
                SetUnAuthorizeResponse(exception);
                await WriteToResponseAsync(exception.Data);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);

                if (_env.EnvironmentName == "Development")
                {
                    Dictionary<string, string> dic = new Dictionary<string, string>
                    {
                        ["Exception"] = exception.Message,
                        ["StackTrace"] = exception.StackTrace,
                    };
                    message = JsonSerializer.Serialize(dic);
                }
                await WriteToResponseAsync(exception.Data);
            }

            async Task WriteToResponseAsync(object data)
            {
                if (context.Response.HasStarted)
                {
                    throw new InvalidOperationException("The response has already started, the http status code middleware will not be executed.");
                }

                var result = new
                {
                    data = data,
                    errors = new[] { message }
                };
                string json = JsonSerializer.Serialize(result);

                context.Response.StatusCode = (int)httpStatusCode;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(json);
            }

            void SetUnAuthorizeResponse(Exception exception)
            {
                httpStatusCode = HttpStatusCode.Unauthorized;

                if (_env.EnvironmentName == "Development")
                {
                    Dictionary<string, string> dic = new Dictionary<string, string>
                    {
                        ["Exception"] = exception.Message,
                        ["StackTrace"] = exception.StackTrace
                    };
                    if (exception is SecurityTokenExpiredException tokenException)
                    {
                        dic.Add("Expires", tokenException.Expires.ToString());
                    }

                    message = JsonSerializer.Serialize(dic);
                }
            }
        }
    }
}
