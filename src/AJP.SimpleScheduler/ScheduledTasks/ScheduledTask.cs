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

        public DateTime Created { get; set; }
        public Interval Interval { get; set; }
        public string Id { get; set; }
        public string Type { get; set; }
        public DateTime Due { get; set; }
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
            var repeatPart = string.Empty;
            switch (Type)
            {
                case TypeAt:
                    timePart = $"{PartSeperator}{Due:s}";
                    break;
                case TypeAfter:
                    timePart = $"{PartSeperator}{Interval}";
                    break;
                case TypeEvery:
                    repeatPart = Repeated != 0 ? $"{PartSeperator}x{Repeated}" : string.Empty;
                    timePart = $"{PartSeperator}{Interval}";
                    break;
                default:
                    break;
            }
            return $"{Type}{timePart}{repeatPart}";
        }

        public bool DetermineNextDueTime()
        {
            // Check conditions and signal for task to be removed
            if (Type == TypeNow && NumberOfPreviousRuns > 0)
            {
                return false; // mark for deletion
            }

            if (Type == TypeAt && NumberOfPreviousRuns > 0)
            {
                return false; // mark for deletion
            }

            if (Type == TypeAfter)
            {
                Due = AddInterval(DateTime.UtcNow);
                return NumberOfPreviousRuns < 1;
            }

            if (Type == TypeEvery)
            {
                Due = AddInterval(DateTime.UtcNow);
                if (Repeated != 0)
                {
                    return NumberOfPreviousRuns < Repeated;
                }
            }
            return true;
        }

        private DateTime AddInterval(DateTime date)
        {
            return Interval.Unit switch
            {
                Interval.YearsUnit => date.AddYears(Interval.Number),
                Interval.MonthsUnit => date.AddMonths(Interval.Number),
                Interval.DaysUnit => date.AddDays(Interval.Number),
                Interval.HoursUnit => date.AddHours(Interval.Number),
                Interval.MinutesUnit => date.AddMinutes(Interval.Number),
                Interval.SecondsUnit => date.AddSeconds(Interval.Number),
                null => date,
                _ => throw new NotSupportedException($"Interval unit of {Interval.Unit} is not supported"),
            };
        }
    }
}
