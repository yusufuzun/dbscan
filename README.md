# DBSCAN
DBSCAN Clustering Algorithm C# Implementation

It is a small project that I implemented DBSCAN Clustering algorithm in C#.

For using this you only need to define your own dataset class and create DbscanAlgorithm class to perform clustering. After that only call the ComputeClusterDbscan with desired clustering parameter.

Example: 
--------
Your dataset items (preference, feature, vector, row, etc.):

    public class MyCustomDatasetItem : DatasetItemBase
    {
        public double X;
        public double Y;
    
        public MyCustomDatasetItem(double x, double y)
        {
            X = x;
            Y = y;
        }
    }

Then for clustering

    MyCustomDatasetItem[] featureData = _clusterRepository.GetPointsToBeClustered();
    HashSet<MyCustomDatasetItem[]> clusters;
    
    //set the metric function for clustering distance computation.
    var dbscan = new DbscanAlgorithm<MyCustomDatasetItem>((x, y) => Math.Sqrt(((x.X - y.X) * (x.X - y.X)) + ((x.Y - y.Y) * (x.Y - y.Y))));
    
    //loads clusters parameter with set of points
    dbscan.ComputeClusterDbscan(allPoints: featureData, epsilon: .01, minPts: 10, clusters: out clusters);

Related article that I wrote for DBSCAN is here : http://www.yzuzun.com/2015/07/dbscan-clustering-algorithm-and-c-implementation/
