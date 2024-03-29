﻿using Comm.Model;
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
		public event Action InitResponseReceived;

		public LumenOnVideoStreamHandler(CommInfra commInfra)
		{
			_commInfra = commInfra;
			_commInfra.UpdateImageResponse += onUpdateImageResponse;
			_commInfra.InitAlgoResponse += onInitAlgoResponse;
		}

		private void onInitAlgoResponse(object sender, string status)
		{
			if (status.ToLower() == "ok")
			{
				InitResponseReceived?.Invoke();
			}
		}

		private void onUpdateImageResponse(object sender, UpdateNewImageResponseMessage response)
		{
			var lumensCoords = response.MessageData.LumensCoordinates;
			var id = response.MessageData.ImageId;
			var deltaInMilli = calculateTimeDiffFromId(id);

			if (lumensCoords.Any()) //deltaInMilli < 1150 &&
			{
				LumensMessageReceived?.Invoke(null, lumensCoords);
			}

		}


		public Task HandleVideoFrame(uint width, uint height, byte[] frame)
		{
			try
			{
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
			var msgData = new UpdateNewImageRequestMessageDataBytes(ts, ts.ToString(), width, height, frame);
			var updateImgReq = new UpdateNewImageMessageBytes(msgData);
			var req = new WebSocketMessageRequest<UpdateNewImageMessageBytes>(updateImgReq.MessageHeader, updateImgReq);
			_commInfra.SendTMessage(req);
		}

		public void SendInitAlgoRequest()
		{
			var initMsgData = new InitLumenDetectionMessageData("string.Empty", "string.Empty", "string.Empty", "string.Empty");
			var initRequest = new InitLumenDetectionRequestMessage(initMsgData);
			var req = new WebSocketMessageRequest<InitLumenDetectionRequestMessage>(initRequest.MessageHeader,
				initRequest);

			_commInfra.SendTMessage(req);
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
