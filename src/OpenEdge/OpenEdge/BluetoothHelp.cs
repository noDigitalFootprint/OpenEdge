using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Navigation;

namespace OpenEdge;

public partial class BluetoothHelp : Grid, IComponentConnector
{
	public BluetoothHelp()
	{
		InitializeComponent();
	}

	private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
	{
		Process.Start(new ProcessStartInfo
		{
			FileName = e.Uri.ToString(),
			UseShellExecute = true
		});
		e.Handled = true;
	}
}
