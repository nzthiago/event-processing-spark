using System;
using System.Diagnostics.Contracts;

namespace EventStreamProcessing.Helpers {
    public static class TimeCalculations {
        public static double GetLatency(DateTime startTime, DateTime endTime)
        {
            if (endTime < startTime)
                throw new ArgumentException("'startTime' must be before 'endTime'");

            return (endTime - startTime).TotalMilliseconds;
        }
    }
}