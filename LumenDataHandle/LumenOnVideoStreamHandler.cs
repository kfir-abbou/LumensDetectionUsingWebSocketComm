using Comm.Model;
using Comm.Model.LumenDetectionApi;
using LumenDetection.Tests.Comm.Client;
using SD.Framework.Infrastructure.IPCCommunication;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
			_commInfra = commInfra;
			_commInfra.UpdateImageResponse += onUpdateImageResponse;
		}

		private void onUpdateImageResponse(object sender, UpdateNewImageResponseMessage response)
		{
			var lumensCoords = response.MessageData.LumensCoordinates;
			var id = response.MessageData.ImageId;
			var deltaInMilli = calculateTimeDiffFromId(id);

			if ( lumensCoords.Any()) //deltaInMilli < 1150 &&
			{
				LumensMessageReceived?.Invoke(null, lumensCoords);
			}
			_sw.Stop();
			var t = _sw.Elapsed.TotalMilliseconds;
			Console.WriteLine($"Invoke: {t}");
		}

		private Stopwatch _sw  = new Stopwatch();

		public Task HandleVideoFrame(uint width, uint height, byte[] frame)
		{
			try
			{
				_sw.Restart();
				sendUpdateImageRequest(width, height, frame);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				return Task.FromException(e);
			}

			return Task.CompletedTask;
		}

		 
		private void sendUpdateImageRequest(uint width, uint height, byte[] frame)
		{
			var ts = DateTime.Now.Ticks;
			var msgData = new UpdateNewImageRequestMessageData(ts, ts.ToString(), width, height, frame);
			var updateImgReq = new UpdateNewImageMessage(msgData);
			var req = new WebSocketMessageRequest<UpdateNewImageMessage>(updateImgReq.MessageHeader, updateImgReq);
			_commInfra.SendUpdateImageMessage(req);
		}


		private int calculateTimeDiffFromId(string responseId)
		{
			var nowTicks = DateTime.Now.Ticks;
			var ticks = Convert.ToDouble(responseId);
 

			var delta = (int)((nowTicks - ticks) / TICKS_IN_MILI);
			return delta;
		}
	}
}
