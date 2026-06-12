using OpenEdge.helper;

namespace OpenEdge.vocab;

internal class EdgeStop : StopStroking
{
	public EdgeStop(Voc voc)
		: base(voc)
	{
		moodHigh.AddRange(new string[3] { "well done @subTitle, you may let the edge go now", "that was impressive, you may let go", "let the edge fade away, I was sure you were going to blow your load there" });
		normal.AddRange(new string[5] { "you may let the edge go", "let the edge go", "you can let the edge go now", "stop edging @subTitle", "stop touching" });
		quickShot.AddRange(new string[2] { "let that @cock go, you almost went over the edge there", "let the edge fade, this is really dangerous for a quickshot like you" });
		moodLow.AddRange(new string[2] { "at least you didn't fuck this up, stop edging", "let go, I fully expected you to cum like the disappointment you are" });
	}
}
