using System;

namespace Tiltify.Events
{
    public class OnTiltifyServiceErrorArgs : EventArgs
    {
        public Exception Exception { get; set; }
    }
}
