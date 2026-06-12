using OpenEdge.helper;

namespace OpenEdge.vocab;

internal class Strip : BaseVocab
{
	public Strip(Voc voc)
		: base(voc)
	{
		normal.AddRange(new string[8] { "strip down for me", "take off your clothes", "strip down", "get naked", "get nude", "show me your nude body", "I want you naked", "show me all of you" });
		sph.AddRange(new string[3] { "show me that pathetic little cock of yours", "undress, don't hide that small thing", "I guess I'll have you strip, although there is nothing to see" });
		degrading.AddRange(new string[2] { "undress right now @subTitle", "you'd better rip those clothes off @subTitle" });
		petPlay.AddRange(new string[2] { "I don't think it's right for pets to wear clothes, strip for me", "puppies don't wear clothes, strip down" });
	}
}
