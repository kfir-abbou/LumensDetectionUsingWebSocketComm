
namespace Comm.Model.LumenDetectionApi
{
	public class UpdateNewImageResponseMessage
    {
	    public string MessageHeader { get; set; } = "UpdateNewImageResponse";
		public UpdateNewImageResponseMessageData MessageData { get; set; }

		public UpdateNewImageResponseMessage()
		{
			
		}

		public UpdateNewImageResponseMessage(UpdateNewImageResponseMessageData data)
		{
			MessageData = data;
		}
    }


}
