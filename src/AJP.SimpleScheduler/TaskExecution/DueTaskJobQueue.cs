using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using AJP.SimpleScheduler.ScheduledTasks;

namespace AJP.SimpleScheduler.TaskExecution
{
    public class DueTaskJobQueue : IDueTaskJobQueue
    {
        private readonly BroadcastBlock<IScheduledTask> _jobs;

        public DueTaskJobQueue()
        {
            _jobs = new BroadcastBlock<IScheduledTask>(job => job);
        }

        public void RegisterHandler<T>(Action<T> handleAction) where T : IScheduledTask
        {
            var executionOptions = new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 2 };
            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };

            Action<IScheduledTask> actionWrapper = (scheduledTask) => handleAction((T)scheduledTask);

            var actionBlock = new ActionBlock<IScheduledTask>((scheduledTask) => actionWrapper(scheduledTask), executionOptions);

            _jobs.LinkTo(actionBlock, linkOptions, predicate: (scheduledTask) => scheduledTask is T);
        }

        public async Task Enqueue(IScheduledTask scheduledJob)
        {
            await _jobs.SendAsync(scheduledJob);
        }

        public void Stop()
        {
            _jobs.Complete();
        }
    }
}
