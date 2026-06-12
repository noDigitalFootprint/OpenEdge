namespace OpenEdge.scripts;

internal class DebugIntro : InteruptTalk
{
	public DebugIntro(MainWindow mw, TalkBaseClass home)
		: base(mw, home)
	{
		allText = mw.lr.getScript("debugIntro");
	}

	private string[] start()
	{
		if (getMyScript() == 21)
		{
			return new string[16]
			{
				"god I love futas so much", "ACTIVATETAGGER:", "ASK: yoo", "[@yes]", "[@no]", "[@boy]", " ADDVAR:0testing,1  SETVAR:0testing,-10 ADDVAR:0testing,1", "I don't think anything else in this world encapsulates eroticism as much as them", "LOCKVIDEO:", "just think about it, a futa is a sex symbol by simply existing",
				"they're similar to succuby in that sense", "but a futa is so much more grounded", "it's the lust of both genders given form", "I kind of went on a tangent there, what I'm trying to say is that you've got good taste", "DELVIDEO:", "(end)"
			};
		}
		return start();
	}
}
