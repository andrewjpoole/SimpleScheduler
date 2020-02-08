using AJP.SimpleScheduler.Intervals;
using System;

namespace AJP.SimpleScheduler.ScheduledTasks
{
    public interface IScheduledTaskBuilder
    {
        DateTime Created { get; set; }
        DateTime Due { get; set; }
        string Id { get; set; }
        Lapse Interval { get; set; }
        string JobData { get; set; }
        int Repeated { get; set; }
        string Type { get; set; }

        ScheduledTaskBuilder After(Lapse interval);
        ScheduledTaskBuilder At(DateTime utcRunAt);
        ScheduledTaskBuilder At(string utcRunAtString);
        ScheduledTaskBuilder Every(Lapse interval);
        ScheduledTaskBuilder Every(Lapse interval, int repeatTimes);
        ScheduledTaskBuilder Now();
        ScheduledTaskBuilder Run(string jobData);

        ScheduledTask CreateTask();
    }
}