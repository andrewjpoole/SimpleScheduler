using System.Collections.Generic;
using AJP.SimpleScheduler.ScheduledTasks;
using AJP.SimpleScheduler.ScheduledTaskStorage;
using NodaTime;

namespace AJP.SimpleScheduler.ElasticBandTaskRepository
{
    public class ElasticBandScheduledTaskRepository : IScheduledTaskRepository
    {
        public IClock ClockInstance { get; private set; }
        private readonly ElasticScheduledTaskRepository<ScheduledTask> _scheduledTaskRepository;

        public ElasticBandScheduledTaskRepository(
            IClock clock,
            ElasticScheduledTaskRepository<ScheduledTask> scheduledTaskRepository)
        {
            ClockInstance = clock;
            _scheduledTaskRepository = scheduledTaskRepository;            
        }

        public ScheduledTask GetById(string id)
        {
            return _scheduledTaskRepository.GetById(id).GetAwaiter().GetResult().Data;
        }

        public void AddScheduledTask(ScheduledTask scheduledTask)
        {
            scheduledTask.Created = ClockInstance.GetCurrentInstant().ToDateTimeUtc();
            _scheduledTaskRepository.Index(scheduledTask.Id, scheduledTask);
        }

        public void AddScheduledTask(ScheduledTaskBuilder taskBuilder)
        {
            var task = taskBuilder.CreateTask();
            _scheduledTaskRepository.Index(task.Id, task);
        }

        public void RemoveScheduledTask(string id)
        {
            _scheduledTaskRepository.Delete(id);
        }

        public List<ScheduledTask> AllTasks()
        {
            var response = _scheduledTaskRepository.Query("").GetAwaiter().GetResult();
            return response.Data;
        }

        public List<ScheduledTask> DetermineIfAnyTasksAreDue()
        {
            var response = _scheduledTaskRepository.Query($"Due<{ClockInstance.GetCurrentInstant().ToDateTimeUtc():s}").GetAwaiter().GetResult(); 
            return response.Data;
        }

        public void UpdateScheduledTask(ScheduledTask scheduledTask)
        {
            _scheduledTaskRepository.Index(scheduledTask.Id, scheduledTask);
        }
    }
}
