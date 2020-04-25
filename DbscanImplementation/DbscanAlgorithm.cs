using System;
using System.Linq;
using DbscanImplementation.Eventing;

namespace DbscanImplementation
{
    /// <summary>
    /// DBSCAN algorithm implementation type
    /// </summary>
    /// <typeparam name="TFeature">Takes dataset item row (features, preferences, vector)</typeparam>
    public class DbscanAlgorithm<TFeature>
    {
        /// <summary>
        /// distance calculation metric function between two feature
        /// </summary>
        public readonly Func<TFeature, TFeature, double> MetricFunction;

        /// <summary>
        /// Curried Function that checking two feature as neighbor 
        /// </summary>
        public readonly Func<TFeature, double, Func<DbscanPoint<TFeature>, bool>> RegionQueryPredicate;

        private readonly IDbscanEventPublisher publisher;

        /// <summary>
        /// Takes metric function to compute distances between two <see cref="TFeature"/>
        /// </summary>
        /// <param name="metricFunc"></param>
        public DbscanAlgorithm(Func<TFeature, TFeature, double> metricFunc)
        {
            MetricFunction = metricFunc ?? throw new ArgumentNullException(nameof(metricFunc));

            RegionQueryPredicate =
                (mainFeature, epsilon) => relatedPoint => MetricFunction(mainFeature, relatedPoint.Feature) <= epsilon;

            this.publisher = new NullDbscanEventPublisher();
        }

        public DbscanAlgorithm(Func<TFeature, TFeature, double> metricFunc, IDbscanEventPublisher publisher)
            : this(metricFunc)
        {
            this.publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
        }

        /// <summary>
        /// Performs the DBSCAN clustering algorithm.
        /// </summary>
        /// <param name="allPoints">feature set</param>
        /// <param name="epsilon">Desired region ball radius</param>
        /// <param name="minimumPoints">Minimum number of points to be in a region</param>
        /// <returns>Overall result of cluster compute operation</returns>
        public DbscanResult<TFeature> ComputeClusterDbscan(TFeature[] allPoints, double epsilon, int minimumPoints)
        {
            if (epsilon <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(epsilon), "Must be greater than zero");
            }

            if (minimumPoints <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(minimumPoints), "Must be greater than zero");
            }

            var allPointsDbscan = allPoints.Select(x => new DbscanPoint<TFeature>(x)).ToArray();

            int clusterId = 0;

            var computeId = Guid.NewGuid();

            publisher.Publish(new ComputeStarted(computeId));

            for (int i = 0; i < allPointsDbscan.Length; i++)
            {
                var currentPoint = allPointsDbscan[i];

                if (currentPoint.PointType.HasValue)
                {
                    publisher.Publish(
                        new PointAlreadyProcessed<TFeature>(currentPoint), 
                        new PointProcessFinished<TFeature>(currentPoint));

                    continue;
                }

                publisher.Publish(
                    new PointProcessStarted<TFeature>(currentPoint),
                    new RegionQueryStarted<TFeature>(currentPoint, epsilon, minimumPoints));

                var neighborPoints = RegionQuery(allPointsDbscan, currentPoint.Feature, epsilon);

                publisher.Publish(new RegionQueryFinished<TFeature>(currentPoint, neighborPoints));

                if (neighborPoints.Length < minimumPoints)
                {
                    currentPoint.PointType = PointType.Noise;

                    publisher.Publish(
                        new PointTypeAssigned<TFeature>(currentPoint, PointType.Noise),
                        new PointProcessFinished<TFeature>(currentPoint));

                    continue;
                }

                clusterId++;

                currentPoint.ClusterId = clusterId;

                currentPoint.PointType = PointType.Core;

                publisher.Publish(
                    new PointTypeAssigned<TFeature>(currentPoint, PointType.Core),
                    new ClusteringStarted<TFeature>(currentPoint, neighborPoints, clusterId, epsilon, minimumPoints));

                ExpandCluster(allPointsDbscan, neighborPoints, clusterId, epsilon, minimumPoints);

                publisher.Publish(
                    new ClusteringFinished<TFeature>(currentPoint, neighborPoints, clusterId, epsilon, minimumPoints),
                    new PointProcessFinished<TFeature>(currentPoint));
            }

            publisher.Publish(new ComputeFinished(computeId));

            return new DbscanResult<TFeature>(allPointsDbscan);
        }

        /// <summary>
        /// Checks current cluster for expanding it
        /// </summary>
        /// <param name="allPoints">Dataset</param>
        /// <param name="neighborPoints">other points in same region</param>
        /// <param name="clusterId">given clusterId</param>
        /// <param name="epsilon">Desired region ball radius</param>
        /// <param name="minimumPoints">Minimum number of points to be in a region</param>
        private void ExpandCluster(DbscanPoint<TFeature>[] allPoints, DbscanPoint<TFeature>[] neighborPoints,
            int clusterId, double epsilon, int minimumPoints)
        {
            for (int i = 0; i < neighborPoints.Length; i++)
            {
                var currentPoint = neighborPoints[i];

                publisher.Publish(new PointProcessStarted<TFeature>(currentPoint));

                if (currentPoint.PointType == PointType.Noise)
                {
                    currentPoint.ClusterId = clusterId;

                    currentPoint.PointType = PointType.Border;

                    publisher.Publish(new PointTypeAssigned<TFeature>(currentPoint, PointType.Border));

                    continue;
                }

                if (currentPoint.PointType.HasValue)
                {
                    continue;
                }

                currentPoint.ClusterId = clusterId;

                publisher.Publish(new RegionQueryStarted<TFeature>(currentPoint, epsilon, minimumPoints));

                var otherNeighborPoints = RegionQuery(allPoints, currentPoint.Feature, epsilon);

                publisher.Publish(new RegionQueryFinished<TFeature>(currentPoint, otherNeighborPoints));

                if (otherNeighborPoints.Length < minimumPoints)
                {
                    currentPoint.PointType = PointType.Border;

                    publisher.Publish(new PointTypeAssigned<TFeature>(currentPoint, PointType.Border));

                    continue;
                }

                currentPoint.PointType = PointType.Core;

                publisher.Publish(new PointTypeAssigned<TFeature>(currentPoint, PointType.Core));

                neighborPoints = neighborPoints.Union(otherNeighborPoints).ToArray();

                publisher.Publish(new PointProcessFinished<TFeature>(currentPoint));
            }
        }

        /// <summary>
        /// Checks and searchs neighbor points for given point
        /// </summary>
        /// <param name="allPoints">Dbscan points converted from feature set</param>
        /// <param name="mainFeature">Focused feature to be searched neighbors</param>
        /// <param name="epsilon">Desired region ball radius</param>
        /// <returns>Calculated neighbor points</returns>
        public DbscanPoint<TFeature>[] RegionQuery(DbscanPoint<TFeature>[] allPoints, TFeature mainFeature, double epsilon)
        {
            return allPoints.Where(RegionQueryPredicate(mainFeature, epsilon)).ToArray();
        }
    }
}