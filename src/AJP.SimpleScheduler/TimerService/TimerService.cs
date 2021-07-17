using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AJP.SimpleScheduler.Intervals;
using AJP.SimpleScheduler.ScheduledTasks;
using AJP.SimpleScheduler.ScheduledTaskStorage;
using AJP.SimpleScheduler.TaskExecution;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NodaTime;
using NodaTime.Extensions;

namespace AJP.SimpleScheduler.TimerService
{
    public class TimerService : ITimerService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<TimerService> _logger;
        private readonly IClock _clock;
        private readonly IScheduledTaskRepository _schedulerState;
        private readonly IDueTaskJobQueue _dueTaskJobQueue;
        private Timer _timer;
        private readonly int _initialDelay = 2000;
        private readonly int _period = 2000;

        public TimerService(IConfiguration config, ILogger<TimerService> logger, IClock clock, IScheduledTaskRepository schedulerState, IDueTaskJobQueue dueTaskJobQueue)
        {
            _config = config;
            _logger = logger;
            _clock = clock;
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
                _logger.LogInformation($"Running Task {dueTask.Id} {dueTask} due @{dueTask.Due}");
                //_dueTaskJobQueue.Enqueue(new ScheduledJob(dueTask.JobData, dueTask));
                _dueTaskJobQueue.Enqueue(dueTask);

                // update the task
                dueTask.LastRunTime = taskRepo.ClockInstance.GetCurrentInstant().ToDateTimeUtc();
                dueTask.NumberOfPreviousRuns += 1;
                var shouldRunAgain = DetermineNextDueTime(dueTask);
                if (shouldRunAgain)
                    taskRepo.UpdateScheduledTask(dueTask);
                else
                    taskRepo.RemoveScheduledTask(dueTask.Id);
            }
        }

        private bool DetermineNextDueTime(ScheduledTask task)
        {
            // Check conditions and signal for task to be removed
            if (task.Type == ScheduledTask.TypeNow && task.NumberOfPreviousRuns > 0)
            {
                return false; // mark for deletion
            }

            if (task.Type == ScheduledTask.TypeAt && task.NumberOfPreviousRuns > 0)
            {
                return false; // mark for deletion
            }

            if (task.Type == ScheduledTask.TypeAfter)
            {
                task.Due = AddInterval(task).ToDateTimeUtc();
                return task.NumberOfPreviousRuns < 1;
            }

            if (task.Type == ScheduledTask.TypeEvery)
            {
                task.Due = AddInterval(task).ToDateTimeUtc();
                if (task.Repeated != 0)
                {
                    return task.NumberOfPreviousRuns < task.Repeated;
                }
            }

            if (task.Type == ScheduledTask.TypeEveryStartingAt)
            {
                task.Due = AddInterval(task).ToDateTimeUtc();
            }

            return true;
        }

        private ZonedDateTime AddInterval(ScheduledTask task)
        {
            var nowUtc = _clock.GetCurrentInstant().InUtc();
            var nowUtcInLondon = nowUtc.LocalDateTime;

            //var london = DateTimeZoneProviders.Tzdb["Europe/London"];
            //var clock = SystemClock.Instance.InZone(london);
            //var londonNow = clock.GetCurrentLocalDateTime();

            return task.Interval.Unit switch
            {
                Lapse.YearsUnit => nowUtcInLondon.PlusYears(task.Interval.Number).InUtc(),
                Lapse.MonthsUnit => nowUtcInLondon.PlusMonths(task.Interval.Number).InUtc(),
                Lapse.DaysUnit => nowUtcInLondon.PlusDays(task.Interval.Number).InUtc(),
                Lapse.HoursUnit => nowUtcInLondon.PlusHours(task.Interval.Number).InUtc(),
                Lapse.MinutesUnit => nowUtcInLondon.PlusMinutes(task.Interval.Number).InUtc(),
                Lapse.SecondsUnit => nowUtcInLondon.PlusSeconds(task.Interval.Number).InUtc(),
                null => nowUtc,
                _ => throw new NotSupportedException($"Interval unit of {task.Interval.Unit} is not supported"),
            };
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
