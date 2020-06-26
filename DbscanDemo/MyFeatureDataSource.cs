using System.Collections.Generic;

namespace DbscanDemo
{
    public class MyFeatureDataSource
    {
        /// <summary>
        /// Builds a feature data that contains two clusters 
        /// according to <paramref name="EuclidienDistance"/> metric and <paramref name="MyFeature"/>
        /// 1- around the coordinates of (1,1) in total of 4001
        /// 2- around the coordinates of (5,5) in total of 4001
        /// Also contains a noise point at coordinate of (10, 10)
        /// </summary>
        /// <returns>List of feature set</returns>
        public List<MyFeature> GetFeatureData()
        {
            var testPoints = new List<MyFeature>() { };

            //points around (1,1) with most 1 distance
            testPoints.Add(new MyFeature(1, 1));
            for (int i = 1; i <= 1000; i++)
            {
                var v = (float)i / 1000;
                testPoints.Add(new MyFeature(1, 1 + v));
                testPoints.Add(new MyFeature(1, 1 - v));
                testPoints.Add(new MyFeature(1 - v, 1));
                testPoints.Add(new MyFeature(1 + v, 1));
            }

            //points around (5,5) with most 1 distance
            testPoints.Add(new MyFeature(5, 5));
            for (int i = 1; i <= 1000; i++)
            {
                var v = (float)i / 1000;
                testPoints.Add(new MyFeature(5, 5 + v));
                testPoints.Add(new MyFeature(5, 5 - v));
                testPoints.Add(new MyFeature(5 - v, 5));
                testPoints.Add(new MyFeature(5 + v, 5));
            }

            //noise point
            testPoints.Add(new MyFeature(10, 10));

            return testPoints;
        }
    }
}
