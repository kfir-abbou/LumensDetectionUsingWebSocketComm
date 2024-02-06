using Comm.Model.LumenDetectionApi;
using System;
 

namespace Comm.Model
{
	public class UpdateImageResponseReceivedEventArgs : EventArgs
	{
		public UpdateNewImageResponseMessageData Response { get; private set; }
		public byte[] FrameBytes { get; private set; }

		public UpdateImageResponseReceivedEventArgs(UpdateNewImageResponseMessageData response, byte[] frame)
		{
			Response = response;
			FrameBytes = frame;
		}
	}
}
