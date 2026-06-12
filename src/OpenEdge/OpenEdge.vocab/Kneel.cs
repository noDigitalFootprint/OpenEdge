using OpenEdge.helper;

namespace OpenEdge.vocab;

internal class Kneel : BaseVocab
{
	public Kneel(Voc voc)
		: base(voc)
	{
		normal.AddRange(new string[6] { "kneel", "get on your knees", "on your knees", "get on your knees, keep your back straight", "kneel before your @missTitle", "I want you on your knees" });
		degrading.AddRange(new string[6] { "kneel @subTitle", "get on your knees @subTitle", "on your knees @subTitle", "get on your knees, keep your back straight @subTitle", "I want you on your knees @subTitle", "get on your fucking knees @subTitle" });
	}
}
