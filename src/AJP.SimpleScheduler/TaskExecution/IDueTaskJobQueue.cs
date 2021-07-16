using System;
using System.Threading.Tasks;
using AJP.SimpleScheduler.ScheduledTasks;

namespace AJP.SimpleScheduler.TaskExecution
{
    public interface IDueTaskJobQueue
    {
        Task Enqueue(IScheduledTask scheduledTask);
        void RegisterHandler<T>(Action<T> handleAction) where T : IScheduledTask;
        void Stop();
    }
}