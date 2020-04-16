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
        private const int NoiseKey = 0;

        public DbscanResult(DbscanPoint<TFeature>[] allPoints)
        {
            var allClusters = allPoints
                .GroupBy(x => x.ClusterId)
                .ToDictionary(x => x.Key ?? NoiseKey, x => x.ToArray());

            Clusters = new Dictionary<int, DbscanPoint<TFeature>[]>(allClusters.Where(x => x.Key > NoiseKey));
            Noise = allClusters.ContainsKey(NoiseKey) ? allClusters[NoiseKey] : new DbscanPoint<TFeature>[NoiseKey];
        }

        public Dictionary<int, DbscanPoint<TFeature>[]> Clusters { get; }

        public DbscanPoint<TFeature>[] Noise { get; set; }
    }
}