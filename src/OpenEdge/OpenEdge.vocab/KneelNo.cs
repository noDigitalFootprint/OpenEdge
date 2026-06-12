using OpenEdge.helper;

namespace OpenEdge.vocab;

internal class KneelNo : BaseVocab
{
	public KneelNo(Voc voc)
		: base(voc)
	{
		normal.AddRange(new string[6] { "get off your knees", "sit down again", "you may get up now", "take a seat", "stop kneeling", "I'll allow you to stop kneeling" });
		degrading.AddRange(new string[5] { "get off your knees @subTitle", "sit down again @subTitle", "you may get up now @subTitle", "take a seat @subTitle", "stop kneeling @subTitle" });
	}
}
