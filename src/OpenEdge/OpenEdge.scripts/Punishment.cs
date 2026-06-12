namespace OpenEdge.scripts;

internal class Punishment : InteruptTalk
{
	public Punishment(MainWindow mw, TalkBaseClass homeTalk)
		: base(mw, homeTalk)
	{
		allText = mw.lr.getScript("punishment");
	}

	private string[] setPunishmentIntro()
	{
		return new string[1] { "STOPSTROKING:" };
	}

	private string[] setPunishment()
	{
		switch (getMyScript())
		{
		case 0:
			return new string[3] { "I'm going to punish you", "MOOD:3", "CBTBALLS:" };
		case 1:
			return new string[3] { "I'm going to punish you", "MOOD:2", "CBTCOCK:" };
		case 2:
			return new string[3] { "I'm going to punish you", "MOOD:3", "ASSSPANKING:" };
		case 3:
			return new string[3] { "I'm going to punish you", "MOOD:2", "NIPPLEPINCHING:" };
		case 4:
			if (mw.mood < 30)
			{
				return new string[3] { "I'm going to punish you", "MOOD:8", "CBTEXTREME:" };
			}
			break;
		case 5:
			if (int.Parse(mw.getVar("noVideoValue")) > 0)
			{
				return new string[2] { "MOOD:2 ADDVAR:noVideoValue,1", "you've just added another day to your video ban" };
			}
			break;
		case 6:
			if (!mw.orgasmDenied && mw.mood < 20)
			{
				return new string[4] { "MOOD:10", "it's too bad, but it seems like you won't be cumming today", "maybe try a bit harder tomorrow to prevent this", "ORGASMDENIED:" };
			}
			break;
		case 7:
			if (mw.isSettingEnabled("anal"))
			{
				return new string[2] { "MOOD:3", "ANAL:" };
			}
			break;
		case 8:
			if (mw.isSettingEnabled("anal") && mw.mood < 30)
			{
				return new string[2] { "MOOD:7", "ANALEXTREME:" };
			}
			break;
		case 9:
			if (!mw.getTFlag("censoredSession") && !mw.getTFlag("noCensors") && mw.mood < 50)
			{
				return new string[5] { "FLAGT:censoredSession", "CENSORON", "MOOD:6", "you've just ruined any chance of seeing your precious pictures without censors in the way", "but I'm sure you know what's behind those bars right?" };
			}
			break;
		case 10:
			if (mw.getFlag("censor") && mw.mood < 20 && !mw.getTFlag("noCensors"))
			{
				return new string[8] { "MOOD:20", "you've really been pissing me off @subTitle", "again and again you've shown disrespect", "and frankly I'm sick of it", "so, I'm increasing your censors", "permanently", "you like denial right? then enjoy staring at a blacked out screen", "ADDVAR:censorIncrease,2" };
			}
			break;
		case 11:
			if (mw.mood < 20)
			{
				return new string[22]
				{
					"MOOD:8", "don't you dare stroke that @cock of yours", "just sit there and stare", "this is what you deserve", "this is what happens when you try to stand up for yourself", "you're too weak to lead but still think that you don't have to follow", "I'll make this painstakingly clear", "you belong beneath me", "crawling on the floor like the worthless trash you are", "but it seems you've forgotten your place",
					"you think that it's unfair what I demand of you", "but even if I wouldn't ever let you touch yourself again, it would still be within my rights", "for tonight I'm done here, I've got no time for disobedient slaves", "but that doesn't mean you're done as well", "before the next time that we meet I want you to spend a full hour watching porn", "you are going to strip naked and wrap your hand around that @cock of yours", "of course you won't stroke yourself for the whole hour", "and I couldn't care less about what kind of porn it is that you're watching", "just make it as arousing as possible", "then, I'll see you next time, hopefully this will teach you just how kind I've been",
					"FLAG:hourSpentWatching FLAG:task", "ENDSESSION"
				};
			}
			break;
		case 12:
			if (mw.isSettingEnabled("petPlay"))
			{
				return new string[2] { "@petPlayScold", "MOOD:1" };
			}
			break;
		case 13:
			if (mw.isSettingEnabled("palming"))
			{
				return new string[2] { "MOOD:2", "CBTPALMING:" };
			}
			break;
		case 14:
			if (mw.isSettingEnabled("taskScreen"))
			{
				return new string[5] { "MOOD:8", "it would seem you need some extra homework", "TASK:2,4", "ah and I've taken the liberty of activating the task as well so you'd better get on that", "I'll just take the favor you would have gotten by activating it yourself as compensation for your behavior" };
			}
			break;
		case 15:
			if (mw.isSettingEnabled("findom"))
			{
				return new string[7] { "MOOD:6", "SHOWFAVOR:", "I had hoped that you would turn more obedient now that I can take your favor from you", "you're not misbehaving just so I'll take your cash from you right?", "TRIBUTE:3", "well either way I'm going to take it", "so just sit there while I take your hard work away from you" };
			}
			break;
		case 16:
			if (mw.isSettingEnabled("findom") && mw.mood < 20)
			{
				return new string[9] { "MOOD:10", "SHOWFAVOR:", "you're pissing me off paypig", "I'm using you for your favor but it hardly seems worth it at this point", "I'm taking more, I've earned it for suffering through your bullshit", "TRIBUTE:8", "did that push you into debt?", "don't bother answering, I truly couldn't care less", "if you run out of funds go and earn some more, it's that simple" };
			}
			break;
		case 17:
			if (mw.isSettingEnabled("taskScreen"))
			{
				return new string[7] { "MOOD:6", "TASK:2,2", "you've been misbehaving quite a bit @subTitle", "I'd almost go as far as to call you a bad @boy", "but don't worry, I've got just the solution in mind", "honest labor", "and I've activated the task already so you'd best get on that once we're done here" };
			}
			break;
		}
		return setPunishment();
	}
}
