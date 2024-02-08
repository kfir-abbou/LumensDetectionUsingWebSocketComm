using Comm.Model.LumenDetectionApi;
using System;
 

namespace Comm.Model
{
	public class UpdateImageResponseReceivedEventArgsBytes : EventArgs
	{
		public UpdateNewImageResponseMessageData Response { get; private set; }
		public byte[] FrameBytes { get; private set; }
	
		public UpdateImageResponseReceivedEventArgsBytes(UpdateNewImageResponseMessageData response, byte[] frame)
		{
			Response = response;
			FrameBytes = frame;
		}
	}

	public class UpdateImageResponseReceivedEventArgs : EventArgs
	{
		public UpdateNewImageResponseMessageData Response { get; private set; }
		public string FrameStr { get; private set; }

		public UpdateImageResponseReceivedEventArgs(UpdateNewImageResponseMessageData response, string frame)
		{
			Response = response;
			FrameStr = frame;
		}
	}
}
