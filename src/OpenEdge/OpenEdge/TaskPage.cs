using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Markup;

namespace OpenEdge;

public partial class TaskPage : Page, IComponentConnector
{
	private Stopwatch stopwatch = new Stopwatch();

	public TaskPage()
	{
		InitializeComponent();
	}

	private void startTask()
	{
		stopwatch.Start();
	}
}
