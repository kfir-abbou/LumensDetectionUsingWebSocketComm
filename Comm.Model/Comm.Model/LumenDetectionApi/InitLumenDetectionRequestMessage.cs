namespace Comm.Model.LumenDetectionApi
{
	public class InitLumenDetectionRequestMessage
	{
		public string MessageHeader { get; set; } = "InitLumenDetectionRequest";
		public InitLumenDetectionMessageData MessageData { get; set; }

		public InitLumenDetectionRequestMessage(InitLumenDetectionMessageData data)
		{
			MessageData = data;
		}
	}
}
