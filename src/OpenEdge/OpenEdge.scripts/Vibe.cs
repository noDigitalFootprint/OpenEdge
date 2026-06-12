namespace OpenEdge.scripts;

internal class Vibe : InteruptTalk
{
	public Vibe(MainWindow mw, TalkBaseClass homeTalk)
		: base(mw, homeTalk)
	{
		allText = mw.lr.getScript("vibe");
	}
}
