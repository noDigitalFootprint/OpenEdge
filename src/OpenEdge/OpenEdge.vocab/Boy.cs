using OpenEdge.helper;

namespace OpenEdge.vocab;

internal class Boy : BaseVocab
{
	public Boy(Voc voc)
		: base(voc)
	{
		pronounsHim.AddRange(new string[1] { "boy" });
		pronounsHer.AddRange(new string[1] { "girl" });
		pronounsThey.AddRange(new string[1] { "sub" });
	}
}
