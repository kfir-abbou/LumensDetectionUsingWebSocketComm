﻿using AForge.Video.Reader;
using Comm.Model;
using Newtonsoft.Json;
using SD.Framework.Infrastructure.IPCCommunication;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Comm.Model.LumenDetectionApi;

namespace Server2
{
	class Program
	{
		private WebSocketServer _webSocketServer;
		private int _clientSocketID = -1;
		private VideoFrameReader _videoFrameReader;
		private bool _sendTaskRunning = false;
		private Random _random;

		static void Main(string[] args)
		{
			Program p = new Program();
			CancellationTokenSource token = new CancellationTokenSource();
			Task ts = p.Run(token);
			Console.ReadKey();
		}


		public async Task Run(CancellationTokenSource token)
		{
			try
			{
				_random = new Random();

				_videoFrameReader = new VideoFrameReader();
				_webSocketServer = new WebSocketServer();
				_webSocketServer.SocketConnected += _webSocketServer_SocketConnected;
				_webSocketServer.TextMessageReceived += _webSocketServer_TextMessageReceived;
				_webSocketServer.BinaryMessageReceived += _webSocketServer_BinaryMessageReceived;
				_webSocketServer.SocketClosed += _webSocketServer_SocketClosed;
				await _webSocketServer.StartServer("localhost", "example2", 8075);
			}
			catch (Exception exception)
			{
				Console.WriteLine(exception);
				throw;
			}
		}

		private void _webSocketServer_SocketConnected(int socketID)
		{
			_clientSocketID = socketID;
		}

	 

		private void _webSocketServer_SocketClosed(int socketID)
		{
			Console.WriteLine($"WebSocket {socketID} closed");
			_clientSocketID = -1;
		}
		
		private void _webSocketServer_BinaryMessageReceived(byte[] buffer, int socketID)
		{
			// Console.WriteLine($"WebSocket BinaryMessageReceived{socketID} closed");
			try
			{
				var jsonText = Encoding.UTF8.GetString(buffer);
				var message = JsonConvert.DeserializeObject<FrameMessage>(jsonText);

				var frameAsBytes = Convert.FromBase64String(message.FrameAsString);
				var frame = _videoFrameReader.ConvertFrameFromBytes(frameAsBytes);

				if (frame != null)
				{
					// TODO: add lumen's data and send response
					var lumenData = simulateAlgoAndGetLumens();
					// var frameAsString = Convert.ToBase64String(frameAsBytes);

					var responseData = new UpdateNewImageResponseMessageData(DateTime.Now.Ticks.ToString(), message.Id, lumenData);
					var response = new UpdateNewImageResponseMessage(responseData);
					// var jsonResponse = JsonConvert.SerializeObject(response);
					var jsonResponse =
						new WebSocketMessageResponse<UpdateNewImageResponseMessage>(response.MessageHeader,
							status: "OK", response).ToJSON();
					var data = Encoding.UTF8.GetBytes(jsonResponse);

					// var delay = _random.Next(10, 20);
					// Thread.Sleep(delay);
					_webSocketServer.SendBinary(data, _clientSocketID);
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}

		private IEnumerable<LumensCoordinates> simulateAlgoAndGetLumens()
		{
			var listOfLumens = new List<LumensCoordinates>();

			for (int i = 0; i < 2; i++)
			{
				var fakeLumen = new LumensCoordinates(x: _random.NextDouble(), y: _random.NextDouble(), radius: _random.NextDouble() * 60.0);
				listOfLumens.Add(fakeLumen);
			}

			return listOfLumens;
		}

		private void _webSocketServer_TextMessageReceived(string msg, int socketID)
		{
			WebSocketMessageRequest wrq = WebSocketMessageRequest.CreateWebSocketMessageRequestFromJSON(msg);
			switch (wrq.requestHeader)
			{
				case "TestRequest":
					WebSocketMessageRequest<SampleData> wrq1 = new WebSocketMessageRequest<SampleData>(msg);
					Console.WriteLine($"Got request {wrq1.requestHeader} with data {wrq1.messageData}");
					WebSocketMessageResponse<string> wrsp = new WebSocketMessageResponse<string>("TestResponse", "Success", "");
					break;
			}
		}
	}
}