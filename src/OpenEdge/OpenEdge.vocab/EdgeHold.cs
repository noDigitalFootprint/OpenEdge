using OpenEdge.helper;

namespace OpenEdge.vocab;

internal class EdgeHold : BaseVocab
{
	public EdgeHold(Voc voc)
		: base(voc)
	{
		moodHigh.AddRange(new string[2] { "show me how good you are at holding it", "be a good @subTitle and hold it" });
		normal.AddRange(new string[5] { "hold it", "stay there", "keep your @cock on the edge", "don't you dare back off", "stay on the edge" });
		degrading.AddRange(new string[3] { "stay on the edge @subTitle", "stay on the edge and entertain me", "ride it for my enjoyment" });
		moodLow.AddRange(new string[2] { "try your hardest not to disappoint me any further, hold it @subTitle", "don't mess this up, hold it" });
		quickShot.AddRange(new string[2] { "struggle to hold your orgasm back for me and hold that edge", "hold it quickshot, I know just how hard that is for you" });
	}
}
