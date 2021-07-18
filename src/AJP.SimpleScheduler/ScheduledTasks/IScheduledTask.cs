using AJP.SimpleScheduler.Intervals;
using System;

namespace AJP.SimpleScheduler.ScheduledTasks
{
    public interface IScheduledTask
    {
        DateTime Created { get; set; }
        string Id { get; }
        string Type { get; set; }
        DateTime Due { get; set; }
        DateTime StartingAt { get; set; }
        int Repeated { get; set; }
        string JobData { get; set; }
        string JobDataTypeName { get; set; }
        Lapse Interval { get; set; }
        int NumberOfPreviousRuns { get; set; }
        DateTime LastRunTime { get; set; }
    }
}
