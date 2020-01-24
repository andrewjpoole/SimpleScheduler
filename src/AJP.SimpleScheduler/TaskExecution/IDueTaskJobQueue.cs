using System;
using System.Threading.Tasks;

namespace AJP.SimpleScheduler.TaskExecution
{
    public interface IDueTaskJobQueue
    {
        Task Enqueue(IJob job);
        void RegisterHandler<T>(Action<T> handleAction) where T : IJob;
        void Stop();
    }
}