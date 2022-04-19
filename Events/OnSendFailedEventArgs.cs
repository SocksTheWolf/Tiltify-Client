using System;

namespace Tiltify.Events
{
    public class OnSendFailedEventArgs : EventArgs
    {
        public string Data;
        public Exception Exception;
    }
}
