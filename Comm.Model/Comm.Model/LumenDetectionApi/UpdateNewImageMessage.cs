
namespace Comm.Model.LumenDetectionApi
{
    public class UpdateNewImageMessage
    {
	    public string MessageHeader { get; set; } = "UpdateNewImage";

		public UpdateNewImageRequestMessageData MessageData { get; set; }

		public UpdateNewImageMessage()
		{
			
		}

		public UpdateNewImageMessage(UpdateNewImageRequestMessageData data)
		{
			MessageData = data;
		}
	}
}
