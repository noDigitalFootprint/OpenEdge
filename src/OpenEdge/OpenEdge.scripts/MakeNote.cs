namespace OpenEdge.scripts;

internal class MakeNote : InteruptTalk
{
	public MakeNote(MainWindow mw, TalkBaseClass homeTalk)
		: base(mw, homeTalk)
	{
		allText = mw.lr.getScript("makeNote");
	}
}
