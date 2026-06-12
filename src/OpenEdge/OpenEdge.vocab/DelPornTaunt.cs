using OpenEdge.helper;

namespace OpenEdge.vocab;

internal class DelPornTaunt : BaseVocab
{
	public DelPornTaunt(Voc voc)
		: base(voc)
	{
		normal.AddRange(new string[14]
		{
			"who knows how many more edges I'm going to make you do", "just take the easy way out @subTitle", "you're going to regret not taking the easy way out", "I know you can't take much more", "I can't imagine you like it enough to go through this", "to be honest, I don't even care if you make it", "it could be just one more edge", "keep going @subTitle, you're almost there!", "you could just give up and you won't have to deal with all these edges", "just let me remove this from your pc",
			"you're struggling so much already, just give in", "have I stumbled onto your favorite porn or something?", "you care way too much about what happens to some file", "you've got plenty of other porn apart from this right?"
		});
		if (voc.mw.isSettingEnabled("censorship"))
		{
			normal.AddRange(new string[4] { "and here I thought you agreed with me that you don't deserve to see such pretty things", "I though you agreed that nudity is too much for you", "you don't deserve to see nudity", "know your place @subTitle, you don't get to look at nudity" });
		}
		moodLow.AddRange(new string[2] { "have you not gone against me too many times already?", "you're really being a brat today aren't you?" });
	}
}
