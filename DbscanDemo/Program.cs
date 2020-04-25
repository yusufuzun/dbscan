using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using DbscanImplementation;
using DbscanImplementation.Eventing;

namespace DbscanDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            MyCustomFeature[] featureData = { };

            var testPoints = new List<MyCustomFeature>() { };

            //points around (1,1) with most 1 distance
            testPoints.Add(new MyCustomFeature(1, 1));
            for (int i = 1; i <= 1000; i++)
            {
                var v = (float)i / 1000;
                testPoints.Add(new MyCustomFeature(1, 1 + v));
                testPoints.Add(new MyCustomFeature(1, 1 - v));
                testPoints.Add(new MyCustomFeature(1 - v, 1));
                testPoints.Add(new MyCustomFeature(1 + v, 1));
            }

            //points around (5,5) with most 1 distance
            testPoints.Add(new MyCustomFeature(5, 5));
            for (int i = 1; i <= 1000; i++)
            {
                var v = (float)i / 1000;
                testPoints.Add(new MyCustomFeature(5, 5 + v));
                testPoints.Add(new MyCustomFeature(5, 5 - v));
                testPoints.Add(new MyCustomFeature(5 - v, 5));
                testPoints.Add(new MyCustomFeature(5 + v, 5));
            }

            //noise point
            testPoints.Add(new MyCustomFeature(10, 10));

            featureData = testPoints.ToArray();

            //INFO: applied euclidean distance as metric calculation function
            var dbscan = new DbscanAlgorithm<MyCustomFeature>(
                (feature1, feature2) =>
                Math.Sqrt(
                        ((feature1.X - feature2.X) * (feature1.X - feature2.X)) +
                        ((feature1.Y - feature2.Y) * (feature1.Y - feature2.Y))
                    )
                );

            var result = dbscan.ComputeClusterDbscan(allPoints: featureData, epsilon: .01, minimumPoints: 10);

            Console.WriteLine($"Noise: {result.Noise.Length}");

            Console.WriteLine($"# of Clusters: {result.Clusters.Count}");


            //INFO: applied euclidean distance as metric calculation function
            //INFO: second argument of constructor takes an instance implemented with IDbscanEventPublisher interface
            var dbscanWithEventing = new DbscanAlgorithm<MyCustomFeature>(
                (feature1, feature2) =>
                Math.Sqrt(
                        ((feature1.X - feature2.X) * (feature1.X - feature2.X)) +
                        ((feature1.Y - feature2.Y) * (feature1.Y - feature2.Y))
                    ),
                    new DbscanLogger()
                );

            var resultWithEventing = dbscanWithEventing.ComputeClusterDbscan(allPoints: featureData, epsilon: .01, minimumPoints: 10);

            Console.WriteLine($"Noise: {resultWithEventing.Noise.Length}");

            Console.WriteLine($"# of Clusters: {resultWithEventing.Clusters.Count}");
        }
    }

    public class DbscanLogger : IDbscanEventPublisher
    {
        public void Publish(params object[] events)
        {
            foreach (var e in events)
            {
                //INFO: match the events you want to process
                var info = e switch
                {
                    PointTypeAssigned<MyCustomFeature> pta => $"{pta.Point.ClusterId}: {pta.AssignedType}",
                    _ => null
                };

                if (info != null)
                {
                    Console.WriteLine(info);
                }
            }
        }
    }
}
