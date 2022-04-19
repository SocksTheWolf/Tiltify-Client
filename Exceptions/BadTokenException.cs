using System;
using System.Collections.Generic;
using System.Text;

namespace Tiltify.Exceptions
{
    /// <inheritdoc />
    /// <summary>Exception representing a token not correctly associated with the given user.</summary>
    public class BadTokenException : Exception
    {
        /// <inheritdoc />
        /// <summary>Exception constructor</summary>
        public BadTokenException(string data)
            : base(data)
        {
        }
    }
}
