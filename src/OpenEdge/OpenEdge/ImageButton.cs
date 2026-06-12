using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace OpenEdge;

public class ImageButton : Image
{
	public Grid grid = new Grid();

	public Border border = new Border();

	public int radioButton = -1;

	private bool active = true;

	public ImageButton[] brothers = new ImageButton[0];

	private ImageTagger tagger;

	public string name = "";

	public TextBlock textBox = new TextBlock
	{
		VerticalAlignment = VerticalAlignment.Center,
		HorizontalAlignment = HorizontalAlignment.Center,
		TextAlignment = TextAlignment.Center,
		FontSize = 13.5,
		LineHeight = 15.0,
		TextWrapping = TextWrapping.Wrap,
		TextTrimming = TextTrimming.CharacterEllipsis,
		IsHitTestVisible = false,
		Foreground = new SolidColorBrush(Color.FromRgb(237, 237, 237)),
		Background = Brushes.Transparent,
		FontFamily = new FontFamily("Segoe UI")
	};

	public ImageButton(int radio = -1)
	{
		radioButton = radio;
		grid.Visibility = Visibility.Hidden;
	}

	public ImageButton(ImageTagger tagger, string text, int radio = -1, string toolTipText = "")
	{
		this.tagger = tagger;
		name = text;
		textBox.Text = text;
		radioButton = radio;
		if (toolTipText != "")
		{
			grid.ToolTip = toolTipText;
		}
		textBox.Padding = new Thickness(7.0, 2.0, 7.0, 2.0);
		textBox.FontWeight = FontWeights.Normal;
		grid.Margin = new Thickness(3.0);
		grid.Width = 112.0;
		grid.Height = 54.0;
		grid.Cursor = Cursors.Hand;
		grid.MouseUp += Image_MouseUp;
		border.CornerRadius = new CornerRadius(3.0);
		border.BorderThickness = new Thickness(1.0);
		border.SnapsToDevicePixels = true;
		textBox.MaxWidth = grid.Width - 10.0;
		grid.Children.Add(border);
		grid.Children.Add(textBox);
		changeState(!active);
	}

	private void Image_MouseUp(object sender, MouseButtonEventArgs e)
	{
		tagger.playClickSound();
		changeState(!active);
		if (!active)
		{
			tagger.setAmounts(1);
		}
		else
		{
			tagger.setAmounts(-1);
		}
		tagger.setVolume();
		tagger.ApplyBrowserSelectionTagChange(this, active);
	}

	private void changeState(bool changedState)
	{
		active = changedState;
		if (active)
		{
			if (radioButton > 0)
			{
				ImageButton[] array = brothers;
				foreach (ImageButton imageButton in array)
				{
					if (imageButton.radioButton == radioButton)
					{
						imageButton.changeState(changedState: false);
					}
				}
			}
			active = true;
			border.Background = new SolidColorBrush(Color.FromArgb(230, 70, 48, 88));
			border.BorderBrush = new SolidColorBrush(Color.FromArgb(230, 155, 112, 190));
			textBox.Foreground = Brushes.White;
		}
		else
		{
			border.Background = new SolidColorBrush(Color.FromArgb(180, 20, 20, 26));
			border.BorderBrush = new SolidColorBrush(Color.FromArgb(120, 70, 70, 82));
			textBox.Foreground = new SolidColorBrush(Color.FromArgb(220, 220, 220, 226));
		}
		textBox.Background = Brushes.Transparent;
	}

	public void setActive(bool isActive)
	{
		changeState(isActive);
	}

	public string isActive()
	{
		if (active)
		{
			return textBox.Text;
		}
		return "";
	}
}
