using System;
using System.Collections.Generic;
using System.Threading;
using DbscanDemo.Eventing;
using DbscanImplementation;

namespace DbscanDemo
{
    public class Program
    {
        public static void Main()
        {
            var features = new MyFeatureDataSource().GetFeatureData();

            RunOfflineDbscan(features);

            RunOfflineDbscanWithEventPublisher(features);

            RunOfflineDbscanWithResultBuilderAsync(features);
        }

        /// <summary>
        /// async compute call and publishes all events inside dbscan to an exchange
        /// By using an async subscriber all events processed and a result created.
        /// </summary>
        /// <param name="features"></param>
        private static void RunOfflineDbscanWithResultBuilderAsync(List<MyFeature> features)
        {
            var exchange = new QueueExchange<object>();

            var publisher = new QueueExchangePublisher(exchange);

            var dbscanAsync = new DbscanAlgorithm<MyFeature>(
                    EuclidienDistance,
                    publisher
                );

            var subscriber = new QueueExchangeSubscriber<object, MyFeature>(exchange);

            var subscriptionTask = subscriber.Subscribe();

            var computeDbscanTask = dbscanAsync.ComputeClusterDbscanAsync(allPoints: features.ToArray(),
                epsilon: .01, minimumPoints: 10, CancellationToken.None);

            Console.WriteLine("Async dbscan operation continues...");

            var computeResult = computeDbscanTask.GetAwaiter().GetResult();

            var eventResult = subscriptionTask.GetAwaiter().GetResult();

            Console.WriteLine($"Clusters from Computed: {computeResult.Clusters.Count}");
            Console.WriteLine($"Clusters from Events: {computeResult.Clusters.Count}");
        }

        /// <summary>
        /// uses console logger to process events and writes to console
        /// </summary>
        /// <param name="features"></param>
        private static void RunOfflineDbscanWithEventPublisher(List<MyFeature> features)
        {
            //INFO: second argument of constructor takes an instance implemented with IDbscanEventPublisher interface
            var dbscanWithEventing = new DbscanAlgorithm<MyFeature>(
                    EuclidienDistance,
                    new MyFeatureConsoleLogger()
                );

            var resultWithEventing = dbscanWithEventing.ComputeClusterDbscan(allPoints: features.ToArray(),
                epsilon: .01, minimumPoints: 10);

            Console.WriteLine($"Noise: {resultWithEventing.Noise.Count}");

            Console.WriteLine($"# of Clusters: {resultWithEventing.Clusters.Count}");
        }

        /// <summary>
        /// Most basic usage of Dbscan implementation, with no eventing and async mechanism
        /// </summary>
        /// <param name="features">Features provided</param>
        private static void RunOfflineDbscan(List<MyFeature> features)
        {
            var simpleDbscan = new DbscanAlgorithm<MyFeature>(EuclidienDistance);

            var result = simpleDbscan.ComputeClusterDbscan(allPoints: features.ToArray(),
                epsilon: .01, minimumPoints: 10);

            Console.WriteLine($"Noise: {result.Noise.Count}");

            Console.WriteLine($"# of Clusters: {result.Clusters.Count}");
        }

        /// <summary>
        /// Euclidien distance function
        /// </summary>
        /// <param name="feature1"></param>
        /// <param name="feature2"></param>
        /// <returns></returns>
        private static double EuclidienDistance(MyFeature feature1, MyFeature feature2)
        {
            return Math.Sqrt(
                    ((feature1.X - feature2.X) * (feature1.X - feature2.X)) +
                    ((feature1.Y - feature2.Y) * (feature1.Y - feature2.Y))
                );
        }

    }

}
