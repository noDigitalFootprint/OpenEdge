using OpenEdge.helper;

namespace OpenEdge.vocab;

internal class FakeEnding : BaseVocab
{
	public FakeEnding(Voc voc)
		: base(voc)
	{
		moodHigh.AddRange(new string[2] { "I was planning to let you go, but you've gotten me all worked up @subTitle", "I'm having too much fun to let you go already" });
		normal.AddRange(new string[3] { "I want to keep you around just a bit longer", "you know what, I'm not done with you for today", "I had expected your @cock to be in a worse state than it is, let's go fix that first" });
		moodLow.AddRange(new string[2] { "you know, I don't think you've earned that rest just yet", "you seem to need some supplemental lessons" });
	}
}
