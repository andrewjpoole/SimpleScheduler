using AJP.SimpleScheduler.DateTimeProvider;
using AJP.SimpleScheduler.ScheduledTasks;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AJP.SimpleScheduler.ScheduledTaskStorage
{
    public class ElasticBandScheduledTaskRepository : IScheduledTaskRepository
    {
        public IDateTimeProvider DateTimeProvider { get; private set; }
        private readonly ElasticScheduledTaskRepository<ScheduledTask> _scheduledTaskRepository;

        public ElasticBandScheduledTaskRepository(IDateTimeProvider dateTimeProvider, ElasticScheduledTaskRepository<ScheduledTask> scheduledTaskRepository)
        {
            DateTimeProvider = dateTimeProvider;
            _scheduledTaskRepository = scheduledTaskRepository;            
        }

        public void AddScheduledTask(ScheduledTask scheduledTask)
        {
            scheduledTask.Created = DateTimeProvider.UtcNow();
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
            //return _allTasks.Values.Where(task => task.Due < DateTimeProvider.UtcNow()).ToList();
            var response = _scheduledTaskRepository.Query("").Result; // TODO add this functionality into eb
            return response.Data.Where(task => task.Due < DateTimeProvider.UtcNow()).ToList();
        }

        public void UpdateScheduledTask(ScheduledTask scheduledTask)
        {
            _scheduledTaskRepository.Index(scheduledTask.Id, scheduledTask);
        }
    }
}
