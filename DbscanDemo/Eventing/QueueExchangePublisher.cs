using DbscanImplementation.Eventing;

namespace DbscanDemo.Eventing
{
    /// <summary>
    /// An example publisher of QueueExchange
    /// </summary>
    public class QueueExchangePublisher : IDbscanEventPublisher
    {
        private readonly QueueExchange<object> exchange;

        public QueueExchangePublisher(QueueExchange<object> exchange)
        {
            this.exchange = exchange;
        }

        public void Publish<TObj>(TObj @event)
        {
            exchange.Queue.Enqueue(@event);
        }
    }
}
