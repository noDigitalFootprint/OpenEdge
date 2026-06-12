using OpenEdge.helper;

namespace OpenEdge.vocab;

internal class Pup : BaseVocab
{
	public Pup(Voc voc)
		: base(voc)
	{
		petPlay.AddRange(new string[1] { "pet" });
		cat.AddRange(new string[2] { "kitty", "kitten" });
		pup.AddRange(new string[3] { "puppy", "pup", "doggie" });
	}
}
