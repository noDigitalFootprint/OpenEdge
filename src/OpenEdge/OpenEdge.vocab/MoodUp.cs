using OpenEdge.helper;

namespace OpenEdge.vocab;

internal class MoodUp : BaseVocab
{
	public MoodUp(Voc voc)
		: base(voc)
	{
		normal.AddRange(new string[5] { "well done @subTitle", "good @subTitle", "I'm proud of you @subTitle", "you're doing well @subTitle", "your training is coming along nicely" });
	}
}
