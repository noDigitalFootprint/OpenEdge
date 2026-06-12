using OpenEdge.helper;

namespace OpenEdge.vocab;

internal class Balls : BaseVocab
{
	public Balls(Voc voc)
		: base(voc)
	{
		normal.AddRange(new string[5] { "balls", "testicles", "nuts", "blue balls", "aching balls" });
	}
}
