using AJP.SimpleScheduler.ScheduledTasks;

namespace AJP.SimpleScheduler.TaskExecution
{
    public class NormalJob : IJob 
    {
        public readonly IScheduledTask Task;

        public string JobData { get; }

        public NormalJob(string jobData, IScheduledTask task)
        {
            JobData = jobData;
            Task = task;
        }
    }
}
