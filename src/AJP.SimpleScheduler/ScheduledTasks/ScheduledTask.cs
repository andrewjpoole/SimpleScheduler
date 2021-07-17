using AJP.SimpleScheduler.Intervals;
using System;

namespace AJP.SimpleScheduler.ScheduledTasks
{
    public partial class ScheduledTask : IScheduledTask
    {
        public const string PartSeperator = "|";
        public const string TypeNow = "now";
        public const string TypeAt = "at";
        public const string TypeAfter = "after";
        public const string TypeEvery = "every";
        public const string TypeEveryStartingAt = "everyStartingAt";

        public DateTime Created { get; set; }
        public Lapse Interval { get; set; }
        public string Id { get; set; }
        public string Type { get; set; }
        public DateTime Due { get; set; }
        public DateTime StartingAt { get; set; }
        public int Repeated { get; set; }
        public string JobData { get; set; }

        public int NumberOfPreviousRuns { get; set; }
        public DateTime LastRunTime { get; set; }

        public ScheduledTask()
        {            
        }

        public override string ToString()
        {
            var timePart = string.Empty;
            var secondPart = string.Empty;
            switch (Type)
            {
                case TypeAt:
                    timePart = $"{PartSeperator}{Due:s}";
                    break;
                case TypeAfter:
                    timePart = $"{PartSeperator}{Interval}";
                    break;
                case TypeEvery:
                    secondPart = Repeated != 0 ? $"{PartSeperator}x{Repeated}" : string.Empty;
                    timePart = $"{PartSeperator}{Interval}";
                    break;
                case TypeEveryStartingAt:
                    secondPart = $"{PartSeperator}{StartingAt:s}";
                    timePart = $"{PartSeperator}{Interval}";
                    break;
                default:
                    break;
            }
            return $"{Type}{timePart}{secondPart}";
        }
    }
}
