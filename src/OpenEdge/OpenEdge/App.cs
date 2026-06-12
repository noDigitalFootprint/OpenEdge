using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace OpenEdge;

public partial class App : Application
{
	protected override void OnStartup(StartupEventArgs e)
	{
		AppDomain.CurrentDomain.UnhandledException += delegate(object sender, UnhandledExceptionEventArgs args)
		{
			SessionTraceLogger.Error("unhandled-exception", "AppDomain unhandled exception terminating=" + args.IsTerminating, args.ExceptionObject as Exception);
		};
		DispatcherUnhandledException += delegate(object sender, DispatcherUnhandledExceptionEventArgs args)
		{
			SessionTraceLogger.Error("unhandled-exception", "Dispatcher unhandled exception", args.Exception);
		};
		TaskScheduler.UnobservedTaskException += delegate(object sender, UnobservedTaskExceptionEventArgs args)
		{
			SessionTraceLogger.Error("unhandled-exception", "Unobserved task exception", args.Exception);
		};
		base.OnStartup(e);
		FrameWindow frameWindow = new FrameWindow();
		MainWindow = frameWindow;
		frameWindow.Show();
		frameWindow.Activate();
	}
}
