using OpenEdge.helper;

namespace OpenEdge.vocab;

internal class BreathEnd : BaseVocab
{
	public BreathEnd(Voc voc)
		: base(voc)
	{
		moodHigh.AddRange(new string[2] { "good @boy, you may breathe again", "you're such a good @boy for me, breathe" });
		normal.AddRange(new string[4] { "that's enough", "breathe @subTitle", "you may breathe again, good job", "good job for making it through that" });
		moodLow.AddRange(new string[2] { "I guess you may breathe again", "you'd best start behaving or I may increase the duration of the holds even further" });
	}
}
