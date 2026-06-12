using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace OpenEdge;

public partial class FrameWindow : Window, IComponentConnector
{
	[Flags]
	private enum ExecutionState : uint
	{
		EsAwaymodeRequired = 0x40u,
		EsContinuous = 0x80000000u,
		EsDisplayRequired = 2u,
		EsSystemRequired = 1u
	}

	private Bluetooth bt;

	private MainWindow strokeScreen;

	private Page1 page1;

	private SecondWindow secondWindow;

	private ImageTagger imgTag;

	private HomeworkScreen homeworkScreen;

	private MediaPlayer span;

	private MediaCatalogService mediaCatalog;

	private CompatibilityStateService compatibilityStateService;

	private SettingsRegistry settingsRegistry;

	private bool launchFullScreen = true;

	private bool shellInitialized;

	public FrameWindow()
	{
		PreventSleep();
		CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
		CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
		CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
		EnsureRuntimeDirectories();
		File.WriteAllText(RuntimePaths.TempFlag("open"), DateTime.Now.ToString());
		InitializeComponent();
		Loaded += FrameWindow_Loaded;
	}

	private void FrameWindow_Loaded(object sender, RoutedEventArgs e)
	{
		ApplyWindowMode(launchFullScreen);
		Activate();
		Focus();
		if (!shellInitialized)
		{
			shellInitialized = true;
			Dispatcher.BeginInvoke(new Action(InitializeShell));
		}
	}

	private void InitializeShell()
	{
		compatibilityStateService = new CompatibilityStateService();
		compatibilityStateService.EnsureInitialized();
		ModService.EnsureModsDirectory();
		settingsRegistry = new SettingsRegistry(compatibilityStateService, ModService.GetEnabledSettingDefinitions());
		mediaCatalog = new MediaCatalogService();
		imgTag = new ImageTagger(mediaCatalog);
		NormalizeEdgeHoldRecord();
		span = CreateUiSoundPlayer();
		secondWindow = new SecondWindow(mediaCatalog);
		secondWindow.eventHandeler = this;
		strokeScreen = new MainWindow(secondWindow, mediaCatalog, compatibilityStateService, settingsRegistry);
		strokeScreen.reloadImagesVideos();
		bt = new Bluetooth();
		page1 = new Page1(this, strokeScreen, imgTag, span, bt, compatibilityStateService, settingsRegistry);
		bt.setP(page1);
		strokeScreen.preLoad(page1);
		homeworkScreen = new HomeworkScreen(page1);
		page1.setHomeWorkScreen(homeworkScreen);
		myFrame.NavigationService.Navigate(page1);
	}

	private static void EnsureRuntimeDirectories()
	{
		if (!Directory.Exists(RuntimePaths.ImagesDir))
		{
			Directory.CreateDirectory(RuntimePaths.ImagesDir);
		}
		if (!Directory.Exists(RuntimePaths.DebugDir))
		{
			Directory.CreateDirectory(RuntimePaths.DebugDir);
		}
		if (!Directory.Exists(RuntimePaths.VideosDir))
		{
			Directory.CreateDirectory(RuntimePaths.VideosDir);
		}
		if (!Directory.Exists(RuntimePaths.FlagsDir))
		{
			Directory.CreateDirectory(RuntimePaths.FlagsDir);
		}
		if (!Directory.Exists(RuntimePaths.TempFlagsDir))
		{
			Directory.CreateDirectory(RuntimePaths.TempFlagsDir);
		}
		if (!Directory.Exists(RuntimePaths.ModsDir))
		{
			Directory.CreateDirectory(RuntimePaths.ModsDir);
		}
	}

	private static void NormalizeEdgeHoldRecord()
	{
		if (File.Exists(RuntimePaths.Flag("edgeHoldRecord")))
		{
			File.WriteAllText(RuntimePaths.Flag("edgeHoldRecord"), File.ReadAllText(RuntimePaths.Flag("edgeHoldRecord")).Split(',')[0]);
		}
	}

