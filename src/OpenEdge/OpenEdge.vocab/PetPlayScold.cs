using OpenEdge.helper;

namespace OpenEdge.vocab;

internal class PetPlayScold : MoodDown
{
	public PetPlayScold(Voc voc)
		: base(voc)
	{
		normal.AddRange(new string[1] { "why do you insist on misbehaving?" });
		pup.AddRange(new string[2] { "bad dog!", "you're acting like a bad dog" });
		cat.AddRange(new string[2] { "bad cat!", "you're acting like a bad kitten" });
		collar.AddRange(new string[1] { "you really don't deserve that collar" });
	}
}
