using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Comm.Model;
using CommunityToolkit.Mvvm.Messaging;
using LumenDetection.Tests.LumenDataHandle;
using SD.Framework.Infrastructure.Commands;

namespace LumenDetection.Tests.ViewModels
{
	public class MainViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		private IMessenger _messenger;
		public ICommand StartCommand => new RelayCommand(onStart);


		public MainViewModel()
		{
			WeakReferenceMessenger.Default.Register<TimeMessage>(this, (recipient, message) =>
			{

			});
		}
		private void onStart(object obj)
		{
			WeakReferenceMessenger.Default.Send(new StartHandlingVideoMessage());
		}

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
