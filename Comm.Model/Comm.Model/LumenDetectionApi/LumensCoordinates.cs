namespace Comm.Model
{
	public class LumensCoordinates
	{
		public double X { get; set; }
		public double Y { get; set; }
		public double Radius { get; set; }

		public LumensCoordinates(double x, double y, double radius)
		{
			X = x;
			Y = y;
			Radius = radius;
		}
	}
}
