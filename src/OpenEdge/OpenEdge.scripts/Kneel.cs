namespace OpenEdge.scripts;

internal class Kneel : InteruptTalk
{
	public Kneel(MainWindow mw, TalkBaseClass homeTalk)
		: base(mw, homeTalk)
	{
		_ = new string[7] { "ISFLAGT:kneel GOTO:end", "ISFLAGT:fours GOTO:end", "STOPSTROKING:", "FLAGT:kneel", "COMMAND: @kneel", "[@yes]", "(end)" };
		allText = mw.lr.getScript("kneel");
	}
}
