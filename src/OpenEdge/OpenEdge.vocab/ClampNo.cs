using OpenEdge.helper;

namespace OpenEdge.vocab;

internal class ClampNo : BaseVocab
{
	public ClampNo(Voc voc)
		: base(voc)
	{
		moodHigh.AddRange(new string[1] { "give your cute nipps a moment of respite, take the clamps off" });
		normal.AddRange(new string[2] { "you may take the clamps off again", "take off the clamps" });
		moodLow.AddRange(new string[2] { "rip off the clamps, no opening them first", "tear off the clamps" });
	}
}
