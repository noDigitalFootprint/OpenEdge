namespace OpenEdge.scripts;

internal class ClampNo : InteruptTalk
{
	public ClampNo(MainWindow mw, TalkBaseClass homeTalk)
		: base(mw, homeTalk)
	{
		_ = new string[8] { "ISFLAGT:clothesPins GOTO:start", "GOTO:end", "(start)", "STOPSTROKING:", "COMMAND: @clampNo", "[@yes]", "DELFLAGT:clothesPins", "(end)" };
		allText = mw.lr.getScript("clampNo");
	}
}
