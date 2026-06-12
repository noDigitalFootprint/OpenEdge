using OpenEdge.helper;

namespace OpenEdge.vocab;

internal class MissTitle : BaseVocab
{
	public MissTitle(Voc voc, MainWindow mw)
		: base(voc)
	{
		if (mw.getFlag("domTitle"))
		{
			normal.AddRange(new string[1] { mw.getVar("domTitle") });
			return;
		}
		normal.AddRange(new string[4] { "domme", "domina", "mistress", "goddess" });
		petPlay.AddRange(new string[2] { "owner", "trainer" });
		chastity.AddRange(new string[1] { "keyholder" });
	}
}
