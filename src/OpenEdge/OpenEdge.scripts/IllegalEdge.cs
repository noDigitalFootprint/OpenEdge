namespace OpenEdge.scripts;

internal class IllegalEdge : InteruptTalk
{
	public IllegalEdge(MainWindow mw, TalkBaseClass homeTalk)
		: base(mw, homeTalk)
	{
		mw.changeMoodBy(-4);
		allText = mw.lr.getScript("illegalEdge");
	}

	private string[] setPunishment()
	{
		switch (getMyScript())
		{
		case 0:
			return new string[10] { "what do you think you're doing?", "oh don't back off now, keep riding the edge @subTitle", "I'll show you just what happens when you edge without explicit permission", "hit the head of your @cock with your other hand", "make sure not to spurt out that watery load from the stimulation", "you can stop now", "STOPSTROKING:", "take a second, so you don't instantly edge again", "ASK:are you ready to continue?", "[@yes]" };
		case 1:
			return new string[8] { "STOPSTROKING:", "you've become too greedy @subTitle", "squeeze the base of your @cock", "look at it bounce around, isn't it pathetic?", "be a dear and smack your @cock with your other hand", "keep going until it's calmed down", "ASK:are you ready to continue?", "[@yes]" };
		case 2:
			return new string[3] { "STOPSTROKING:", "PUNISHMENT:", "stay away from the edge this time" };
		case 3:
			return new string[3] { "STOPSTROKING:", "PUNISHMENT:", "stay away from the edge this time" };
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
			if (mw.isSettingEnabled("plug") && mw.getTFlag("plug"))
			{
				return new string[10] { "STOPSTROKING:", "take your plug from your ass and re-insert it", "keep fucking your tight little hole like that", "push it in deep and then pull it out", "again and again", "do you feel your ass getting looser?", "soon you'll be able to take bigger and bigger plugs", "stop fucking your hole", "ASK:re-apply the lube on your plug if necessary and get ready to stroke again", "[@yes]" };
			}
			break;
		case 9:
			if (mw.isSettingEnabled("cei"))
			{
				return new string[20]
				{
					"STOPSTROKING:", "(start)", "grab your @cock from the base and squeeze towards the tip", "really milk your rod until you get yourself a nice drop of precum", "ASK:have you got it?", "[@yes]", "GOTO:mid", "@no", "luckily we've got an easy solution for that", "EDGE:",
					"EDGE:", "EDGE:", "GOTO:start", "(mid)", "put the precum on your index and middle finger", "deepthroat your fingers with your cum still on there, keep your fingers there", "you're a joke of a @man, sitting here with cum dripping down your throat", "are you enjoying your little treat? we went through a lot of trouble making it", "you can take your fingers out of your throat now", "be sure to suck them clean, not that your hungry throat left a lot on there"
				};
			}
			break;
		case 10:
			if (mw.isSettingEnabled("cei"))
			{
				return new string[21]
				{
					"STOPSTROKING:", "(start)", "grab your @cock from the base and squeeze towards the tip", "really milk your rod until you get yourself a nice drop of precum", "ASK:have you got it?", "[@yes]", "GOTO:mid", "@no", "luckily we've got an easy solution for that", "EDGE:",
					"EDGE:", "EDGE:", "GOTO:start", "(mid)", "put the precum on your index and middle finger", "spread it on your lips", "you must be hungry, but don't lick it up", "just leave your sticky precum there as a lip gloss", "when you've truly forgotten about the cum on your lips I'll let you lick them", "instantly aroused as the taste takes you back to this moment",
					"(end)"
				};
			}
			break;
		case 11:
			if (mw.isSettingEnabled("cei"))
			{
				return new string[22]
				{
					"STOPSTROKING:", "(start)", "grab your @cock from the base and squeeze towards the tip", "really milk your rod until you get yourself a nice drop of precum", "ASK:have you got it?", "[@yes]", "GOTO:mid", "@no", "luckily we've got an easy solution for that", "EDGE:",
					"EDGE:", "EDGE:", "GOTO:start", "(mid)", "put the precum on your index and middle finger", "smear it directly under your nose", "breathe in deeply", "it stinks a bit doesn't it?", "but it also smells of sex", "it smells like desire, lust and ache",
					"too bad you won't find out what your cum smells like", "you will have to make do with this watery substitute"
				};
			}
			break;
		case 12:
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
		case 13:
			if (!mw.getFlagAsked("quickshot") && mw.isSettingEnabled("humiliation") && !mw.getTFlag("petPlay"))
			{
				return new string[28]
				{
					"you're on the edge already?", "ASK: be honest with me, you're a quickshot aren't you?", "[I'm a quickshot]", "FLAG:quickshot", "(quickshot)", "I thought as much", "still hearing you say it is hilarious", "I'll let you get away with edging just this once since you've entertained me", "but from next time forward, I'll punish you again", "so try your hardest to not edge without permission",
					"let's get back to it, my little quickshot", "GOTO:end", "[I'm not a quickshot]", "you're dick is telling a different story", "if it's dripping and shivering from just this much you're going to have a hard time", "do you really believe you've got a strong dick?", "one that can handle what I'm going to throw at it?", "then why did you edge without permission?", "if your dick is supposed to be so strong you should be able to handle this", "so, I'll ask you once more,",
					"ASK: are you a quickshot?", "[yes I'm a quickshot]", "GOTO:quickshot", "[no, I'm not a quickshot]", "so why did you edge without permission then?", "FLAG:quickshotNo", "CBTEXTREME:", "(end)"
				};
			}
			break;
		case 14:
			return new string[6] { "STOPSTROKING:", "when will you get it through your thick skull that you're not allowed to edge without permission?", "I don't care how difficult of a time you're having, just do as you're told", "keep your breathing slow and deep", "relax the muscles in your legs and ass", "you can totally do better than this, my @subTitle" };
		case 15:
			return new string[3] { "STOPSTROKING:", "how hard is it to do as you're told?", "keep your seed inside, that's all you have to do" };
		case 16:
			return new string[5] { "STOPSTROKING:", "you're starting to annoy me @subTitle", "I don't care if it's hard, I just care if you obey", "and I haven't given permission for you to edge yet", "so what the hell do you think you're doing?" };
		case 17:
			return new string[3] { "SPEEDDOWN:", "keep stroking, but don't you dare cum", "even if this whole section turns into an edgehold for you, you're not letting go" };
		case 18:
			return new string[3] { "SPEEDUP:", "stay on the edge then, if you want to edge so badly", "but don't come complaining to me that it's too hard later" };
		case 19:
			if (mw.getFlag("quickshot") && int.Parse(getVar("denied")) > 2 && mw.getFlag("removed"))
			{
				return new string[12]
				{
					"STOPSTROKING:", "I should have left you the second I found out you were a quickshot", "I honestly believe you are well beyond saving", "if just this much makes you edge you won't last through the most basic of training", "and I can hear your excuses already, your stamina is bad because of the denial", "you'll edge faster after holding back for such a long time", "but I truly don't care about any of that", "the fact of the matter is that you would cum as soon as you inserted your @cock in a pussy", "and that is not only pathetic but also selfish", "you can't make a partner feel good if you're cumming the second they brush against your @cock",
					"just admit it, right now you wouldn't make it past the labia before spurting out your load", "and if you're incapable of giving pleasure, then why should we give pleasure to you?"
				};
			}
			break;
		case 20:
			if (mw.mood < 30)
			{
				return new string[11]
				{
					"STOPSTROKING:", "why are you so intent on disobeying me?", "I've been rather cordial with you as far as I'm concerned", "and you've been acting like a whiney little bitch for a while now", "you don't care do you?", "you don't care about the effort I'm putting into this", "you don't care about the rules I so graciously made for you", "you only care about your @cock", "CBTEXTREME:", "keep acting like this and you'll regret it",
					"and that's a promise not a threat"
				};
			}
			break;
		case 21:
			if (!mw.getTFlag("tagPunishment"))
			{
				return new string[9] { "STOPSTROKING:", "it seems you need to calm down for a minute", "but that doesn't mean you just get to sit on your hands", "our time together is short enough as is", "so I may as well make you do something useful", "you're going to tag some of your images for me", "you should have calmed down a little by the time you're done", "ACTIVATETAGGER:", "FLAGT:tagPunishment" };
			}
			break;
		case 22:
			if (!mw.getTFlag("tagPunishment"))
			{
				return new string[6] { "STOPSTROKING:", "have I not made myself clear?", "you do not edge unless allowed to do so", "just go and tag some media while your @cock calms down", "ACTIVATETAGGER:", "FLAGT:tagPunishment" };
			}
			break;
		case 23:
			if (!mw.getTFlag("tagPunishment"))
			{
				return new string[5] { "STOPSTROKING:", "then I guess it's time for a bit of a break", "since you can't handle what I've got in store for you without it", "ACTIVATETAGGER:", "FLAGT:tagPunishment" };
			}
			break;
		case 24:
			return new string[4] { "STOPSTROKING:", "you seem to be having a hard time, so I'm lowering the average stroke speed", "maybe you'll stop whining now", "ADDVAR:strokeMod,-1" };
		case 25:
			return new string[4] { "STOPSTROKING:", "maybe I expected a bit too much from that @cock", "I'll have you stroke a bit slower from now on", "ADDVAR:strokeMod,-1" };
		case 26:
			return new string[4] { "STOPSTROKING:", "you're actually trying right?", "maybe that dick is a bit weaker than expected", "ADDVAR:strokeMod,-1" };
		case 27:
			if (mw.isSettingEnabled("findom"))
			{
				return new string[12]
				{
					"STOPSTROKING:", "you know that you weren't allowed to edge right?", "SHOWFAVOR: I think a bit of a fine is in order", "TRIBUTE:2 oh, and don't worry about handing it over", "ISFLAGT:petPlay GOTO:end", "I can take it from you whenever I want", "so thank me for only taking two favor", "ASK: since I can financially ruin you whenever I want", "[thank you for taking my favor @missTitle]", "you're such a good wallet",
					"happy to be used", "(end)"
				};
			}
			break;
		}
		return setPunishment();
	}
}
