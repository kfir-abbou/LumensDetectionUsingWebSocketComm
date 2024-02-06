using AForge.Video.Reader;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using LumenDetection.Tests.CommonHelper;

namespace LumenDetection.Tests.ViewModels
{
	public class PlayNaturalVideoViewModel : INotifyPropertyChanged
	{
		private BitmapSource _currentFrame;
		private string _playVideoText;
		private readonly VideoFrameReader _vfr;
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
		public string PlayVideoText
		{
			get => _playVideoText;
			set
			{
				_playVideoText = value;
				OnPropertyChanged();
			}
		}

		public PlayNaturalVideoViewModel()
		{
			Application.Current.Activated += onActivated;
			_vfr = new VideoFrameReader();
		}

		private void onActivated(object sender, EventArgs e)
		{
			Application.Current.Dispatcher.Invoke(() => PlayVideoText = "Natural Video");
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
				var frames = _vfr.GetVideoFileBytes(@"C:\Temp\Video\ct.avi");
				foreach (var frame in frames)
				{
					Application.Current.Dispatcher.InvokeAsync(() =>
						CurrentFrame = DrawLumensHelper.ConvertVideoFrameByteArrayToBitmapSource(frame));
					
					await Task.Delay(33);
				}
			});
		}





		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

	}
}
