using System.Collections.Generic;
using System.Drawing;

namespace Comm.Model
{

	public class VideoFrameWithLumenMessage
	{
		public string Id { get; set; }
		public string FrameAsString { get; set; }
		public IEnumerable<LumensCoordinates> LumenData { get; set; }


		public VideoFrameWithLumenMessage(string id, string frameAsString, IEnumerable<LumensCoordinates> lumenData)
		{
			Id = id;
			FrameAsString = frameAsString;
			LumenData = lumenData;
		}
	}
}
