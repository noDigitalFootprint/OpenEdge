namespace OpenEdge.scripts;

internal class LostConnection : TalkBaseClass
{
	public LostConnection(MainWindow mw)
		: base(mw)
	{
		mw.changeMoodBy(-3);
		setFlag("FLAGT:sessionIntro", temp: true);
		allText = mw.lr.getScript("lostConnection");
	}

	private string[] createSessionIntro()
	{
		switch (getMyScript())
		{
		case 0:
			if (!mw.getTFlag("petPlay"))
			{
				return new string[7] { "ah, you're back", "it seems we lost each other for a bit", "I hope you've left everything as it was before we got disconnected", "ISFLAGT:ballsBound so your @balls should still be tied", "ISFLAGT:plug your ass should be plugged", "ASK:are you ready to get back into it?", "[I'm ready]" };
			}
			break;
		case 1:
			if (!mw.getTFlag("petPlay"))
			{
				return new string[7] { "this shitty connection of mine!", "I'm sorry about that, seems like my internet dropped the ball there", "good job checking in with me, I hope you haven't undone anything yet", "ISFLAGT:ballsBound so your @balls should still be tied", "ISFLAGT:plug your ass should be plugged", "ASK:are you ready to get back into it?", "[I'm ready]" };
			}
			break;
		case 2:
			if (!mw.getTFlag("petPlay"))
			{
				return new string[6] { "god these connection issues are annoying", "I'm going to attack the next IT specialist I see", "but at least the connection is back up, so let's continue", "ISFLAGT:plug I hope your plug is still sitting firmly in your behind", "ASK: are you ready to pick up where we left off?", "[I'm ready]" };
			}
			break;
		case 3:
			if (mw.getTFlag("petPlay"))
			{
				return new string[7] { "hey GETVAR:petName that must have been scary", "all alone, no owner in sight", "don't worry you found me again, good @boy!", "you're such a good @pup!", "ISSETTING:treats you may take a treat as a reward", "ASK: are you ready to pick up where we left off?", "[@yes]" };
			}
			break;
		}
		return createSessionIntro();
	}
}
