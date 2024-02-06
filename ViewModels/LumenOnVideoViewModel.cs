using Comm.Model;
using LumenDetection.Tests.Comm.Client;
using LumenDetection.Tests.LumenDataHandle;
using System;
using System.Collections.Generic;
using LumenDetection.Tests.CommonHelper;
using LumenDetection.Tests.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using AForge.Video.Reader;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

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

		public LumenOnVideoViewModel()
		{
			_vfr = new VideoFrameReader();
			Application.Current.Activated += onActivated;

			_lumenOnVideoStreamHandler = new LumenOnVideoStreamHandler(new CommInfra("localhost", "example2", 8075));
			_lumenOnVideoStreamHandler.LumensMessageReceived += LumenOnVideoStreamHandlerOnLumensMessageReceived;
		}

		private void onActivated(object sender, EventArgs e)
		{
			Task.Run(async () =>
			{
				try
				{
					var frames = _vfr.GetVideoFileBytes(@"C:\Temp\Video\ct.avi");
					foreach (var frame in frames)
					{
						// TODO: handle thread switch:
						await _lumenOnVideoStreamHandler.HandleVideoFrame(frame);
						Application.Current.Dispatcher.InvokeAsync(() =>
						{
							var bitmapSource = DrawLumensHelper.ConvertVideoFrameByteArrayToBitmapSource(frame);
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

		private void LumenOnVideoStreamHandlerOnLumensMessageReceived(object sender, LumenDataMessage response)
		{
			try
			{
				Application.Current.Dispatcher.InvokeAsync(() =>
				{
					var lumens = DrawLumensHelper.ConvertDataToFitScreen(response.Lumens);
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
