using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Win32;

namespace OpenEdge;

public partial class Page1 : Page, IComponentConnector
{
	public bool muteVideos;

	public bool hideBeatBar;

	private bool fullScreen = true;

	public int pronounsInt;

	private string tD = "";

	public ImageTagger imageTagger;

	public HomeworkScreen homeworkScreen;

	private MediaSourcesPage mediaSourcesPage;

	private CompatibilityToolsPage compatibilityToolsPage;

	private SettingsPage settingsPage;

	private readonly CompatibilityStateService compatibilityStateService;

	private readonly SettingsRegistry settingsRegistry;

	public Bluetooth bt;

	public double textSpeed = 3.0;

	public double masterVolumeValue = 1.0;

	public double videoVolumeValue = 1.0;

	public double uiVolumeValue = 1.0;

	public double asmrVolumeValue = 1.0;

	public double ttsVolumeValue = 1.0;

	public double mediaSpeed = 3.0;

	private FrameWindow fw;

	public string nameOfDebugFile = "";

	public string backgroundImg;

	private MediaPlayer spa;

	public MainWindow strokePage;

	public Page1(FrameWindow fw, MainWindow strokePage, ImageTagger imageTagger, MediaPlayer span, Bluetooth bt, CompatibilityStateService compatibilityStateService, SettingsRegistry settingsRegistry)
	{
		this.fw = fw;
		this.strokePage = strokePage;
		this.compatibilityStateService = compatibilityStateService;
		this.settingsRegistry = settingsRegistry;
		nameOfDebugFile = Path.Combine(RuntimePaths.DebugDir, DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year + " " + DateTime.Now.Hour + ";" + DateTime.Now.Minute + ";" + DateTime.Now.Second + ".txt");
		this.imageTagger = imageTagger;
		InitializeComponent();
		imageTagger.parent = this;
		modsPopup.setParent(this);
		RefreshMenuButtons();
		this.bt = bt;
		loadOptions();
		spa = span;
		gotEnoughImages();
	}

	public void setHomeWorkScreen(HomeworkScreen homeworkScreen)
	{
		this.homeworkScreen = homeworkScreen;
	}

	public void RefreshMenuButtons()
	{
		if (strokePage.isSettingEnabled("taskScreen"))
		{
			homeworkBtn.Visibility = Visibility.Visible;
		}
		else
		{
			homeworkBtn.Visibility = Visibility.Collapsed;
		}
	}

	public void setStartSessionBtnDebt(int currency)
	{
		if (currency >= 0)
		{
			favorDebtText.Visibility = Visibility.Collapsed;
			startSessionGrid.IsEnabled = true;
		}
		else
		{
			favorDebtText.Visibility = Visibility.Visible;
			startSessionGrid.IsEnabled = false;
		}
	}

	public void gotEnoughImages()
	{
		int num = imageTagger.GetAvailableImageCount();
		int num2 = 0;
		beginButton.IsEnabled = true;
		tagBtn.IsEnabled = true;
		errorLable.Text = "";
		string[] array = imageTagger.searchSubFolders(RuntimePaths.ImagesDir);
		for (int i = 0; i < array.Length; i++)
		{
			if ((array[i].EndsWith(".gif") || array[i].EndsWith(".GIF")) && new FileInfo(array[i]).Length > 2000000)
			{
				num2++;
			}
		}
		if (num2 != 0)
		{
			errorLable.Text = "There are " + num2 + " GIF's that are causing a significant amount of latency, consider removing, optimizing or converting to MP4.\n";
		}
		if (num < 20)
		{
			beginButton.IsEnabled = false;
			tagBtn.IsEnabled = false;
			TextBlock textBlock = errorLable;
			textBlock.Text = textBlock.Text + "please add at least " + (20 - num) + " more images before starting";
		}
	}

	public void resumeSession()
	{
		if (strokePage.sessionActive)
		{
			strokePage.resumeSession();
		}
	}

	public void setButtonToResume()
	{
		beginButton.Content = "Resume Session";
	}

	private void Button_Click(object sender, RoutedEventArgs e)
	{
		playClickSound();
		strokePage.setObjects(this);
		strokePage.secWindow.videoVolumeValue = masterVolumeValue / 2.0 * videoVolumeValue / 2.0;
		base.NavigationService.Navigate(strokePage);
		if (!strokePage.sessionActive)
		{
			strokePage.startSession();
		}
		else
		{
			resumeSession();
		}
		saveOptions();
	}

