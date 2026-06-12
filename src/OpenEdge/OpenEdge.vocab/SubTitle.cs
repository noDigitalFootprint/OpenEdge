using OpenEdge.helper;

namespace OpenEdge.vocab;

internal class SubTitle : Pup
{
	public SubTitle(Voc voc)
		: base(voc)
	{
		if (!voc.mw.getTFlag("petPlay"))
		{
			normal.AddRange(new string[2] { "@boy", "perv" });
			degrading.AddRange(new string[6] { "slave", "loser", "bitch", "whore", "slut", "degenerate" });
			cuck.AddRange(new string[2] { "cuck", "cuckie" });
			sissy.AddRange(new string[1] { "sissy" });
			cei.AddRange(new string[2] { "cumslut", "cumdump" });
			feet.AddRange(new string[2] { "footlover", "footslut" });
			gayHumiliation.AddRange(new string[4] { "cock addict", "dicklover", "cockslut", "faggot" });
			quickShot.AddRange(new string[2] { "minute @man", "quickshot" });
			humiliation.AddRange(new string[4] { "toy", "plaything", "subby", "weakling" });
			moodLow.AddRange(new string[4] { "loser", "trash", "failure", "virmin" });
			findom.AddRange(new string[3] { "paypig", "human ATM", "tribute slave" });
		}
	}
}
