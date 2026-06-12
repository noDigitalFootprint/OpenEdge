using OpenEdge.helper;

namespace OpenEdge.vocab;

internal class PetPlayPraise : MoodUp
{
	public PetPlayPraise(Voc voc)
		: base(voc)
	{
		normal.AddRange(new string[12]
		{
			"you're just the cutest little thing", "you're such a good @pup", "my cute @pup is doing so well", "you've been acting so cute and obedient for me, good @boy", "you're such a good pet", "I'm proud of you", "you make me so happy", "you're the best @pup you can be right now", "good @boy", "I love seeing you like this",
			"you're so precious to me", "I love how you look right now"
		});
		collar.AddRange(new string[1] { "that collar suits your pretty neck so well" });
	}
}
