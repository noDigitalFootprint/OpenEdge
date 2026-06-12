using OpenEdge.helper;

namespace OpenEdge.vocab;

internal class BindBallsNo : BaseVocab
{
	public BindBallsNo(Voc voc)
		: base(voc)
	{
		normal.AddRange(new string[4] { "those @balls have been tortured enough for now, undo the string", "I'll allow your @balls some freedom again, undo the string", "undo the string around your @balls", "you can undo the string around your @balls now" });
	}
}
