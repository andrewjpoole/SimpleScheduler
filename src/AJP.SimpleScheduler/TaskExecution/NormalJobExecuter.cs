using AJP.SimpleScheduler.ScheduledTasks;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace AJP.SimpleScheduler.TaskExecution
{
    public class NormalJobExecuter: INormalJobExecuter // TODO move this class into the test app?
    {
        private readonly ILogger<NormalJobExecuter> _logger;
        private readonly IDueTaskJobQueue _dueTaskJobQueue;

        public NormalJobExecuter(ILogger<NormalJobExecuter> logger, IDueTaskJobQueue dueTaskJobQueue)
        {
            _logger = logger;
            _dueTaskJobQueue = dueTaskJobQueue;

            _dueTaskJobQueue.RegisterHandler<NormalJob>(job => HandleJob(job)); // TODO maybe move this to app.cs?
        }

        private void HandleJob(NormalJob job)
        {
            _logger.LogInformation($"{job.JobData} {job.Task.Id}");
        }
    }

    public interface INormalJobExecuter 
    {
    }
}
