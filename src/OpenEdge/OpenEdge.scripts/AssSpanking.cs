namespace OpenEdge.scripts;

internal class AssSpanking : InteruptTalk
{
	public AssSpanking(MainWindow mw, TalkBaseClass homeTalk)
		: base(mw, homeTalk)
	{
		allText = mw.lr.getScript("assSpanking");
	}

	private string[] setPunishment()
	{
		return getMyScript() switch
		{
			0 => new string[7] { "KNEEL", "you are going to slap your ass for me", "ISFLAGT:plug make sure to avoid your plug as you swing", "now get ready, slap it with your flat palm", "CBTSTART:", "REPEAT @cbtSpank", "now, where were we" }, 
			1 => new string[7] { "KNEEL", "you're going to hurt yourself for me", "use a flat palm to slap yourself on the ass", "ISFLAGT:plug make sure to avoid your plug as you swing", "CBTSTART:", "REPEAT @cbtSpank", "now, where were we" }, 
			2 => new string[7] { "KNEEL", "you do not know how to behave do you?", "we're going to spank that defiant ass of yours", "ISFLAGT:plug make sure to avoid your plug as you swing", "CBTSTART:", "REPEAT @cbtSpank", "now, where were we" }, 
			_ => setPunishment(), 
		};
	}
}
