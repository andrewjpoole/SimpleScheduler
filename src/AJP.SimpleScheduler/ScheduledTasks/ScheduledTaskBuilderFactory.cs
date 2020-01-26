using AJP.SimpleScheduler.DateTimeProvider;

namespace AJP.SimpleScheduler.ScheduledTasks
{
    public class ScheduledTaskBuilderFactory : IScheduledTaskBuilderFactory
    {
        private readonly IDateTimeProvider _dateTimeProvider;

        public ScheduledTaskBuilderFactory(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        public ScheduledTaskBuilder BuildTask()
        {
            return new ScheduledTaskBuilder(_dateTimeProvider);
        }
    }
}
