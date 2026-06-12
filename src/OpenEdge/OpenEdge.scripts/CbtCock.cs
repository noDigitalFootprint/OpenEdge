namespace OpenEdge.scripts;

internal class CbtCock : InteruptTalk
{
	public CbtCock(MainWindow mw, TalkBaseClass homeTalk)
		: base(mw, homeTalk)
	{
		allText = mw.lr.getScript("cbtCock");
	}

	private string[] setCbtPunishment()
	{
		return getMyScript() switch
		{
			0 => new string[5] { "you are going to slap your @cock for me", "now get ready, slap it with your flat palm", "CBTSTART:", "REPEAT @cbtCock", "now, where were we" }, 
			1 => new string[4] { "flick the head of your @cock on every beat", "CBTSTART:", "REPEAT @cbtCock", "lets get back to it" }, 
			2 => new string[4] { "tightly grab onto your glans, twist your hand on every beat", "CBTSTART:", "REPEAT @cbtCock", "lets get back to it" }, 
			3 => new string[3] { "I want you to stretch out your hand and get ready to smack your @cock for me", "CBTSTART:", "REPEAT @cbtCock" }, 
			_ => setCbtPunishment(), 
		};
	}
}
