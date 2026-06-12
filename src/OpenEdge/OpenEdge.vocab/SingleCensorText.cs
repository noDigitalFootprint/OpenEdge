using OpenEdge.helper;

namespace OpenEdge.vocab;

internal class SingleCensorText : BaseVocab
{
	public SingleCensorText(Voc voc)
		: base(voc)
	{
		moodHigh.AddRange(new string[5] { "good @boy", "so obedient", "you're doing so well", "keep going just like that", "your @missTitle is so proud of you!" });
		normal.AddRange(new string[16]
		{
			"love censorship", "no nudity", "pleasure is denial", "denial is pleasure", "can't resist", "obey your @missTitle", "crave denial", "stroke to pixels", "blurred", "blocked",
			"not for you", "you deserve this", "boobs are too much for you", "you don't deserve to look", "stay pussyfree", "love censors"
		});
		degrading.AddRange(new string[2] { "you're fucking pathetic", "trash doesn't deserve to see" });
		cuck.AddRange(new string[8] { "she is being held by someone else", "do you deserve to be there?", "at the end of the bed", "peeking in from the closet", "she'll send you a video", "listening in from the hallway", "you gave her away", "she used to love you" });
		gayHumiliation.AddRange(new string[1] { "faggots don't even get to watch" });
		sissy.AddRange(new string[10] { "sissy's get denied", "sissy free", "you look so cute", "love pink", "you should swallow", "cum is your reward", "you love the smell of cum", "thoughts go pop pop pop", "love makeup", "high heels and cute dresses" });
		virgin.AddRange(new string[4] { "virgins deserve to suffer", "no virgins allowed", "can't see what you can't have", "this is too much for a virgin" });
		moodLow.AddRange(new string[3] { "you know who to blame", "this is your fault", "you made me do this" });
		findom.AddRange(new string[4] { "UNLOCK FOR A SMALL DONATION", "missing funds", "pay to play", "$imp and $end" });
		pup.AddRange(new string[2] { "bark for me my @pup", "you're such a cute @pup" });
		cat.AddRange(new string[2] { "meow for me my @pup", "you're such a cute @pup" });
	}
}
