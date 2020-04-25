namespace DbscanImplementation.Eventing
{
    public interface IDbscanEventPublisher
    {
        /// <summary>
        /// Use for publishing single event
        /// </summary>
        void Publish(object @event) => Publish(new[] { @event });

        /// <summary>
        /// Use for publishing multiple events consecutive
        /// </summary>
        /// <param name="@events">Given events consecutive</param>
        void Publish(params object[] @events);
    }

    public class NullDbscanEventPublisher : IDbscanEventPublisher
    {
        public void Publish(object @event)
        {

        }

        public void Publish(params object[] @events)
        {
            
        }
    }
}