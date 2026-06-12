namespace OpenEdge.scripts;

internal class Ending : TalkBaseClass
{
	public Ending(MainWindow mw)
		: base(mw)
	{
		_ = new string[7] { "CHANCE:80 GOTO:endCont", "LENGTH:360", "@fakeEnding", "GOTO:skipOD", "(endCont)", "ORGASMDECIDE:", "(skipOD)" };
		allText = mw.lr.getScript("ending");
	}

	private string[] createStory()
	{
		switch (getMyScript())
		{
		case 0:
			return new string[6] { "we've come to the end of the session", "I've got things to do so I'll keep it brief this time", "EDGE:", "and once more", "EDGEHOLD:", "GOTO:endCont" };
		case 1:
			return new string[5] { "I've had enough of you for today", "so lets just cut this off here", "see you tomorrow then", "ah, I should probably give you a chance to cum right?", "although I wouldn't have high hopes for a full orgasm if I were you" };
		case 2:
			if (mw.orgasmDenied)
			{
				return new string[5] { "once again we're at the end", "not that it matters to you", "@EDGE:", "I mean, you already know that I'm going to deny you", "and I intend to keep my word" };
			}
			break;
		case 3:
			if (3 <= int.Parse(mw.getVar("denied")))
			{
				return new string[7] { "and with that you've made it through yet another session", "it's starting to hurt right?", "that primal urge to cum is coursing through your whole body", "infecting every thought with desire", "I may just give you the release you so desperately need", "but you're looking so sexy right now", "I may just want to keep you like this indefinitely" };
			}
			break;
		case 4:
			return new string[3] { "if you want that orgasm you'd better stay on your best behavior", "as long as you obey pleasure will come to you", "and even if I deny you, I'll be so very proud of you for suffering for me" };
		case 5:
			return new string[34]
			{
				"your odds for getting an orgasm are looking pretty abysmal", "maybe try a bit harder if you want to feel anything other than pain", "but I'm a gracious domme", "I'll let you edge as much as you want to improve your odds", "but I expect you to do all these edges back to back", "and I won't let you back out once you've started", "ASK:so, how many edges do you want to do?", "[1]", "are you really having that hard of a time today @subTitle?", "I'll show you just how agonizing a single edge can be",
				"EDGEHOLD:40000", "GOTO:end", "[5]", "just five huh? and here I was trying to help you out", "if you end up denied, you know who to blame", "EDGE:5", "GOTO:end", "[10]", "a modest amount, for a dull @man", "you'd better make these edges absolutely excruciating if you want to keep my interest",
				"EDGE:10", "GOTO:end", "[15]", "I wonder if you can handle another fifteen edges", "your @cock is twitching and jerking around already", "EDGE:15", "GOTO:end", "[20]", "how desperate you must have been to let me make you edge another twenty times", "you've brought this torture upon yourself so I don't want to hear any complaints",
				"EDGE:20", "GOTO:end", "(end)", "so let's see if all that edging has helped or if you've just increased your suffering tenfold"
			};
		case 6:
			if (3 <= int.Parse(mw.getVar("sessions")))
			{
				return new string[6] { "it's that time once again", "that time where I decide if you get to sleep tonight", "or if you'll be squirming throughout", "rubbing your legs against each other involuntarily", "your body demanding release", "but we both know that your body is weaker than my commands" };
			}
			break;
		case 7:
			if (3 <= int.Parse(mw.getVar("denied")))
			{
				return new string[7] { "you're turning more obedient by the day", "so pent up", "so full of ache", "but there is nothing you can do", "except pray that I'll let you have that blissful release", "worship me as your deity and I may just be amused enough to let you cum", "though if it turns me on your denial is almost guaranteed" };
			}
			break;
		case 8:
			if (3 <= int.Parse(mw.getVar("denied")))
			{
				return new string[8] { "we've reached the end of the session", "so it's time for me to make a choice", "have you earned that orgasm yet?", "have you suffered enough?", "aww, you look so unsure", "what will even more denial do to you?", "what will you look like if I let my desires run free?", "EDGE:" };
			}
			break;
		}
		return createStory();
	}
}
