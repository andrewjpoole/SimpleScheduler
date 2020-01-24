using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace AJP.SimpleScheduler.TaskExecution
{
    public class DueTaskJobQueue : IDueTaskJobQueue
    {
        private readonly BroadcastBlock<IJob> _jobs;

        public DueTaskJobQueue()
        {
            _jobs = new BroadcastBlock<IJob>(job => job);
        }

        public void RegisterHandler<T>(Action<T> handleAction) where T : IJob
        {
            var executionOptions = new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 2 };
            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };

            Action<IJob> actionWrapper = (job) => handleAction((T)job);

            var actionBlock = new ActionBlock<IJob>((job) => actionWrapper(job), executionOptions);

            _jobs.LinkTo(actionBlock, linkOptions, predicate: (job) => job is T);
        }

        public async Task Enqueue(IJob job)
        {
            await _jobs.SendAsync(job);
        }

        public void Stop()
        {
            _jobs.Complete();
        }
    }
}
