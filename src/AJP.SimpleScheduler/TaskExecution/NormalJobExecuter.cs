using System;
using System.Collections.Generic;
using System.Text;

namespace AJP.SimpleScheduler.TaskExecution
{
    public class NormalJobExecuter: INormalJobExecuter
    {
        private readonly IDueTaskJobQueue _dueTaskJobQueue;

        public NormalJobExecuter(IDueTaskJobQueue dueTaskJobQueue)
        {
            _dueTaskJobQueue = dueTaskJobQueue;

            _dueTaskJobQueue.RegisterHandler<NormalJob>(job => HandleJob(job));
        }

        private void HandleJob(NormalJob job)
        {
            Console.WriteLine(job.JobData);            
        }
    }

    public interface INormalJobExecuter 
    {
    }
}
