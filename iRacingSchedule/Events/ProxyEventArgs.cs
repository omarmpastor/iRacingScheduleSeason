using System;

namespace iRacingSchedule.Events
{
    public class ProxyEventArgs : EventArgs
    {
        public bool? ProxyEnabled { get; set; }
        public string ProxyServer { get; set; }
        public bool? ProxyUser { get; set; }
        public string ProxyUsername { get; set; }
        public string ProxyPassword { get; set; }
    }
}
