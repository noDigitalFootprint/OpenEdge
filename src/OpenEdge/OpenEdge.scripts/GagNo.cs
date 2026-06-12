namespace OpenEdge.scripts;

internal class GagNo : InteruptTalk
{
	public GagNo(MainWindow mw, TalkBaseClass homeTalk)
		: base(mw, homeTalk)
	{
		allText = mw.lr.getScript("gagNo");
	}
}
