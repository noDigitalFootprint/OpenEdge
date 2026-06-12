using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace OpenEdge;

public partial class HomeworkScreen : Page, IComponentConnector
{
	public Homework activeTask;

	public int totalCash;

	private int maxOwnedTasks = 6;

	private int maxAcceptedTasks = 3;

	public Page1 optionsPage;

	private string[] titles = new string[6] { "Add Videos", "Add Images", "Add Tags", "Watch Porn", "Write Lines", "Censor Media" };

	private string[] descriptions = new string[6] { "Look up videos online to add to the video folder", "Look up images online to add to the images folder", "Add tags to the images and videos you've already gathered", "Watch porn for me, you aren't allowed to touch yourself of course", "Write lines for me, if you repeat them often enough they won't ever leave you", "Censor your own media, make sure to overwrite the original" };

	private int[] rewards = new int[12]
	{
		1, 2, 3, 4, 5, 6, 7, 8, 9, 10,
		11, 12
	};

	private int[] minutes = new int[12]
	{
		5, 10, 16, 22, 29, 36, 44, 52, 61, 70,
		80, 90
	};

	public List<Homework> allHomework = new List<Homework>();

	public void discardHomework(Homework hw)
	{
		base.Dispatcher.Invoke(delegate
		{
			acceptedHomeworkStack.Children.Remove(hw);
			allHomework.Remove(hw);
			titleAcceptedTasks.Text = "Accepted Tasks (" + acceptedHomeworkStack.Children.Count + "/" + maxAcceptedTasks + ")";
			setCurrency(totalCash - (hw.reward + 1));
		});
	}

	private void setCurrency(int currency)
	{
		totalCurrency.Text = currency.ToString() ?? "";
		totalCash = currency;
		optionsPage.setStartSessionBtnDebt(currency);
	}

	public void payForPause(int amount)
	{
		setCurrency(totalCash - amount);
	}

	public void actOnTask(Homework homework)
	{
		HomeworkPlayPauseScreen homeworkPlayPauseScreen = (HomeworkPlayPauseScreen)playPausePopContent.Children[playPausePopContent.Children.Count - 1];
		if (homework.title == "Add Tags")
		{
			optionsPage.imageTagger.setForcedTagAmount(homework.amount);
			optionsPage.TagImages(this);
			optionsPage.imageTagger.displayShowTaskBtn(show: true, this);
			homeworkPlayPauseScreen.setFolderBtnVis();
		}
		else if (homework.title == "Write Lines")
		{
			WriteTask root = new WriteTask(this, optionsPage);
			base.NavigationService.Navigate(root);
			homeworkPlayPauseScreen.setFolderBtnVis();
		}
	}

	public HomeworkScreen(Page1 optionsPage)
	{
		InitializeComponent();
		this.optionsPage = optionsPage;
		loadTasks();
		titleOwnedTasks.Text = "Stored Tasks (" + homeworkStack.Children.Count + "/" + maxOwnedTasks + ")";
		titleAcceptedTasks.Text = "Accepted Tasks (" + acceptedHomeworkStack.Children.Count + "/" + maxAcceptedTasks + ")";
		setCurrency(totalCash);
	}

	public void createNewTask(int state = 0, int length = -1, int type = -1)
	{
		Random random = new Random();
		List<int> list = new List<int> { 0, 1, 2, 4 };
		if (optionsPage.imageTagger.hasManyUntaggedImages())
		{
			list.Add(3);
		}
		if (File.Exists(RuntimePaths.Flag("censorship")))
		{
			list.Add(5);
		}
		int temp1 = list[random.Next(list.Count)];
		int temp2 = random.Next(rewards.Length);
		if (type != -1)
		{
			temp1 = type;
		}
		if (length != -1)
		{
			temp2 = length;
		}
		Homework homework;
		base.Dispatcher.Invoke(delegate
		{
			if (temp1 != 3)
			{
				homework = new Homework(this, titles[temp1], descriptions[temp1], rewards[temp2], TimeSpan.FromMinutes(minutes[temp2]), state, minutes[temp2], TimeSpan.FromMinutes(minutes[temp2]), minutes[temp2]);
			}
			else
			{
				homework = new Homework(this, titles[temp1], descriptions[temp1], rewards[temp2], TimeSpan.FromMinutes(minutes[temp2]), state, 3 * minutes[temp2], TimeSpan.FromMinutes(minutes[temp2]), 3 * minutes[temp2]);
			}
			allHomework.Add(homework);
			if (state == 2)
			{
				homework.acceptedMoment = DateTime.Now;
			}
			saveTasks();
		});
	}

