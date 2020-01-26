using AJP.SimpleScheduler.DateTimeProvider;
using AJP.SimpleScheduler.ScheduledTasks;
using System.Collections.Generic;

namespace AJP.SimpleScheduler.ScheduledTaskStorage
{
    public interface IScheduledTaskRepository
    {
        IDateTimeProvider DateTimeProvider { get; }
        void AddScheduledTask(ScheduledTask simpleSchedule);
        void AddScheduledTask(ScheduledTaskBuilder taskBuilder);
        void RemoveScheduledTask(string id);
        void UpdateScheduledTask(ScheduledTask dueTask);
        List<ScheduledTask> AllTasks();
        List<ScheduledTask> DetermineIfAnyTasksAreDue();
    }
}