namespace OpenEdge.scripts;

internal class ChastityLostKey : InteruptTalk
{
	public ChastityLostKey(MainWindow mw, TalkBaseClass homeTalk)
		: base(mw, homeTalk)
	{
		allText = mw.lr.getScript("chastityLostKey");
	}
}
