using AJP.SimpleScheduler.Intervals;
using System;

namespace AJP.SimpleScheduler.ScheduledTasks
{
    public interface IScheduledTask
    {
        DateTime Created { get; set; }
        string Id { get; }
        string Type { get; }
        DateTime Due { get; }
        int Repeated { get; }
        string JobData { get; }
        Lapse Interval { get; set; }

        int NumberOfPreviousRuns { get; set; }
        DateTime LastRunTime { get; set; }
        
        bool DetermineNextDueTime();
    }
}
