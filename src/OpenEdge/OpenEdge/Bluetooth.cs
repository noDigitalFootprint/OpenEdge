using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using Buttplug.Client;
using Microsoft.Win32;

namespace OpenEdge;

public partial class Bluetooth : Page, IComponentConnector
{
	private ButtplugClient client;

	private ButtplugWebsocketConnector buttPlugServer;

	private Page1 p;

	private bool scanning;

	private bool launchOn;

	public Bluetooth()
	{
		InitializeComponent();
		client = new ButtplugClient("OpenEdge Client");
		try
		{
			serverAddress.Text = File.ReadAllText(RuntimePaths.ICAddressFile);
		}
		catch
		{
		}
		client.DeviceAdded += HandleDeviceAdded;
		client.DeviceRemoved += HandleDeviceRemoved;
		Task.Run((Action)connectToIC);
		void HandleDeviceAdded(object aObj, DeviceAddedEventArgs aArgs)
		{
			base.Dispatcher.Invoke(delegate
			{
				setBtText("Device connected: " + aArgs.Device.Name);
				addDevice(aArgs.Device);
				string text = "";
				ButtplugClientDevice[] devices = client.Devices;
				foreach (ButtplugClientDevice buttplugClientDevice in devices)
				{
					text = text + buttplugClientDevice.Name + "\n";
				}
			});
		}
		void HandleDeviceRemoved(object aObj, DeviceRemovedEventArgs aArgs)
		{
			base.Dispatcher.Invoke(delegate
			{
				setBtText("Device removed: " + aArgs.Device.Name + "\n");
				removeDevice(aArgs.Device.Name);
				string text = "";
				ButtplugClientDevice[] devices = client.Devices;
				foreach (ButtplugClientDevice buttplugClientDevice in devices)
				{
					text = text + buttplugClientDevice.Name + "\n";
				}
			});
		}
	}

	public void setP(Page1 p)
	{
		this.p = p;
	}

	public void setLaunchIC(bool launchOn)
	{
		this.launchOn = launchOn;
		onStartupIC.IsChecked = launchOn;
	}

	public bool stillConnected(ButtplugClientDevice device, int type)
	{
		new List<ButtplugClientDevice>();
		bool result = false;
		base.Dispatcher.Invoke(delegate
		{
			foreach (BluetoothDeviceTester child in bluetoothDevicesStack.Children)
			{
				if (device == child.device)
				{
					result = child.stillConnected(type);
					break;
				}
			}
		});
		return result;
	}

	public bool getLaunchOn()
	{
		return launchOn;
	}

	private void ButtPlugServer_Disconnected(object sender, EventArgs e)
	{
		setBtText("disconnected, did you shut down IC?");
		Task.Run((Action)connectToIC);
	}

	private void saveAddress()
	{
		base.Dispatcher.Invoke(delegate
		{
			File.WriteAllText(RuntimePaths.ICAddressFile, serverAddress.Text.Trim());
		});
	}

	private async void connectToIC()
	{
		try
		{
			base.Dispatcher.Invoke(delegate
			{
				try
				{
					buttPlugServer = new ButtplugWebsocketConnector(new Uri(serverAddress.Text));
				}
				catch
				{
					btText.Text = "FAILED to connected to IC\n";
				}
			});
			await client.ConnectAsync(buttPlugServer);
			setBtText("SUCCESFULLY connected to IC");
			buttPlugServer.Disconnected += ButtPlugServer_Disconnected;
			await client.StartScanningAsync();
			await Task.Delay(4000);
			await client.StopScanningAsync();
		}
		catch
		{
			await Task.Delay(1000);
			connectToIC();
		}
	}

	private void btnBackClick(object sender, RoutedEventArgs e)
	{
		playClickSound();
		backToMenu();
	}

	public void backToMenu()
	{
		playClickSound();
		base.NavigationService.GoBack();
	}

	public void playClickSound()
	{
		p.playClickSound();
	}

