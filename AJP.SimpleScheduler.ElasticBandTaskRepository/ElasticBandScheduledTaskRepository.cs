using AJP.SimpleScheduler.ScheduledTasks;
using NodaTime;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AJP.SimpleScheduler.ScheduledTaskStorage
{
    public class ElasticBandScheduledTaskRepository : IScheduledTaskRepository
    {
        public IClock Clock { get; private set; }
        private readonly ElasticScheduledTaskRepository<ScheduledTask> _scheduledTaskRepository;

        public ElasticBandScheduledTaskRepository(
            IClock clock,
            ElasticScheduledTaskRepository<ScheduledTask> scheduledTaskRepository)
        {
            Clock = clock;
            _scheduledTaskRepository = scheduledTaskRepository;            
        }

        public void AddScheduledTask(ScheduledTask scheduledTask)
        {
            scheduledTask.Created = Clock.GetCurrentInstant().ToDateTimeUtc();
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
            var response = _scheduledTaskRepository.Query("").Result;
            return response.Data;
        }

        public List<ScheduledTask> DetermineIfAnyTasksAreDue()
        {
            //var response = _scheduledTaskRepository.Query("").Result;
            //return response.Data.Where(task => task.Due < DateTimeProvider.UtcNow()).ToList();
            
            var response = _scheduledTaskRepository.Query($"Due<{Clock.GetCurrentInstant().ToDateTimeUtc():s}").Result; 
            return response.Data;
        }

        public void UpdateScheduledTask(ScheduledTask scheduledTask)
        {
            _scheduledTaskRepository.Index(scheduledTask.Id, scheduledTask);
        }
    }
}
