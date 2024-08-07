﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Tiltify.Exceptions
{
    /// <inheritdoc />
    /// <summary>Exception representing a Helix request sent without an OAuth access token</summary>
    public class ClientIdAndOAuthTokenRequired : Exception
    {
        /// <inheritdoc />
        /// <summary>Exception constructor</summary>
        public ClientIdAndOAuthTokenRequired(string explanation)
            : base(explanation)
        {
        }
    }
}
