using System;
using System.Collections.Generic;
using System.Text;

namespace Tiltify.Exceptions
{
    /// <inheritdoc />
    /// <summary>Exception representing a response received from Tiltify that is not expected by the library</summary>
    public class UnexpectedResponseException : Exception
    {
        /// <inheritdoc />
        /// <summary>Exception constructor</summary>
        public UnexpectedResponseException(string data)
            : base(data)
        {
        }
    }
}
