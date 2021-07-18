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

        public void RegisterHandlerForAllTasks(Action<IScheduledTask> handleAction)
        {
            var executionOptions = new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 2 };
            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };

            Action<IScheduledTask> actionWrapper = (scheduledTask) => handleAction(scheduledTask);

            var actionBlock = new ActionBlock<IScheduledTask>((scheduledTask) => actionWrapper(scheduledTask), executionOptions);

            _jobs.LinkTo(actionBlock, linkOptions, predicate: (scheduledTask) => true); // executes for all tasks
        }

        public void RegisterHandlerWhen(Action<IScheduledTask> handleAction, Predicate<IScheduledTask> predicate)
        {
            var executionOptions = new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 2 };
            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };

            Action<IScheduledTask> actionWrapper = (scheduledTask) => handleAction(scheduledTask);

            var actionBlock = new ActionBlock<IScheduledTask>((scheduledTask) => actionWrapper(scheduledTask), executionOptions);

            _jobs.LinkTo(actionBlock, linkOptions, predicate: (scheduledTask) => predicate(scheduledTask)); // executes when predicate evaluates to true
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
