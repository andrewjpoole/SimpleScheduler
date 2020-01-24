using System;

namespace AJP.SimpleScheduler.DateTimeProvider
{
    public interface IDateTimeProvider 
    {
        DateTime UtcNow();
    }
}
