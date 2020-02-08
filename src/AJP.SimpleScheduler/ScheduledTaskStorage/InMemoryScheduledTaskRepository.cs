using AJP.SimpleScheduler.ScheduledTasks;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AJP.SimpleScheduler.ScheduledTaskStorage
{
    public class InMemoryScheduledTaskRepository : IScheduledTaskRepository
    {
        public IClock Clock { get; private set; }
        private readonly Dictionary<string, ScheduledTask> _allTasks = new Dictionary<string, ScheduledTask>();

        public InMemoryScheduledTaskRepository(IClock clock)
        {
            Clock = clock;
        }

        public void AddScheduledTask(ScheduledTask task)
        {
            _allTasks.Add(task.Id, task);
        }

        public void AddScheduledTask(ScheduledTaskBuilder taskBuilder)
        {
            var task = taskBuilder.CreateTask();
            _allTasks.Add(task.Id, task);
        }

        public void RemoveScheduledTask(string id)
        {
            if (_allTasks.ContainsKey(id))
                _allTasks.Remove(id);
        }

        public List<ScheduledTask> AllTasks()
        {
            return _allTasks.Values.ToList();
        }

        public List<ScheduledTask> DetermineIfAnyTasksAreDue()
        {
            return _allTasks.Values.Where(task => task.Due < Clock.GetCurrentInstant().ToDateTimeUtc()).ToList();
        }

        public void UpdateScheduledTask(ScheduledTask dueTask)
        {
            _allTasks[dueTask.Id] = dueTask;
        }
    }
}
