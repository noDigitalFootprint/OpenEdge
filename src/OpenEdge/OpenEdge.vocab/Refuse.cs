using OpenEdge.helper;

namespace OpenEdge.vocab;

internal class Refuse : BaseVocab
{
	public Refuse(Voc voc, MainWindow mw)
		: base(voc)
	{
		if (mw.getTFlag("petPlay"))
		{
			if (mw.getFlag("cat"))
			{
				normal.AddRange(new string[1] { "hiss" });
			}
			else
			{
				normal.AddRange(new string[1] { "growl" });
			}
		}
		else
		{
			normal.AddRange(new string[3] { "no @missTitle", "I don't want to do that", "I'm not doing that" });
		}
	}
}
