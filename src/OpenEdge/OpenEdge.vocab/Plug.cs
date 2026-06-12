using OpenEdge.helper;

namespace OpenEdge.vocab;

internal class Plug : BaseVocab
{
	public Plug(Voc voc)
		: base(voc)
	{
		moodHigh.AddRange(new string[2] { "push your plug deep inside your cute little hole", "you get to wear your plug" });
		normal.AddRange(new string[4] { "plug up your ass", "put your plug deep inside of you", "insert your plug", "slide your plug into your ass" });
		moodLow.AddRange(new string[1] { "stick a plug up your ass" });
	}
}
