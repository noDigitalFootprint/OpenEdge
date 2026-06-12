namespace OpenEdge.scripts;

internal class Plug : InteruptTalk
{
	public Plug(MainWindow mw, TalkBaseClass homeTalk)
		: base(mw, homeTalk)
	{
		_ = new string[7] { "ISFLAGT:plug GOTO:end", "STOPSTROKING:", "COMMAND: @plug", "[@yes]", "FLAGT:plug", "now you're nice and full for me", "(end)" };
		allText = mw.lr.getScript("plug");
	}
}
