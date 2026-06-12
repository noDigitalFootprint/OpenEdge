using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using OpenEdge.helper;

namespace OpenEdge;

public partial class WriteTask : Page, IComponentConnector
{
	private string activeSentence = "";

	private int repeatAmount = 4;

	private int pointerX;

	private int pointerY;

	private ImageTagger imgTagger;

	private HomeworkScreen hwScreen;

	private Voc voc;

	private LineReader lr;

	public WriteTask(HomeworkScreen hwScreen, Page1 p1)
	{
		imgTagger = p1.imageTagger;
		voc = new Voc(p1.strokePage);
		lr = new LineReader(voc.mw);
		this.hwScreen = hwScreen;
		InitializeComponent();
		pickSentence();
		repeatAmount = hwScreen.activeTask.amount;
		createTextBlocks();
		base.Loaded += WriteTask_Loaded;
	}

	private void pickSentence()
	{
		textBox.Focus();
		activeSentence = lr.getVocab("writeTask");
	}

	private void createTextBlocks()
	{
		for (int i = 0; i < repeatAmount; i++)
		{
			TextBlock element = new TextBlock
			{
				FontFamily = new FontFamily("Times New Roman"),
				FontWeight = FontWeights.Bold,
				Margin = new Thickness(2.0, -2.0, 2.0, -2.0)
			};
			textStack.Children.Add(element);
		}
	}

	private void WriteTask_Loaded(object sender, RoutedEventArgs e)
	{
		firstSentence.Text = activeSentence;
		textStack.Rows = 20;
		setNewMedia();
		Task.Run((Action)keepSettingNewMedia);
	}

	private void setNewMedia()
	{
		Random random = new Random();
		if (lr.talkBaseClass.tags == "")
		{
			List<string> images = imgTagger.images;
			images.AddRange(imgTagger.videos);
			Uri source = new Uri(RuntimePaths.ResolveRuntimePath(images[random.Next(images.Count - 1)]));
			focussedBg.Source = source;
			blurredBg.Source = source;
		}
		else
		{
			Uri source2 = new Uri(imgTagger.allTaggedWith(lr.talkBaseClass.tags)[random.Next(imgTagger.allTaggedWith(lr.talkBaseClass.tags).Count - 1)]);
			focussedBg.Source = source2;
			blurredBg.Source = source2;
		}
		mediaElement_OnMediaEnded(null, null);
	}

	private void keepSettingNewMedia()
	{
		base.Dispatcher.Invoke(setNewMedia);
		Random random = new Random();
		Thread.Sleep(10000 + random.Next(100000));
		keepSettingNewMedia();
	}

	private void mediaElement_OnMediaEnded(object sender, RoutedEventArgs e)
	{
		focussedBg.Position = new TimeSpan(0L);
		focussedBg.Play();
		blurredBg.Position = new TimeSpan(0L);
		blurredBg.Play();
	}

	private void showTaskClick(object sender, RoutedEventArgs e)
	{
		hwScreen.playClickSound();
		base.NavigationService.GoBack();
	}

	private void textBox_KeyUp(object sender, KeyEventArgs e)
	{
		textBox.Focus();
		string text = textBox.Text;
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		if (text.Last().ToString().ToLower() == activeSentence[pointerX].ToString().ToLower())
		{
			TextBlock obj = (TextBlock)textStack.Children[pointerY + 1];
			pointerX++;
			obj.Text = activeSentence.Remove(pointerX);
			if (pointerX >= activeSentence.Length)
			{
				pointerX = 0;
				pointerY++;
				if (pointerY == 40)
				{
					textStack.Rows = 38;
				}
				hwScreen.activeTask.changeAmount(-1);
				if (pointerY >= repeatAmount)
				{
					base.NavigationService.GoBack();
				}
			}
		}
		textBox.Text = "";
	}

	private void textBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
	{
		textBox.Focus();
	}
}