	private static MediaPlayer CreateUiSoundPlayer()
	{
		MediaPlayer mediaPlayer = new MediaPlayer();
		mediaPlayer.MediaEnded += delegate
		{
			mediaPlayer.Pause();
			mediaPlayer.Position = new TimeSpan(0L);
		};
		mediaPlayer.Volume = 0.0;
		mediaPlayer.Pause();
		mediaPlayer.Open(new Uri(Path.Combine(RuntimePaths.AudioDir, "click.wav")));
		return mediaPlayer;
	}

	private void Span_MediaEnded(object sender, EventArgs e)
	{
		span.Pause();
		span.Position = new TimeSpan(0L);
	}

	public void closeApp()
	{
		if (page1 != null)
		{
			page1.shutDown();
		}
	}

	public void fullScreen(bool fullScreenOn)
	{
		launchFullScreen = fullScreenOn;
		if (IsLoaded)
		{
			ApplyWindowMode(fullScreenOn);
		}
	}

	private void ApplyWindowMode(bool fullScreenOn)
	{
		base.WindowStyle = (fullScreenOn ? WindowStyle.None : WindowStyle.SingleBorderWindow);
		base.ResizeMode = (fullScreenOn ? ResizeMode.NoResize : ResizeMode.CanResize);
		if (base.WindowState == WindowState.Minimized)
		{
			base.WindowState = WindowState.Normal;
		}
		base.WindowState = WindowState.Maximized;
	}

	protected override void OnClosing(CancelEventArgs e)
	{
		AllowSleep();
		if (strokeScreen != null && strokeScreen.currentScript != null && !strokeScreen.getTFlag("closeWithoutSession"))
		{
			File.WriteAllText(RuntimePaths.Flag("failedSessionEnd"), DateTime.Now.ToString());
		}
		if (File.Exists(RuntimePaths.TempFlag("open")))
		{
			File.Delete(RuntimePaths.TempFlag("open"));
		}
		if (strokeScreen != null && strokeScreen.currentScript != null)
		{
			strokeScreen.currentScript.deleteFlag("closeWithoutSession", temp: true);
		}
		closeApp();
		Environment.Exit(0);
	}

	public void OnButtonKeyDown(object sender, KeyEventArgs e)
	{
		if (e.Key.ToString() == "Escape")
		{
			page1.playClickSound();
			if (myFrame.NavigationService.Content == strokeScreen)
			{
				secondWindow.muteVideo(mute: true);
				strokeScreen.pauseSession();
				myFrame.NavigationService.GoBack();
				page1.setButtonToResume();
			}
			else
			{
				if (myFrame.NavigationService.Content == imgTag && !imgTag.getForced())
				{
					imgTag.backToMenu();
					return;
				}
				if (myFrame.NavigationService.Content == page1 && !page1.popUpOpen())
				{
					page1.shutDown();
				}
				else if (myFrame.NavigationService.Content == page1 && page1.popUpOpen())
				{
					page1.popUpClose();
				}
				else if (myFrame.NavigationService.Content == homeworkScreen && homeworkScreen.activeTask == null)
				{
					myFrame.NavigationService.GoBack();
				}
				else if (myFrame.NavigationService.Content == bt)
				{
					myFrame.NavigationService.GoBack();
				}
			}
		}
		else if (e.Key.ToString() == "F5" && myFrame.NavigationService.Content == strokeScreen)
		{
			strokeScreen.safeWord();
		}
	}

	protected override void OnSourceInitialized(EventArgs e)
	{
		base.OnSourceInitialized(e);
	}

	private void mainWindow_Activated(object sender, EventArgs e)
	{
		if (page1 == null || strokeScreen == null)
		{
			return;
		}
		page1.gotEnoughImages();
		imgTag.reloadImagesVideosTags();
		strokeScreen.reloadImagesVideos();
	}

	public static void PreventSleep()
	{
		SetThreadExecutionState(ExecutionState.EsContinuous | ExecutionState.EsSystemRequired);
	}

	public static void AllowSleep()
	{
		SetThreadExecutionState(ExecutionState.EsContinuous);
	}

	[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	private static extern ExecutionState SetThreadExecutionState(ExecutionState esFlags);
}
