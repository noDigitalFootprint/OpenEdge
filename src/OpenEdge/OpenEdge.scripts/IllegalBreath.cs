namespace OpenEdge.scripts;

internal class IllegalBreath : InteruptTalk
{
	public IllegalBreath(MainWindow mw, TalkBaseClass homeTalk)
		: base(mw, homeTalk)
	{
		allText = mw.lr.getScript("illegalBreath");
	}
}
