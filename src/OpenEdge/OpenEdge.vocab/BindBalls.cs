using OpenEdge.helper;

namespace OpenEdge.vocab;

internal class BindBalls : BaseVocab
{
	public BindBalls(Voc voc)
		: base(voc)
	{
		normal.AddRange(new string[5] { "we're going to tie your @balls up", "I want those @balls tied", "you're going to strangle those @balls for me", "tie your @balls up tightly", "tie a string around your @balls" });
	}
}
