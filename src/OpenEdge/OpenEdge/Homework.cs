using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace OpenEdge;

public partial class Homework : Grid, IComponentConnector
{
	public string title;

	private string description;

	public int reward;

	public int cost;

	private TimeSpan timeToFinish;

	public bool active;

	public int state;

	public int amount = -100;

	private bool timeBased;

	public int originalAmount;

	public DateTime? acceptedMoment;

	public Stopwatch stopwatch = new Stopwatch();

	public TimeSpan originalTimeToFinish;

	private HomeworkScreen homeworkScreen;

	public Homework(HomeworkScreen homeworkScreen, string title, string description, int reward, TimeSpan timeToFinish, int state, int amount, TimeSpan originalTimeToFinish, int originalAmount)
	{
		InitializeComponent();
		this.homeworkScreen = homeworkScreen;
		this.title = title;
		this.description = description;
		this.reward = reward;
		this.amount = amount;
		this.originalTimeToFinish = originalTimeToFinish;
		this.originalAmount = originalAmount;
		cost = (reward - 1) / 2;
		this.timeToFinish = timeToFinish;
		homeworkScreen.addTo(this, state);
		timeRemainingOnTask.Text = timeToFinish.ToString("hh\\:mm\\:ss");
		progressBarRect.Width = 0.0;
		titleText.Text = title;
		if (title != "Add Tags" && title != "Write Lines")
		{
			timeBased = true;
		}
		if (state != 0)
		{
			costBuyText.Visibility = Visibility.Collapsed;
			costText.Visibility = Visibility.Collapsed;
			heartsCost.Visibility = Visibility.Collapsed;
			rewardBuyText.Visibility = Visibility.Collapsed;
		}
		if (state == 2)
		{
			gotAccepted();
		}
		costText.Text = cost.ToString() ?? "";
		descriptionText.Text = description;
		this.state = state;
		setCheckmarkArt();
		rewardText.Text = reward.ToString() ?? "";
		timeText.Text = "complete within " + reward * 6 + " hours";
		if (timeBased)
		{
			durationText.Text = Math.Round(timeToFinish.TotalMinutes, 1) + " minutes";
		}
		else if (title == "Write Lines")
		{
			durationText.Text = "write " + amount + " lines";
		}
		else if (title == "Add Tags")
		{
			durationText.Text = "tag " + amount + " medias";
		}
	}

	public void timeLeft()
	{
		if (!acceptedMoment.HasValue)
		{
			return;
		}
		bool deleted = false;
		base.Dispatcher.Invoke(delegate
		{
			TimeSpan value = TimeSpan.FromHours(reward * 6);
			value = (acceptedMoment + value).Value - DateTime.Now;
			string text = Math.Round(value.TotalHours, 1).ToString() ?? "";
			if (value.TotalHours < 0.0)
			{
				deleted = true;
				Task.Run(delegate
				{
					homeworkScreen.failedTask(this);
				});
				rectWithDetect.IsHitTestVisible = false;
				timeText.Text = "Failed";
				timeText.Foreground = Brushes.Red;
			}
			else
			{
				timeText.Text = "time remaining " + text + " hours";
			}
		});
		if (!deleted)
		{
			Task.Delay(30000).ContinueWith(delegate
			{
				timeLeft();
			});
		}
	}

	private void setCheckmarkArt()
	{
		BitmapImage bitmapImage = new BitmapImage();
		bitmapImage.BeginInit();
		bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
		if (state < 2)
		{
			bitmapImage.UriSource = new Uri(RuntimePaths.Resource("checkGreen.png"));
		}
		else
		{
			bitmapImage.UriSource = new Uri(RuntimePaths.Resource("playGreen.png"));
		}
		bitmapImage.EndInit();
		bitmapImage.Freeze();
		checkMark.Source = bitmapImage;
	}

	public int changeAmount(int change)
	{
		amount += change;
		if (amount == 0)
		{
			homeworkScreen.taskDone();
		}
		return amount;
	}

	public void navigateForward()
	{
		if (homeworkScreen.NavigationService.CanGoForward)
		{
			homeworkScreen.NavigationService.GoForward();
		}
	}

