using NodaTime;

namespace AJP.SimpleScheduler.ScheduledTasks
{
    public class ScheduledTaskBuilderFactory : IScheduledTaskBuilderFactory
    {
        private readonly IClock _clock;

        public ScheduledTaskBuilderFactory(IClock clock)
        {
            _clock = clock;
        }

        public ScheduledTaskBuilder CreateBuilder()
        {
            return new ScheduledTaskBuilder(_clock);
        }
    }
}
