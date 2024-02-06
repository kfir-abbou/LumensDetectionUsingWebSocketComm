using System.Collections.Generic;

namespace Comm.Model.LumenDetectionApi
{
	public class UpdateNewImageResponseMessageData
    {
	    public string TimeStamp { get; set; }
		public string ImageId { get; set; }
        public IEnumerable<LumensCoordinates> LumensCoordinates { get; set; }

        public UpdateNewImageResponseMessageData()
        {
	        
        }

        public UpdateNewImageResponseMessageData(string ts, string imgId, IEnumerable<LumensCoordinates> lumensCoordinates)
        {
            TimeStamp = ts;
	        ImageId = imgId;
	        LumensCoordinates = lumensCoordinates;
        }
    }
}
