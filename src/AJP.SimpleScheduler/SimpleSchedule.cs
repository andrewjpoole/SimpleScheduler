using AJP.SimpleScheduler.Intervals;
using System;

namespace SimpleScheduler
{
    public class SimpleSchedule : ISimpleSchedule
    {
        private string _type;
        private int _repeatTimes;
        private string _jobData;
        private DateTime _runAt;
        private IInterval _interval;

        public const string PartSeperator = "|";
        private const string _typeNow = "now";
        private const string _typeAt = "at";
        private const string _typeAfter = "after";
        private const string _typeEvery = "every";

        public string Type => _type;

        public DateTime Due => _runAt;

        public int Repeated => _repeatTimes;

        public string JobData => _jobData;

        public ISimpleSchedule Run(string jobData)
        {
            if (string.IsNullOrEmpty(jobData))
            {
                throw new ArgumentException("message", nameof(jobData));
            }

            _jobData = jobData;

            return this;
        }

        public ISimpleSchedule Now() 
        {
            _type = _typeNow;
            _runAt = DateTime.UtcNow; // TODO this needs to be IDateTimeProvider? Factory?
            return this;
        }

        public ISimpleSchedule At(string utcRunAtString)
        {
            var utcRunAt = DateTime.Parse(utcRunAtString);
            return At(utcRunAt);
        }

        public ISimpleSchedule At(DateTime utcRunAt)
        {
            _type = _typeAt;
            _runAt = utcRunAt;
            return this;
        }

        public ISimpleSchedule After(IInterval interval)
        {
            _type = _typeAfter;
            _interval = interval;
            DetermineNextDueTime();
            return this;
        }

        public ISimpleSchedule Every(IInterval interval)
        {
            _type = _typeEvery;
            _interval = interval;
            DetermineNextDueTime();
            return this;
        }

        public ISimpleSchedule Every(IInterval interval, int repeatTimes)
        {
            if (repeatTimes < 0)
                throw new ArgumentException("repeatTimes must not be negative");

            _repeatTimes = repeatTimes;
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
                case _typeNow:
                    return new SimpleSchedule().Now();

                case _typeAt:
                    var runAt = DateTime.Parse(parts[1]);
                    return new SimpleSchedule().At(runAt);

                case _typeAfter:
                    var afterInterval = Interval.Parse(parts[1]);
                    return new SimpleSchedule().After(afterInterval);

                case _typeEvery:
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
            switch (_type)
            {
                case _typeAt:
                    timePart = $"{PartSeperator}{_runAt:s}";
                    break;
                case _typeAfter:
                    timePart = $"{PartSeperator}{_interval}";
                    break;
                case _typeEvery:
                    repeatPart = _repeatTimes != 0 ? $"{PartSeperator}x{_repeatTimes}" : string.Empty;
                    timePart = $"{PartSeperator}{_interval}";
                    break;
                default:
                    break;
            }
            return $"{_type}{timePart}{repeatPart}";
        }

        public void DetermineNextDueTime()
        {
            // TODO if now and already run, signal for task to be removed??

            if (_type == _typeAfter)
            {
                _runAt = AddInterval(DateTime.UtcNow);
            }

            if (_type == _typeEvery) 
            {
                // check last run time and number of run times etc??
                _runAt = AddInterval(DateTime.UtcNow);
            }
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
