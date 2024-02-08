using AForge.Video.Reader;
using Comm.Model;
using SD.Framework.Infrastructure.IPCCommunication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Comm.Model.LumenDetectionApi;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Server2
{
	class Program
	{
		private WebSocketServer _webSocketServer;
		private int _clientSocketID = -1;
		private VideoFrameReader _videoFrameReader;
		private bool _sendTaskRunning = false;
		private Random _random;
		private ConcurrentQueue<byte[]> _messagesQueue = new ConcurrentQueue<byte[]>();

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

				await _webSocketServer.StartServer("localhost", "example", 8074);
				var cancellationToken = new CancellationToken();
				await Task.Run(handleMessageOnQueue, cancellationToken);


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
			_messagesQueue.Enqueue(buffer);
		}


		private readonly Stopwatch _sw = new Stopwatch();
		private readonly UpdateNewImageResponseMessageData _updateNewImageResponseMessageData = new UpdateNewImageResponseMessageData();
		private readonly UpdateNewImageResponseMessage _updateNewImageResponseMessage = new UpdateNewImageResponseMessage();


		private async Task handleMessageOnQueue()
		{
			while (true)
			{
				if (_messagesQueue.Any())
				{
					_sw.Start();

					try
					{
						var dequeued = _messagesQueue.TryDequeue(out byte[] bytes);
						if (dequeued)
						{

							// _sw.Stop();
							// var deserializeJson = _sw.Elapsed.TotalMilliseconds;
							// _sw.Restart();
							//
							var header = WebSocketMessageRequest.GetMessageHeader(bytes);

							switch (header)
							{
								case "InitLumenDetectionRequest":
									{
										var response = new InitLumenDetectionResponseMessage("OK");
										var jsonResponse =
											new WebSocketMessageResponse<InitLumenDetectionResponseMessage>(
												"InitLumenDetectionResponse", "OK", response);
										var json = jsonResponse.ToJSON();
										var data = Encoding.UTF8.GetBytes(json);
										_webSocketServer.SendBinary(data, _clientSocketID);
										break;
									}
								case "UpdateNewImage":
									{
										var message = WebSocketMessageRequest<UpdateNewImageMessage>.FromJson(bytes);

										var frame = _videoFrameReader.ConvertFrameFromBytes(message.messageData.MessageData.ImageData);

										if (frame != null)
										{
											// TODO: add lumen's data and send response
											var lumenData = simulateAlgoAndGetLumens();

											initMessageData(DateTime.Now.Ticks.ToString(), message.messageData.MessageData.ImageId, lumenData);
											var response = initResponse(_updateNewImageResponseMessageData);

											// _sw.Stop();
											// var t = _sw.Elapsed;
											// _sw.Restart();

											var jsonResponse =
												new WebSocketMessageResponse<UpdateNewImageResponseMessage>(response.MessageHeader,
													status: "OK", response).ToJSON();
											var data = Encoding.UTF8.GetBytes(jsonResponse);

											_webSocketServer.SendBinary(data, _clientSocketID);
										}

										_sw.Stop();
										var total = _sw.Elapsed.TotalMilliseconds;
										break;
									}
							}
						}
					}
					catch (Exception e)
					{
						Console.WriteLine(e);
						throw;
					}
				}

				await Task.Delay(1);
			}
		}

		private UpdateNewImageResponseMessage initResponse(UpdateNewImageResponseMessageData responseData)
		{
			_updateNewImageResponseMessage.MessageData = responseData;
			return _updateNewImageResponseMessage;
		}

		private void initMessageData(string id, string msgId, IEnumerable<LumensCoordinates> lumensCoordinates)
		{
			_updateNewImageResponseMessageData.TimeStamp = id;
			_updateNewImageResponseMessageData.ImageId = msgId;
			_updateNewImageResponseMessageData.LumensCoordinates = lumensCoordinates;
		}

		// private void _webSocketServer_BinaryMessageReceived(byte[] buffer, int socketID)
		// {
		// 	// Console.WriteLine($"WebSocket BinaryMessageReceived{socketID} closed");
		// 	try
		// 	{
		// 		var jsonText = Encoding.UTF8.GetString(buffer);
		// 		var message = JsonConvert.DeserializeObject<FrameMessage>(jsonText);
		//
		// 		var frameAsBytes = Convert.FromBase64String(message.FrameAsString);
		// 		var frame = _videoFrameReader.ConvertFrameFromBytes(frameAsBytes);
		//
		// 		if (frame != null)
		// 		{
		// 			// TODO: add lumen's data and send response
		// 			var lumenData = simulateAlgoAndGetLumens();
		// 			// var frameAsString = Convert.ToBase64String(frameAsBytes);
		//
		// 			var responseData = new UpdateNewImageResponseMessageData(DateTime.Now.Ticks.ToString(), message.Id, lumenData);
		// 			var response = new UpdateNewImageResponseMessage(responseData);
		// 			// var jsonResponse = JsonConvert.SerializeObject(response);
		// 			var jsonResponse =
		// 				new WebSocketMessageResponse<UpdateNewImageResponseMessage>(response.MessageHeader,
		// 					status: "OK", response).ToJSON();
		// 			var data = Encoding.UTF8.GetBytes(jsonResponse);
		//
		// 			// var delay = _random.Next(10, 20);
		// 			// Thread.Sleep(delay);
		// 			_webSocketServer.SendBinary(data, _clientSocketID);
		// 		}
		// 	}
		// 	catch (Exception e)
		// 	{
		// 		Console.WriteLine(e);
		// 		throw;
		// 	}
		// }

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




// using System;
// using System.Collections.Generic;
// using System.Text;
// using System.Threading;
// using System.Threading.Tasks;
// using AForge.Video.Reader;
// using Comm.Model;
// using Comm.Model.LumenDetectionApi;
// using SD.Framework.Infrastructure.IPCCommunication;
//
//
// namespace Server
// {
// 	class Program
// 	{
// 		private WebSocketServer _webSocketServer;
// 		private int _clientSocketID = -1;
// 		private VideoFrameReader _videoFrameReader;
// 		// private bool _sendTaskRunning = false;
// 		private Random _random;
//
// 		static void Main(string[] args)
// 		{
// 			Program p = new Program();
// 			CancellationTokenSource token = new CancellationTokenSource();
// 			Task ts = p.Run(token);
// 			Console.ReadKey();
// 		}
//
//
// 		public async Task Run(CancellationTokenSource token)
// 		{
// 			try
// 			{
// 				_random = new Random();
//
// 				_videoFrameReader = new VideoFrameReader();
// 				_webSocketServer = new WebSocketServer();
// 				_webSocketServer.SocketConnected += _webSocketServer_SocketConnected;
// 				_webSocketServer.TextMessageReceived += _webSocketServer_TextMessageReceived;
// 				_webSocketServer.BinaryMessageReceived += _webSocketServer_BinaryMessageReceived;
// 				_webSocketServer.SocketClosed += _webSocketServer_SocketClosed;
// 				await _webSocketServer.StartServer("localhost", "example", 8074);
//
// 			}
// 			catch (Exception exception)
// 			{
// 				Console.WriteLine(exception);
// 				throw;
// 			}
// 		}
//
// 		private void _webSocketServer_SocketConnected(int socketID)
// 		{
// 			_clientSocketID = socketID;
// 		}
// 		 
// 		private void _webSocketServer_SocketClosed(int socketID)
// 		{
// 			Console.WriteLine($"WebSocket {socketID} closed");
// 			_clientSocketID = -1;
// 		}
//
// 		private void _webSocketServer_BinaryMessageReceived(byte[] buffer, int socketID)
// 		{
// 			try
// 			{	
// 				var message = WebSocketMessageRequest<UpdateNewImageMessage>.FromJson(buffer);
// 				var frame = _videoFrameReader.ConvertFrameFromBytes(message.messageData.MessageData.ImageData);
//
// 				if (frame != null)
// 				{
// 					// TODO: add lumen's data and send response
// 					var lumenData = simulateAlgoAndGetLumens();
//
// 					var responseData = new UpdateNewImageResponseMessageData(DateTime.Now.Ticks.ToString(), message.messageData.MessageData.ImageId, lumenData);
// 					var response = new UpdateNewImageResponseMessage(responseData);
// 					
// 					var jsonResponse =
// 						new WebSocketMessageResponse<UpdateNewImageResponseMessage>(response.MessageHeader, status: "OK", response).ToJSON();
// 					var data = Encoding.UTF8.GetBytes(jsonResponse);
//
// 					_webSocketServer.SendBinary(data, _clientSocketID);
// 				}
// 			}
// 			catch (Exception e)
// 			{
// 				Console.WriteLine(e);
// 				throw;
// 			}
// 		}
//
// 		private IEnumerable<LumensCoordinates> simulateAlgoAndGetLumens()
// 		{
// 			var listOfLumens = new List<LumensCoordinates>();
//
// 			for (int i = 0; i < 2; i++)
// 			{
// 				var fakeLumen = new LumensCoordinates(x: _random.NextDouble(), y: _random.NextDouble(), radius: _random.NextDouble() * 60.0);
// 				listOfLumens.Add(fakeLumen);
// 			}
//
// 			return listOfLumens;
// 		}
//
// 		private void _webSocketServer_TextMessageReceived(string msg, int socketID)
// 		{
// 			WebSocketMessageRequest wrq = WebSocketMessageRequest.CreateWebSocketMessageRequestFromJSON(msg);
// 			switch (wrq.requestHeader)
// 			{
// 				case "TestRequest":
// 					WebSocketMessageRequest<SampleData> wrq1 = new WebSocketMessageRequest<SampleData>(msg);
// 					Console.WriteLine($"Got request {wrq1.requestHeader} with data {wrq1.messageData}");
// 					WebSocketMessageResponse<string> wrsp = new WebSocketMessageResponse<string>("TestResponse", "Success", "");
// 					break;
// 			}
// 		}
//
// 	 
// 		 
// 	}
// }
