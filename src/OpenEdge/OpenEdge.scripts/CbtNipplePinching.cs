namespace OpenEdge.scripts;

internal class CbtNipplePinching : InteruptTalk
{
	public CbtNipplePinching(MainWindow mw, TalkBaseClass homeTalk)
		: base(mw, homeTalk)
	{
		allText = mw.lr.getScript("cbtNipplePinching");
	}

	private string[] setPunishment()
	{
		switch (getMyScript())
		{
		case 0:
			return new string[4] { "pinch both of your nipples hard", "roll them in between your thumb and index-finger", "CBTSTART:", "REPEAT @cbtNipples" };
		case 1:
			return new string[5] { "since you clearly need a physical reminder as to who your owner is", "I'm going to bully your sensitive nipples", "squeeze down hard on every beat", "CBTSTART:", "REPEAT @cbtNipples" };
		case 2:
			return new string[5] { "you belong to me to do with as I please", "and right now I want you squirming", "pull your nipples away from your body on every beat", "CBTSTART:", "REPEAT @cbtNipples" };
		case 3:
			return new string[16]
			{
				"pull on your nipples for me", "harder", "@cbtNipples", "keep holding them like that", "@cbtNipples", "almost done, pull extra hard for the last stretch", "@cbtNipples", "just like that", "@cbtNipples", "only a little bit longer",
				"@cbtNipples", "don't you dare ease up on the pressure", "you may let go of your abused nipps for now", "just look at them for a bit", "they're standing straight", "your nipples are more erect than your @cock"
			};
		case 4:
			if (mw.isSettingEnabled("clothesPins") && !mw.getTFlag("clothesPins"))
			{
				return new string[2] { "STOPSTROKING:", "CLAMP" };
			}
			break;
		}
		return setPunishment();
	}
}
