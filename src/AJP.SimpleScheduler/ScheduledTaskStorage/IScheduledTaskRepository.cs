using AJP.SimpleScheduler.ScheduledTasks;
using NodaTime;
using System.Collections.Generic;

namespace AJP.SimpleScheduler.ScheduledTaskStorage
{
    public interface IScheduledTaskRepository
    {
        IClock ClockInstance { get; }
        void AddScheduledTask(ScheduledTask simpleSchedule);
        void AddScheduledTask(ScheduledTaskBuilder taskBuilder);
        void RemoveScheduledTask(string id);
        void UpdateScheduledTask(ScheduledTask dueTask);
        List<ScheduledTask> AllTasks();
        List<ScheduledTask> DetermineIfAnyTasksAreDue();
        ScheduledTask GetById(string id);
    }
}