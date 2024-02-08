using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Comm.Model;
using Comm.Model.LumenDetectionApi;
using Newtonsoft.Json;
using SD.Framework.Infrastructure.IPCCommunication;

namespace LumenDetection.Tests.Comm.Client
{
	public class CommInfra : WebsocketInfra
	{
		private readonly Queue<byte[]> _messageQueue = new();

		public event EventHandler<UpdateNewImageResponseMessage> UpdateImageResponse;
		public event EventHandler<string> InitAlgoResponse;

		public CommInfra(string address, string addressPostfix, int port) : base(address, addressPostfix, port)
		{
			BinaryMessageReceived += handleBinaryMessage;
			Task.Run(handleMessagesInQueue);
		}

		private async Task handleMessagesInQueue()
		{
			while (true)
			{
				if (_messageQueue.Any())
				{
					var bytes = _messageQueue.Dequeue();
					var header = WebSocketMessageResponse.GetMessageHeader(bytes);

					switch (header)
					{
						case "UpdateNewImageResponse":
						{
							var msg = WebSocketMessageResponse<UpdateNewImageResponseMessage>.FromJson(bytes);
							UpdateImageResponse?.Invoke(null, msg.messageData);
							break;
						}
						case "InitLumenDetectionResponse":
						{
							var msg = WebSocketMessageResponse<InitLumenDetectionResponseMessage>.FromJson(bytes);
							InitAlgoResponse?.Invoke(null, msg.messageData.Status);
							break;
						}
					}
				}
				await Task.Delay(1);
			}
		}

		private void handleBinaryMessage(object sender, byte[] msgBytes)
		{
			
			// add message to queue and handle on different thread
			_messageQueue.Enqueue(msgBytes);
		}

		// public string SendFrameMessage(string id, byte[] frame)
		// {
		// 	var data = createBinaryFrameMessage(id, frame);
		//
		// 	_websocketClient.SendBinary(data);
		//
		// 	return id;
		// }

		// public string SendUpdateImageMessage(string id, uint width, uint height, byte[] frame)
		// {
		// 	var ts = DateTime.Now.Ticks;
		//
		// 	var msgData = new UpdateNewImageRequestMessageData(ts, id, width, height, frame);
		// 	var updateImageRequest = new UpdateNewImageMessage(msgData);
		//
		// 	var msgJson = JsonConvert.SerializeObject(updateImageRequest);
		// 	var msgBuffer = Encoding.UTF8.GetBytes(msgJson);
		//
		// 	_websocketClient.SendBinary(msgBuffer);
		//
		// 	return id;
		// }

		public void SendTMessage<T>(WebSocketMessageRequest<T> msg)
		{
			var msgJson = msg.ToJSON();
			var msgBuffer = Encoding.UTF8.GetBytes(msgJson);

			_ = _websocketClient.SendBinary(msgBuffer);
		}



		// private byte[] createBinaryFrameMessage(string id, byte[] frame)
		// {
		// 	string frameBase64 = Convert.ToBase64String(frame);
		// 	var msg = new FrameMessage(id, frameBase64);
		// 	var jsonMessage = JsonConvert.SerializeObject(msg);
		//
		// 	var bytes = Encoding.UTF8.GetBytes(jsonMessage);
		//
		// 	return bytes;
		// }
	}
}
