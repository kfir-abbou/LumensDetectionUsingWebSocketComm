using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using Comm.Model;
using Comm.Model.LumenDetectionApi;
using LumenDetection.Tests.Comm.Client;
using Newtonsoft.Json;
using SD.Framework.Infrastructure.IPCCommunication;

namespace LumenDetection.Tests.LumenDataHandle
{
	public class LumenInMemoryHandler
	{
		// private readonly Queue<FrameMessage> _framesHandled = new();
		// private object _locker = new();
		private readonly CommInfra _commInfra;
		private readonly IDictionary<string, UpdateNewImageRequestMessageData> _framesHandled = new ConcurrentDictionary<string, UpdateNewImageRequestMessageData>();
		// private object _locker;

		public event EventHandler<UpdateImageResponseReceivedEventArgs> LumensMessageReceived;

		public LumenInMemoryHandler(CommInfra commInfra)
		{
			_commInfra = commInfra;
			_commInfra.UpdateImageResponse += onUpdateImageResponse;
			// _commInfra.BinaryMessageReceived += onBinaryMessageReceived;
			// _commInfra.ClientSocketClosed += onClientSocketClosed;
		}

		private void onUpdateImageResponse(object sender, UpdateNewImageResponseMessage response)	
		{
			if (_framesHandled.TryGetValue(response.MessageData.ImageId, out var updateImgRequest))
			{
				if (updateImgRequest.ImageId == response.MessageData.ImageId)
				{
					LumensMessageReceived?.Invoke(null, new UpdateImageResponseReceivedEventArgs(response.MessageData, updateImgRequest.ImageData));

					_framesHandled.Remove(response.MessageData.ImageId);
				}
			}
		}

		private void onClientSocketClosed(object sender, int socketId)
		{

		}

		// private void onBinaryMessageReceived(object sender, byte[] responseMessageFromAlgo)
		// {
		// 	try
		// 	{
		// 		var response =
		// 			WebSocketMessageResponse<UpdateNewImageResponseMessage>.FromJson(responseMessageFromAlgo);
		//
		// 		if (_framesHandled.TryGetValue(response.messageData.MessageData.ImageId, out var updateImgRequest))
		// 		{
		// 			if (updateImgRequest.ImageId == response.messageData.MessageData.ImageId)
		// 			{
		// 				LumensMessageReceived?.Invoke(null, new UpdateImageResponseReceivedEventArgs(response.messageData.MessageData, updateImgRequest.ImageData));
		//
		// 				_framesHandled.Remove(response.messageData.MessageData.ImageId);
		//
		//
		// 			}
		// 		}
		// 	}
		// 	catch (Exception e)
		// 	{
		// 		Console.WriteLine(e);
		// 		throw;
		// 	}
		// }

		public Task HandleVideoFrame(uint width, uint height, byte[] frame)
		{
			try
			{
				var ts = DateTime.Now.Ticks;
				var id = ts.ToString();

				sendRequest(id, width, height, frame);

				var frameAsString = Convert.ToBase64String(frame);

				if (!_framesHandled.ContainsKey(id))
				{
					_framesHandled.Add(id, new UpdateNewImageRequestMessageData(ts, id, width, height, frame));
				}

			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				return Task.FromException(e);
			}

			return Task.CompletedTask;
		}

		public byte[] TryGetFrame(string id)
		{

			var frameExists = _framesHandled.TryGetValue(id, out var frameMsg);
			if (frameExists)
			{

				return frameMsg.ImageData;
			}

			return null;
		}

		private void sendFrameMessage(string id, uint width, uint height, byte[] frame)
		{
			_commInfra.SendUpdateImageMessage(id, width, height, frame);
		}
		private void sendRequest(string id, uint width, uint height, byte[] frame)
		{
			var ts = DateTime.Now.Ticks;

			var msgData = new UpdateNewImageRequestMessageData(ts, id, width, height, frame);
			var updateImgReq = new UpdateNewImageMessage(msgData);
			var req = new WebSocketMessageRequest<UpdateNewImageMessage>(updateImgReq.MessageHeader, updateImgReq);
			_commInfra.SendUpdateImageMessage(req);
		}
	}


	public interface ILumanHandler
	{

	}
}
