using OpenEdge.helper;

namespace OpenEdge.vocab;

internal class CbtBallsTaunt : CbtTauntBase
{
	public CbtBallsTaunt(Voc voc)
		: base(voc)
	{
		normal.AddRange(new string[3] { "keep hitting those @balls @subTitle", "hit those @balls @subTitle", "your @balls should be red after this" });
		virgin.AddRange(new string[2] { "you're acting like you'll still need those balls after this, for what exactly virgin?", "stop holding back, you aren't going to use your balls anyway, you pathetic virgin" });
	}
}
