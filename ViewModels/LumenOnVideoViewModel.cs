using Comm.Model;
using LumenDetection.Tests.Comm.Client;
using LumenDetection.Tests.LumenDataHandle;
using System;
using System.Collections.Generic;
using LumenDetection.Tests.CommonHelper;
using LumenDetection.Tests.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using AForge.Video.Reader;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.Messaging;

namespace LumenDetection.Tests.ViewModels
{
	public class LumenOnVideoViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private readonly LumenOnVideoStreamHandler _lumenOnVideoStreamHandler;
		private readonly VideoFrameReader _vfr;
		private BitmapSource _currentFrame;
		
		public ObservableCollection<Circle> Circles { get; set; } = new();
		public BitmapSource CurrentFrame
		{
			get => _currentFrame;
			set
			{
				_currentFrame = value;
				OnPropertyChanged();
			}
		}


		private IMessenger _messenger;
		public LumenOnVideoViewModel()
		{
			WeakReferenceMessenger.Default.Register<StartHandlingVideoMessage>(this, sendInitToAlgo);
			_vfr = new VideoFrameReader();
			 
			_lumenOnVideoStreamHandler = new LumenOnVideoStreamHandler(new CommInfra("localhost", "example2", 8075));
			_lumenOnVideoStreamHandler.InitResponseReceived += LumenOnVideoStreamHandlerOnInitResponseReceived;
			_lumenOnVideoStreamHandler.LumensMessageReceived += LumenOnVideoStreamHandlerOnLumensMessageReceived;
		}

		private void LumenOnVideoStreamHandlerOnInitResponseReceived()
		{
			startStreamingVideo();
		}

		private void sendInitToAlgo(object recipient, StartHandlingVideoMessage message)
		{
			if (message.Start)
			{
				_lumenOnVideoStreamHandler.SendInitAlgoRequest();
			}
		}
		private void startStreamingVideo()
		{
			Task.Run(async () =>
			{
				try
				{
					var frames = _vfr.GetVideoFrames(@"C:\Temp\Video\ct.avi");
					// var frames = _vfr.GetVideoFrames(@"C:\Temp\Video\video.mp4");
					foreach (var frame in frames)
					{
						_swCycle.Restart();
						var frameBytes = _vfr.ConvertFrameToBytes(frame);

						await _lumenOnVideoStreamHandler.HandleVideoFrame((uint)frame.Width, (uint)frame.Height, frameBytes);
						
						Application.Current.Dispatcher.InvokeAsync(() =>
						{
							var bitmapSource = DrawLumensHelper.ConvertVideoFrameByteArrayToBitmapSource(frameBytes);
							CurrentFrame = bitmapSource;
						});
						 
					 
						await Task.Delay(33);
					}
				}
				catch (Exception exception)
				{
					Console.WriteLine(exception);
					throw;
				}
			});
		}


		private readonly Stopwatch _sw = new();
		private readonly Stopwatch _swCycle = new();
		 
		private void LumenOnVideoStreamHandlerOnLumensMessageReceived(object sender, IEnumerable<LumensCoordinates> lumensCoordinates)
		{
			try
			{
				Application.Current.Dispatcher.InvokeAsync(() =>
				{
					var lumens = DrawLumensHelper.ConvertDataToFitScreen(lumensCoordinates);
					Circles.Clear();
					addCircles(lumens);
				});
				_swCycle.Stop();
				var t = _swCycle.Elapsed.TotalMilliseconds;
				// Console.WriteLine($"Cycle on: lumen on video -> {t}ms.");
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}

		private void addCircles(IEnumerable<LumensCoordinates> lumens)
		{
			foreach (var lumenData in lumens)
			{
				addCircle(lumenData.X, lumenData.Y, lumenData.Radius);
			}
		}

		private void addCircle(double x, double y, double radius)
		{
			Application.Current.Dispatcher.Invoke(() => // Ensure UI updates on the UI thread
			{
				Circles.Add(new Circle { X = x, Y = y, Radius = radius });
			});
		}

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
