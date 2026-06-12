using OpenEdge.helper;

namespace OpenEdge.vocab;

internal class Greetings : BaseVocab
{
	public Greetings(Voc voc)
		: base(voc)
	{
		moodHigh.AddRange(new string[1] { "I hope you're doing well today, because I'm going to tease the hell out of you" });
		normal.AddRange(new string[2] { "hello @subTitle", "hey @subTitle" });
		degrading.AddRange(new string[4] { "don't you have anything better to do?", "back already? that's just sad", "you've long lost your way out of this agreement haven't you", "let's torture that @cock some more" });
		sph.AddRange(new string[3] { "that tiny dick of yours sure asks for a lot of attention", "do your best @jerkingOff, although I really can't call it that", "do you expect your dick to grow with practice or something?" });
		humiliation.AddRange(new string[1] { "it's hilarious that you come crawling back again and again" });
		cuck.AddRange(new string[2] { "are you worried that I had fun with someone else while you were gone?", "I've enjoyed myself plenty while you weren't looking, maybe I'll tell you about it" });
		moodLow.AddRange(new string[3] { "oh, it's you again, don't you have anything else to do?", "lets get this over with", "fucking hell, I have to deal with this dipshit again?" });
	}
}
