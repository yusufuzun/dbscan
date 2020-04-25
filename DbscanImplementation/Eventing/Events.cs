
using System;

namespace DbscanImplementation.Eventing
{
    public class ComputeFinished
    {
        public Guid ComputeId { get; }

        public ComputeFinished(Guid computeId)
        {
            ComputeId = computeId;
        }
    }

    public class ComputeStarted
    {
        public Guid ComputeId { get; }

        public ComputeStarted(Guid computeId)
        {
            ComputeId = computeId;
        }
    }

    public class PointProcessFinished<TFeature>
    {
        public DbscanPoint<TFeature> Point { get; }

        public PointProcessFinished(DbscanPoint<TFeature> point)
        {
            Point = point;
        }
    }

    public class ClusteringFinished<TFeature>
    {
        public DbscanPoint<TFeature> Point { get; }

        public DbscanPoint<TFeature>[] NeighborPoints { get; }

        public int ClusterId { get; }

        public double Epsilon { get; }

        public int MinimumPoints { get; }

        public ClusteringFinished(DbscanPoint<TFeature> point, DbscanPoint<TFeature>[] neighborPoints,
            int clusterId, double epsilon, int minimumPoints)
        {
            Point = point;
            NeighborPoints = neighborPoints;
            ClusterId = clusterId;
            Epsilon = epsilon;
            MinimumPoints = minimumPoints;
        }
    }

    public class ClusteringStarted<TFeature>
    {

        public DbscanPoint<TFeature> Point { get; }

        public DbscanPoint<TFeature>[] NeighborPoints { get; }

        public int ClusterId { get; }

        public double Epsilon { get; }

        public int MinimumPoints { get; }

        public ClusteringStarted(DbscanPoint<TFeature> point, DbscanPoint<TFeature>[] neighborPoints,
            int clusterId, double epsilon, int minimumPoints)
        {
            Point = point;
            NeighborPoints = neighborPoints;
            ClusterId = clusterId;
            Epsilon = epsilon;
            MinimumPoints = minimumPoints;
        }
    }

    public class PointTypeAssigned<TFeature>
    {
        public DbscanPoint<TFeature> Point { get; }

        public PointType AssignedType { get; }

        public PointTypeAssigned(DbscanPoint<TFeature> point, PointType assignedType)
        {
            Point = point;
            AssignedType = assignedType;
        }
    }

    public class RegionQueryFinished<TFeature>
    {
        public DbscanPoint<TFeature> Point { get; }

        public DbscanPoint<TFeature>[] NeighborPoints { get; }

        public RegionQueryFinished(DbscanPoint<TFeature> point, DbscanPoint<TFeature>[] neighborPoints)
        {
            Point = point;
            NeighborPoints = neighborPoints;
        }
    }

    public class RegionQueryStarted<TFeature>
    {
        public DbscanPoint<TFeature> Point { get; private set; }

        public double Epsilon { get; }

        public int MinimumPoints { get; }

        public RegionQueryStarted(DbscanPoint<TFeature> point, double epsilon, int minimumPoints)
        {
            Point = point;
            Epsilon = epsilon;
            MinimumPoints = minimumPoints;
        }
    }

    public class PointAlreadyProcessed<TFeature>
    {
        public DbscanPoint<TFeature> Point { get; private set; }

        public PointAlreadyProcessed(DbscanPoint<TFeature> point)
        {
            Point = point;
        }
    }

    public class PointProcessStarted<TFeature>
    {
        public DbscanPoint<TFeature> Point { get; private set; }

        public PointProcessStarted(DbscanPoint<TFeature> point)
        {
            Point = point;
        }
    }
}