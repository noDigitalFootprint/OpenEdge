using OpenEdge.helper;

namespace OpenEdge.vocab;

internal class BreathStart : BaseVocab
{
	public BreathStart(Voc voc)
		: base(voc)
	{
		moodHigh.AddRange(new string[2] { "don't worry I'm sure you can do it", "I'll make the breath holds bearable, just for you" });
		normal.AddRange(new string[4] { "I'm going to toy with your oxygen", "I'm going to make you feel all woozy", "soon your body will be screaming out for air", "lets see if you can handle this" });
		moodLow.AddRange(new string[2] { "make sure you're as prepared as can be", "you don't need that much oxygen do you?" });
	}
}
