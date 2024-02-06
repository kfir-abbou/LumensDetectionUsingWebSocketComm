using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using AForge.Video.Reader;
using LumenDetection.Tests.LumenDataHandle;
using LumenDetection.Tests.Model;

namespace LumenDetection.Tests.ViewModels
{
	public class MainViewModel : INotifyPropertyChanged
	{
		// private readonly WebsocketInfra _webSocketInfra;
		// private double _screenHeight;
		// private double _screenWidth;
		private BitmapSource _currentFrame;
		public ObservableCollection<Circle> Circles  = new();
		public event PropertyChangedEventHandler PropertyChanged;
		public BitmapSource CurrentFrame
		{
			get => _currentFrame;
			set
			{
				_currentFrame = value;
				OnPropertyChanged();
			}
		}

		private string _playVideoText;
		// private readonly CommInfra _commInfra;
		// private readonly VideoFrameReader _vfr;
		// private readonly LumenInMemoryHandler _lumenDataHandler;

		public string PlayVideoText
		{
			get => _playVideoText;
			set
			{
				_playVideoText = value;
				OnPropertyChanged();
			}
		}

		public MainViewModel()
		{
			// _vfr = new VideoFrameReader();
			// Application.Current.Activated += onActivated;
			//
			// _lumenDataHandler = new LumenInMemoryHandler(new CommInfra("localhost", "example", 8074));
			// _lumenDataHandler.LumensMessageReceived += onLumensMessageReceived;
		}

		// private void onLumensMessageReceived(object sender, VideoFrameWithLumenMessage response)
		// {
		// 	try
		// 	{
		// 		Application.Current.Dispatcher.InvokeAsync(() => // Ensure UI updates on the UI thread
		// 		{
		// 			var frameAsBytes = Convert.FromBase64String(response.FrameAsString);
		// 			var bitmapImage = DrawLumensHelper.ConvertVideoFrameByteArrayToBitmapSource(frameAsBytes);
		// 			var lumens = DrawLumensHelper.ConvertDataToFitScreen(response.LumenData, 302, 302);
		//
		// 			Circles.Clear();
		// 			addCircles(lumens);
		// 			CurrentFrame = bitmapImage;
		// 		});
		// 	}
		// 	catch (Exception e)
		// 	{
		// 		Console.WriteLine(e);
		// 		throw;
		// 	}
		// }

		// private void onActivated(object sender, EventArgs e)
		// {
		// 	_screenWidth = Application.Current.MainWindow.ActualWidth;
		// 	_screenHeight = Application.Current.MainWindow.ActualHeight;
		// 	PlayVideoText = "Play video frame:";
		//
		// 	Task.Run(async () =>
		// 	{
		// 		try
		// 		{
		// 			await startHandlingVideo();
		// 		}
		// 		catch (Exception exception)
		// 		{
		// 			Console.WriteLine(exception);
		// 			throw;
		// 		}
		// 	});
		// }


		// private async Task startHandlingVideo()
		// {
		//
		//
		// 	// TODO:
		// 	// 1. convert to json containing-> id, frame -> convert to byte[] - done
		// 	// 2. On receiving end deserialize into frame and id - done
		// 	// 3. on receiving end generate lumens data - done 
		// 	// 4. on receiving end return data with frame id 
		// 	// 5. once lumen's data return add to queue 
		// 	// 6. play according to frame order
		// 	var frames = _vfr.GetVideoFrames(@"C:\Temp\Video\ct.avi");
		// 	foreach (var frame in frames)
		// 	{
		// 		var frameAsBytes = _vfr.ConvertFrameToBytes(frame);
		// 		// var id = DateTime.Now.Ticks.ToString();
		// 		// _commInfra.SendFrameMessage(id, frameAsBytes);
		// 		await Task.Run(()=> _lumenDataHandler.HandleVideoFrame(frameAsBytes));
		// 		await Task.Delay(33);
		// 	}
		//
		// 	// return Task.CompletedTask;
		// }

		// private void WebSocketInfraOnTextMessageReceived(object sender, string message)
		// {
		// 	// var data = JsonConvert.DeserializeObject<WebSocketMessageRequest<List<SampleData>>>(message);
		// 	// convertDataToFitScreen(data.messageData);
		// }


	 

		
		// private void addCircles(IEnumerable<LumenData> lumens)
		// {
		// 	foreach (var lumenData in lumens)
		// 	{
		// 		addCircle(lumenData.X, lumenData.Y, lumenData.Radius);
		// 	}
		// }
		//
		// private void addCircle(double x, double y, double radius)
		// {
		// 	Application.Current.Dispatcher.Invoke(() => // Ensure UI updates on the UI thread
		// 	{
		// 		Circles.Add(new Circle { X = x, Y = y, Radius = radius });
		// 	});
		// }

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
