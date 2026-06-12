namespace OpenEdge.scripts;

internal class CbtBalls : InteruptTalk
{
	public CbtBalls(MainWindow mw, TalkBaseClass homeTalk)
		: base(mw, homeTalk)
	{
		allText = mw.lr.getScript("cbtBalls");
	}

	private string[] setCbtPunishment()
	{
		switch (getMyScript())
		{
		case 0:
			if (!mw.getTFlag("kneel"))
			{
				return new string[6] { "KNEEL", "you are going to slap your @balls for me", "COMMAND:now get ready, slap them with your flat palm", "CBTSTART:", "REPEAT @cbtBalls", "now, where were we" };
			}
			break;
		case 1:
			return new string[4] { "grab your @balls, squeeze every beat", "CBTSTART:", "REPEAT @cbtBalls", "lets get back to it" };
		case 2:
			return new string[4] { "take your @balls into your hand, pull on them and squeeze on every beat", "CBTSTART:", "REPEAT @cbtBalls", "lets get back to it" };
		case 3:
			return new string[4] { "I want you to ball up your hand and get ready to hit your @balls for me", "CBTSTART:", "REPEAT @cbtBalls", "lets get back to it" };
		case 4:
			return new string[4] { "flick those testicles on every beat", "CBTSTART:", "REPEAT @cbtBalls", "lets get back to it" };
		}
		return setCbtPunishment();
	}
}
