using AJP.SimpleScheduler.ScheduledTasks;
using NodaTime;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NodaTime.Extensions;

namespace AJP.SimpleScheduler.ScheduledTaskStorage
{
    public class InMemoryScheduledTaskRepository : IScheduledTaskRepository
    {
        private readonly ILogger<InMemoryScheduledTaskRepository> _logger;
        public IClock ClockInstance { get; private set; }
        private readonly Dictionary<string, ScheduledTask> _allTasks = new Dictionary<string, ScheduledTask>();

        public InMemoryScheduledTaskRepository(IClock clockInstance, ILogger<InMemoryScheduledTaskRepository> logger)
        {
            _logger = logger;
            ClockInstance = clockInstance;
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
            var nowUtc = ClockInstance.GetCurrentInstant().ToDateTimeUtc();
            return _allTasks.Values.Where(task => task.Due < nowUtc).ToList();
        }

        public void UpdateScheduledTask(ScheduledTask dueTask)
        {
            _allTasks[dueTask.Id] = dueTask;
        }

        public ScheduledTask GetById(string id)
        {
            return _allTasks.ContainsKey(id) ? _allTasks[id] : null;
        }
    }
}
