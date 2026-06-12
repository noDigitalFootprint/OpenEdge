namespace OpenEdge.scripts;

internal class ChastityWear : TalkBaseClass
{
	public ChastityWear(MainWindow mw)
		: base(mw)
	{
		allText = mw.lr.getScript("chastityWear");
	}
}
