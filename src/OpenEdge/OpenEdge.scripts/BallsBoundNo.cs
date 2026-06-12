namespace OpenEdge.scripts;

internal class BallsBoundNo : InteruptTalk
{
	public BallsBoundNo(MainWindow mw, TalkBaseClass homeTalk)
		: base(mw, homeTalk)
	{
		_ = new string[7] { "ISFLAGT:ballsBound GOTO:start", "GOTO:end", "(start)", "COMMAND: @bindBallsNo", "[@yes]", "DELFLAGT:ballsBound", "(end)" };
		allText = mw.lr.getScript("ballsBoundNo");
	}
}
