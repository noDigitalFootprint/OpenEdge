namespace OpenEdge.scripts;

internal class Anal : InteruptTalk
{
	public Anal(MainWindow mw, TalkBaseClass homeTalk, bool extreme = false)
		: base(mw, homeTalk)
	{
		mw.changeMoodBy(10);
		setFlag("FLAGT:sessionIntro", temp: true);
		allText = mw.lr.getScript("anal");
		setFlag("FLAGT:anal", temp: true);
	}
}
