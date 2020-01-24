using AJP.SimpleScheduler.Intervals;
using System;

namespace SimpleScheduler
{
    public class SimpleSchedule : ISimpleSchedule
    {        
        private IInterval _interval;

        public const string PartSeperator = "|";
        public const string TypeNow = "now";
        public const string TypeAt = "at";
        public const string TypeAfter = "after";
        public const string TypeEvery = "every";

        public string Id { get; set; }
        public string Type { get; set; }
        public DateTime Due { get; set; }
        public int Repeated { get; set; }
        public string JobData { get; set; }

        public int NumberOfPreviousRuns { get; set; }
        public DateTime LastRunTime { get; set; }

        public SimpleSchedule(string id = null)
        {
            if(string.IsNullOrEmpty(id))
                Id = Guid.NewGuid().ToString();
        }

        public ISimpleSchedule Run(string jobData)
        {
            if (string.IsNullOrEmpty(jobData))
            {
                throw new ArgumentException("message", nameof(jobData));
            }

            JobData = jobData;

            return this;
        }

        public ISimpleSchedule Now() 
        {
            Type = TypeNow;
            Due = DateTime.UtcNow; // TODO this needs to be IDateTimeProvider? Factory?
            return this;
        }

        public ISimpleSchedule At(string utcRunAtString)
        {
            var utcRunAt = DateTime.Parse(utcRunAtString);
            return At(utcRunAt);
        }

        public ISimpleSchedule At(DateTime utcRunAt)
        {
            Type = TypeAt;
            Due = utcRunAt;
            return this;
        }

        public ISimpleSchedule After(IInterval interval)
        {
            Type = TypeAfter;
            _interval = interval;
            DetermineNextDueTime();
            return this;
        }

        public ISimpleSchedule Every(IInterval interval)
        {
            Type = TypeEvery;
            _interval = interval;
            DetermineNextDueTime();
            return this;
        }

        public ISimpleSchedule Every(IInterval interval, int repeatTimes)
        {
            if (repeatTimes < 0)
                throw new ArgumentException("repeatTimes must not be negative");

            Repeated = repeatTimes;
            return Every(interval);
        }

        public ISimpleSchedule FromString(string scheduleString) 
        {
            if (string.IsNullOrEmpty(scheduleString))
                throw new ArgumentNullException("scheduleString must not be null or empty");

            var parts = scheduleString.Split(PartSeperator);
            if (parts.Length == 0)
                throw new ArgumentException("scheduleString is not a valid SimpleSchedule string, it should consist of parts separated by the pipe symbol '|'.");

            switch (parts[0]) 
            {
                case TypeNow:
                    return new SimpleSchedule().Now();

                case TypeAt:
                    var runAt = DateTime.Parse(parts[1]);
                    return new SimpleSchedule().At(runAt);

                case TypeAfter:
                    var afterInterval = Interval.Parse(parts[1]);
                    return new SimpleSchedule().After(afterInterval);

                case TypeEvery:
                    var everyInterval = Interval.Parse(parts[1]);
                    var repeatTimes = 0;
                    if(parts.Length > 2)
                        repeatTimes = Interval.ParseRepeatTimes(parts[2]);
                    return new SimpleSchedule().Every(everyInterval, repeatTimes);

                default:
                    throw new ArgumentException("scheduleString is not a valid SimpleSchedule string, it should start with 'now', 'at', 'after' or 'every'.");
            }
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
                    timePart = $"{PartSeperator}{_interval}";
                    break;
                case TypeEvery:
                    repeatPart = Repeated != 0 ? $"{PartSeperator}x{Repeated}" : string.Empty;
                    timePart = $"{PartSeperator}{_interval}";
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
                return NumberOfPreviousRuns > 0;
            }

            if (Type == TypeEvery) 
            {
                Due = AddInterval(DateTime.UtcNow);
                if (Repeated != 0)
                {
                    return NumberOfPreviousRuns <= Repeated;
                }
            }
            return true;
        }

        private DateTime AddInterval(DateTime date) 
        {
            switch (_interval.Unit) 
            {
                case Interval.YearsUnit:
                    return date.AddYears(_interval.Number);

                case Interval.MonthsUnit:
                    return date.AddMonths(_interval.Number);

                case Interval.DaysUnit:
                    return date.AddDays(_interval.Number);

                case Interval.HoursUnit:
                    return date.AddHours(_interval.Number);

                case Interval.MinutesUnit:
                    return date.AddMinutes(_interval.Number);

                case Interval.SecondsUnit:
                    return date.AddSeconds(_interval.Number);

                default:
                    throw new NotSupportedException($"Interval unit of {_interval.Unit} is not supported");

            }
        }
    }    
}
