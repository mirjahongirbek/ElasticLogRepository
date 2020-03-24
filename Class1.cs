using System;

namespace ElasticLogRepository
{
    public static class LogConfig
    {
        public static string DefaultIndex { get; set; }
        public static string DefaultErrorIndex { get; set; }
        public static string DefaultStatistic { get; set; }
        public static string ConnectionString { get; set; }
        public static bool AddEvent { get; set; }
    }
}

