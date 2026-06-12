using OpenEdge.helper;

namespace OpenEdge.vocab;

internal class StrokingSlowStart : StrokingStart
{
	public StrokingSlowStart(Voc voc)
		: base(voc)
	{
		normal.AddRange(new string[4] { "slowly start @jerkingOff", "slowly start stroking that @cock", "slowly begin stroking", "start @jerkingOff at a relaxed pace" });
		quickShot.AddRange(new string[3] { "start stroking that @cock of yours slowly", "I'll allow you to stroke slowly, so don't cum quickshot", "I would make you stroke faster but I'm not sure you can handle it" });
		kneeling.AddRange(new string[1] { "stay on your knees and stroke slowly" });
	}
}
