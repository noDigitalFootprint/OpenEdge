using OpenEdge.helper;

namespace OpenEdge.vocab;

internal class BreathTease : Tease
{
	public BreathTease(Voc voc)
		: base(voc)
	{
		moodHigh.AddRange(new string[1] { "you look cute when your face is flushed pink" });
		quickShot.AddRange(new string[2] { "you wouldn't cum from a short hold like this right?", "a quickshot like you is going to have some trouble with this" });
		normal.AddRange(new string[6] { "keep going", " I'm almost gonna let you breathe again, promise", "if I tell you that you must hold your breath, there isn't even any air to breathe", "just like that", "let me see you struggle", "just a bit more" });
		degrading.AddRange(new string[5] { "you're not even allowed to breathe in my presence", "you'll obey me till you pass out", "don't you fucking dare give up", "trash like you doesn't deserve air", "god this is pathetic" });
		petPlay.AddRange(new string[1] { "good pets obey" });
		pup.AddRange(new string[1] { "good pups obey" });
		cat.AddRange(new string[1] { "good kittens obey" });
		humiliation.AddRange(new string[2] { "you look ridiculous", "your head is getting red" });
	}
}
