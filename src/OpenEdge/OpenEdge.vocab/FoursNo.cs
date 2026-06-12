using OpenEdge.helper;

namespace OpenEdge.vocab;

internal class FoursNo : BaseVocab
{
	public FoursNo(Voc voc)
		: base(voc)
	{
		normal.AddRange(new string[3] { "you may take a seat again @pup", "take a seat @pup", "you can stop crawling around on all fours for now @pup" });
	}
}
