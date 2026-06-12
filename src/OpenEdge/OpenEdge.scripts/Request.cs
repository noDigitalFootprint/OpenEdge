namespace OpenEdge.scripts;

internal class Request : InteruptTalk
{
	public Request(MainWindow mw, TalkBaseClass homeTalk, string currentState)
		: base(mw, homeTalk, currentState)
	{
		allText = mw.lr.getScript("request");
	}
}
