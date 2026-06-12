using OpenEdge.helper;

namespace OpenEdge.vocab;

internal class CollarNo : BaseVocab
{
	public CollarNo(Voc voc)
		: base(voc)
	{
		moodHigh.AddRange(new string[2] { "you've been good, but take the collar off for now @pup", "take off the collar my cute @pup" });
		normal.AddRange(new string[3] { "too bad @pup, lose the collar", "take your collar off", "you don't get to wear the collar anymore" });
		moodLow.AddRange(new string[1] { "you've been bad, so you don't get to wear that cute collar anymore" });
	}
}
