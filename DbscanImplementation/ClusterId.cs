namespace DbscanImplementation
{
    public enum ClusterId
    {
        /// <summary>
        /// This Id shows when a point is IsVisited value equals to false.
        /// </summary>
        Unclassified = -1,

        /// <summary>
        /// This Id shows when a point is not a member of a cluster.
        /// </summary>
        Noise = 0
    }
}