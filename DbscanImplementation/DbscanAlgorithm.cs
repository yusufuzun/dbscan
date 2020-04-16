using System;
using System.Linq;

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

        /// <summary>
        /// Takes metric function to compute distances between two <see cref="TFeature"/>
        /// </summary>
        /// <param name="metricFunc"></param>
        public DbscanAlgorithm(Func<TFeature, TFeature, double> metricFunc)
        {
            MetricFunction = metricFunc;

            RegionQueryPredicate =
                (mainFeature, epsilon) => relatedPoint => MetricFunction(mainFeature, relatedPoint.Feature) <= epsilon;
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
            var allPointsDbscan = allPoints.Select(x => new DbscanPoint<TFeature>(x)).ToArray();

            int clusterId = 0;

            for (int i = 0; i < allPointsDbscan.Length; i++)
            {
                var lookupPoint = allPointsDbscan[i];

                if (lookupPoint.PointType.HasValue)
                {
                    continue;
                }

                var neighborPoints = RegionQuery(allPointsDbscan, lookupPoint.Feature, epsilon);

                if (neighborPoints.Length < minimumPoints)
                {
                    lookupPoint.PointType = PointType.Noise;

                    continue;
                }

                clusterId++;

                lookupPoint.ClusterId = clusterId;

                lookupPoint.PointType = PointType.Core;

                ExpandCluster(allPointsDbscan, neighborPoints, clusterId, epsilon, minimumPoints);
            }

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
                var neighborPoint = neighborPoints[i];

                if (neighborPoint.PointType == PointType.Noise)
                {
                    neighborPoint.ClusterId = clusterId;

                    neighborPoint.PointType = PointType.Border;
                }

                if (neighborPoint.PointType.HasValue)
                {
                    continue;
                }

                neighborPoint.ClusterId = clusterId;

                var otherNeighborPoints = RegionQuery(allPoints, neighborPoint.Feature, epsilon);

                if (otherNeighborPoints.Length < minimumPoints)
                {
                    neighborPoint.PointType = PointType.Border;

                    continue;
                }

                neighborPoint.PointType = PointType.Core;

                neighborPoints = neighborPoints.Union(otherNeighborPoints).ToArray();
            }
        }

        /// <summary>
        /// Checks and searchs neighbor points for given point
        /// </summary>
        /// <param name="allPoints">Dbscan points converted from feature set</param>
        /// <param name="mainFeature">Focused feature to be searched neighbors</param>
        /// <param name="epsilon">Desired region ball radius</param>
        /// <returns>Calculated neighbor points</returns>
        private DbscanPoint<TFeature>[] RegionQuery(DbscanPoint<TFeature>[] allPoints, TFeature mainFeature, double epsilon)
        {
            return allPoints.Where(RegionQueryPredicate(mainFeature, epsilon)).ToArray();
        }
    }
}