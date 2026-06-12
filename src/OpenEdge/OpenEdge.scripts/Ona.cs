namespace OpenEdge.scripts;

internal class Ona : InteruptTalk
{
	public Ona(MainWindow mw, TalkBaseClass homeTalk)
		: base(mw, homeTalk)
	{
		allText = mw.lr.getScript("ona");
	}
}
