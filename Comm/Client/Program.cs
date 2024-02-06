using System;
using System.Threading;
using System.Threading.Tasks;
using Comm.Model;
using SD.Framework.Infrastructure.IPCCommunication;


namespace Client
{
	
	class Program
	{
		WebSocketClient _websocketClient;
		static void Main(string[] args)
		{
			Program p = new Program();
			CancellationTokenSource token = new CancellationTokenSource();
			Task ts = p.Run(token);
			Console.ReadKey();
		}

		public async Task Run(CancellationTokenSource token)
		{
			_websocketClient = new WebSocketClient();
			_websocketClient.TextMessageReceived += _websocketClient_TextMessageReceived;
			_websocketClient.BinaryMessageReceived += _websocketClient_BinaryMessageReceived;
			_websocketClient.SocketClosed += _websocketClient_SocketClosed;
			// Task pascHelloTask = Task.Factory.StartNew(() => DoClientWork(token.Token), token.Token);
			await _websocketClient.StartClient("localhost", "example", 8074, token.Token);

		}
		// private void DoClientWork(CancellationToken ct)
		// {
		//
		// 	Random random = new Random();
		// 	while (!ct.IsCancellationRequested)
		// 	{
		// 		if (_websocketClient.Connected)
		// 		{
		// 			SampleData sample = new SampleData() { ID = random.Next(100), X = random.NextDouble() * 40.0 };
		// 			WebSocketMessageRequest<SampleData> data = new WebSocketMessageRequest<SampleData>(header: "TestRequest", sample);
		// 			_websocketClient.SendText(data.ToJSON());
		// 		}
		// 		Thread.Sleep(500);
		// 	}
		// }
		private void _websocketClient_SocketClosed(int socketID)
		{
			Console.WriteLine($"WebSocket {socketID} closed");
		}

		private void _websocketClient_BinaryMessageReceived(byte[] buffer, int socketID)
		{
			Console.WriteLine($"WebSocket BinaryMessageReceived{socketID} closed");
		}

		private void _websocketClient_TextMessageReceived(string msg, int socketID)
		{
			WebSocketMessageRequest wrq = WebSocketMessageRequest.CreateWebSocketMessageRequestFromJSON(msg);
			if (wrq != null)
			{
				switch (wrq.requestHeader)
				{
					case "CTCoordsRequest":
						WebSocketMessageRequest<SampleData2> wrq1 = new WebSocketMessageRequest<SampleData2>(msg);
						Console.WriteLine($"Got request {wrq1.requestHeader} with data {wrq1.messageData}");
						break;
				}
			}
			else
			{
				WebSocketMessageResponse wrsp = WebSocketMessageResponse.CreateWebSocketMessageResponseFromJSON(msg);
				if (wrq != null)
				{
					switch (wrsp.responseHeader)
					{
						case "TestResponse":
							WebSocketMessageResponse<string> wrsp1 = new WebSocketMessageResponse<string>(msg);
							Console.WriteLine($"Got response {wrsp1.responseHeader} with status {wrsp1.responseStatus} with data {wrsp1.messageData}");
							break;
					}
				}
			}
		}
	}
}
