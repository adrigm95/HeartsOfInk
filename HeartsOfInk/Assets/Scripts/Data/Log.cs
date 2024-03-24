using System;

namespace AnalyticsServer.Logic
{
        public class Log
        {
            public enum LogType { Log, Warning, Error }

            public bool IsActive { get; set; }
            public DateTime CreateTime { get; set; }
            public DateTime UpdateTime { get; set; }
            public string Content { get; set; }
            public LogType Log_Type { get; set; }
        }
}
