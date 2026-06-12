using OpenEdge.helper;

namespace OpenEdge.vocab;

internal class StrokingFaster : BaseVocab
{
	public StrokingFaster(Voc voc)
		: base(voc)
	{
		normal.AddRange(new string[4] { "faster", "hurry up", "increase your pace", "stroke faster" });
		degrading.AddRange(new string[4] { "faster @subTitle", "hurry up @subTitle", "increase your pace @subTitle", "stroke faster @subTitle" });
	}
}
