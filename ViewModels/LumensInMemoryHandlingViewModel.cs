﻿using System;
using AForge.Video.Reader;
using LumenDetection.Tests.Comm.Client;
using LumenDetection.Tests.LumenDataHandle;
using LumenDetection.Tests.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media.Imaging;
using Comm.Model;
using LumenDetection.Tests.CommonHelper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LumenDetection.Tests.ViewModels
{
	public class LumensInMemoryHandlingViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private readonly VideoFrameReader _vfr;
		private readonly LumenInMemoryHandler _lumenDataHandler;

		private BitmapSource _currentFrame;
		private double _screenWidth;
		private double _screenHeight;
		private string _playVideoText;

		public ObservableCollection<Circle> Circles { get; set; }
		public BitmapSource CurrentFrame
		{
			get => _currentFrame;
			set
			{
				_currentFrame = value;
				OnPropertyChanged();
			}
		}
		public string PlayVideoText
		{
			get => _playVideoText;
			set
			{
				_playVideoText = value;
				OnPropertyChanged();
			}
		}


		public LumensInMemoryHandlingViewModel()
		{
			Circles = new ObservableCollection<Circle>();
			_vfr = new VideoFrameReader();
			Application.Current.Activated += onActivated;

			_lumenDataHandler = new LumenInMemoryHandler(new CommInfra("localhost", "example", 8074));
			_lumenDataHandler.LumensMessageReceived += onLumensMessageReceived;
		}

		 

		private void onLumensMessageReceived(object sender, UpdateImageResponseReceivedEventArgs eventArgs)
		{
			try
			{
				Application.Current.Dispatcher.InvokeAsync(() => // Ensure UI updates on the UI thread
				{
					// var frameAsBytes = Convert.FromBase64String(response.FrameAsString);
					var bitmapImage = DrawLumensHelper.ConvertVideoFrameByteArrayToBitmapSource(eventArgs.FrameBytes);
					var win = Application.Current.MainWindow;
					var lumens = DrawLumensHelper.ConvertDataToFitScreen(eventArgs.Response.LumensCoordinates);

					CurrentFrame = bitmapImage;
					Circles.Clear();
					addCircles(lumens);

				});
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
			Circles.Add(new Circle { X = x, Y = y, Radius = radius });
		}


		private void onActivated(object sender, EventArgs e)
		{
			_screenWidth = Application.Current.MainWindow.ActualWidth;
			_screenHeight = Application.Current.MainWindow.ActualHeight;
			Application.Current.Dispatcher.Invoke(() => PlayVideoText = "In Memory handler:");

			Task.Run(async () =>
			{
				try
				{
					await startHandlingVideo();
				}
				catch (Exception exception)
				{
					Console.WriteLine(exception);
					throw;
				}
			});
		}

		private async Task startHandlingVideo()
		{

			await Task.Run(async () =>
			{
				// TODO:
				// 1. convert to json containing-> id, frame -> convert to byte[] - done
				// 2. On receiving end deserialize into frame and id - done
				// 3. on receiving end generate lumens data - done 
				// 4. on receiving end return data with frame id 
				// 5. once lumen's data return add to queue 
				// 6. play according to frame order
				var frames = _vfr.GetVideoFrames(@"C:\Temp\Video\ct.avi");
				foreach (var frame in frames)
				{
					var frameAsBytes = _vfr.ConvertFrameToBytes(frame);
					// var id = DateTime.Now.Ticks.ToString();
					// _commInfra.SendFrameMessage(id, frameAsBytes);
					await _lumenDataHandler.HandleVideoFrame((uint)frame.Width, (uint)frame.Height, frameAsBytes);
					await Task.Delay(33);
				}
			});
			// return Task.CompletedTask;
		}


		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}


	}
}
