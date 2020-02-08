using System;
using System.Threading;
using System.Threading.Tasks;
using AJP.SimpleScheduler.Intervals;
using AJP.SimpleScheduler.ScheduledTasks;
using AJP.SimpleScheduler.ScheduledTaskStorage;
using AJP.SimpleScheduler.TaskExecution;
using Microsoft.Extensions.Hosting;

namespace AJP.SimpleScheduler.TestApp
{
    public class App : IHostedService
    {
        private readonly IScheduledTaskRepository _taskRepository;
        private readonly IScheduledTaskBuilderFactory _taskBuilderFactory;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Need to 'wake up' the normalJobExecuter so it registers as a task handler")]
        public App(IScheduledTaskRepository scheduledTaskRepository, IScheduledTaskBuilderFactory scheduledTaskBuilderFactory, INormalJobExecuter normalJobExecuter)
        {
            _taskRepository = scheduledTaskRepository;
            _taskBuilderFactory = scheduledTaskBuilderFactory;
        }
           
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _taskRepository.AddScheduledTask(_taskBuilderFactory.BuildTask().Run("* run task now!").Now());
            _taskRepository.AddScheduledTask(_taskBuilderFactory.BuildTask().Run("* run task after 10 seconds").After(Lapse.Seconds(10)));
            _taskRepository.AddScheduledTask(_taskBuilderFactory.BuildTask().Run("* run task at a specified DateTime").At(DateTime.Now.AddSeconds(20)));
            _taskRepository.AddScheduledTask(_taskBuilderFactory.BuildTask().Run("* run task every 5 seconds for 3 times").Every(Lapse.Seconds(5), 3));
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
        }
    }
}
