using OpenEdge.helper;

namespace OpenEdge.vocab;

internal class Yes : BaseVocab
{
	public Yes(Voc voc, MainWindow mw)
		: base(voc)
	{
		if (mw.getTFlag("petPlay"))
		{
			if (mw.getFlag("cat"))
			{
				normal.AddRange(new string[2] { "meow", "nya" });
			}
			else
			{
				normal.AddRange(new string[2] { "woof", "bark" });
			}
		}
		else
		{
			normal.AddRange(new string[1] { "yes @missTitle" });
		}
	}
}
