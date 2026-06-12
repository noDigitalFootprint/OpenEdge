namespace OpenEdge.scripts;

internal class BackFromBreak : TalkBaseClass
{
	public BackFromBreak(MainWindow mw)
		: base(mw)
	{
		allText = mw.lr.getScript("backFromBreak");
	}

	private string[] createSessionIntro()
	{
		if (getMyScript() == 0)
		{
			return new string[4] { "hey @subTitle, I'm back, did you miss me?", "FLAGT:sessionIntro DELFLAG:break I hope you've reflected on your behavior by now", "after your horrible performance last time, I really did lose hope for a bit", "but as long as you've been good while I was away I'll let it slide" };
		}
		return createSessionIntro();
	}
}