	public void TagImages(object sender, RoutedEventArgs e)
	{
		playClickSound();
		imageTagger.setMuted(muteVideos);
		base.NavigationService.Navigate(imageTagger);
		saveOptions();
	}

	public void TagImages(Page activePage)
	{
		imageTagger.setMuted(muteVideos);
		activePage.NavigationService.Navigate(imageTagger);
		saveOptions();
	}

	private void MediaSources_Click(object sender, RoutedEventArgs e)
	{
		playClickSound();
		mediaSourcesPage = new MediaSourcesPage(imageTagger, strokePage);
		base.NavigationService.Navigate(mediaSourcesPage);
		saveOptions();
	}

	public void OpenMediaSources(NavigationService navigationService)
	{
		playClickSound();
		mediaSourcesPage = new MediaSourcesPage(imageTagger, strokePage);
		navigationService?.Navigate(mediaSourcesPage);
		saveOptions();
	}

	private void Settings_Click(object sender, RoutedEventArgs e)
	{
		playClickSound();
		settingsPage = new SettingsPage(settingsRegistry, this);
		base.NavigationService.Navigate(settingsPage);
		saveOptions();
	}

	private void MigrationTools_Click(object sender, RoutedEventArgs e)
	{
		playClickSound();
		compatibilityToolsPage = new CompatibilityToolsPage(compatibilityStateService, settingsRegistry, imageTagger.MediaCatalog);
		base.NavigationService.Navigate(compatibilityToolsPage);
		saveOptions();
	}

	public void OpenMigrationTools(NavigationService navigationService)
	{
		playClickSound();
		compatibilityToolsPage = new CompatibilityToolsPage(compatibilityStateService, settingsRegistry, imageTagger.MediaCatalog);
		navigationService?.Navigate(compatibilityToolsPage);
		saveOptions();
	}

	public void createNewTask(int state = 0, int length = -1, int type = -1)
	{
		homeworkScreen.createNewTask(state, length, type);
	}

	private void Button_Click_ImageFolder(object sender, RoutedEventArgs e)
	{
		playClickSound();
		Process.Start(Environment.GetEnvironmentVariable("WINDIR") + "\\explorer.exe", RuntimePaths.ImagesDir);
	}

	public bool popUpOpen()
	{
		return modsPopup.open;
	}

	public void popUpClose()
	{
		modsPopup.closePopup();
	}

	private void Button_Click_ModFolder(object sender, RoutedEventArgs e)
	{
		playClickSound();
		modsPopup.openPopup();
	}

	private void Button_Click_VideoFolder(object sender, RoutedEventArgs e)
	{
		playClickSound();
		Process.Start(Environment.GetEnvironmentVariable("WINDIR") + "\\explorer.exe", RuntimePaths.VideosDir);
	}

	private void Button_Click_CloseOpenEdge(object sender, RoutedEventArgs e)
	{
		playClickSound();
		shutDown();
	}

	public void shutDown()
	{
		saveOptions();
		Application.Current.Shutdown();
	}

	public void removeTemp()
	{
		string[] files = Directory.GetFiles(RuntimePaths.TempFlagsDir);
		for (int i = 0; i < files.Length; i++)
		{
			File.Delete(files[i]);
		}
	}

	public string getBools()
	{
		tD = "";
		tD = tD + "muteVideos:" + muteVideos + "|";
		tD = tD + "hideBeatBar:" + hideBeatBar + "|";
		tD = tD + "pronouns:" + getPronouns() + "|";
		tD = tD + "fullScreen:" + fullScreen + "|";
		tD = tD + "textSpeed:" + textSpeed + "|";
		tD = tD + "mediaSpeed:" + mediaSpeed + "|";
		tD = tD + "masterVolumeValue:" + masterVolumeValue + "|";
		tD = tD + "videoVolumeValue:" + videoVolumeValue + "|";
		tD = tD + "uiVolumeValue:" + uiVolumeValue + "|";
		tD = tD + "asmrVolumeValue:" + asmrVolumeValue + "|";
		tD = tD + "ttsVolumeValue:" + ttsVolumeValue + "|";
		tD = tD + "launchIC:" + bt.getLaunchOn() + "|";
		tD = tD + "bgPath:" + backgroundImg + "|";
		return tD;
	}

