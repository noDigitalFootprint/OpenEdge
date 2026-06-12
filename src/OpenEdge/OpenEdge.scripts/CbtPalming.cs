namespace OpenEdge.scripts;

internal class CbtPalming : InteruptTalk
{
	public CbtPalming(MainWindow mw, TalkBaseClass homeTalk)
		: base(mw, homeTalk)
	{
		allText = mw.lr.getScript("cbtPalming");
	}

	private string[] setCbtPunishment()
	{
		if (getMyScript() == 0)
		{
			return new string[7] { "get ready for some palming", "use one hand to hold on to the base", "and use the other to torture the head of that @cock", "every beat is a rotation, always keep your palm pressed against that dick", "CBTSTART:", "REPEAT @cbtPalming", "you may let go" };
		}
		return setCbtPunishment();
	}
}
