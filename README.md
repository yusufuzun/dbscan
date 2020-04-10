# DBSCAN
DBSCAN Clustering Algorithm C# Implementation

It is a small project that implements DBSCAN Clustering algorithm in C# and dotnet-core.

For using this you only need to define your own dataset class and create DbscanAlgorithm class to perform clustering. After that only call the ComputeClusterDbscan with desired clustering parameter.

Example: 
--------
Your dataset items (preference, feature, vector, row, etc.):

    public class MyCustomFeature
    {
        public double X;
        public double Y;
    
        public MyCustomFeature(double x, double y)
        {
            X = x;
            Y = y;
        }
    }

Then for clustering

    MyCustomFeature[] featureData = _clusterRepository.GetPointsToBeClustered();
    
    //INFO: applied euclidean distance as metric calculation function
    var dbscan = new DbscanAlgorithm<MyCustomFeature>(
    (feature1, feature2) =>
    Math.Sqrt(
            ((feature1.X - feature2.X) * (feature1.X - feature2.X)) +
            ((feature1.Y - feature2.Y) * (feature1.Y - feature2.Y))
        )
    );
    
    //returns DbscanResult typed object of algorithm's process
    var result = dbscan.ComputeClusterDbscan(allPoints: featureData, epsilon: .01, minimumPoints: 10);
