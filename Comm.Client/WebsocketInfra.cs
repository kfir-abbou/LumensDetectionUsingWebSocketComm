using System;
using System.Threading;
using System.Threading.Tasks;
using SD.Framework.Infrastructure.IPCCommunication;

namespace LumenDetection.Tests.Comm.Client
{
	public class WebsocketInfra
	{
		protected WebSocketClient _websocketClient;
		private readonly string _address;
		private readonly string _addressPostfix;
		private readonly int _port;

		public event EventHandler<string> TextMessageReceived;
		public event EventHandler<byte[]> BinaryMessageReceived;
		public event EventHandler<int> ClientSocketClosed;


		public WebsocketInfra(string address, string addressPostfix, int port)
		{
			_websocketClient = new WebSocketClient();
			_address = address;
			_addressPostfix = addressPostfix;
			_port = port;
			CancellationTokenSource tokenSource = new CancellationTokenSource();
			Task.Run(()=> run(tokenSource.Token));
		}

		private async Task run(CancellationToken token)
		{
			_websocketClient.TextMessageReceived += _websocketClient_TextMessageReceived;
			_websocketClient.BinaryMessageReceived += _websocketClient_BinaryMessageReceived;
			_websocketClient.SocketClosed += _websocketClient_SocketClosed;
			
			//Task pascHelloTask = Task.Factory.StartNew(() => DoClientWork(token), token);
			await _websocketClient.StartClient(_address,_addressPostfix, _port, token);
		}

		private void _websocketClient_SocketClosed(int socketid)
		{
			ClientSocketClosed?.Invoke(null, socketid);
		}

		private void _websocketClient_BinaryMessageReceived(byte[] buffer, int socketid)
		{	
			BinaryMessageReceived?.Invoke(null, buffer);
		}

		private void _websocketClient_TextMessageReceived(string msg, int socketid)
		{
			TextMessageReceived?.Invoke(null, msg);
		}
		//
		// public void SendMessage(SampleData data)
		// {
		// 	WebSocketMessageRequest<SampleData> message = new WebSocketMessageRequest<SampleData>(header: "TestRequest", data);
		// 	_websocketClient.SendText(message.ToJSON());
		// }

		// public void SendBinary(byte[] data)
		// {
		// 	_websocketClient.SendBinary(data);
		// }

		// private void DoClientWork(CancellationToken ct)
		// {
		// 	Random random = new Random();
		// 	while (!ct.IsCancellationRequested)
		// 	{
		// 		if (_websocketClient.Connected)
		// 		{
		// 			SampleData sample = new SampleData() { ID = random.Next(100), X = random.NextDouble() * 40.0 };
		// 			
		// 		}
		// 		Thread.Sleep(500);
		// 	}
		// }
	}

}
