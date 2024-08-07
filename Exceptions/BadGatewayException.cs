﻿using System;

namespace Tiltify.Exceptions
{
    /// <inheritdoc />
    /// <summary>Exception representing a 502 Http Statuscode</summary>
    public class BadGatewayException : Exception
    {
        /// <inheritdoc />
        /// <summary>Exception constructor</summary>
        public BadGatewayException(string data)
            : base(data)
        {
        }
    }
}