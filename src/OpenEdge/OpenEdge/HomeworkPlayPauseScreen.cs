using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Imaging;

namespace OpenEdge;

public partial class HomeworkPlayPauseScreen : Grid, IComponentConnector
{
	private Homework homework;

	private bool playing;

	public HomeworkPlayPauseScreen(Homework homework)
	{
		InitializeComponent();
		this.homework = homework;
		new Thread(setPopupValues).Start();
		if (homework.title == "Write Lines" || homework.title == "Add Tags")
		{
			folderBtn.Visibility = Visibility.Collapsed;
			folderBtn.Content = "return";
		}
		else if (homework.title != "Add Images" && homework.title != "Add Videos")
		{
			folderBtn.Visibility = Visibility.Collapsed;
		}
	}

	public void setFolderBtnVis()
	{
		folderBtn.Visibility = Visibility.Visible;
	}

	private void setPopupValues()
	{
		base.Dispatcher.Invoke(delegate
		{
			titleText.Text = homework.titleText.Text;
			descriptionText.Text = homework.descriptionText.Text;
			timeText.Text = homework.timeText.Text;
			timeRemainingOnTask.Text = homework.timeRemainingOnTask.Text;
			progressBarRect.Width = homework.progressBarRect.Width * 2.5;
			costOfDiscard.Text = "-" + (homework.reward + 1);
			if (playing)
			{
				pauseGrid.Visibility = Visibility.Visible;
			}
			else
			{
				pauseGrid.Visibility = Visibility.Hidden;
			}
		});
		Thread.Sleep(100);
		Task.Delay(100).ContinueWith(delegate
		{
			setPopupValues();
		});
	}

	private void Button_ClickX(object sender, RoutedEventArgs e)
	{
		homework.playClickSound();
		((Grid)((Grid)base.Parent).Parent).Visibility = Visibility.Collapsed;
		homework.noLongerActive();
	}

	private void PlayBtn(object sender, RoutedEventArgs e)
	{
		homework.playClickSound();
		BitmapImage bitmapImage = new BitmapImage();
		bitmapImage.BeginInit();
		bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
		if (!playing)
		{
			homework.startRunning();
			playing = true;
			if (homework.title == "Add Videos" || homework.title == "Add Images")
			{
				folderBtn.Visibility = Visibility.Visible;
			}
			bitmapImage.UriSource = new Uri(RuntimePaths.Resource("pauseGreen.png"));
			pauseGrid.Visibility = Visibility.Visible;
			closePopUp.Visibility = Visibility.Collapsed;
		}
		else
		{
			homework.pauseTimer();
			playing = false;
			folderBtn.Visibility = Visibility.Collapsed;
			bitmapImage.UriSource = new Uri(RuntimePaths.Resource("playGreen.png"));
			pauseGrid.Visibility = Visibility.Hidden;
			closePopUp.Visibility = Visibility.Visible;
		}
		bitmapImage.EndInit();
		bitmapImage.Freeze();
		playPauseImg.Source = bitmapImage;
	}

	private void DiscardBtn(object sender, RoutedEventArgs e)
	{
		homework.playClickSound();
		((Grid)((Grid)base.Parent).Parent).Visibility = Visibility.Collapsed;
		homework.discard();
		homework.noLongerActive();
	}

	private void FolderBtn(object sender, RoutedEventArgs e)
	{
		homework.playClickSound();
		if (homework.title == "Add Images")
		{
			Process.Start(Environment.GetEnvironmentVariable("WINDIR") + "\\explorer.exe", RuntimePaths.ImagesDir);
		}
		else if (homework.title == "Add Videos")
		{
			Process.Start(Environment.GetEnvironmentVariable("WINDIR") + "\\explorer.exe", RuntimePaths.VideosDir);
		}
		else if (homework.title == "Write Lines" || homework.title == "Add Tags")
		{
			homework.navigateForward();
		}
	}

	public void taskComplete()
	{
		base.Dispatcher.Invoke(delegate
		{
			((Grid)((Grid)base.Parent).Parent).Visibility = Visibility.Visible;
			completeScreen.Visibility = Visibility.Visible;
			timeRemainingOnTask.Text = "";
		});
		Thread.Sleep(5000);
		base.Dispatcher.Invoke(delegate
		{
			((Grid)((Grid)base.Parent).Parent).Visibility = Visibility.Collapsed;
		});
	}
}
