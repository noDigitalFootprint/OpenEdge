namespace OpenEdge.scripts;

internal class CollarNo : InteruptTalk
{
	public CollarNo(MainWindow mw, TalkBaseClass homeTalk)
		: base(mw, homeTalk)
	{
		_ = new string[7] { "ISFLAGT:collar GOTO:start", "GOTO:end", "(start)", "COMMAND: @collarNo", "[@yes]", "DELFLAGT:collar", "(end)" };
		allText = mw.lr.getScript("collarNo");
	}
}
