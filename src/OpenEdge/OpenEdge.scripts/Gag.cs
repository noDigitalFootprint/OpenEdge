namespace OpenEdge.scripts;

internal class Gag : InteruptTalk
{
	public Gag(MainWindow mw, TalkBaseClass homeTalk)
		: base(mw, homeTalk)
	{
		allText = mw.lr.getScript("gag");
	}
}
