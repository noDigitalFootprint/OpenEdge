namespace OpenEdge.scripts;

internal class VibeNo : InteruptTalk
{
	public VibeNo(MainWindow mw, TalkBaseClass homeTalk)
		: base(mw, homeTalk)
	{
		allText = mw.lr.getScript("vibeNo");
	}
}
