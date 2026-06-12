namespace OpenEdge.scripts;

internal class FoursNo : InteruptTalk
{
	public FoursNo(MainWindow mw, TalkBaseClass homeTalk)
		: base(mw, homeTalk)
	{
		_ = new string[8] { "ISFLAGT:fours GOTO:start", "GOTO:end", "(start)", "STOPSTROKING:", "COMMAND: @foursNo", "[@yes]", "DELFLAGT:fours", "(end)" };
		allText = mw.lr.getScript("foursNo");
	}
}
