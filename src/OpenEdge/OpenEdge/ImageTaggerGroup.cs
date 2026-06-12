using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace OpenEdge;

public partial class ImageTaggerGroup : Grid, IComponentConnector
{
	private ImageTagger tagger;

	public ImageTaggerGroup(ImageTagger tagger)
	{
		InitializeComponent();
		this.tagger = tagger;
	}

	private void Button_Click(object sender, RoutedEventArgs e)
	{
		tagger.playClickSound();
		int num = int.Parse(((Button)sender).Tag.ToString());
		tagger.tagGroups[num] = tagger.getTagsOfCurrentImage();
		textWithShortcut.Text = "saved the current tags, press " + (num + 1) + " to apply them to a media \nthe current tags are: " + tagger.tagGroups[num];
		textWithShortcut.Visibility = Visibility.Visible;
		saveGroups();
	}

	private void closePopUp_Click(object sender, RoutedEventArgs e)
	{
		tagger.playClickSound();
		tagger.removeTaggerGroup(this);
	}

	private void saveGroups()
	{
		List<string> list = new List<string>();
		for (int i = 0; i < 5; i++)
		{
			list.Add(tagger.tagGroups[i]);
		}
		File.WriteAllLines(RuntimePaths.TagGroupsFile, list);
	}

	private void Button_MouseEnter(object sender, MouseEventArgs e)
	{
		int num = int.Parse(((Button)sender).Tag.ToString());
		textWithShortcut.Text = "Group " + (num + 1) + ", the current tags are: " + tagger.tagGroups[num];
		textWithShortcut.Visibility = Visibility.Visible;
	}

	private void Button_MouseLeave(object sender, MouseEventArgs e)
	{
		textWithShortcut.Visibility = Visibility.Hidden;
	}
}
