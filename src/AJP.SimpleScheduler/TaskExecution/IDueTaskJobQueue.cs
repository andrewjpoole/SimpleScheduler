using System;
using System.Threading.Tasks;
using AJP.SimpleScheduler.ScheduledTasks;

namespace AJP.SimpleScheduler.TaskExecution
{
    public interface IDueTaskJobQueue
    {
        Task Enqueue(IScheduledTask scheduledTask);
        void RegisterHandlerForAllTasks(Action<IScheduledTask> handleAction);
        void RegisterHandlerWhen(Action<IScheduledTask> handleAction, Predicate<IScheduledTask> predicate);
        void Stop();
    }
}