using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using AForge.Video.Reader;
using LumenDetection.Tests.CommonHelper;
using LumenDetection.Tests.LumenDataHandle;
using LumenDetection.Tests.Model;
using SD.Framework.Infrastructure.Commands;
using SD.Framework.Infrastructure.Scs.Communication.Messengers;

namespace LumenDetection.Tests.ViewModels
{
	public class MainViewModel : INotifyPropertyChanged
	{
		private readonly IMessenger _messenger;
		private readonly CommonService _commonService;
		public event EventHandler StartEvent; 
		public event PropertyChangedEventHandler PropertyChanged;

		public ICommand StartCommand => new RelayCommand(onStart);

		private void onStart(object obj)
		{
			 // _commonService.FireEvent();
		}

		// public MainViewModel(CommonService commonService)
		// {
		// 	_commonService = commonService;
		// }


		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
