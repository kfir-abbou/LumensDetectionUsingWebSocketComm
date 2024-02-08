using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using AForge.Video.Reader;
using CommunityToolkit.Mvvm.Messaging;
using LumenDetection.Tests.CommonHelper;
using LumenDetection.Tests.LumenDataHandle;
using SD.Framework.Infrastructure.Commands;

namespace LumenDetection.Tests.ViewModels
{
	public class MainViewModel : INotifyPropertyChanged
	{
		private readonly CommonService _commonService;
		public event PropertyChangedEventHandler PropertyChanged;
		private IMessenger _messenger;
		public ICommand StartCommand => new RelayCommand(onStart);

		
		public MainViewModel()
		{
			// _commonService = commonService;

		}

		private void onStart(object obj)
		{
			WeakReferenceMessenger.Default.Send(new StartHandlingVideoMessage());

			// _commonService.FireEvent();
		}

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
