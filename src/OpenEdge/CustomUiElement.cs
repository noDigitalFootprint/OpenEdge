using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using OpenEdge;

public class CustomUiElement : Grid
{
	private Image mediaElement;

	private Image copyImage;

	private bool landscapeMode;

	public TextBlock textBlock;

	private bool currentlyCensored;

	public string location = "";

	public CustomUiElement(Image mediaElement, bool landscapeMode, TextBlock block, Brush foreground, Brush background)
	{
		this.mediaElement = mediaElement;
		this.landscapeMode = landscapeMode;
		textBlock = block;
		copyImage = new Image
		{
			Source = mediaElement.Source
		};
		setChildren();
	}

	private void setChildren()
	{
		base.Children.Add(copyImage);
		base.Children.Add(mediaElement);
	}

	public void setText(TextBlock block)
	{
		textBlock = block;
		base.Children.Add(textBlock);
		if (currentlyCensored)
		{
			textBlockVisible();
		}
		else
		{
			textBlock.Visibility = Visibility.Collapsed;
		}
	}

	public void blurMediaElement(SecondWindow sW)
	{
		new Random();
		if (sW.censorMode == 3)
		{
			currentlyCensored = true;
			BlurEffect blurEffect = new BlurEffect();
			blurEffect.KernelType = KernelType.Gaussian;
			blurEffect.Radius = 60f * sW.censorIntensity;
			mediaElement.Effect = blurEffect;
			textBlockVisible();
		}
		else
		{
			currentlyCensored = false;
			mediaElement.Effect = null;
			textBlock.Visibility = Visibility.Hidden;
		}
	}

	public bool getLandscapeMode()
	{
		return landscapeMode;
	}

	public void setMediaElement(BitmapImage bitmap, bool hypnosisOn, string location)
	{
		this.location = location;
		base.Dispatcher.Invoke(delegate
		{
			mediaElement.Source = bitmap;
			if (hypnosisOn)
			{
				copyImage.Source = null;
			}
			else
			{
				copyImage.Source = bitmap;
			}
		});
	}

	public void textBlockVisible()
	{
		textBlock.Visibility = Visibility.Visible;
	}

	public Image getMediaElement()
	{
		return mediaElement;
	}

	public Image getMediaCopy()
	{
		return copyImage;
	}

	public void setImgCopyAsBg()
	{
		BlurEffect blurEffect = new BlurEffect();
		copyImage.HorizontalAlignment = HorizontalAlignment.Center;
		copyImage.VerticalAlignment = VerticalAlignment.Center;
		copyImage.Margin = new Thickness(-60.0);
		blurEffect.KernelType = KernelType.Gaussian;
		blurEffect.Radius = 60.0;
		copyImage.Effect = blurEffect;
		copyImage.Stretch = Stretch.UniformToFill;
	}
}
