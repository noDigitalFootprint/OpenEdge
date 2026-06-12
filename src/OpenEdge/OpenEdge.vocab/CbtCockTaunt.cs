using OpenEdge.helper;

namespace OpenEdge.vocab;

internal class CbtCockTaunt : CbtTauntBase
{
	public CbtCockTaunt(Voc voc)
		: base(voc)
	{
		normal.AddRange(new string[4] { "keep hitting your @cock @subTitle", "hit that @cock @subTitle", "harder now @subTitle", "your @cock should be red after this" });
		virgin.AddRange(new string[1] { "you're not using that virgin cock for anything important anyway so it doesn't matter if it breaks" });
	}
}
