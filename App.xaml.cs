using System.Windows;
using LumenDetection.Tests.CommonHelper;
using LumenDetection.Tests.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace LumenDetection.Tests
{
	public partial class App : Application
	{
		// public App()
		// {
		// 	var serviceCollection = new ServiceCollection();
		// 	serviceCollection.AddSingleton<CommonService>();
		// 	serviceCollection.AddSingleton<MainViewModel>();
		// 	serviceCollection.AddSingleton<LumenOnVideoViewModel>();
		// 	serviceCollection.AddSingleton<LumensInMemoryHandlingViewModel>();
		// 	serviceCollection.AddSingleton<PlayNaturalVideoViewModel>();
		// 	
		// 	
		// 	using (var serviceProvider = serviceCollection.BuildServiceProvider())
		// 	{
		// 		MainViewModel mainViewModel = serviceProvider.GetRequiredService<MainViewModel>();
		// 		LumenOnVideoViewModel receiverViewModel = serviceProvider.GetRequiredService<LumenOnVideoViewModel>();
		// 		LumensInMemoryHandlingViewModel LumensInMemoryHandlingViewModel = serviceProvider.GetRequiredService<LumensInMemoryHandlingViewModel>();
		// 		PlayNaturalVideoViewModel playNaturalVideoViewModel = serviceProvider.GetRequiredService<PlayNaturalVideoViewModel>();
		// 		// Your code here
		// 		var win = new Tests.MainWindow();
		// 		win.Show();}
		// 		//
		//
		// 	}
	}
}
