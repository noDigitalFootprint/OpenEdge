namespace OpenEdge.scripts;

internal class ChastityTaunt : TalkBaseClass
{
	public ChastityTaunt(MainWindow mw)
		: base(mw)
	{
		allText = mw.lr.getScript("chastityTaunt");
	}
}