	public void addTo(Homework homework, int state)
	{
		switch (state)
		{
		case 0:
			shop.Children.Add(homework);
			break;
		case 1:
			homeworkStack.Children.Add(homework);
			break;
		default:
			acceptedHomeworkStack.Children.Add(homework);
			break;
		}
	}

	private void btnBackClick(object sender, RoutedEventArgs e)
	{
		playClickSound();
		backToMenu();
	}

	private void Button_Click_ClosePopup(object sender, RoutedEventArgs e)
	{
		playClickSound();
		popQuestionmark.Visibility = Visibility.Hidden;
	}

	private void Button_Click_OpenPopup(object sender, MouseButtonEventArgs e)
	{
		playClickSound();
		popQuestionmark.Visibility = Visibility.Visible;
	}

	private void Button_Click_OpenPopup(object sender, RoutedEventArgs e)
	{
		playClickSound();
		popQuestionmark.Visibility = Visibility.Visible;
	}

	public void newShop()
	{
		List<Homework> list = new List<Homework>();
		foreach (Homework item in allHomework)
		{
			if (item.state == 0)
			{
				shop.Children.Remove(item);
			}
			else
			{
				list.Add(item);
			}
		}
		allHomework = list;
		for (int i = 0; i < 5; i++)
		{
			createNewTask();
		}
	}

	public void failedTask(Homework homework)
	{
		base.Dispatcher.Invoke(delegate
		{
			allHomework.Remove(homework);
			setCurrency(totalCash - homework.reward);
			saveTasks();
		});
		Thread.Sleep(10000);
		base.Dispatcher.Invoke(delegate
		{
			homeworkStack.Children.Remove(homework);
			acceptedHomeworkStack.Children.Remove(homework);
			titleAcceptedTasks.Text = "Accepted Tasks (" + acceptedHomeworkStack.Children.Count + "/" + maxAcceptedTasks + ")";
		});
	}

	private void acceptChallengesClick(object sender, RoutedEventArgs e)
	{
		playClickSound();
		int num = 0;
		int num2 = 0;
		foreach (Homework item in allHomework)
		{
			if (item.active)
			{
				if (item.state == 0)
				{
					shop.Children.Remove(item);
					homeworkStack.Children.Add(item);
					item.boughtHomework();
					num2 += item.cost;
					titleOwnedTasks.Text = "Stored Tasks (" + homeworkStack.Children.Count + "/" + maxOwnedTasks + ")";
				}
				else if (item.state == 1)
				{
					item.acceptedMoment = DateTime.Now;
					Task.Run((Action)item.timeLeft);
					homeworkStack.Children.Remove(item);
					acceptedHomeworkStack.Children.Add(item);
					item.gotAccepted();
					item.resetHomework();
					num += item.reward;
					titleAcceptedTasks.Text = "Accepted Tasks (" + acceptedHomeworkStack.Children.Count + "/" + maxAcceptedTasks + ")";
					titleOwnedTasks.Text = "Stored Tasks (" + homeworkStack.Children.Count + "/" + maxOwnedTasks + ")";
				}
			}
		}
		acceptTasksBtn.Visibility = Visibility.Collapsed;
		buyTasksBtn.Visibility = Visibility.Collapsed;
		setCurrency(totalCash - num2);
		setCurrency(totalCash + num);
		saveTasks();
	}

	public void taskDone()
	{
		if (playPausePop.Children.Count >= 1)
		{
			Task.Run((Action)((HomeworkPlayPauseScreen)playPausePopContent.Children[playPausePopContent.Children.Count - 1]).taskComplete);
		}
		acceptedHomeworkStack.Children.Remove(activeTask);
		allHomework.Remove(activeTask);
		titleAcceptedTasks.Text = "Accepted Tasks (" + acceptedHomeworkStack.Children.Count + "/" + maxAcceptedTasks + ")";
		activeTask = null;
		saveTasks();
	}

