namespace AJP.SimpleScheduler.ScheduledTasks
{
    public interface IScheduledTaskBuilderFactory
    {
        ScheduledTaskBuilder CreateBuilder();
    }
}