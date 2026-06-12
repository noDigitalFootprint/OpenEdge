namespace OpenEdge.scripts;

internal class Break : TalkBaseClass
{
	public Break(MainWindow mw)
		: base(mw)
	{
		allText = mw.lr.getScript("break");
	}

	private string[] createSessionIntro()
	{
		if (getMyScript() == 0)
		{
			return new string[2] { "AUTOREPLY: I'm still on break", "ENDSESSION" };
		}
		return createSessionIntro();
	}
}
