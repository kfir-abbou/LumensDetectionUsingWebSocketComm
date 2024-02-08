using System;
using System.Windows;
using LumenDetection.Tests.CommonHelper;
using LumenDetection.Tests.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LumenDetection.Tests
{
	public partial class App 
	{

		// [STAThread]
		// public static void Main()
		// {
		// 	var app = new App();
		// 	app.InitializeComponent();
		// 	app.Run();
		// }
		// protected override void OnStartup(StartupEventArgs e)
		// {
		// 	base.OnStartup(e);
		//
		// 	var serviceProvider = new ServiceCollection()
		// 		.AddSingleton<MainViewModel>()
		// 		.AddSingleton<LumenOnVideoViewModel>()
		// 		.BuildServiceProvider();
		//
		// 	var mainWindow = new MainWindow();
		// 	mainWindow.Show();
		// }
		//
		//

		// public App()
		// {
		// 	InitializeComponent();
		//
		// 	var serviceCollection = new ServiceCollection();
		// 	serviceCollection.AddSingleton<CommonService>();
		// 	serviceCollection.AddSingleton<MainViewModel>();
		// 	serviceCollection.AddSingleton<LumenOnVideoViewModel>();
		// 	serviceCollection.AddSingleton<LumensInMemoryHandlingViewModel>();
		// 	serviceCollection.AddSingleton<PlayNaturalVideoViewModel>();
		//
		// 	using (var serviceProvider = serviceCollection.BuildServiceProvider())
		// 	{
		// 		var mainViewModel = serviceProvider.GetRequiredService<MainViewModel>();
		// 		var lumensOnVideoViewModel = serviceProvider.GetRequiredService<LumenOnVideoViewModel>();
		// 		var LumensInMemoryHandlingViewModel = serviceProvider.GetRequiredService<LumensInMemoryHandlingViewModel>();
		// 		var playNaturalVideoViewModel = serviceProvider.GetRequiredService<PlayNaturalVideoViewModel>();
		// 	}
		//
		// 	// Start main window 
		// 	// var mainWindow = new MainWindow();
		// 	// mainWindow.Show();
		// }
		//
		// protected override void OnStartup(StartupEventArgs e)
		// {
		// 	base.OnStartup(e);
		// 	var builder = new ContainerBuilder();
		// }
	}
}
