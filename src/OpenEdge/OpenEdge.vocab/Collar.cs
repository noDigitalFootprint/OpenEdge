using OpenEdge.helper;

namespace OpenEdge.vocab;

internal class Collar : BaseVocab
{
	public Collar(Voc voc)
		: base(voc)
	{
		moodHigh.AddRange(new string[2] { "show me what a good @boy you are, put your collar on", "you get to wear your collar, aren't you happy @pup?" });
		normal.AddRange(new string[3] { "put your collar on @pup", "snap the collar shut around your pretty little neck", "pull your collar tight around your neck" });
		moodLow.AddRange(new string[3] { "you'd better start behaving @pup, put on your collar", "maybe wearing your collar will remind you of your place", "put on your collar and make it tight" });
	}
}
