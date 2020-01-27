using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AJP.SimpleScheduler.ScheduledTaskStorage;
using AJP.SimpleScheduler.TaskExecution;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AJP.SimpleScheduler.TimerService
{
    public class TimerService : ITimerService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<TimerService> _logger;
        private readonly IScheduledTaskRepository _schedulerState;
        private readonly IDueTaskJobQueue _dueTaskJobQueue;
        private Timer _timer;
        private readonly int _initialDelay = 2000;
        private readonly int _period = 2000;

        public TimerService(IConfiguration config, ILogger<TimerService> logger, IScheduledTaskRepository schedulerState, IDueTaskJobQueue dueTaskJobQueue)
        {
            _config = config;
            _logger = logger;
            _schedulerState = schedulerState;
            _dueTaskJobQueue = dueTaskJobQueue;

            if (_config["timerInitialDelay"] != null)
                _initialDelay = int.Parse(_config["timerInitialDelay"]);

            if (_config["timerPeriod"] != null)
                _period = int.Parse(_config["timerPeriod"]);

            _logger.LogInformation($"TimerService initialised with initial delay of {_initialDelay}ms and period of {_period}ms");
        }
        
        private void TimerTask(object timerState)
        {
            var taskRepo = timerState as IScheduledTaskRepository;

            var dueTasks = taskRepo.DetermineIfAnyTasksAreDue();

            _logger.LogInformation($"{DateTime.Now:HH:mm:ss}: timer elapsed. {dueTasks.Count} due out of {taskRepo.AllTasks().Count} Tasks");
            
            foreach (var dueTask in dueTasks)
            {
                _dueTaskJobQueue.Enqueue(new NormalJob(dueTask.JobData, dueTask));

                // update the task
                dueTask.LastRunTime = taskRepo.DateTimeProvider.UtcNow();
                dueTask.NumberOfPreviousRuns += 1;
                var shouldRunAgain = dueTask.DetermineNextDueTime();
                if (shouldRunAgain)
                    taskRepo.UpdateScheduledTask(dueTask);
                else
                    taskRepo.RemoveScheduledTask(dueTask.Id);
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(
                callback: new TimerCallback(TimerTask),
                state: _schedulerState,
                dueTime: _initialDelay,
                period: _period);

            _logger.LogInformation($"Timer started");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _timer.Dispose();
            _logger.LogInformation($"Timer stopped");
        }
    }
}
