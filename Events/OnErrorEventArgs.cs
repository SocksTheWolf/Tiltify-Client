using System;

namespace Tiltify.Events
{
    public class OnErrorEventArgs : EventArgs
    {
        public Exception Exception { get; set; }
    }
}
