using System.Drawing;

namespace Comm.Model
{
	public class FrameMessage
	{
		public string Id { get; set; }
		public string FrameAsString { get; set; }

		public FrameMessage(string id, string frameAsString)
		{
			Id = id;
			FrameAsString = frameAsString;
		}
	}
}
