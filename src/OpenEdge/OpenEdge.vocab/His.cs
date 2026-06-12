using OpenEdge.helper;

namespace OpenEdge.vocab;

internal class His : BaseVocab
{
	public His(Voc voc)
		: base(voc)
	{
		pronounsHim.AddRange(new string[1] { "his" });
		pronounsHer.AddRange(new string[1] { "her" });
		pronounsThey.AddRange(new string[1] { "their" });
	}
}
