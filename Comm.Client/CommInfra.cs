using System;
using System.Drawing;
using System.Text;
using Comm.Model;
using Comm.Model.LumenDetectionApi;
using Newtonsoft.Json;
using SD.Framework.Infrastructure.IPCCommunication;

namespace LumenDetection.Tests.Comm.Client
{
	public class CommInfra : WebsocketInfra
	{
		public CommInfra(string address, string addressPostfix, int port) : base(address, addressPostfix, port)
		{
			this.BinaryMessageReceived += handleBinaryMessage;
		}

		private void handleBinaryMessage(object sender, byte[] e)
		{
			

		}

		public string SendFrameMessage(string id, byte[] frame)
		{
			var data = createBinaryFrameMessage(id, frame);

			_websocketClient.SendBinary(data);

			return id;
		}

		public string SendUpdateImageMessage(string id, uint width, uint height, byte[] frame)
		{
			var ts = DateTime.Now.Ticks;

			var msgData = new UpdateNewImageRequestMessageData(ts, id, width, height, frame);
			var updateImageRequest = new UpdateNewImageMessage(msgData);

			var msgJson = JsonConvert.SerializeObject(updateImageRequest);
			var msgBuffer = Encoding.UTF8.GetBytes(msgJson);

			_websocketClient.SendBinary(msgBuffer);

			return id;
		}

		public void SendUpdateImageMessage<T>(WebSocketMessageRequest<T> msg)
		{
			var msgJson = msg.ToJSON(); // JsonConvert.SerializeObject(msg);
			var msgBuffer = Encoding.UTF8.GetBytes(msgJson);

			_ = _websocketClient.SendBinary(msgBuffer);
		}

		private byte[] createBinaryFrameMessage(string id, byte[] frame)
		{
			string frameBase64 = Convert.ToBase64String(frame);
			var msg = new FrameMessage(id, frameBase64);
			var jsonMessage = JsonConvert.SerializeObject(msg);

			var bytes = Encoding.UTF8.GetBytes(jsonMessage);

			return bytes;
		}
	}
}
