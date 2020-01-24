﻿using System;
using System.Text;
using System.Threading;
using AJP.SimpleScheduler.TaskExecution;
using Microsoft.Extensions.Configuration;

namespace AJP.SimpleScheduler.TimerService
{
    public class TimerService : ITimerService
    {
        private readonly IConfiguration _config;
        private readonly ISchedulerState _schedulerState;
        private readonly IDueTaskJobQueue _dueTaskJobQueue;
        private Timer _timer;
        private readonly int _initialDelay = 2000;
        private readonly int _period = 2000;

        public TimerService(IConfiguration config, ISchedulerState schedulerState, IDueTaskJobQueue dueTaskJobQueue)
        {
            _config = config;
            _schedulerState = schedulerState;
            _dueTaskJobQueue = dueTaskJobQueue;

            if (_config["timerInitialDelay"] != null)
                _initialDelay = int.Parse(_config["timerInitialDelay"]);

            if (_config["timerPeriod"] != null)
                _initialDelay = int.Parse(_config["timerPeriod"]);
        }

        public void Start()
        {
            _timer = new Timer(
                callback: new TimerCallback(TimerTask),
                state: _schedulerState,
                dueTime: _initialDelay,
                period: _period);
        }

        public void Stop()
        {
            _timer.Dispose();
        }

        private void TimerTask(object timerState)
        {
            var state = timerState as SchedulerState;

            var dueTasks = state.DetermineIfAnyTasksAreDue();
            Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: timer elapsed. {dueTasks.Count} Task(s) due.");

            foreach (var dueTask in dueTasks)
            {
                _dueTaskJobQueue.Enqueue(new NormalJob(dueTask.JobData));

                // update the task
                dueTask.LastRunTime = state.DateTimeProvider.UtcNow();
                dueTask.NumberOfPreviousRuns += 1;
                var shouldRunAgain = dueTask.DetermineNextDueTime();
                if (shouldRunAgain)
                    state.UpdateScheduledTask(dueTask);
                else
                    state.RemoveScheduledTask(dueTask.Id);
            }
        }
    }
}
