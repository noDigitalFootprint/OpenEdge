using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace OpenEdge;

public class TagType : TextBlock
{
	public TagType(string text)
	{
		base.Text = text.Trim();
		base.FontSize = 40.0;
		base.Foreground = Brushes.White;
		base.FontFamily = new FontFamily("Times New Roman");
		base.MinWidth = 100.0;
		base.MaxWidth = 1800.0;
		base.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
		base.HorizontalAlignment = HorizontalAlignment.Center;
		base.VerticalAlignment = VerticalAlignment.Center;
		base.TextAlignment = TextAlignment.Center;
	}
}
