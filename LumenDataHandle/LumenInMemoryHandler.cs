using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Comm.Model;
using Comm.Model.LumenDetectionApi;
using LumenDetection.Tests.Comm.Client;
using SD.Framework.Infrastructure.IPCCommunication;

namespace LumenDetection.Tests.LumenDataHandle
{
	public class LumenInMemoryHandler
	{
		private readonly CommInfra _commInfra;
		private readonly IDictionary<string, UpdateNewImageRequestMessageData> _framesHandled = new ConcurrentDictionary<string, UpdateNewImageRequestMessageData>();

		public event EventHandler<UpdateImageResponseReceivedEventArgs> LumensMessageReceived;
		public event Action InitAlgoResponseReceived;

		public LumenInMemoryHandler(CommInfra commInfra)
		{
			_commInfra = commInfra;
			_commInfra.UpdateImageResponse += onUpdateImageResponse;
			_commInfra.InitAlgoResponse += onInitAlgoResponse;

		}

		private void onInitAlgoResponse(object sender, string status)
		{
			if (status.ToLower() == "ok")
			{
				InitAlgoResponseReceived?.Invoke();
			}
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
 
		public Task HandleVideoFrame(uint width, uint height, byte[] frame)
		{
			try
			{
				var ts = DateTime.Now.Ticks;
				var id = ts.ToString();

				sendUpdateImageRequest(id, width, height, frame);

				if (!_framesHandled.ContainsKey(id))
				{
					_framesHandled.Add(id, new UpdateNewImageRequestMessageData(ts, id, width, height, Convert.ToBase64String(frame)));
				}

			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				return Task.FromException(e);
			}

			return Task.CompletedTask;
		}
 
		private Stopwatch _stopwatch = new Stopwatch();
		private void sendUpdateImageRequest(string id, uint width, uint height, byte[] frame)
		{
			var ts = DateTime.Now.Ticks;

			var msgData = new UpdateNewImageRequestMessageData(ts, ts.ToString(), width, height, Convert.ToBase64String(frame));
			var updateImgReq = new UpdateNewImageMessage(msgData);
			_stopwatch.Restart();
			var req = new WebSocketMessageRequest<UpdateNewImageMessage>(updateImgReq.MessageHeader, updateImgReq).ToJSON();
			var bytes = Encoding.UTF8.GetBytes(req);
			_stopwatch.Stop();
			var t = _stopwatch.Elapsed.TotalMilliseconds;
			// _commInfra.SendTMessage(req);
			_commInfra.SendBytes(bytes);
		}

		public void SendInitAlgoRequest()
		{
			var initMsgData = new InitLumenDetectionMessageData("string.Empty", "string.Empty", "string.Empty", "string.Empty");
			var initRequest = new InitLumenDetectionRequestMessage(initMsgData);
			var req = new WebSocketMessageRequest<InitLumenDetectionRequestMessage>(initRequest.MessageHeader,
				initRequest);

			_commInfra.SendTMessage(req);
		}

	}


	public interface ILumanHandler
	{

	}
}
