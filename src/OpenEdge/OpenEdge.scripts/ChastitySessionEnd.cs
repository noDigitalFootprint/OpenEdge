namespace OpenEdge.scripts;

internal class ChastitySessionEnd : TalkBaseClass
{
	public ChastitySessionEnd(MainWindow mw)
		: base(mw)
	{
		allText = mw.lr.getScript("chastitySessionEnd");
	}
}
