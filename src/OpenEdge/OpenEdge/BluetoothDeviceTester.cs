using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using Buttplug.Client;
using Buttplug.Core.Messages;

namespace OpenEdge;

public partial class BluetoothDeviceTester : UserControl, IComponentConnector
{
	public ButtplugClientDevice device;

	private Bluetooth parent;

	public bool wand;

	public bool plug;

	public bool ona;

	private uint duration = 1000u;

	public BluetoothDeviceTester(ButtplugClientDevice device, Bluetooth parent)
	{
		InitializeComponent();
		this.device = device;
		deviceName.Text = device.Name;
		this.parent = parent;
		readyBluetoothDeviceTester(plug: false, wand: false, ona: false);
	}

	public void readyBluetoothDeviceTester(bool plug, bool wand, bool ona)
	{
		setState(0, plug);
		setState(1, wand);
		setState(2, ona);
	}

	public void setState(int mode, bool change)
	{
		string text = "";
		BitmapImage bitmapImage = new BitmapImage();
		switch (mode)
		{
		case 0:
			plug = change;
			if (wand && change)
			{
				setState(1, change: false);
			}
			if (ona && change)
			{
				setState(2, change: false);
			}
			if (plug)
			{
				selectedType.Text = "Buttplug";
				text = RuntimePaths.Resource("plug.png");
			}
			else
			{
				text = RuntimePaths.Resource("plugNo.png");
			}
			bitmapImage.BeginInit();
			bitmapImage.UriSource = new Uri(text);
			bitmapImage.EndInit();
			plugImage.Source = bitmapImage;
			break;
		case 1:
			wand = change;
			if (plug && change)
			{
				setState(0, change: false);
			}
			if (ona && change)
			{
				setState(2, change: false);
			}
			if (wand)
			{
				selectedType.Text = "Vibrator";
				text = RuntimePaths.Resource("wand.png");
			}
			else
			{
				text = RuntimePaths.Resource("wandNo.png");
			}
			bitmapImage.BeginInit();
			bitmapImage.UriSource = new Uri(text);
			bitmapImage.EndInit();
			wandImage.Source = bitmapImage;
			break;
		case 2:
			ona = change;
			if (plug && change)
			{
				setState(0, change: false);
			}
			if (wand && change)
			{
				setState(1, change: false);
			}
			if (ona)
			{
				selectedType.Text = "Stroker";
				text = RuntimePaths.Resource("ona.png");
			}
			else
			{
				text = RuntimePaths.Resource("onaNo.png");
			}
			bitmapImage.BeginInit();
			bitmapImage.UriSource = new Uri(text);
			bitmapImage.EndInit();
			onaImage.Source = bitmapImage;
			break;
		}
		if (!plug && !wand && !ona)
		{
			selectedType.Text = "Not Set";
		}
	}

	private void stroker(double position, int repeat = 0)
	{
		device.LinearAsync(duration, position);
		if (position == 1.0)
		{
			position = 0.0;
		}
		else if (position == 0.0)
		{
			position = 1.0;
		}
		Thread.Sleep((int)duration);
		if (repeat < 8)
		{
			stroker(position, repeat + 1);
		}
	}

	private void plugImage_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		setState(0, !plug);
		parent.saveBluetoothDevices();
	}

	private void wandImage_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		setState(1, !wand);
		parent.saveBluetoothDevices();
	}

	private void onaImage_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		setState(2, !ona);
		parent.saveBluetoothDevices();
	}

	public bool stillConnected(int type)
	{
		return type switch
		{
			0 => plug, 
			1 => wand, 
			2 => ona, 
			_ => false, 
		};
	}

	private void Button_Click(object sender, RoutedEventArgs e)
	{
		Task.Run(delegate
		{
			device.VibrateAsync(0.3);
			device.OscillateAsync(0.3);
			device.RotateAsync(0.3, clockwise: true);
			Task.Run(delegate
			{
				stroker(1.0);
			});
			Thread.Sleep(800);
			device.Stop();
		});
	}

	private async void btLinearAsync(ButtplugClientDevice device, float bpm)
	{
		_ = 60f / bpm / 2f;
		List<LinearCmd.VectorCommand> cmds = new List<LinearCmd.VectorCommand>();
		await device.LinearAsync(cmds);
	}

	public string deviceString()
	{
		return device.Name + "|" + plug + "|" + wand + "|" + ona;
	}
}
