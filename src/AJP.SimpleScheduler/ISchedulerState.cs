using SimpleScheduler;
using System.Collections.Generic;

namespace AJP.SimpleScheduler
{
    public interface ISchedulerState
    {
        void AddScheduledTask(ISimpleSchedule simpleSchedule);
        void RemoveScheduledTask(string id);
        void UpdateScheduledTask(ISimpleSchedule dueTask);
        List<(string Id, ISimpleSchedule scheduledTask)> AllTasks();
        List<ISimpleSchedule> DetermineIfAnyTasksAreDue();
    }
}