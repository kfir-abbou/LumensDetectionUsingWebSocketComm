

namespace Comm.Model.LumenDetectionApi
{
    public class UpdateNewImageRequestMessageData
    {
	    public long TimeStamp { get; set; }
        public string ImageId { get; set; }
        public uint ImageWidth { get; set; }
        public uint ImageHeight { get; set; }
        public byte[] ImageData { get; set; }

        public UpdateNewImageRequestMessageData()
        {
	        
        }
        public UpdateNewImageRequestMessageData(long timeStamp, string imgId, uint width, uint height, byte[] imageBytes)
        {
	        TimeStamp = timeStamp;
	        ImageId = imgId;
	        ImageWidth = width;
	        ImageHeight = height;
	        ImageData = imageBytes;
        }
    }
}
