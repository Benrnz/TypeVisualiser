using System;

namespace TypeVisualiser.Model
{
    internal class ConnectionRouteEventArgs : EventArgs
    {
        public ConnectionRouteEventArgs(ConnectionLine route)
        {
            this.Line = route;
        }

        public ConnectionLine Line { get; private set; }
    }
}