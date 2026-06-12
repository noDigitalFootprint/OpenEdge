namespace OpenEdge.scripts;

internal class ChangeState : TalkBaseClass
{
	public ChangeState(MainWindow mw)
		: base(mw)
	{
		allText = mw.lr.getScript("changeState");
	}

	private string[] setState()
	{
		switch (getMyScript())
		{
		case 0:
			if (mw.isSettingEnabled("collar") && mw.getTFlag("collar"))
			{
				return new string[2] { "STOPSTROKING:", "COLLARNO" };
			}
			break;
		case 1:
			if (mw.isSettingEnabled("plug") && mw.getTFlag("plug"))
			{
				return new string[10] { "STOPSTROKING:", "take your plug from your ass and re-insert it", "keep fucking your tight little hole like that", "push it in deep and then pull it out", "again and again", "do you feel your ass getting looser?", "soon you'll be able to take bigger and bigger plugs", "stop fucking your hole", "ASK:re-apply the lube on your plug if necessary and get ready to stroke again", "[I'm ready @missTitle]" };
			}
			break;
		case 2:
			if (mw.isSettingEnabled("clothesPins") && !mw.getTFlag("clothesPins"))
			{
				return new string[2] { "STOPSTROKING:", "CLAMP" };
			}
			break;
		case 3:
			if (mw.isSettingEnabled("clothesPins") && mw.getTFlag("clothesPins"))
			{
				return new string[2] { "STOPSTROKING:", "CLAMPNO" };
			}
			break;
		case 4:
			if (mw.isSettingEnabled("plug") && !mw.getTFlag("plug"))
			{
				return new string[2] { "STOPSTROKING:", "PLUGASS" };
			}
			break;
		case 5:
			if (mw.isSettingEnabled("plug") && mw.getTFlag("plug"))
			{
				return new string[2] { "STOPSTROKING:", "PLUGASSNO" };
			}
			break;
		case 6:
			if (mw.isSettingEnabled("string") && !mw.getTFlag("ballsBound"))
			{
				return new string[3] { "STOPSTROKING:", "I'll strangle your cum back in your @balls", "BINDBALLS" };
			}
			break;
		case 7:
			if (mw.isSettingEnabled("string") && mw.getTFlag("ballsBound"))
			{
				return new string[3] { "STOPSTROKING:", "having your @balls bound makes everything so much more sensitive", "BINDBALLSNO" };
			}
			break;
		case 8:
			if (mw.isSettingEnabled("cei"))
			{
				return new string[22]
				{
					"STOPSTROKING:", "(start)", "grab your @cock from the base and squeeze towards the tip", "really milk your rod until you get yourself a nice drop of precum", "ASK:have you got it?", "[@yes]", "GOTO:mid", "@no", "luckily we've got an easy solution for that", "EDGE:",
					"EDGE:", "EDGE:", "GOTO:start", "(mid)", "put the precum on your index and middle finger", "lick your fingers clean, really run your tongue over the whole length", "don't swallow, just let it pool in your mouth", "tilt your head backwards and gargle that mixture of precum and spit for me", "I want you to get messy for me", "slowly drool all over yourself, cover your @cock with spit",
					"yes I think this look serves you much better", "now the outside is just as dirty as the inside"
				};
			}
			break;
		case 9:
			if (mw.isSettingEnabled("cei"))
			{
				return new string[23]
				{
					"STOPSTROKING:", "(start)", "grab your @cock from the base and squeeze towards the tip", "really milk your rod until you get yourself a nice drop of precum", "ASK:have you got it?", "[@yes]", "GOTO:mid", "@no", "luckily we've got an easy solution for that", "EDGE:",
					"EDGE:", "EDGE:", "GOTO:start", "(mid)", "put the precum on your index and middle finger", "deepthroat your fingers with your cum still on there, keep your fingers there", "you're a joke of a @man, sitting here with cum dripping down your throat", "CHANCE:40 EDGE:", "CHANCE:30 EDGEHOLD:", "CHANCE:40 EDGE:",
					"are you enjoying your little treat? we went through a lot of trouble making it", "you can take your fingers out of your throat now", "be sure to suck them clean, not that your hungry throat left a lot on there"
				};
			}
			break;
		case 10:
			if (mw.isSettingEnabled("cei"))
			{
				return new string[24]
				{
					"STOPSTROKING:", "(start)", "grab your @cock from the base and squeeze towards the tip", "really milk your rod until you get yourself a nice drop of precum", "ASK:have you got it?", "[@yes]", "GOTO:mid", "@no", "luckily we've got an easy solution for that", "EDGE:",
					"EDGE:", "EDGE:", "GOTO:start", "(mid)", "put the precum on your index and middle finger", "spread it on your lips", "you must be hungry, but don't lick it up", "CHANCE:40 EDGE:", "just leave your sticky precum there as a lip gloss", "CHANCE:30 EDGEHOLD:",
					"when you've truly forgotten about the cum on your lips I'll let you lick them", "CHANCE:40 EDGE:", "instantly aroused as the taste takes you back to this moment", "(end)"
				};
			}
			break;
		case 11:
			if (mw.isSettingEnabled("cei"))
			{
				return new string[25]
				{
					"STOPSTROKING:", "(start)", "grab your @cock from the base and squeeze towards the tip", "really milk your rod until you get yourself a nice drop of precum", "ASK:have you got it?", "[@yes]", "GOTO:mid", "@no", "luckily we've got an easy solution for that", "EDGE:",
					"EDGE:", "EDGE:", "GOTO:start", "(mid)", "put the precum on your index and middle finger", "smear it directly under your nose", "CHANCE:40 EDGE:", "breathe in deeply", "it stinks a bit doesn't it?", "but it also smells of sex",
					"it smells like desire, lust and ache", "CHANCE:30 EDGEHOLD:", "CHANCE:40 EDGE:", "too bad you won't find out what your cum smells like", "you will have to make do with this watery substitute"
				};
			}
			break;
		case 12:
			if (mw.isSettingEnabled("collar") && !mw.getTFlag("collar"))
			{
				return new string[2] { "STOPSTROKING:", "COLLAR" };
			}
			break;
		case 13:
			return new string[0];
		}
		return setState();
	}
}
