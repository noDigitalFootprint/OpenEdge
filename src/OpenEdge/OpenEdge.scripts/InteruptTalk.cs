namespace OpenEdge.scripts;

internal class InteruptTalk : TalkBaseClass
{
	public InteruptTalk(MainWindow mw, TalkBaseClass homeTalk, string currentState = "")
		: base(mw)
	{
		base.homeTalk = homeTalk;
		base.currentState = currentState;
	}
}
