using OpenEdge.helper;

namespace OpenEdge.vocab;

internal class No : BaseVocab
{
	public No(Voc voc, MainWindow mw)
		: base(voc)
	{
		if (mw.getTFlag("petPlay"))
		{
			normal.AddRange(new string[1] { "*shake your head*" });
		}
		else
		{
			normal.AddRange(new string[1] { "no @missTitle" });
		}
	}
}
