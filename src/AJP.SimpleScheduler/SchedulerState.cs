using AJP.SimpleScheduler.DateTimeProvider;
using SimpleScheduler;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AJP.SimpleScheduler
{
    public class SchedulerState : ISchedulerState
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private Dictionary<string, ISimpleSchedule> _allTasks = new Dictionary<string, ISimpleSchedule>();

        public SchedulerState(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
            
            // load tasks from persistence if configured in DI
        }

        public void AddScheduledTask(ISimpleSchedule simpleSchedule)
        {
            var id = Guid.NewGuid().ToString();
            _allTasks.Add(id, simpleSchedule);
        }

        public void RemoveScheduledTask(string id)
        {
            if (_allTasks.ContainsKey(id))
                _allTasks.Remove(id);
        }

        public List<(string Id, ISimpleSchedule scheduledTask)> AllTasks()
        {
            return _allTasks.Select(t => (t.Key, t.Value)).ToList();
        }

        public List<ISimpleSchedule> DetermineIfAnyTasksAreDue()
        {
            return (_allTasks.Values.Where(task => task.Due < _dateTimeProvider.UtcNow())).ToList();
        }

        
    }
}
