namespace AJP.SimpleScheduler.TaskExecution
{
    public interface IJob
    {

    }

    public class NormalJob : IJob 
    {
        public string JobData { get; }

        public NormalJob(string jobData)
        {
            JobData = jobData;
            // deserialise and execute?
        }
    }
}
