using System;
using DbscanImplementation.Eventing;

namespace DbscanDemo.Eventing
{
    public class MyFeatureConsoleLogger : IDbscanEventPublisher
    {
        public void Publish<TObj>(TObj e)
        {
            //INFO: match the events you want to process
            var info = e switch
            {
                PointTypeAssigned<MyFeature> pta => $"{pta.Point.ClusterId}: {pta.AssignedType}",
                _ => null
            };

            if (info != null)
            {
                Console.WriteLine(info);
            }
        }
    }
}
