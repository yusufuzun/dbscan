namespace DbscanImplementation
{
    /// <summary>
    /// Algorithm point definition
    /// </summary>
    /// <typeparam name="TFeature">Feature data contribute into algorithm</typeparam>
    public class DbscanPoint<TFeature>
    {
        public TFeature Feature { get; internal set; }

        public bool IsVisited { get; internal set; }

        public int ClusterId { get; internal set; }

        public PointType PointType { get; internal set; }

        public DbscanPoint(TFeature feature)
        {
            Feature = feature;
            IsVisited = false;
            ClusterId = (int)DbscanImplementation.ClusterId.Unclassified;
            PointType = PointType.Unclassified;
        }
    }
}