	private static int NormalizePronounValue(int pronounValue)
	{
		switch (pronounValue)
		{
		case 1:
		case 2:
			return pronounValue;
		default:
			return 0;
		}
	}

	private void ApplyPronounSelection(int pronounValue, bool mirrorToRegistry = false)
	{
		pronounsInt = NormalizePronounValue(pronounValue);
		radioHeHim.IsChecked = pronounsInt == 0;
		radioSheHer.IsChecked = pronounsInt == 1;
		radioTheyThem.IsChecked = pronounsInt == 2;
		strokePage.ApplyPronounSetting(pronounsInt);
		if (mirrorToRegistry)
		{
			settingsRegistry.SetNumericValue("pronoun", pronounsInt);
		}
	}

	public void SyncPronounSelectionFromSettings()
	{
		if (settingsRegistry.GetRawValue("pronoun") == null)
		{
			return;
		}
		ApplyPronounSelection(settingsRegistry.GetNumericValue("pronoun", pronounsInt));
		saveOptions();
	}

	private int getPronouns()
	{
		if (radioHeHim.IsChecked == true)
		{
			return 0;
		}
		if (radioSheHer.IsChecked == true)
		{
			return 1;
		}
		return 2;
	}

	private void CheckBox_Click(object sender, RoutedEventArgs e)
	{
		playClickSound();
		CheckBox checkBox = (CheckBox)sender;
		switch (checkBox.Tag as string)
		{
		case "0":
			muteVideos = checkBox.IsChecked.Value;
			imageTagger.setVolume();
			break;
		case "1":
			hideBeatBar = checkBox.IsChecked.Value;
			break;
		case "2":
			fullScreen = checkBox.IsChecked.Value;
			fw.fullScreen(fullScreen);
			break;
		}
	}

	public void saveOptions()
	{
		using StreamWriter streamWriter = File.CreateText(RuntimePaths.OptionsFile);
		streamWriter.Write(getBools());
	}

