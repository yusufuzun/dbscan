namespace DbscanImplementation
{
    public class MyCustomDatasetItem : DatasetItemBase
    {
        public double X;
        public double Y;

        public MyCustomDatasetItem(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
}