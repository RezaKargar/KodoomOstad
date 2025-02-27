﻿using System;
using System.Net;

namespace MadaniOstad.Common.Exceptions
{
    public class AppException : Exception
    {
        public readonly HttpStatusCode StatusCode;

        public AppException(string message, HttpStatusCode statusCode)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
