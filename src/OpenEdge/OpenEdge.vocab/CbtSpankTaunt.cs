using OpenEdge.helper;

namespace OpenEdge.vocab;

internal class CbtSpankTaunt : CbtTauntBase
{
	public CbtSpankTaunt(Voc voc)
		: base(voc)
	{
		moodHigh.AddRange(new string[2] { "I love impact-play like this", "can you feel that shockwave traveling through your body with every hit?" });
		normal.AddRange(new string[4] { "spank harder @subTitle", "make those slaps ring out", "those swings better be leaving red marks", "hit with your palm, not with your fingers" });
		moodLow.AddRange(new string[1] { "I really should bend you over my knee" });
	}
}