	private void searchBt_Click(object sender, RoutedEventArgs e)
	{
		playClickSound();
		if (scanning)
		{
			client.StopScanningAsync();
			searchBt.Content = "Start Scanning";
			setBtText("stopped scanning for new devices");
		}
		else
		{
			client.StartScanningAsync();
			searchBt.Content = "Stop Scanning";
			setBtText("started scanning for new devices");
		}
		scanning = !scanning;
	}

	private void launchBt_Click(object sender, RoutedEventArgs e)
	{
		playClickSound();
		launchIC();
	}

	public void launchIC()
	{
		try
		{
			Process.Start(File.ReadAllText(RuntimePaths.ICPathFile));
			setBtText("started IC, make sure to start the server");
			string text = "";
			ButtplugClientDevice[] devices = client.Devices;
			foreach (ButtplugClientDevice buttplugClientDevice in devices)
			{
				text = text + buttplugClientDevice.Name + "\n";
				addDevice(buttplugClientDevice);
			}
		}
		catch
		{
			setBtText("failed to start IC, have you selected Intiface_Central.exe using Find IC?");
		}
	}

	private void onStartupIC_Click(object sender, RoutedEventArgs e)
	{
		playClickSound();
		launchOn = onStartupIC.IsChecked.Value;
		p.saveOptions();
	}

	private void clickCheckmark()
	{
		playClickSound();
		onStartupIC.IsChecked = !onStartupIC.IsChecked;
		launchOn = onStartupIC.IsChecked.Value;
		p.saveOptions();
	}

	private void setBtText(string text)
	{
		base.Dispatcher.Invoke(delegate
		{
			TextBlock textBlock = btText;
			textBlock.Text = textBlock.Text + text + "\n";
			TextBlock textBlock2 = btTextTimestamp;
			textBlock2.Text = textBlock2.Text + DateTime.Now.ToShortTimeString() + "\n";
			btTextScroll.ScrollToVerticalOffset(btText.ActualHeight);
		});
	}

	private void addDevice(ButtplugClientDevice device)
	{
		bool flag = false;
		foreach (BluetoothDeviceTester child in bluetoothDevicesStack.Children)
		{
			if (child.device.Name == device.Name)
			{
				flag = true;
			}
		}
		if (flag)
		{
			return;
		}
		BluetoothDeviceTester bluetoothDeviceTester = new BluetoothDeviceTester(device, this);
		if (File.Exists(RuntimePaths.BtDevicesFile))
		{
			string[] array = File.ReadAllLines(RuntimePaths.BtDevicesFile);
			foreach (string text in array)
			{
				if (!string.IsNullOrEmpty(text))
				{
					string text2 = text.Split("|")[0];
					if (device.Name == text2)
					{
						bluetoothDeviceTester.readyBluetoothDeviceTester(text.Split("|")[1].Equals("True"), text.Split("|")[2].Equals("True"), text.Split("|")[3].Equals("True"));
					}
				}
			}
		}
		bluetoothDevicesStack.Children.Add(bluetoothDeviceTester);
	}

	private void removeDevice(string name)
	{
		for (int i = 0; i < bluetoothDevicesStack.Children.Count; i++)
		{
			if (((BluetoothDeviceTester)bluetoothDevicesStack.Children[i]).device.Name == name)
			{
				bluetoothDevicesStack.Children.RemoveAt(i);
				break;
			}
		}
	}

	public void saveBluetoothDevices()
	{
		string text = "";
		foreach (BluetoothDeviceTester child in bluetoothDevicesStack.Children)
		{
			text = text + child.deviceString() + "\n";
		}
		if (File.Exists(RuntimePaths.BtDevicesFile))
		{
			text += File.ReadAllText(RuntimePaths.BtDevicesFile);
		}
		string[] array = text.Split("\n");
		string text2 = "";
		string[] array2 = array;
		foreach (string text3 in array2)
		{
			string value = text3.Split("|")[0];
			if (!text2.Contains(value) && text3 != "\n")
			{
				text2 = text2 + text3 + "\n";
			}
		}
		File.WriteAllText(RuntimePaths.BtDevicesFile, text2);
	}