	public void loadOptions()
	{
		bool flag = false;
		if (!File.Exists(RuntimePaths.OptionsFile))
		{
			SessionTraceLogger.Info("first-run", "options.txt missing; writing default options");
			saveOptions();
		}
		try
		{
			string text;
			using (StreamReader streamReader = File.OpenText(RuntimePaths.OptionsFile))
			{
				text = streamReader.ReadLine();
			}
			string[] array = text.Split("|");
			string[][] array2 = new string[array.Length][];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = array[i].Split(":");
			}
			hideBeatBar = loadDataString("hideBeatBar", array2) == "True";
			optionsHideBeatbar.IsChecked = hideBeatBar;
			backgroundImg = loadDataString("bgPath", array2);
			strokePage.setBackground(backgroundImg);
			imageTagger.setBackground(backgroundImg);
			muteVideos = loadDataString("muteVideos", array2) == "True";
			optionsCheckMute.IsChecked = muteVideos;
			fullScreen = loadDataString("fullScreen", array2) == "False";
			fullScreen = !fullScreen;
			fw.fullScreen(fullScreen);
			optionsFullScreen.IsChecked = fullScreen;
			bool flag2 = loadDataString("launchIC", array2) == "True";
			bt.setLaunchIC(flag2);
			if (flag2)
			{
				bt.launchIC();
			}
			string text2 = loadDataString("pronouns", array2);
			if (int.TryParse(text2, out int result))
			{
				ApplyPronounSelection(result, mirrorToRegistry: true);
				flag = true;
			}
			textSpeedSlider.Value = double.Parse(loadDataString("textSpeed", array2));
			textSpeed = textSpeedSlider.Value;
			strokePage.setTalkSpeed(textSpeed);
			mediaSpeedSlider.Value = double.Parse(loadDataString("mediaSpeed", array2));
			mediaSpeed = mediaSpeedSlider.Value;
			strokePage.imageSpeedAdditive = (int)mediaSpeed;
			masterVolume.Value = double.Parse(loadDataString("masterVolumeValue", array2));
			masterVolumeValue = masterVolume.Value;
			videoVolume.Value = double.Parse(loadDataString("videoVolumeValue", array2));
			videoVolumeValue = videoVolume.Value;
			uiVolume.Value = double.Parse(loadDataString("uiVolumeValue", array2));
			uiVolumeValue = uiVolume.Value;
			asmrVolume.Value = double.Parse(loadDataString("asmrVolumeValue", array2));
			asmrVolumeValue = asmrVolume.Value;
			ttsVolume.Value = double.Parse(loadDataString("ttsVolumeValue", array2));
			ttsVolumeValue = ttsVolume.Value;
		}
		catch (Exception ex)
		{
			SessionTraceLogger.Error("options", "Failed to load options.txt; keeping in-memory defaults", ex);
		}
		if (!flag && settingsRegistry.GetRawValue("pronoun") != null)
		{
			ApplyPronounSelection(settingsRegistry.GetNumericValue("pronoun", pronounsInt));
		}
	}

	private string loadDataString(string variableName, string[][] obj)
	{
		for (int i = 0; i < obj.Length; i++)
		{
			if (obj[i][0].Equals(variableName))
			{
				return obj[i][1];
			}
		}
		return null;
	}

	private void buttonChanged(CheckBox check, Panel panel)
	{
		if (check.IsChecked.Value)
		{
			panel.IsEnabled = true;
			panel.Opacity = 1.0;
		}
		else
		{
			panel.IsEnabled = false;
			panel.Opacity = 0.5;
		}
	}

	private void homework_Click(object sender, RoutedEventArgs e)
	{
		playClickSound();
		base.NavigationService.Navigate(homeworkScreen);
		if (strokePage.getFlagTimeDays("tasksGiven") >= 1 || !File.Exists(RuntimePaths.Flag("tasksGiven")))
		{
			File.WriteAllText(RuntimePaths.Flag("tasksGiven"), DateTime.Now.ToString());
			homeworkScreen.newShop();
		}
		saveOptions();
	}

	private void textSpeedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
	{
		playClickSound();
		textSpeed = textSpeedSlider.Value;
		strokePage.setTalkSpeed(textSpeed);
	}

	private void mediaSpeedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
	{
		playClickSound();
		mediaSpeed = mediaSpeedSlider.Value;
		strokePage.imageSpeedAdditive = (int)mediaSpeed;
	}

	private void Button_Click_1(object sender, RoutedEventArgs e)
	{
		playClickSound();
		OpenFileDialog openFileDialog = new OpenFileDialog();
		openFileDialog.Filter = "Supported Images|*.jpg;*.png|Jpeg file(*.jpg)|*.jpg|Png file(*.png)|*.png|All files|*.*";
		if (openFileDialog.ShowDialog() == true)
		{
			string fileName = openFileDialog.FileName;
			string runtimeRelativePath = Path.GetRelativePath(RuntimePaths.RuntimeRoot, fileName);
			if (!runtimeRelativePath.StartsWith(".." + Path.DirectorySeparatorChar) && runtimeRelativePath != "..")
			{
				backgroundImg = runtimeRelativePath.Replace(Path.DirectorySeparatorChar, '/');
				strokePage.setBackground(backgroundImg);
				imageTagger.setBackground(backgroundImg);
			}
		}
	}

	private void masterVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
	{
		masterVolumeValue = masterVolume.Value;
		strokePage.setMasterVolume(masterVolumeValue);
		playClickSound();
	}

	private void videoVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
	{
		videoVolumeValue = videoVolume.Value;
		strokePage.videoVolumeValue = videoVolumeValue;
		playClickSound();
	}

	private void uiVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
	{
		uiVolumeValue = uiVolume.Value;
		strokePage.uiVolumeValue = uiVolumeValue;
		playClickSound();
	}

	private void asmrVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
	{
		asmrVolumeValue = asmrVolume.Value;
		strokePage.setAsmrVolume(asmrVolumeValue);
		playClickSound();
	}

	private void ttsVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
	{
		ttsVolumeValue = ttsVolume.Value;
		strokePage.ttsVolumeValue = ttsVolumeValue;
		playClickSound();
	}

	private void radio_Click(object sender, RoutedEventArgs e)
	{
		playClickSound();
		ApplyPronounSelection(getPronouns(), mirrorToRegistry: true);
	}

	public void playClickSound()
	{
		if (spa != null)
		{
			spa.Volume = masterVolumeValue / 2.0 * uiVolumeValue / 2.0;
			spa.Play();
		}
	}

	private void bluetoothBtn_Click(object sender, RoutedEventArgs e)
	{
		playClickSound();
		base.NavigationService.Navigate(bt);
	}

	public void OpenBluetooth(NavigationService navigationService)
	{
		playClickSound();
		navigationService?.Navigate(bt);
	}

}
