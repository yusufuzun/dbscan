using System;
using System.Collections.Generic;

namespace DbscanImplementation
{
    class Program
    {
        static void Main(string[] args)
        {
            MyCustomDatasetItem[] featureData = { };

            List<MyCustomDatasetItem> testPoints = new List<MyCustomDatasetItem>();
            for (int i = 0; i < 1000; i++)
            {
                //points around (1,1) with most 1 distance
                testPoints.Add(new MyCustomDatasetItem(1, 1 + ((float)i / 1000)));
                testPoints.Add(new MyCustomDatasetItem(1, 1 - ((float)i / 1000)));
                testPoints.Add(new MyCustomDatasetItem(1 - ((float)i / 1000), 1));
                testPoints.Add(new MyCustomDatasetItem(1 + ((float)i / 1000), 1));

                //points around (5,5) with most 1 distance
                testPoints.Add(new MyCustomDatasetItem(5, 5 + ((float)i / 1000)));
                testPoints.Add(new MyCustomDatasetItem(5, 5 - ((float)i / 1000)));
                testPoints.Add(new MyCustomDatasetItem(5 - ((float)i / 1000), 5));
                testPoints.Add(new MyCustomDatasetItem(5 + ((float)i / 1000), 5));
            }
            featureData = testPoints.ToArray();
            HashSet<MyCustomDatasetItem[]> clusters;

            var dbs = new DbscanAlgorithm<MyCustomDatasetItem>((x, y) => Math.Sqrt(((x.X - y.X) * (x.X - y.X)) + ((x.Y - y.Y) * (x.Y - y.Y))));
            dbs.ComputeClusterDbscan(allPoints: featureData, epsilon: .01, minPts: 10, clusters: out clusters);
        }
    }
}
