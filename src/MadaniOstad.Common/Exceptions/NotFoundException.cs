﻿using System.Net;

namespace MadaniOstad.Common.Exceptions
{
    public class NotFoundException : AppException
    {
        public NotFoundException(string message = "Not Found")
            : base(message, HttpStatusCode.NotFound)
        {
        }
    }
}
