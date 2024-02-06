using Comm.Model;
using LumenDetection.Tests.Comm.Client;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace LumenDetection.Tests.LumenDataHandle
{
	public class LumenOnVideoStreamHandler
	{

		private readonly CommInfra _commInfra;
		private readonly double TICKS_IN_MILI = 10_000;
		private readonly int MAX_DELAY = 10;

		public event EventHandler<LumenDataMessage> LumensMessageReceived;

		public LumenOnVideoStreamHandler(CommInfra commInfra)
		{
			_commInfra = commInfra;
			_commInfra.BinaryMessageReceived += onBinaryMessageReceived;
		}

		private void onBinaryMessageReceived(object sender, byte[] responseMessageFromAlgo)
		{
			var jsonResponse = Encoding.UTF8.GetString(responseMessageFromAlgo);
			var response = JsonConvert.DeserializeObject<LumenDataMessage>(jsonResponse);

			var timeDiffInMilliseconds = calculateTimeDiffFromId(response.Id);

			if (timeDiffInMilliseconds < MAX_DELAY)
			{
				var data = new LumenDataMessage(response.Id, response.Lumens);
				LumensMessageReceived?.Invoke(null, data);
			}

		}

		private long _idCounter = 0;
		public Task HandleVideoFrame(byte[] frame)
		{
			try
			{
				var id = DateTime.Now.Ticks.ToString();
				// _idCounter++;
				sendFrameMessage(id, frame);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				return Task.FromException(e);
			}

			return Task.CompletedTask;
		}

		private void sendFrameMessage(string id, byte[] frame)
		{
			_commInfra.SendFrameMessage(id, frame);
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
