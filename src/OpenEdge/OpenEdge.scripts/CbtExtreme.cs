namespace OpenEdge.scripts;

internal class CbtExtreme : InteruptTalk
{
	public CbtExtreme(MainWindow mw, TalkBaseClass homeTalk)
		: base(mw, homeTalk)
	{
		allText = mw.lr.getScript("cbtExtreme");
	}

	private string[] setCbtPunishment()
	{
		switch (getMyScript())
		{
		case 0:
			return new string[6] { "KNEEL", "you're going to hit your @balls", "and you're going to hit them hard", "CBTEXTREMESTART:", "REPEAT @cbtBalls", "now, where were we" };
		case 1:
			return new string[7] { "in a bit you'll start hitting your balls lightly", "every hit has to be a bit harder than the previous one", "the difference can be minimal, but you should always hit harder than the previous hit", "so I'd recommend you start out with light taps", "CBTEXTREMESTART:", "REPEAT @cbtBalls", "now, where were we" };
		case 2:
			if (mw.isSettingEnabled("humiliation"))
			{
				return new string[7] { "you fucking degenerate", "I'm going to knock some sense into those @balls of yours", "stroke once on ever hit", "and hit hard @subTitle", "CBTEXTREMESTART:", "REPEAT @cbtBalls", "now, where were we" };
			}
			break;
		case 3:
			if (mw.getFlag("degrading"))
			{
				return new string[9] { "you piece of shit", "you're going to spank your ass", "ready yourself for the beating of a lifetime", "I want you crying by the end of this", "begging for forgiveness", "KNEEL", "CBTEXTREMESTART:", "REPEAT @cbtSpank", "now, where were we" };
			}
			break;
		}
		return setCbtPunishment();
	}
}
