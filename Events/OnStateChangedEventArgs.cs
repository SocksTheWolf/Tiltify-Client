using System;

namespace Tiltify.Events
{
    public class OnStateChangedEventArgs : EventArgs
    {
        public bool IsConnected;
        public bool WasConnected;
    }
}
