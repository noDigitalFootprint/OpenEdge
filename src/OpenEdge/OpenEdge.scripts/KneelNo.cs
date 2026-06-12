namespace OpenEdge.scripts;

internal class KneelNo : InteruptTalk
{
	public KneelNo(MainWindow mw, TalkBaseClass homeTalk)
		: base(mw, homeTalk)
	{
		_ = new string[8] { "ISFLAGT:kneel GOTO:start", "GOTO:end", "(start)", "STOPSTROKING:", "COMMAND: @kneelNo", "[@yes]", "DELFLAGT:kneel", "(end)" };
		allText = mw.lr.getScript("kneelNo");
	}
}
