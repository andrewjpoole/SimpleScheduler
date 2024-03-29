﻿using AJP.SimpleScheduler.Intervals;
using NodaTime;
using System;
using System.Text.Json;
using NodaTime.Extensions;

namespace AJP.SimpleScheduler.ScheduledTasks
{
    public class ScheduledTaskBuilder : IScheduledTaskBuilder
    {
        private readonly IClock _clock;

        public string Id { get; set; }
        public DateTime Created { get; set; }
        public Lapse Interval { get; set; }
        public string JobData { get; set; }
        public string JobDataTypeName { get; set; }
        public string Type { get; set; }
        public DateTime Due { get; set; }
        public DateTime StartingAt { get; set; }
        public int Repeated { get; set; }

        public ScheduledTaskBuilder(IClock clock)
        {
            _clock = clock;
            
            Created = _clock.GetCurrentInstant().ToDateTimeUtc();
        }

        public ScheduledTaskBuilder WithJobData(string jobData)
        {
            if (string.IsNullOrEmpty(jobData))
            {
                throw new ArgumentException("message", nameof(jobData));
            }

            JobData = jobData;
            JobDataTypeName = nameof(String);

            return this;
        }

        public ScheduledTaskBuilder WithJobData<T>(T jobData)
        {
            var jobDataJson = JsonSerializer.Serialize(jobData);

            JobData = jobDataJson;
            JobDataTypeName = typeof(T).Name;

            return this;
        }

        public ScheduledTaskBuilder Now()
        {
            Type = ScheduledTask.TypeNow;
            Due = _clock.GetCurrentInstant().ToDateTimeUtc();
            return this;
        }

        public ScheduledTaskBuilder At(string utcRunAtString)
        {
            var utcRunAt = DateTime.Parse(utcRunAtString);
            return At(utcRunAt);
        }

        public ScheduledTaskBuilder At(DateTime utcRunAt)
        {
            Type = ScheduledTask.TypeAt;
            Due = utcRunAt;
            return this;
        }

        public ScheduledTaskBuilder After(Lapse interval)
        {
            Type = ScheduledTask.TypeAfter;
            Interval = interval;
            return this;
        }

        public ScheduledTaskBuilder Every(Lapse interval)
        {
            Type = ScheduledTask.TypeEvery;
            Interval = interval;
            return this;
        }

        public ScheduledTaskBuilder Every(Lapse interval, int repeatTimes)
        {
            if (repeatTimes < 0)
                throw new ArgumentException("repeatTimes must not be negative");

            Repeated = repeatTimes;
            return Every(interval);
        }

        public ScheduledTaskBuilder EveryStartingAt(Lapse interval, string utcStartingAt)
        {
            var utcStartingAtDateTime = DateTime.Parse(utcStartingAt);

            return EveryStartingAt(interval, utcStartingAtDateTime);
        }

        public ScheduledTaskBuilder EveryStartingAt(Lapse interval, DateTime utcStartingAt)
        {
            Type = ScheduledTask.TypeEveryStartingAt;
            Interval = interval;
            StartingAt = utcStartingAt;
            Due = utcStartingAt;
            return this;
        }

        public ScheduledTask FromString(string scheduleString, object jobData)
        {
            if (string.IsNullOrEmpty(scheduleString))
                throw new ArgumentNullException("scheduleString must not be null or empty");

            var parts = scheduleString.Split(ScheduledTask.PartSeperator);
            if (parts.Length == 0)
                throw new ArgumentException("scheduleString is not a valid SimpleSchedule string, it should consist of parts separated by the pipe symbol '|'.");

            switch (parts[0])
            {
                case ScheduledTask.TypeNow:
                    return Now().WithJobData(jobData).CreateTask();

                case ScheduledTask.TypeAt:
                    var runAt = DateTime.Parse(parts[1]);
                    return At(runAt).WithJobData(jobData).CreateTask();

                case ScheduledTask.TypeAfter:
                    var afterInterval = Lapse.Parse(parts[1]);
                    return After(afterInterval).WithJobData(jobData).CreateTask();

                case ScheduledTask.TypeEvery:
                    var everyInterval = Lapse.Parse(parts[1]);
                    var repeatTimes = 0;
                    if (parts.Length > 2)
                        repeatTimes = Lapse.ParseRepeatTimes(parts[2]);
                    return Every(everyInterval, repeatTimes).WithJobData(jobData).CreateTask();

                case ScheduledTask.TypeEveryStartingAt:
                    var everyStartingAtInterval = Lapse.Parse(parts[1]);
                    
                    var startingAtPart = parts[2];

                    return EveryStartingAt(everyStartingAtInterval, startingAtPart).WithJobData(jobData).CreateTask();

                default:
                    throw new ArgumentException("scheduleString is not a valid SimpleSchedule string, it should start with 'now', 'at', 'after' or 'every'.");
            }
        }

        public ScheduledTask CreateTask()
        {
            if (string.IsNullOrEmpty(JobData))
                throw new ApplicationException("Task must have JobData");

            return new()
            {
                Created = Created,
                JobData = JobData,
                JobDataTypeName = JobDataTypeName,
                Type = Type,
                Due = Due,
                StartingAt = StartingAt,
                Repeated = Repeated,
                Interval = Interval
            };
        }
    }
}
