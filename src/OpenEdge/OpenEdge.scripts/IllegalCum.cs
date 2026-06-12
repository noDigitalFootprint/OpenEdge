using System.Linq;

namespace OpenEdge.scripts;

internal class IllegalCum : InteruptTalk
{
	public IllegalCum(MainWindow mw, TalkBaseClass homeTalk)
		: base(mw, homeTalk)
	{
		mw.changeMoodBy(-50);
		allText = mw.lr.getScript("illegalCum");
	}

	private string[] setPunishment()
	{
		string[] first = new string[53]
		{
			"are you fucking serious?", "I'm putting all this effort into building up your need and you just throw it all away", "ADDVAR:illegalCum,1", "ASK:you'd better have had the decency to at least ruin it", "[I had a full orgasm]", "you did what?", "why would you keep stroking after tipping over the edge?", "you didn't earn that pleasure", "you didn't earn that release", "you know you can just leave and not come back right?",
			"then you can stroke in whatever fashion you see fit", "we're done for today", "oh and don't bother coming back tomorrow", "I'm taking a break for the next week", "FLAG:fullOrgasm", "ISSETTINGASKED:cockControl ISNOSETTING:cockControl FLAG:cockControl DELFLAG:cockControlNo and I'm taking away your right to your little stroke sessions without me since it's clear that you can't behave yourself", "FLAG:break", "ENDSESSION", "[I ruined my orgasm]", "FLAG:ruinedOrgasm",
			"that was the absolute minimum", "not that I'm going to expect even that from you from now on", "I thought you could at least be trusted to edge without spurting your load", "ISSETTINGASKED:cei ISNOSETTING:cei GOTO:endOfStart", "ASK:needless to say that you're going to swallow anything that came out", "[yes @missTitle]", "that is better", "of course this isn't even your punishment yet", "it's just that you should clean up when you spill something", "GOTO:endOfStart",
			"[I don't want to]", "I'm not asking you, I'm telling you", "you are going to swallow down every little bit of that load", "you think it's disgusting to eat your cum don't you?", "so maybe this will help with your motivation for next time", "ASK:you are going to eat it right?", "[yes @missTitle]", "that is better", "of course this isn't even your punishment yet", "it's just that you should clean up when you spill something",
			"GOTO:endOfStart", "[I refuse to eat my cum]", "FLAG:ceiNo", "you know you can just leave and not come back right?", "then you can stroke in whatever fashion you see fit", "we're done for today", "oh and don't bother coming back tomorrow", "I'm taking a break for the next week", "ISSETTINGASKED:cockControl ISNOSETTING:cockControl FLAG:cockControl DELFLAG:cockControlNo and I'm taking away your right to your little stroke sessions without me since it's clear that you can't behave yourself", "FLAG:break",
			"ENDSESSION", "(endOfStart)", "ISSETTINGASKED:cockControl ISNOSETTING:cockControl FLAG:cockControl DELFLAG:cockControlNo and I'm taking away your right to your little stroke sessions without me since it's clear that you can't behave yourself"
		};
		switch (getMyScript())
		{
		case 0:
			if (mw.getFlagTimeDays("noVideo") >= int.Parse(mw.getVar("noVideoValue")))
			{
				return first.Concat(new string[6] { "I'm taking away all rights to video porn for now", "if you're good I'll let you watch your little videos again in a week", "as long as you don't do anything this stupid before then", "FLAG:noVideo", "ADDVAR:noVideoValue,7", "ENDSESSION" }).ToArray();
			}
			break;
		case 1:
			return first.Concat(new string[12]
			{
				"once we're done here you're going to look up some audio porn for me", "make sure it's at least an hour long", "it can be ASMR or an erotic hypnosis file, as long as it's sound based it's fine", "tonight in bed you're going to play that file", "and all you have to do is sleep", "you can do that right? just ignore the moans and sounds of sex", "you wouldn't get distracted would you?", "after all you just came, clearly you're as satisfied as you can be", "and @subTitle you aren't allowed to touch yourself of course", "sweet dreams @subTitle",
				"FLAG:audioPorn FLAG:task", "ENDSESSION"
			}).ToArray();
		case 2:
			if (mw.isSettingEnabled("plug"))
			{
				return first.Concat(new string[9] { "tomorrow you're going for a walk", "I want you to walk for at least half an hour", "and you're going to do so plugged", "you're not allowed to use earbuds", "I want you to hear the world around you", "see every passerby as they make their way past you", "I'm looking forward to tomorrow already", "FLAG:plugWalk FLAG:task", "ENDSESSION" }).ToArray();
			}
			break;
		case 3:
			return first.Concat(new string[6] { "I want you to ensure that you have a lot of time the next time you see me", "I'm going to get every little bit of the need that you let out back", "so make sure you've got the whole evening free", "don't come back until you have the time", "FLAG:longSession", "ENDSESSION" }).ToArray();
		case 4:
			if (mw.isSettingEnabled("humiliation") && !mw.getFlag("note"))
			{
				return first.Concat(new string[8] { "I'm disappointed in you @subTitle", "but maybe it's not all your fault", "I'm sure you simply forgot about not being allowed to cum without permission", "I must have not made it clear to you", "so let's amend that now", "MAKENOTE:", "sweet dreams @subTitle", "ENDSESSION" }).ToArray();
			}
			break;
		}
		return setPunishment();
	}
}
