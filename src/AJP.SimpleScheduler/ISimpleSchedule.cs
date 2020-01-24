using AJP.SimpleScheduler.Intervals;
using System;

namespace SimpleScheduler
{

    public interface ISimpleSchedule 
    {
        string Type { get; }
        DateTime Due { get; }
        int Repeated { get; }
        string JobData { get; }

        ISimpleSchedule Run(string jobData);
        ISimpleSchedule Now();
        ISimpleSchedule At(DateTime utcRunAt);
        ISimpleSchedule At(string utcRunAtString);
        ISimpleSchedule After(IInterval interval);
        ISimpleSchedule Every(IInterval interval);
        ISimpleSchedule Every(IInterval interval, int times);

        ISimpleSchedule FromString(string scheduleString);
        void DetermineNextDueTime();
    }    
}
