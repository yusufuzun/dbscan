using System;
using System.Collections.Generic;
using System.Linq;

namespace DbscanImplementation
{
    /// <summary>
    /// DBSCAN algorithm class
    /// </summary>
    /// <typeparam name="T">Takes dataset item row (features, preferences, vector) type</typeparam>
    public class DbscanAlgorithm<T> where T : DatasetItemBase
    {
        private readonly Func<T, T, double> _metricFunc;

        /// <summary>
        /// Takes metric function to compute distances between dataset items T
        /// </summary>
        /// <param name="metricFunc"></param>
        public DbscanAlgorithm(Func<T, T, double> metricFunc)
        {
            _metricFunc = metricFunc;
        }

        /// <summary>
        /// Performs the DBSCAN clustering algorithm.
        /// </summary>
        /// <param name="allPoints">Dataset</param>
        /// <param name="epsilon">Desired region ball radius</param>
        /// <param name="minPts">Minimum number of points to be in a region</param>
        /// <param name="clusters">returns sets of clusters, renew the parameter</param>
        public void ComputeClusterDbscan(T[] allPoints, double epsilon, int minPts, out HashSet<T[]> clusters)
        {
            DbscanPoint<T>[] allPointsDbscan = allPoints.Select(x => new DbscanPoint<T>(x)).ToArray();
            int clusterId = 0;
            for (int i = 0; i < allPointsDbscan.Length; i++)
            {
                DbscanPoint<T> p = allPointsDbscan[i];
                if (p.IsVisited)
                    continue;
                p.IsVisited = true;

                DbscanPoint<T>[] neighborPts = null;
                RegionQuery(allPointsDbscan, p.ClusterPoint, epsilon, out neighborPts);
                if (neighborPts.Length < minPts)
                    p.ClusterId = (int)ClusterIds.Noise;
                else
                {
                    clusterId++;
                    ExpandCluster(allPointsDbscan, p, neighborPts, clusterId, epsilon, minPts);
                }
            }
            clusters = new HashSet<T[]>(
                allPointsDbscan
                    .Where(x => x.ClusterId > 0)
                    .GroupBy(x => x.ClusterId)
                    .Select(x => x.Select(y => y.ClusterPoint).ToArray())
                );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="allPoints">Dataset</param>
        /// <param name="point">point to be in a cluster</param>
        /// <param name="neighborPts">other points in same region with point parameter</param>
        /// <param name="clusterId">given clusterId</param>
        /// <param name="epsilon">Desired region ball range</param>
        /// <param name="minPts">Minimum number of points to be in a region</param>
        private void ExpandCluster(DbscanPoint<T>[] allPoints, DbscanPoint<T> point, DbscanPoint<T>[] neighborPts, int clusterId, double epsilon, int minPts)
        {
            point.ClusterId = clusterId;
            for (int i = 0; i < neighborPts.Length; i++)
            {
                DbscanPoint<T> pn = neighborPts[i];
                if (!pn.IsVisited)
                {
                    pn.IsVisited = true;
                    DbscanPoint<T>[] neighborPts2 = null;
                    RegionQuery(allPoints, pn.ClusterPoint, epsilon, out neighborPts2);
                    if (neighborPts2.Length >= minPts)
                    {
                        neighborPts = neighborPts.Union(neighborPts2).ToArray();
                    }
                }
                if (pn.ClusterId == (int)ClusterIds.Unclassified)
                    pn.ClusterId = clusterId;
            }
        }

        /// <summary>
        /// Checks and searchs neighbor points for given point
        /// </summary>
        /// <param name="allPoints">Dataset</param>
        /// <param name="point">centered point to be searched neighbors</param>
        /// <param name="epsilon">radius of center point</param>
        /// <param name="neighborPts">result neighbors</param>
        private void RegionQuery(DbscanPoint<T>[] allPoints, T point, double epsilon, out DbscanPoint<T>[] neighborPts)
        {
            neighborPts = allPoints.Where(x => _metricFunc(point, x.ClusterPoint) <= epsilon).ToArray();
        }
    }
}