using System.Windows.Controls;

namespace OpenEdge;

public class Btn : Button
{
	public int type;

	public Btn(int type)
	{
		this.type = type;
	}
}
