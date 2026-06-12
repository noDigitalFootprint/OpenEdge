namespace OpenEdge.scripts;

internal class ChastityRemove : TalkBaseClass
{
	public ChastityRemove(MainWindow mw)
		: base(mw)
	{
		allText = mw.lr.getScript("chastityRemove");
	}
}
