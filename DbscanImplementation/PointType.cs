namespace DbscanImplementation
{
    public enum PointType
    {        
        /// <summary>
        /// This Id shows when a point is IsVisited value equals to false.
        /// </summary>
        Unclassified = -1,

        Noise = 0,
        Core = 1,
        Border = 2
    }
}