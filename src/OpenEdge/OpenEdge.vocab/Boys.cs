using OpenEdge.helper;

namespace OpenEdge.vocab;

internal class Boys : BaseVocab
{
	public Boys(Voc voc)
		: base(voc)
	{
		pronounsHim.AddRange(new string[1] { "boys" });
		pronounsHer.AddRange(new string[1] { "girls" });
		pronounsThey.AddRange(new string[1] { "subs" });
	}
}
