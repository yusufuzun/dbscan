using System.Threading.Tasks;
using DbscanImplementation;
using DbscanImplementation.Eventing;
using DbscanImplementation.ResultBuilding;

namespace DbscanDemo.Eventing
{
    /// <summary>
    /// An example for how to get data from an exchange and build result asynchronously 
    /// </summary>
    /// <typeparam name="TQ">queue item type</typeparam>
    public class QueueExchangeSubscriber<TQ, TF> : IDbscanEventSubscriber<DbscanResult<TF>>
    {
        private readonly QueueExchange<TQ> exchange;

        public QueueExchangeSubscriber(QueueExchange<TQ> exchange)
        {
            this.exchange = exchange;
        }

        public Task<DbscanResult<TF>> Subscribe()
        {
            return Task.Factory.StartNew(() =>
            {
                var resultBuilder = new DbscanResultBuilder<TF>();

                var stopDequeue = false;

                while (!stopDequeue)
                {
                    if (exchange.Queue.Count == 0)
                    {
                        Task.Delay(100).GetAwaiter().GetResult();
                        continue;
                    }

                    if (!exchange.Queue.TryDequeue(out TQ qEvent))
                    {
                        continue;
                    }

                    //you can get extra event information from algorithm here and process as you wish
                    switch (qEvent)
                    {
                        case PointProcessFinished<TF> e:
                            resultBuilder.Process(e.Point);
                            break;
                        case ComputeFinished e:
                            stopDequeue = true;
                            break;
                    }
                    if (stopDequeue)
                    {
                        break;
                    }
                }

                return resultBuilder.Result;
            });
        }
    }
}
