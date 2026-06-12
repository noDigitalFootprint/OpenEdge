using OpenEdge.helper;

namespace OpenEdge.vocab;

internal class EdgeTaunt : BaseVocab
{
	public EdgeTaunt(Voc voc)
		: base(voc)
	{
		moodHigh.AddRange(new string[3] { "try to push yourself a little bit closer", "you look wonderful when you're this close", "show me what I do to you" });
		normal.AddRange(new string[10] { "you're going to remember this edge tonight", "that cock better be ready to burst at the end of this", "you need to suffer so much more", "you belong on the edge @subTitle", "can you feel the cum churning in your balls?", "your dick needs to cum, too bad it doesn't get to decide anymore", "push yourself a bit closer to the edge for me", "I'm sure you can take more right?", "don't let yourself go past the point of no return", "get close enough for a gust of air to cause orgasm" });
		degrading.AddRange(new string[7] { "edging is the only way you stay useful to me", "edging is your only purpose from now on", "this is what every @cock as useless as yours should feel", "no woman would ever want to see your @cock cum", "your knees better be shaking after this edge", "just stay there and suffer", "your dick belongs to me now, and I'm telling you to hold that edge" });
		humiliation.AddRange(new string[1] { "imagine of you were here fucking me instead of edging like a loser" });
		moodLow.AddRange(new string[3] { "you call that edging? push yourself or lose my interest @subTitle", "this is just pathetic", "closer @subTitle, I'm about to fall asleep over here" });
		orgasmDenied.AddRange(new string[1] { "you know your getting denied right?" });
		findom.AddRange(new string[1] { "closer now, or I'm taking tribute" });
	}
}