	public void backToMenu()
	{
		base.NavigationService.GoBack();
	}

	public void setActiveTask(Homework hw)
	{
		if (activeTask == null)
		{
			HomeworkPlayPauseScreen element = new HomeworkPlayPauseScreen(hw);
			playPausePopContent.Children.Add(element);
			playPausePop.Visibility = Visibility.Visible;
			activeTask = hw;
			hw.setActiveTask();
		}
	}

	public void activateHomework(int state)
	{
		beginTaskBtn.Visibility = Visibility.Collapsed;
		int num = 0;
		int num2 = 0;
		foreach (Homework item in allHomework)
		{
			if (item.active)
			{
				if (item.state == state && state != 0)
				{
					num += item.reward;
					num2++;
				}
				else if (item.state == state)
				{
					num += item.cost;
					num2++;
				}
				else
				{
					item.setActiveFalse();
				}
			}
		}
		if (num2 > 0)
		{
			if (state == 0 && num2 + homeworkStack.Children.Count <= maxOwnedTasks)
			{
				buyTasksBtn.Visibility = Visibility.Visible;
				acceptTasksBtn.Visibility = Visibility.Collapsed;
				priceOfTask.Text = num.ToString() ?? "";
			}
			else if (state == 1 && num2 + acceptedHomeworkStack.Children.Count <= maxAcceptedTasks)
			{
				acceptTasksBtn.Visibility = Visibility.Visible;
				buyTasksBtn.Visibility = Visibility.Collapsed;
				numberOfFavor.Text = num.ToString() ?? "";
			}
			else
			{
				acceptTasksBtn.Visibility = Visibility.Collapsed;
				buyTasksBtn.Visibility = Visibility.Collapsed;
			}
		}
		else
		{
			acceptTasksBtn.Visibility = Visibility.Collapsed;
			buyTasksBtn.Visibility = Visibility.Collapsed;
		}
	}

	public void saveTasks()
	{
		List<string> list = new List<string>();
		foreach (Homework item in allHomework)
		{
			list.Add(item.taskAsString());
		}
		File.WriteAllText(RuntimePaths.Flag("favor"), totalCash.ToString() ?? "");
		File.WriteAllLines(RuntimePaths.TasksFile, list);
	}

	public void loadTasks()
	{
		if (File.Exists(RuntimePaths.TasksFile))
		{
			string[] array = File.ReadAllLines(RuntimePaths.TasksFile);
			if (array.Length != 0)
			{
				for (int i = 0; i < array.Length; i++)
				{
					string[] array2 = array[i].Split("|", StringSplitOptions.RemoveEmptyEntries);
					try
					{
						allHomework.Add(new Homework(this, array2[0], array2[1], int.Parse(array2[2]), TimeSpan.Parse(array2[3]), int.Parse(array2[4]), int.Parse(array2[5]), TimeSpan.Parse(array2[6]), int.Parse(array2[7])));
					}
					catch
					{
					}
					if (array2.Length > 8)
					{
						try
						{
							allHomework[allHomework.Count - 1].acceptedMoment = DateTime.Parse(array2[8]);
						}
						catch
						{
							allHomework[allHomework.Count - 1].acceptedMoment = DateTime.Now;
						}
						Task.Run((Action)allHomework[allHomework.Count - 1].timeLeft);
					}
				}
			}
		}
		if (File.Exists(RuntimePaths.Flag("favor")))
		{
			string text = File.ReadAllText(RuntimePaths.Flag("favor"));
			if (text.Length > 0)
			{
				setCurrency(int.Parse(text));
			}
			else
			{
				setCurrency(5);
			}
		}
	}

	public void playClickSound()
	{
		optionsPage.playClickSound();
	}

	public void activateTask(Homework hw)
	{
		if (hw.active)
		{
			for (int i = 0; i < allHomework.Count; i++)
			{
				if (hw != allHomework[i])
				{
					allHomework[i].setActiveFalse();
				}
			}
			beginTaskBtn.Visibility = Visibility.Visible;
		}
		else
		{
			beginTaskBtn.Visibility = Visibility.Collapsed;
		}
		acceptTasksBtn.Visibility = Visibility.Collapsed;
		buyTasksBtn.Visibility = Visibility.Collapsed;
	}
}
