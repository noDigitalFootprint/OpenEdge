namespace OpenEdge.scripts;

internal class PlugNo : InteruptTalk
{
	public PlugNo(MainWindow mw, TalkBaseClass homeTalk)
		: base(mw, homeTalk)
	{
		_ = new string[8] { "ISFLAGT:plug GOTO:start", "GOTO:end", "(start)", "STOPSTROKING:", "COMMAND: @plugNo", "[@yes]", "DELFLAGT:plug", "(end)" };
		allText = mw.lr.getScript("plugNo");
	}
}
