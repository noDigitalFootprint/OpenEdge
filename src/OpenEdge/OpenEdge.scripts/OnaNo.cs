namespace OpenEdge.scripts;

internal class OnaNo : InteruptTalk
{
	public OnaNo(MainWindow mw, TalkBaseClass homeTalk)
		: base(mw, homeTalk)
	{
		allText = mw.lr.getScript("onaNo");
	}
}
