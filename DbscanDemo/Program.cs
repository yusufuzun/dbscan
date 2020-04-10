using System;
using System.Collections.Generic;
using DbscanImplementation;

namespace DbscanDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            MyCustomFeature[] featureData = { };

            var testPoints = new List<MyCustomFeature>();

            //points around (1,1) with most 1 distance
            testPoints.Add(new MyCustomFeature(1, 1));
            for (int i = 1; i <= 1000; i++)
            {
                float v = ((float)i / 1000);
                testPoints.Add(new MyCustomFeature(1, 1 + v));
                testPoints.Add(new MyCustomFeature(1, 1 - v));
                testPoints.Add(new MyCustomFeature(1 - v, 1));
                testPoints.Add(new MyCustomFeature(1 + v, 1));
            }

            //points around (5,5) with most 1 distance
            testPoints.Add(new MyCustomFeature(5, 5));
            for (int i = 1; i <= 1000; i++)
            {
                float v = ((float)i / 1000);
                testPoints.Add(new MyCustomFeature(5, 5 + v));
                testPoints.Add(new MyCustomFeature(5, 5 - v));
                testPoints.Add(new MyCustomFeature(5 - v, 5));
                testPoints.Add(new MyCustomFeature(5 + v, 5));
            }

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
        }
    }
}
