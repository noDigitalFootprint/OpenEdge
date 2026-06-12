using OpenEdge.helper;

namespace OpenEdge.vocab;

internal class Man : BaseVocab
{
	public Man(Voc voc)
		: base(voc)
	{
		pronounsHim.AddRange(new string[1] { "man" });
		pronounsHer.AddRange(new string[1] { "woman" });
		pronounsThey.AddRange(new string[1] { "person" });
	}
}
