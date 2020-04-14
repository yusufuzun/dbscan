using System.Collections.Generic;
using System.Linq;

namespace DbscanImplementation
{
    /// <summary>
    /// Result object of algorithm after clusters computed
    /// </summary>
    /// <typeparam name="TFeature">Feature data contribute into algorithm</typeparam>
    public class DbscanResult<TFeature>
    {
        public DbscanResult(DbscanPoint<TFeature>[] allPoints)
        {
            var allClusters = allPoints
                .GroupBy(x => x.ClusterId)
                .ToDictionary(x => x.Key, x => x.ToArray());

            Clusters = new Dictionary<int, DbscanPoint<TFeature>[]>(allClusters.Where(x => x.Key > 0));
            Noise = allClusters.ContainsKey((int)ClusterId.Noise) ? allClusters[(int)ClusterId.Noise] : new DbscanPoint<TFeature>[0];
            Unclassified = allClusters.ContainsKey((int)ClusterId.Unclassified) ? allClusters[(int)ClusterId.Unclassified] : new DbscanPoint<TFeature>[0];
        }

        public Dictionary<int, DbscanPoint<TFeature>[]> Clusters { get; }

        public DbscanPoint<TFeature>[] Unclassified { get; }

        public DbscanPoint<TFeature>[] Noise { get; set; }
    }
}