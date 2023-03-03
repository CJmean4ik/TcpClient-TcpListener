using System;
using System.IO;

namespace Sevice
{
    public class ServerEventArgs : EventArgs
    {
        public string Message { get; set; }
        public Stream stream { get; set; }
            
    }
}