	public void setActiveTask()
	{
		homeworkScreen.activateTask(this);
		Task.Run((Action)checkTimeConstant);
	}

	public void startRunning()
	{
		homeworkScreen.actOnTask(this);
		stopwatch.Start();
	}

	public void discard()
	{
		homeworkScreen.discardHomework(this);
		stopwatch.Stop();
		homeworkScreen.activeTask = null;
	}

	public void checkTimeConstant()
	{
		if (timeBased)
		{
			base.Dispatcher.Invoke(delegate
			{
				timeRemainingOnTask.Text = (timeToFinish - stopwatch.Elapsed).ToString("hh\\:mm\\:ss");
				progressBarRect.Width = (stopwatch.Elapsed + originalTimeToFinish - timeToFinish) / originalTimeToFinish * 200.0;
			});
			if (stopwatch.Elapsed > timeToFinish)
			{
				base.Dispatcher.Invoke(homeworkScreen.taskDone);
			}
		}
		else
		{
			base.Dispatcher.Invoke(delegate
			{
				timeRemainingOnTask.Text = amount.ToString() ?? "";
				if (amount > originalAmount)
				{
					originalAmount = amount;
				}
				progressBarRect.Width = (double)(originalAmount - amount) / (double)originalAmount * 200.0;
			});
		}
		if (homeworkScreen.activeTask == this)
		{
			Thread.Sleep(100);
			Task.Delay(100).ContinueWith(delegate
			{
				checkTimeConstant();
			});
		}
	}

	public void resetHomework()
	{
		active = false;
		state = 2;
		checkMark.Visibility = Visibility.Collapsed;
		acceptBtn.Visibility = Visibility.Collapsed;
		heartsEarned.Visibility = Visibility.Collapsed;
		rewardText.Visibility = Visibility.Collapsed;
		setCheckmarkArt();
	}

	public void noLongerActive()
	{
		timeToFinish -= stopwatch.Elapsed;
		homeworkScreen.activeTask = null;
		stopwatch.Reset();
		homeworkScreen.saveTasks();
		gotAccepted();
	}

	public void pauseTimer()
	{
		homeworkScreen.payForPause(1);
		stopwatch.Stop();
	}

	public void gotAccepted()
	{
		resetHomework();
		checkTimeConstant();
		heartsEarned.Visibility = Visibility.Collapsed;
		rewardText.Visibility = Visibility.Collapsed;
		rewardBuyText.Visibility = Visibility.Collapsed;
		durationText.Visibility = Visibility.Collapsed;
		progressbarGrid.Visibility = Visibility.Visible;
	}

	public void boughtHomework()
	{
		active = false;
		homeworkScreen.activateHomework(state);
		state = 1;
		costBuyText.Visibility = Visibility.Collapsed;
		costText.Visibility = Visibility.Collapsed;
		heartsCost.Visibility = Visibility.Collapsed;
		rewardBuyText.Visibility = Visibility.Collapsed;
		checkMark.Visibility = Visibility.Collapsed;
	}

	public void setActiveFalse()
	{
		checkMark.Visibility = Visibility.Hidden;
		active = false;
	}

	private void acceptBtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		homeworkScreen.playClickSound();
		if (state == 2)
		{
			homeworkScreen.setActiveTask(this);
			return;
		}
		if (checkMark.Visibility == Visibility.Visible)
		{
			checkMark.Visibility = Visibility.Hidden;
			active = false;
		}
		else
		{
			checkMark.Visibility = Visibility.Visible;
			active = true;
		}
		if (state != 2)
		{
			homeworkScreen.activateHomework(state);
		}
	}

	public void playClickSound()
	{
		homeworkScreen.playClickSound();
	}

	public string taskAsString()
	{
		string[] obj = new string[17]
		{
			title,
			"|",
			description,
			"|",
			reward.ToString(),
			"|",
			timeToFinish.ToString(),
			"|",
			state.ToString(),
			"|",
			amount.ToString(),
			"|",
			originalTimeToFinish.ToString(),
			"|",
			originalAmount.ToString(),
			"|",
			null
		};
		DateTime? dateTime = acceptedMoment;
		obj[16] = dateTime.ToString();
		return string.Concat(obj);
	}
}
