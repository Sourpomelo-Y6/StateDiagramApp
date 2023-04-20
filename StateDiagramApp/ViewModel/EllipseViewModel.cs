namespace StateDiagramApp.ViewModel
{
    public class EllipseViewModel
    {
        public double Left { get; set; }
        public double Top { get; set; }

        public EllipseViewModel(double x, double y)
        {
            Left = x;
            Top = y;
        }
    }


}