	public ButtplugClientDevice getOneWand()
	{
		List<ButtplugClientDevice> deviceList = new List<ButtplugClientDevice>();
		base.Dispatcher.Invoke(delegate
		{
			foreach (BluetoothDeviceTester child in bluetoothDevicesStack.Children)
			{
				if (child.wand)
				{
					deviceList.Add(child.device);
				}
			}
		});
		if (deviceList.Count > 0)
		{
			Random random = new Random();
			return deviceList[random.Next(0, deviceList.Count)];
		}
		return null;
	}

	public ButtplugClientDevice getOneOna()
	{
		List<ButtplugClientDevice> deviceList = new List<ButtplugClientDevice>();
		base.Dispatcher.Invoke(delegate
		{
			foreach (BluetoothDeviceTester child in bluetoothDevicesStack.Children)
			{
				if (child.ona)
				{
					deviceList.Add(child.device);
				}
			}
		});
		if (deviceList.Count > 0)
		{
			Random random = new Random();
			return deviceList[random.Next(0, deviceList.Count)];
		}
		return null;
	}

	public ButtplugClientDevice getOnePlug()
	{
		List<ButtplugClientDevice> deviceList = new List<ButtplugClientDevice>();
		base.Dispatcher.Invoke(delegate
		{
			foreach (BluetoothDeviceTester child in bluetoothDevicesStack.Children)
			{
				if (child.plug)
				{
					deviceList.Add(child.device);
				}
			}
		});
		if (deviceList.Count > 0)
		{
			Random random = new Random();
			return deviceList[random.Next(0, deviceList.Count)];
		}
		return null;
	}

	public List<ButtplugClientDevice> getAllWands()
	{
		List<ButtplugClientDevice> deviceList = new List<ButtplugClientDevice>();
		base.Dispatcher.Invoke(delegate
		{
			foreach (BluetoothDeviceTester child in bluetoothDevicesStack.Children)
			{
				if (child.wand)
				{
					deviceList.Add(child.device);
				}
			}
		});
		return deviceList;
	}

	public List<ButtplugClientDevice> getAllOnas()
	{
		List<ButtplugClientDevice> deviceList = new List<ButtplugClientDevice>();
		base.Dispatcher.Invoke(delegate
		{
			foreach (BluetoothDeviceTester child in bluetoothDevicesStack.Children)
			{
				if (child.ona)
				{
					deviceList.Add(child.device);
				}
			}
		});
		return deviceList;
	}

	public List<ButtplugClientDevice> getAllPlugs()
	{
		List<ButtplugClientDevice> deviceList = new List<ButtplugClientDevice>();
		base.Dispatcher.Invoke(delegate
		{
			foreach (BluetoothDeviceTester child in bluetoothDevicesStack.Children)
			{
				if (child.plug)
				{
					deviceList.Add(child.device);
				}
			}
		});
		return deviceList;
	}

	private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		playClickSound();
		popQuestionmark.Visibility = Visibility.Visible;
	}

	private void Button_Click(object sender, RoutedEventArgs e)
	{
		playClickSound();
		popQuestionmark.Visibility = Visibility.Hidden;
	}

	private void findIC_Click(object sender, RoutedEventArgs e)
	{
		playClickSound();
		OpenFileDialog openFileDialog = new OpenFileDialog();
		openFileDialog.Filter = "Find Intiface Central on your pc|*.exe;";
		if (openFileDialog.ShowDialog() == true)
		{
		File.WriteAllText(RuntimePaths.ICPathFile, openFileDialog.FileName);
		}
	}

	private void Grid_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		clickCheckmark();
	}

	private void serverAddress_KeyUp(object sender, KeyEventArgs e)
	{
		saveAddress();
	}
}
