namespace OpenEdge.scripts;

internal class LOB : TalkBaseClass
{
	public LOB(MainWindow mw)
		: base(mw)
	{
		allText = mw.lr.getScript("LOB");
		setFlag("LOBScript", temp: true);
		deleteFlag("shutDown");
	}

	private string[] createLOB()
	{
		switch (getMyScript())
		{
		case 0:
			if (mw.getTFlag("shutDown"))
			{
				return new string[8] { "did you have a hard time keeping focus with my little program?", "since you shut it down and all", "ASK: I can make it a little less intense for you if you'd like", "[please make it less intense]", "ADDVAR:LOBMod,-1 alright I've made it a bit less oppressive GOTO:end", "[the program is fine as is]", "so why did you shut it down then?", "(end)" };
			}
			break;
		case 1:
			if (mw.getFlag("shutDown"))
			{
				return new string[8] { "it looks like you've shut down my gift for you", "was it too much?", "ASK: I can make it a little less intense for you if you'd like", "[please make it less intense]", "ADDVAR:LOBMod,-1 it should be a little easier to deal with now GOTO:end", "[the program is fine as is]", "then there should be no reason to turn it off right?", "(end)" };
			}
			break;
		case 2:
			if (mw.getFlag("shutDown"))
			{
				return new string[8] { "I can see you shut down my little program MOOD:-4", "I've put in a lot of effort in making it especially for you", "ASK: if it's too much we can tone it down a little", "[please make it less intense]", "ADDVAR:LOBMod,-1 GOTO:end", "[the program is fine as is]", "then there should be no reason to turn it off right?", "(end)" };
			}
			break;
		case 3:
			if (mw.getFlag("shutDown"))
			{
				return new string[2] { "you turned my program off didn't you? MOOD:-4", "ADDVAR:LOBMod,-1 I'm going to make it a little less intense, then you won't have to turn it off anymore" };
			}
			break;
		case 4:
			if (mw.getFlag("shutDown"))
			{
				return new string[4] { "you really are missing out by shutting down my program MOOD:-4", "missing out on ache", "on so much frustration that your @cock won't ever stop leaking", "ADDVAR:LOBMod,1 I'm increasing the intensity of the program, since you need to make up some ground" };
			}
			break;
		case 5:
			if (mw.getFlag("shutDown"))
			{
				return new string[7] { "did you think I wouldn't notice you shutting down my program? MOOD:-4", "ASK: can you not handle the ache?", "[please make it less intense]", "ADDVAR:LOBMod,-1 GOTO:end", "[the program is fine as is]", "so don't shut it down then @subTitle", "(end)" };
			}
			break;
		case 6:
			if (!mw.getFlag("shutDown"))
			{
				return new string[9] { "you're being such a good @boy for keeping my program running MOOD:4", "ASK: do you want to be even better? just let me increase the intensity a little", "[okay, make it more intense]", "ADDVAR:LOBMod,1 you're such a good @boy for me", "don't worry about the ache that's coming", "since you can't escape it anyway GOTO:end", "[no, the program is fine as is]", "can't take even a little more then?", "(end)" };
			}
			break;
		case 7:
			if (!mw.getFlag("shutDown"))
			{
				return new string[7] { "you've not turned off my program since last time, that's honestly pretty impressive MOOD:4", "ASK: so I'll give you a bit of a reward, would you like for the program to get even more intense?", "[please make it more intense]", "ADDVAR:LOBMod,1 you're going to regret this, you know that right? GOTO:end", "[the program is fine as is]", "can't take even a little more then?", "(end)" };
			}
			break;
		case 8:
			if (!mw.getFlag("shutDown"))
			{
				return new string[9] { "you're doing a fine job, suffering at my programs hand MOOD:4", "now I could make it easier", "but where's the fun in that?", "ASK:making everything even harder though, now that sounds interesting", "[please make it more intense]", "ADDVAR:LOBMod,1 that's what I thought GOTO:end", "[the program is fine as is]", "can't take even a little more then?", "(end)" };
			}
			break;
		case 9:
			if (!mw.getFlag("shutDown"))
			{
				return new string[8] { "I know you've been having a hard time staying focused while at your pc lately MOOD:4", "but it really would make me very happy if you'd let me increase the intensity just a bit", "ASK:it'll be just a little bit harder, you can take that right?", "[please make it more intense]", "ADDVAR:LOBMod,1 that's a good @boy GOTO:end", "[don't make it more intense]", "have you reached your breaking point already?", "(end)" };
			}
			break;
		case 10:
			if (!mw.getFlag("shutDown"))
			{
				return new string[3] { "you're doing a good job so far with my program MOOD:4", "to be honest, you're doing almost too good of a job", "ADDVAR:LOBMod,1 I'm increasing the intensity" };
			}
			break;
		}
		return createLOB();
	}

	private string[] reenableLOB()
	{
		return new string[4] { "I'll let you run my program again @subTitle", "and I hope you've learned something here", "if you want something, just ask", "DELFLAG:LOBBreak well maybe it's more realistic to say that you should beg and plead" };
	}

	private string[] disabledLOB()
	{
		return new string[11]
		{
			"I know I said that the program could be turned off whenever", "but that doesn't mean you can just disable it from booting up without telling me", "did you think I wouldn't notice you turning off my program by hand?", "I was more than happy to disable it for you, that was the deal", "but you seem to think you don't have to ask for permission", "you don't have to beg and plead", "rubbing your forehead on the ground as you prostrate yourself", "I've disabled the program for now", "it'll be a couple of days before I'll allow you to turn it back on", "oh and there's no way you're stupid enough to need me to tell you this",
			"FLAG:LOBBreak but re-enable my program in your launch sequence again"
		};
	}
}
