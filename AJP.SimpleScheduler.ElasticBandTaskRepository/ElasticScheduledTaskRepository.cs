using AJP.ElasticBand;

namespace AJP.SimpleScheduler.ElasticBandTaskRepository
{
    public class ElasticScheduledTaskRepository<T> : ElasticRepository<T>
    {
        public ElasticScheduledTaskRepository(IElasticBand elasticBand) : base("scheduledtasks", elasticBand)
        {
        }
    }
}
