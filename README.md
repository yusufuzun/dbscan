# DBSCAN
DBSCAN Clustering Algorithm C# Implementation

It is a small project that implements DBSCAN Clustering algorithm in C# and dotnet-core.

For using this you only need to define your own dataset class and create DbscanAlgorithm class to perform clustering. After that only call the ComputeClusterDbscan with desired clustering parameter.

You can check previous git tags for more primitive DBSCAN implementation.

Example: 
--------
Your dataset items (preference, feature, vector, row, etc.):
```cs
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
```

Your Distance Function
```cs
private static double EuclidienDistance(MyFeature feature1, MyFeature feature2)
{
    return Math.Sqrt(
            ((feature1.X - feature2.X) * (feature1.X - feature2.X)) +
            ((feature1.Y - feature2.Y) * (feature1.Y - feature2.Y))
        );
}
```

Then for clustering with simple form
```cs
var features = new MyFeatureDataSource().GetFeatureData();

//INFO: applied euclidean distance as metric calculation function
var simpleDbscan = new DbscanAlgorithm<MyFeature>(EuclidienDistance);

//returns DbscanResult typed object of algorithm's process
var result = dbscan.ComputeClusterDbscan(allPoints: features.ToArray(), epsilon: .01, minimumPoints: 10);
```

If you want to get events happening inside algorithm then you can create algorithm with other constructor which takes a publisher type as instance
```cs    
//INFO: second argument of constructor takes an instance implemented with IDbscanEventPublisher interface
var dbscanWithEventing = new DbscanAlgorithm<MyFeature>(
        EuclidienDistance,
        new MyFeatureConsoleLogger()
    );

var resultWithEventing = dbscanWithEventing.ComputeClusterDbscan(allPoints: features.ToArray(), epsilon: .01, minimumPoints: 10);
```

An example of the implementation for IDbscanEventPublisher interface:
```cs
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
```

Another example of IDbscanEventPublisher could be a Pub/Sub application, like this:
```cs
var exchange = new QueueExchange<object>();

var publisher = new QueueExchangePublisher(exchange);

var dbscanAsync = new DbscanAlgorithm<MyFeature>(
    EuclidienDistance,
    publisher
);

var subscriber = new QueueExchangeSubscriber<object, MyFeature>(exchange);

var subscriptionTask = subscriber.Subscribe();
```
This can be anything that succeeds the Pub/Sub design. You can asyncronously build your own analytics result by subscription.
