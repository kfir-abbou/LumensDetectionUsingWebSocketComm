using Comm.Model;
using Comm.Model.LumenDetectionApi;
using LumenDetection.Tests.Comm.Client;
using Newtonsoft.Json;
using SD.Framework.Infrastructure.IPCCommunication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LumenDetection.Tests.LumenDataHandle
{
	public class LumenOnVideoStreamHandler
	{

		private readonly CommInfra _commInfra;
		private readonly double TICKS_IN_MILI = 10_000;
		private readonly int MAX_DELAY = 10;

		public event EventHandler<IEnumerable<LumensCoordinates>> LumensMessageReceived;

		public LumenOnVideoStreamHandler(CommInfra commInfra)
		{
			// _commInfra = commInfra;
			// _commInfra.BinaryMessageReceived += onBinaryMessageReceived;
			_commInfra = commInfra;
			_commInfra.UpdateImageResponse += onUpdateImageResponse;
		}

		private void onUpdateImageResponse(object sender, UpdateNewImageResponseMessage response)
		{
			var lumensCoords = response.MessageData.LumensCoordinates;

			if (lumensCoords.Any())
			{
				LumensMessageReceived?.Invoke(null, lumensCoords);
			}
		}

		// private void onBinaryMessageReceived(object sender, byte[] responseMessageFromAlgo)
		// {
		// 	var jsonResponse = Encoding.UTF8.GetString(responseMessageFromAlgo);
		// 	var response = JsonConvert.DeserializeObject<LumenDataMessage>(jsonResponse);
		//
		// 	var timeDiffInMilliseconds = calculateTimeDiffFromId(response.Id);
		//
		// 	if (timeDiffInMilliseconds < MAX_DELAY)
		// 	{
		// 		var data = new LumenDataMessage(response.Id, response.Lumens);
		// 		LumensMessageReceived?.Invoke(null, data);
		// 	}
		//
		// }

		private long _idCounter = 0;
		public Task HandleVideoFrame(uint width, uint height, byte[] frame)
		{
			try
			{
				var id = DateTime.Now.Ticks.ToString();
				// _idCounter++;
				sendUpdateImageRequest(id, width, height, frame);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				return Task.FromException(e);
			}

			return Task.CompletedTask;
		}

		// private void sendFrameMessage(string id, byte[] frame)
		// {
		// 	// _commInfra.SendFrameMessage(id, frame);
		// 	_commInfra.
		// }

		private void sendUpdateImageRequest(string id, uint width, uint height, byte[] frame)
		{
			var ts = DateTime.Now.Ticks;

			var msgData = new UpdateNewImageRequestMessageData(ts, id, width, height, frame);
			var updateImgReq = new UpdateNewImageMessage(msgData);
			var req = new WebSocketMessageRequest<UpdateNewImageMessage>(updateImgReq.MessageHeader, updateImgReq);
			_commInfra.SendUpdateImageMessage(req);
		}


		private int calculateTimeDiffFromId(string responseId)
		{
			// var ticks = Convert.ToDouble(responseId);
			// var nowTicks = DateTime.Now.Ticks;
			

			// return (int)((nowTicks - ticks) / TICKS_IN_MILI);
			var resId = Convert.ToDouble(responseId);

			return (int) (resId - _idCounter);
		}
	}
}
