using System;

namespace OpenEdge.scripts;

internal class AskFlag : TalkBaseClass
{
	public AskFlag(MainWindow mw, string forcedSettingKey = "")
		: base(mw)
	{
		if (!string.IsNullOrWhiteSpace(forcedSettingKey))
		{
			allText = createTaunt(forcedSettingKey);
			if (allText.Length != 0)
			{
				return;
			}
			allText = mw.lr.getScript(forcedSettingKey);
			if (allText.Length != 0)
			{
				return;
			}
			SessionTraceLogger.Info("queued-ask", "no dedicated ask script for " + forcedSettingKey + "; falling back to generic ask");
		}
		if (mw.isSettingAnswered("safeWord") && mw.isSettingAnswered("edgeIntro") && mw.isSettingAnswered("edgeHold") && mw.isSettingAnswered("string") && mw.isSettingAnswered("clothesPins") && mw.isSettingAnswered("humiliation") && (!mw.isSettingEnabled("humiliation") || mw.isSettingAnswered("virgin")) && mw.isSettingAnswered("asmr") && mw.isSettingAnswered("taskScreen") && (mw.isSettingAnswered("findom") || int.Parse(mw.getVar("totalTribute")) <= 10) && mw.isSettingAnswered("gay") && mw.isSettingAnswered("petPlay") && mw.isSettingAnswered("cei") && (!mw.isSettingEnabled("humiliation") || mw.isSettingAnswered("cuck")) && (!mw.isSettingEnabled("humiliation") || mw.isSettingAnswered("censorship")) && mw.isSettingAnswered("feet") && mw.isSettingAnswered("palming") && mw.isSettingAnswered("hypno") && mw.isSettingAnswered("hands") && (!mw.isSettingEnabled("gayHumiliation") || mw.isSettingAnswered("sissy")) && mw.isSettingAnswered("anal") && mw.isSettingAnswered("cockControl") && mw.isSettingAnswered("breathPlay") && (mw.isSettingAnswered("outsideSession") || int.Parse(mw.getVar("sessions")) <= 3) && mw.isSettingAnswered("LOB") && mw.isSettingAnswered("canRemove") && !mw.getFlagAsked("petPlayAdvanced") && mw.isSettingEnabled("petPlay") && !mw.isSettingEnabled("treats"))
		{
			mw.getFlag("collar");
		}
		allText = mw.lr.getScript("ask");
	}

	private string[] createTaunt(string thingToAsk)
	{
		new Random();
		return thingToAsk switch
		{
			"safeWord" => new string[12]
			{
				"I've got a lot of ideas that I want to try out on you", "and some of those ideas are rather rough", "now I know that you probably want me to fuck you up", "but I'm really not interested in doing you any long lasting harm, physical or mental", "so if you're at any point truly at your wits end", "then you can press the F5 key on your keyboard", "ASK: remember this, just press F5 to say your safeword whenever you need it", "[I'll remember that it's the F5 key]", "good @boy", "just remember that we're both here to have fun in our own twisted ways",
				"and I'd hate to break you before I'm done playing with you", "FLAG:safeWord"
			}, 
			"hands" => new string[14]
			{
				"hey, I'm sure that this is some kind of a mistake right?", "ASK: you're not into hands are you?", "[no I'm not into hands]", "FLAG:handsNo that's what I expected, still you do seem to think about them quite a bit", "GOTO:end", "[yes I'm into hands]", "FLAG:hands this is even weirder than being into feet", "not that being into weird stuff doesn't suit you", "rather, in a way this makes sense",
				"why would you sexualize something that you've got zero hope of ever feeling around your @cock", "with a hand there's still the hope of a pity handjob somewhere in the far future", "not that you should count on it, I most certainly won't be the one handing out such favors", "(end)", ""
			}, 
			"virgin" => new string[35]
			{
				"STOPSTROKING:", "KNEEL", "I could leave you like this", "on your knees, drooling at all the pretty pictures on the screen", "but unable to touch", "unable to release all of that pent up desire", "your hands trembling softly as you resist the urge for my pleasure", "maybe I should call it amusement instead?", "ISSETTING:sph with that shrimp dick of yours I'm pretty certain that you've never given a woman pleasure", "ISSETTING:sph if you've ever touched one in the first place",
				"ASK: are you a virgin @subTitle?", "(virgin)", "[yes @missTitle]", "I expected as much but it's still weirdly satisfying to hear you say it", "say it again, out loud this time \"I am a virgin\"", "\"I am a virgin who doesn't know what a pussy feels like\"", "isn't that just amazing, here you are jerking off to the thought of pussy while you've never even felt what it's like", "the heat and wetness, its sweet scent that makes your @cock as hard as a rock", "you will never experience any of these things", "you'll just stay here kneeling in front of your @missTitle",
				"and in a dark corner of your mind I think you are also excited by the thought of it", "FLAG:virgin", "GOTO:end", "[no @missTitle]", "good for you", "to be honest I was kind of hoping you were still a virgin just to ridicule you with it", "sadly that won't be possible now", "unless you want me to act like you're a virgin as roleplay", "ASK:do you want me to ridicule you like you're a virgin?", "[yes @missTitle]",
				"GOTO:virgin", "[no @missTitle]", "okay, noted", "FLAG:virginNo", "(end)"
			}, 
			"gay" => new string[28]
			{
				"you know @subTitle, I've noticed something", "something about the porn you so love to stroke to", "something thick and veiny", "I'm talking about all the dicks you keep circling back to", "you even went as far as calling it the focus of the piece", "so I take it you're not going to act like you didn't want this", "I'll still ask you, just in case", "ASK: are you okay with gay content or would you rather block it", "[no, I'd rather you block it]", "FLAG:gayNo",
				"FLAG:gayHumiliationNo", "okay, I won't bring it up again", "GOTO:end", "[yes I'm fine with gay content]", "FLAG:gay", "ISSETTINGASKED:humiliation ISNOSETTING:humiliation GOTO:end FLAG:gayHumiliationNo", "you're such a cute little faggot for me", "ASK:how does that feel, being called a faggot?", "[thank you for humiliating me]", "that's a good slut",
				"I'll make sure you don't forget who your owner is", "FLAG:gayHumiliation", "GOTO:end", "[please do not call me that]", "I understand, sexuality is often a sensitive topic, just know that this was for roleplays sake", "FLAG:gayHumiliationNo", "(end)", ""
			}, 
			"sounding" => new string[33]
			{
				"you've so far shown yourself to be rather \"creative\" with your sexual outlets", "don't get me wrong here, I don't mind", "I find it absolutely adorable how depraved you are", "and I'm the one who made you that way in the first place", "though I'm sure that even before you met me you were the adventurous sort", "you've simply gotten too horny to hold back on your desires", "but to be honest I've got a desire of my own", "and this is kind of out there so be sure to think this through before answering", "I've always wanted to try sounding", "ASK:are you familiar with it?",
				"[yes, I know what sounding is]", "I could have known that just looking at your porn", "GOTO:requestSounding", "[no, I don't know what sounding is]", "sounding is amazing, though it can look rather strange", "your @cock doesn't only have nerves that can be teased on the outside", "your urethra is incredibly sensitive as well", "sounding is the practice of stimulating these rarely touched nerves", "and that's done by sticking something slim and smooth up there", "many people have their reservations about sticking something up their penises",
				"but I honestly just want you to give it a try, I'd be satisfied with just that", "(requestSounding)", "ASK: would you try it with me?", "[yes @missTitle]", "really!", "I totally expected you to turn me down", "oh, this is going to be amazing", "ASK: I take it you've got something in mind that you can use then?", "[yes @missTitle]", "[no @missTitle]",
				"[no @missTitle]", "okay, I get that this isn't for everyone", "if you ever change your mind I'd love for you to tell me"
			}, 
			"feet" => new string[24]
			{
				"just look at those pretty little toes", "ASK: you're into feet aren't you", "[yes I'm into feet]", "FLAG:feet", "of course, I barely even had to ask", "even before you admitted it I had this vague feeling in my head", "a feeling that standard nudity would simply be too much for you", "a feeling that few things make as much sense in this world as you being a foot freak", "you just can't help yourself",
				"how are you supposed to resist", "those perfect soles", "and those cute toes", "GOTO:end", "[no, I'm not into feet]", "are you sure?", "you're really going to act like the idea does nothing for you?", "ASK:or are you too ashamed to admit to being a pathetic little foot lover?", "[I'm really not into feet]", "[I don't like feet that much]",
				"that's fine, denial can take time", "if you ever decide to be true to your desires I'll indulge you", "FLAG:feetNo", "(end)", ""
			}, 
			"taskScreen" => new string[14]
			{
				"I told a friend of mine about our sessions", "and she made a good point", "why am I putting in all this effort even outside of our sessions", "while you just sit there twiddling your thumbs", "and sure that @cock is screaming out for attention constantly", "but apart from pre-cum it's not making anything", "so I want you to put in some work as well", "next time before starting a session with me give the Tasks screen a look", "SHOWFAVOR:", "now, all of these tasks are optional and they'll reward you with favor",
				"since tasks have to be bought I'll give you five favor to start out with", "TRIBUTE:-5", "TASK: TASK: TASK: TASK: TASK: TASK: TASK: TASK:", "FLAG:taskScreen"
			}, 
			"findom" => new string[31]
			{
				"SHOWFAVOR:", "you've been busy haven't you?", "every time I ask for it you hand over favor", "so either you're in a shit-ton of debt or you've been working your ass off for me", "either one's fine with me, all that I see is you handing over funds", "ASK: do you like it, handing over your hard earned cash to me?", "[no @missTitle]", "so it's just been your @dick that's pushing you to hand over your belongings?", "FLAG:findomNo", "GOTO:end",
				"[yes @missTitle]", "then can I just skip out on the whole asking part from now on?", "I mean if it feels good I can save the both of us some effort", "sure, there was the small issue of it being unfair of me to not give anything back when I take your cash", "but now that you've admitted that it feels good to hand over favor that's been resolved as well", "and naturally if I increase how much I take from you the pleasure increases as well", "ASK: isn't that right my tribute slave?", "[yes @missTitle, please take my favor]", "FLAG:findom", "that's what I thought",
				"EDGE: TRIBUTE:1", "I hope you know what you've just gotten yourself into", "EDGE: TRIBUTE:1", "EDGE: TRIBUTE:1", "I'll keep taking more and more", "and you'll love every second of it", "GOTO:end", "[no @missTitle, please let me keep it]", "I thought it was a rather fair trade, but you do you", "FLAG:findomNo",
				"(end)"
			}, 
			"sissy" => new string[19]
			{
				"are you into feminization?", "wearing cute clothes, applying makeup", "and watching the mascara trickle down your cheeks as you choke on a dick", "you'd be nothing more than a tool for pleasure", "used and discarded like the sissy you are", "ASK: would you like that?", "[please turn me into a sissy]", "FLAG:sissy", "that's a good sissy slut", "you've fantasized about getting on your knees to serve someone before haven't you?",
				"and who hasn't had these fantasies before", "the difference is just that you'll follow through", "and to help you along on your journey, I'll train you into the perfect little slut", "ready to be used", "GOTO:end", "[no, I wouldn't]", "FLAG:sissyNo", "that's fine as well, I'm just going to have to make you submit in different ways", "(end)"
			}, 
			"breathPlay" => new string[29]
			{
				"STOPSTROKING:", "there are so many more ways to torture you", "are you familiar with breath play?", "the basics of it are pretty simple", "you hold your breath when you're told to do so", "and breathe as instructed", "ASK: do you want to try it?", "[no @missTitle]", "too bad, I'll just have to find some other way to torture you", "FLAG:breathPlayNo",
				"GOTO:end", "[yes @missTitle]", "since the whole point of breath play is getting lightheaded, it's important that the holds are just barely possible", "so I'm going to ask you to hold your breath as long as possible in a bit", "make sure to take in a deep breath beforehand and keep still, don't hyperventilate though", "ASK:just tell me when you start holding your breath", "[I'm starting now]", "FLAG:fullBreathHold", "ASK:the timer starts now!", "[please let me breathe]",
				"not yet, just a bit more", "you're almost there, keep going", "SETVAR:breathTime,GETTIME:fullBreathHold", "you may breathe again, good job", "you lasted for GETVAR:breathTime seconds and I'll adjust our breath play to match it", "this also means that you have no excuse about being unable to last as long as I ask", "you'll find out just how difficult holding the edge is when your whole body is screaming for air", "FLAG:breathPlay", "(end)"
			}, 
			"cei" => new string[28]
			{
				"STOPSTROKING:", "ASK: do you know what your precum tastes like?", "[yes @missTitle]", "GOTO:yesQuestion1", "[no @missTitle]", "now that's just strange", "it's your own body, why are you scared of it?", "and even more than that, how are you not curious?", "ASK:would you try it for me?", "[yes @missTitle]",
				"that's a good @boy", "FLAG:cei", "GOTO:end", "[no @missTitle]", "just get back to stroking then @subTitle", "since you refuse to experiment", "FLAG:ceiNo", "GOTO:end", "(yesQuestion1)", "ASK:oh? and did you like it?",
				"[I liked it]", "[I disliked it]", "ASK:did someone make you eat it or were you interested in your own taste?", "[someone made me taste myself]", "[I wanted to taste myself]", "that's pretty kinky", "FLAG:cei", "(end)"
			}, 
			"humiliation" => new string[49]
			{
				"STOPSTROKING:", "you may think I have been pretty rough on you, but to be honest I've been holding back", "I try not to scare away new subs before their addiction to me grows strong enough", "only when any semblance of choice has been taken from you will I show my true colors", "however, there is always the chance of you being even more of a masochist than expected", "STOPSTROKING:", "so I'm going to test you", "and the results will dictate how I treat you from now on", "KNEEL", "don't hide anything from me, show me your dick",
				"ASK: it's a bit on the smaller side isn't it?", "[no @missTitle]", "I'll admit you're a bit above average", "even still, you're kneeling in front of me", "obeying my every command", "FLAG:sphNo", "GOTO:sphComplete", "[yes @missTitle]", "FLAG:sph", "ASK: yes what?",
				"[yes my dick is small, @missTitle]", "at least you can admit it yourself", "does this make your little dicklet hard", "being called a small-dicked masochist", "we both know your little twig is too tiny to satisfy anyone", "(sphComplete)", "but that doesn't mean you can't have any fun anymore", "it just means you need someone to guide and instruct you on how to have fun the right way", "the days of you jerking off whenever and however you want are over", "so much freedom isn't suited for you",
				"you want to be controlled", "you want to be left aching and desperate", "I have seen how that dick of yours twitches as I berate you", "and the hunger in your eyes when I tell you to hurt yourself", "this test is mainly to show you what I already know", "and what you've felt in the back of your head", "so just accept it", "ASK: do you want to be humiliated", "[yes @missTitle]", "god you're such a loser",
				"but you're my loser", "I'll take care of you from now on", "FLAG:humiliation", "FLAG:degrading", "GOTO:humilAskEnd", "[no @missTitle]", "more of a @man than expected aren't you?", "FLAG:humiliationNo", "(humilAskEnd)"
			}, 
			"cuck" => new string[24]
			{
				"STOPSTROKING:", "I want to try something new", "since you have admitted to being into humiliation before this should be right up your alley", "see, I'm a needy gall", "these sessions and my toys help me out immensely", "but with our relationship being long distance I miss the physical side of things", "someone to bend me over and put me in my place", "ASK: do you want to try cuckolding with me?", "[no @missTitle]", "FLAG:cuckNo",
				"I understand, and won't bring it up again", "you'll have me all to yourself", "now where were we", "GOTO:cuckEnd", "[yes @missTitle]", "FLAG:cuck", "oh, this will be great", "for me more so than for you, but still", "and don't worry about our relationship deteriorating because of it", "I'll make sure to report in extraordinary detail how it felt to be held down by strong arms",
				"maybe I'll make you imitate me or something", "one thing is certain, that humiliation fetish of yours will only grow stronger", "and so will my influence over you", "(cuckEnd)"
			}, 
			"string" => new string[11]
			{
				"STOPSTROKING:", "do you own a piece of string?", "ASK: it should be at least 30 centimeter long and be about as thick as a shoelace", "[no @missTitle]", "FLAG:stringNo", "that's too bad, try to get your hands on some", "GOTO:stringEnd", "[yes @missTitle]", "FLAG:string", "good, we'll use that in the future so make sure it's clean and ready to use from the next session on",
				"(stringEnd)"
			}, 
			"anal" => new string[76]
			{
				"STOPSTROKING:", "most of your masculinity went out the door the second that you accepted to be my little plaything", "a little still remains though", "that ends today", "I'll take whatever you let me take from you", "and now that I have your @cock and your thoughts", "my next prey is your ass", "don't look so scared, we'll take it slow", "for now we'll just be using your fingers", "I long for the day you'll be able to cum from anal stimulation alone",
				"ASK: do you want to give it a go?", "[no @missTitle]", "then I'll have to find something else to conquer", "for now though, I'll let you off the hook", "FLAG:analNo", "GOTO:end", "[yes @missTitle]", "ASK: are you experienced with anal or is this the first time you're going to try this?", "[I'm experienced]", "then we can skip most of the basics",
				"FLAG:analExperienced", "GOTO:q0", "[I'm a beginner]", "I'll take you through it slowly", "you have to let your body get used to the sensation", "so please prepare yourself for anal next session", "this mainly means going to the bathroom before and cleaning up", "shaving your behind is also recommended but I'll leave that up to you", "try to get your hands on some lube, either water or silicone based", "you can't use silicone based lube with toys, so if you're getting new lube pick something water based",
				"spit works as well but for a comfortable experience I would recommend lube", "but all this is for next time, I'm really happy you're trusting me to guide you through this", "FLAG:analBeginner", "(q0)", "ASK: do you own water based lube?", "[yes @missTitle]", "that's great, it should leave everything nice and slippery", "FLAG:waterLube", "GOTO:q1", "[no @missTitle]",
				"make sure to buy some, it'll make the whole experience way more comfortable", "FLAG:waterLubeNo", "(q1)", "do you own a dildo or a plug?", "ASK: to be clear, I'm talking about one you are currently able to use, not something that's too big to insert", "[I own a dildo]", "so, you got yourself some equipment", "ISSETTING:sph and let me guess it's way bigger than that nub between your legs", "FLAG:plugNo", "FLAG:dildo",
				"GOTO:endAnal", "[I own a plug]", "so, you got yourself some equipment", "ISSETTING:sph and let me guess it's way bigger than that nub between your legs", "FLAG:dildoNo", "FLAG:plug", "GOTO:endAnal", "[I've got both]", "so, you got yourself some equipment", "FLAG:plug",
				"FLAG:dildo", "GOTO:endAnal", "[neither]", "you don't have to, but get yourself a plug training kit if you can", "don't buy anything too big or expensive, it's important that we increase the size in small increments", "ANALSTAGE:experienced since you've been using your fingers for some time it may be fine to skip a size or two", "for the first couple of sessions it's fine to be using your fingers", "but I'd like to move on from that some time in the future", "FLAG:dildoNo", "FLAG:plugNo",
				"GOTO:endAnal", "(endAnal)", "FLAG:anal", "FLAGT:anal", "(end)", "now, let's get back to what you came here for"
			}, 
			"palming" => new string[12]
			{
				"ASK: do you know what palming is @subTitle?", "[yes @missTitle]", "then I'm sure you know the torture that is coming", "but that knowledge won't save you", "if anything it makes it worse", "GOTO:end", "[no @missTitle]", "palming is the practice of running your palm over the head of your @cock", "now that doesn't sound all that bad right?", "you'll soon find out just how wrong you are",
				"(end)", "FLAG:palming"
			}, 
			"edgeIntro" => new string[13]
			{
				"keep stroking for me", "and follow my instructions", "in a bit I will tell you to edge", "and I expect you to get close when I say it", "I want your breathing to get ragged after just this one edge", "you should bring yourself close without going over", "now get ready", "EDGE:", "good @boy, that's what I mean when I tell you to edge",
				"however my main priority will always be to stop you from reaching orgasm without permission", "it's fine to go slower than I command, if it's to prevent orgasm", "FLAG:edgeIntro", ""
			}, 
			"edgeHold" => new string[25]
			{
				"STOPSTROKING:", "there is one edging trick I haven't asked you about yet", "sometimes I can tell you to reach the edge and hold yourself there", "not just touch the edge and stop", "hold it", "stay right on that awful line until I let you back down", "I don't use it unless you say yes", "and if you don't like it, we can leave it disabled", "ASK:do you want to try edge holding?", "[yes @missTitle]",
				"good", "then I'll be allowed to use edge holds from now on", "but remember, avoiding an accidental orgasm still matters more than obeying perfectly", "FLAG:edgeHold", "GOTO:end", "[no @missTitle]", "that's fine", "I'll keep edge holding disabled", "normal edges are more than enough to play with you", "FLAG:edgeHoldNo",
				"GOTO:end", "(end)", "FLAGT:edgeHold", "", ""
			}, 
			"cockControl" => new string[44]
			{
				"STOPSTROKING:", "that @cock just screams out for attention", "and I don't think you have the willpower to deny it pleasure on your own", "so I'll give you a hand, you can do it if it's for me right?", "from now on your @cock belongs to me, it may still be attached to you, but don't mistake the owner", "and wouldn't it be so very wrong to touch someone's genitals without explicit permission", "so you'll wait patiently until I give you the go ahead", "ASK: can you do that for me?", "[no @missTitle]", "(noDomina)",
				"so you want to keep stroking outside of our sessions?", "that's too bad, I hope you're strong enough to hold back on your own", "if you cum because of your little stroke sessions I'll make you hand your @cock over", "so if you want to keep your privilege you'd better behave", "FLAG:cockControlNo", "GOTO:end", "[yes @missTitle]", "KNEEL", "I want you to look down in a bit and inspect your cock closely", "really take in every detail, the shape, the texture and even the smell",
				"in a bit you are going to give it away, so enjoy your @cock while you've got it", "looking at someone's genitals without permission is also so very wrong", "you shouldn't stare once you've handed your @cock over to me", "a flash of nudity is unavoidable, but I expect you to turn your head away or close your eyes after that", "but in the fleeting moments before that happens you can look as much as you want", "so look down and stare", "follow the veins with your fingers", "brush over the head with just a fingernail", "and feel yourself shudder with the need for another, stronger, touch", "always, more, more, more",
				"more pleasure, more porn, more edging", "ASK: and once you're ready hand your @cock over to me", "[I won't hand over my cock]", "GOTO:noDomina", "[please take my cock from me @missTitle]", "good @subTitle", "then it is done, you'll no longer touch my @cock without permission", "and while it's fine during our sessions you shouldn't stare at my @cock", "every time you feel my @cock growing hard, you'll remember this moment", "where before that @cock between your legs didn't seem that interesting to you",
				"now that you're not even allowed to look at it, you desire to stare", "like a forbidden fruit, restricting your view causes it to feel so much more exclusive", "FLAG:cockControl", "(end)"
			}, 
			"asmr" => new string[30]
			{
				"STOPSTROKING:", "ASK:do you think that you've got sensitive ears?", "[yes I do]", "[no I don't]", "well, let's find out together", "ASMRON:0 ASMRON:1", "just close your eyes for a bit, you can open them again when the audio fades", "close your eyes @subTitle", "close your eyes @subTitle", "close your eyes @subTitle",
				"close your eyes @subTitle", "close your eyes @subTitle", "close your eyes @subTitle", "close your eyes @subTitle", "close your eyes @subTitle", "close your eyes @subTitle", "ASMROFF: SNAP:", "you can open your eyes again", "ASK: so, do you have sensitive ears?", "[yes I do]",
				"you just can't help yourself can you", "when someone licks your ears or moans you can't focus on anything else", "it's almost trance-like in nature", "and I'll have you zoning out whenever I want from now on", "FLAG:asmr", "GOTO:end", "[no I don't]", "that is really sad to hear, but I'm sure you'll come around with time", "FLAG:asmrNo", "(end)"
			}, 
			"clothesPins" => new string[14]
			{
				"STOPSTROKING:", "there are many more ways to bully you than what I've shown you so far", "ASK:do you own nipple clamps or something similar like clothespins?", "[yes @missTitle]", "that's great, lets put them to use in our little sessions then", "make sure they're clean, you'll be putting them on more than just your nipples", "FLAG:clothesPins", "GOTO:end", "[no @missTitle]", "try and get some if you can",
				"I'd personally recommend plastic clothespins since they can be used in a bunch of different ways and they're cheap", "FLAG:clothesPinsNo", "GOTO:end", "(end)"
			}, 
			"hypno" => new string[32]
			{
				"STOPSTROKING:", "just imagine how much easier it would be to submit if your thoughts didn't get in the way", "you would be a mindless drone just looking to serve", "all thoughts of things being fair and justified would leave you", "and you would simply accept the world as it comes to you", "many people have a hard time not thinking of anything", "but you may have had more practice than you realize", "edging can feel similar to being in a trance", "where the concept of time seems to slip away a bit", "of just being in the moment, calm and relaxed",
				"where it feels like thoughts have to make it through a thick pink cloud", "anything important will make it through that cloud in your head", "but everything that can wait till later will get lost in the fog", "ASK:would you like that?", "[no @missTitle]", "that's too bad, then we'll have to make you stroke that brain away instead", "FLAG:hypnoNo", "GOTO:end", "[yes @missTitle]", "I knew you would want to submit even deeper",
				"ASK:you're not photosensitive are you?", "[yes I am photosensitive]", "then we really shouldn't go down this path", "I'm sorry but your safety comes first", "FLAG:hypnoNo", "GOTO:end", "[no I'm not photosensitive]", "then we can really cut loose", "FLAG:hypno", "I've been cooking something up just for an occasion like this",
				"you'll see just what I mean with time", "(end)"
			}, 
			"outsideSession" => new string[30]
			{
				"STOPSTROKING:", "I'm sure that denied little dick is making it very hard to focus", "every time you lose your concentration, even for a second, your mind drifts towards porn", "and it's only going to get harder from here", "would you like me to solve that little problem?", "of course I won't let you cum just because it's hard", "instead I'll give you some new rules to follow", "some commandment to keep you on the straight and narrow", "and you in your submissive state will simply obey", "no more doubt and maybe, just the certainty that everything will end up exactly as I want",
				"and this happens to be the same as what you want", "your full submission, like a collar around your neck", "affecting you outside of our little sessions here", "ASK:would you like that?", "[no @missTitle]", "so you would rather keep some freedom outside of our sessions?", "that's fine, but a bit unexpected if I'm being honest", "FLAG:outsideSessionNo", "GOTO:end", "[yes @missTitle]",
				"just these sessions aren't enough anymore", "every time the session ends and you feel the collar slip off it feels horrible", "you're back in the real world, where you have to make decisions", "where you have to think", "so let me help you to keep that mental collar on even outside of these sessions", "now to be clear, you are free to refuse these commands, but you are to do so as soon as I give them", "and if you choose to accept my commandment you can't go against it anymore", "just remember that good @boys get rewards", "FLAG:outsideSession", "(end)"
			}, 
			"fullControl" => new string[9] { "STOPSTROKING:", "I've taken a lot from you over the past couple of days", "but I would argue that I've given you more than that I've taken so far", "I've granted you pain and denial, sleepless nights and an attention deficit", "and all you've had to give up is your @cock", "now clearly this just won't do", "I've seen just how dumb being horny can make a guy", "--------------TODOCHASTITY---------", "(end)" }, 
			"hobby" => new string[38]
			{
				"STOPSTROKING:", "you know, I've been commanding you on how to stroke", "but I kind of want to control what it is you're stroking too as well", "now since I can't see your screen, you'll have to tag the images for me", "however since this is obviously time intensive I'd like to do it outside of our regular session time", "could you spare some extra time outside of sessions?", "then I would be able to give you some tasks as a form of homework", "things like the aforementioned tagging, finding new images, editing and something else, but that's a secret", "ASK:it doesn't have to be longer than fifteen minutes per week, can you spare that?", "[yes, I can spare the time]",
				"GOTO:yesHobby", "[no, I can't spare the time]", "how busy you must be if you can't spend 15 minutes per week", "you can still use the image tagger from the main menu", "so if you ever feel inclined to do so please make use of it", "GOTO:end", "(yesHobby)", "great! next time you open up your app you'll see a homework button", "some tasks work with a timer so always check if you've activated it", "ASK:how much time can you spare per week?",
				"[two hours]", "SETVAR:taskTime,7200", "GOTO:preEnd", "[one and a half hours]", "SETVAR:taskTime,5400", "GOTO:preEnd", "[one hour]", "SETVAR:taskTime,3600", "GOTO:preEnd", "[thirty minutes]",
				"SETVAR:taskTime,1800", "GOTO:preEnd", "[fifteen minutes]", "SETVAR:taskTime,900", "GOTO:preEnd", "(preEnd)", "then I'll send some tasks your way that take that amount of time", "(end)"
			}, 
			"LOB" => new string[203]
			{
				"I want you to reach ever greater heights in your neediness", "now, I'd love nothing more than to make you edge again and again", "but there's only so many hours in the day", "we've both got things to do that are slightly more productive than torturing that @cock", "I know I do at least", "that doesn't mean I'm going to give up on reaching the peak of horniness", "it just means I'll have to get a little creative", "ASK: do you consider yourself to be a good multitasker @subTitle?", "[@yes]", "you'll hardly notice anything has changed then GOTO:cont1",
				"@no", "just view this as a learning opportunity then", "(cont1)", "see, that I'm not present doesn't mean that you can't get horny", "your dick will get just as hard", "it'll drip just as much precum", "and that stack of porn is still firmly sitting on that hard-drive", "so really there's no reason for you not to get hornier and hornier just waiting for my arrival", "I've made a gift just for you", "something to make sure you're getting as pent up as possible",
				"something that guarantees that you don't ever have a clear thought again", "with it I can show you porn whenever", "I've made it so you can just keep doing what you're already doing", "a tool that will drag your mind back into a horny haze", "now, while I'm sure you'd love nothing more than to be teased forever", "sometimes you've got... let's just call them boring people around you", "so I'll give you three ways to protect yourself against prying eyes", "the first is a panic button that can shut the whole program down until a reboot", "the second is a schedule for when the program can activate in the first place", "and last but not least you'll get a button that will suspend the program for five minutes",
				"with those three safety features you're more secure than if you'd be looking at porn normally", "and I'll let you disable it at any point", "ASK: so, do you want to give it a go?", "[@yes]", "GOTO:yes1", "@no", "GOTO:noEnd", "(yes1)", "then there are some things you should know", "if you want the program shut down until a reboot hit the F8 key",
				"remember it @subTitle F8", "let's fill out that schedule as well", "ASK:what's the EARLIEST that you'd like for the program to activate?", "[1 AM]", "SETVAR:earlyLOB,1", "GOTO:endOfEarly", "[2 AM]", "SETVAR:earlyLOB,2", "GOTO:endOfEarly", "[3 AM]",
				"SETVAR:earlyLOB,3", "GOTO:endOfEarly", "[4 AM]", "SETVAR:earlyLOB,4", "GOTO:endOfEarly", "[5 AM]", "SETVAR:earlyLOB,5", "GOTO:endOfEarly", "[6 AM]", "SETVAR:earlyLOB,6",
				"GOTO:endOfEarly", "[7 AM]", "SETVAR:earlyLOB,7", "GOTO:endOfEarly", "[8 AM]", "SETVAR:earlyLOB,8", "GOTO:endOfEarly", "[9 AM]", "SETVAR:earlyLOB,9", "GOTO:endOfEarly",
				"[10 AM]", "SETVAR:earlyLOB,10", "GOTO:endOfEarly", "[11 AM]", "SETVAR:earlyLOB,11", "GOTO:endOfEarly", "[12 PM]", "SETVAR:earlyLOB,12", "GOTO:endOfEarly", "[1 PM]",
				"SETVAR:earlyLOB,13", "GOTO:endOfEarly", "[2 PM]", "SETVAR:earlyLOB,14", "GOTO:endOfEarly", "[3 PM]", "SETVAR:earlyLOB,15", "GOTO:endOfEarly", "[4 PM]", "SETVAR:earlyLOB,16",
				"GOTO:endOfEarly", "[5 PM]", "SETVAR:earlyLOB,17", "GOTO:endOfEarly", "[6 PM]", "SETVAR:earlyLOB,18", "GOTO:endOfEarly", "[7 PM]", "SETVAR:earlyLOB,19", "GOTO:endOfEarly",
				"[8 PM]", "SETVAR:earlyLOB,20", "GOTO:endOfEarly", "[9 PM]", "SETVAR:earlyLOB,21", "GOTO:endOfEarly", "[10 PM]", "SETVAR:earlyLOB,22", "GOTO:endOfEarly", "[11 PM]",
				"SETVAR:earlyLOB,23", "GOTO:endOfEarly", "[12 AM]", "SETVAR:earlyLOB,24", "GOTO:endOfEarly", "(endOfEarly)", "ASK:and what's the LATEST it can still activate?", "[1 AM]", "SETVAR:lateLOB,1", "GOTO:endOfLate",
				"[2 AM]", "SETVAR:lateLOB,2", "GOTO:endOfLate", "[3 AM]", "SETVAR:lateLOB,3", "GOTO:endOfLate", "[4 AM]", "SETVAR:lateLOB,4", "GOTO:endOfLate", "[5 AM]",
				"SETVAR:lateLOB,5", "GOTO:endOfLate", "[6 AM]", "SETVAR:lateLOB,6", "GOTO:endOfLate", "[7 AM]", "SETVAR:lateLOB,7", "GOTO:endOfLate", "[8 AM]", "SETVAR:lateLOB,8",
				"GOTO:endOfLate", "[9 AM]", "SETVAR:lateLOB,9", "GOTO:endOfLate", "[10 AM]", "SETVAR:lateLOB,10", "GOTO:endOfLate", "[11 AM]", "SETVAR:lateLOB,11", "GOTO:endOfLate",
				"[12 PM]", "SETVAR:lateLOB,12", "GOTO:endOfLate", "[1 PM]", "SETVAR:lateLOB,13", "GOTO:endOfLate", "[2 PM]", "SETVAR:lateLOB,14", "GOTO:endOfLate", "[3 PM]",
				"SETVAR:lateLOB,15", "GOTO:endOfLate", "[4 PM]", "SETVAR:lateLOB,16", "GOTO:endOfLate", "[5 PM]", "SETVAR:lateLOB,17", "GOTO:endOfLate", "[6 PM]", "SETVAR:lateLOB,18",
				"GOTO:endOfLate", "[7 PM]", "SETVAR:lateLOB,19", "GOTO:endOfLate", "[8 PM]", "SETVAR:lateLOB,20", "GOTO:endOfLate", "[9 PM]", "SETVAR:lateLOB,21", "GOTO:endOfLate",
				"[10 PM]", "SETVAR:lateLOB,22", "GOTO:endOfLate", "[11 PM]", "SETVAR:lateLOB,23", "GOTO:endOfLate", "[12 AM]", "SETVAR:lateLOB,24", "GOTO:endOfLate", "(endOfLate)",
				"and with that the configuration is complete", "remember that F8 key we talked about?", "sometimes you don't need to shut the whole program down and would much rather suspend it for a bit", "if you need five minutes of peace and quiet just hit F6", "so it's F6 for a 5 minute stop and F8 for a shutdown", "remember to hit the F6 or F8 key if you need it", "I promise I won't punish you too harshly", "you'll have to re-boot your pc to get it running the first time", "FLAG:LOB FLAG:LOBOn LOB: GOTO:end", "(noEnd)",
				"more a fan of the hands-on approach then?", "FLAG:LOBNo GOTO:end", "(end)"
			}, 
			"canRemove" => new string[56]
			{
				"so far I've not touched your porn, apart from some non-permanent censors", "but that's going to change from now on", "and don't worry I won't start deleting your porn outright", "I'll just give you a challenge every so often", "and only if you fail that challenge, I'll delete your porn", "so anything that I end up removing is your own fault", "and it'll always be clear that removing porn is the punishment for failing", "isn't that fair?", "if you mess up you get punished", "I'm going to use our connection to request some permissions",
				"it would make me happy if you'd accept them", "SYSTEM MESSAGE: Extra permissions have been requested by guest43213069.", "(startSysAsk)", "SYSTEM MESSAGE: Permissions requested are: MediaPermissions.", "ASK: SYSTEM MESSAGE: Allow guest43213069 these permissions?", "[Accept]", "GOTO:acceptCont", "[Decline]", "GOTO:declineCont", "[Help]",
				"SYSTEM MESSAGE: MediaPermissions allow the user to read, write, delete or alter files.", "SYSTEM MESSAGE: Do not grant MediaPermissions unless it's a trusted source.", "SYSTEM MESSAGE: MediaPermissions cannot be rescinded once granted. GOTO:startSysAsk", "(acceptCont)", "SYSTEM MESSAGE: Warning, there is a significant risk in granting MediaPermissions.", "ASK: SYSTEM MESSAGE: Continue anyway?", "[yes]", "GOTO:yesCont", "[no]", "GOTO:startSysAsk",
				"(yesCont)", "SYSTEM MESSAGE: Granting permissions.", "SYSTEM MESSAGE: 25% complete", "SYSTEM MESSAGE: 41% complete", "SYSTEM MESSAGE: 80% complete", "SYSTEM MESSAGE: 97% complete", "SYSTEM MESSAGE: 100% complete", "FLAG:canRemove good @boy", "we're going to have so much fun with this", "LOCKIMG: see this image here",
				"ASK:I want you to beg me to let you keep it", "[please don't remove it!]", "remember that I can remove your porn whenever I feel like it", "so stay obedient or I'll take your little collection from you", "UNLOCKIMG: GOTO:end", "[*stay quiet*]", "you know what happens now right?", "DELIMG: still if you're losing porn to a task as simple as this already", "soon you won't have anything to stroke to", "GOTO:end",
				"(declineCont)", "SYSTEM MESSAGE: No permissions will be granted to guest43213069.", "so you don't want to hand over anymore rights to me?", "I'm a little disappointed in you", "FLAG:canRemoveNo though it's not something you can't make up for in other ways", "(end)"
			}, 
			"petPlay" => new string[46]
			{
				"STOPSTROKING:", "you've been acting more and more like a wild animal instead of a @man", "with every edge you're becoming more vocal as the need sets in", "before long we'll have you foaming at the mouth", "all speech forgotten as no more than growls and moans escape your lips", "I've always wanted a pet", "and I think you'll make a fine substitute for the real thing", "and while your cuteness leaves much to be desired compared to a cat or a dog", "I'm sure you'll be more loyal than either of them", "as long as you are properly trained that is",
				"but while you're still acting like a human, I guess I'll ask you", "although we both know what you're going to answer", "ASK: do you want to be my pet?", "[yes @missTitle]", "truly a huge surprise", "who could have seen this coming?", "you were just dying for me to ask you that weren't you?", "even before you met me, you've fantasized about living the life of a pet", "fed and pampered, loved and adored by all", "who wouldn't want that?",
				"however, I'm not like most pet owners", "since you've only just become my adorable little pet you're comparable to a young puppy", "if I want to raise you into a proper bitch, I'll have to put you through training", "and for that you're going to get some things for me", "there are two things you'll need", "a collar and treats", "the treats should be small but powerful in flavor", "some examples that come to mind are nougat or chocolate", "even sweet fruits will suffice if you're health conscious", "the main point behind the small size is the wish for more",
				"so if you're satisfied at the end reduce the size of your treats", "the collar is a different matter entirely", "if you've got a collar of your own use that, if you don't use a thick rope", "look up online how to tie the knot for a collar so you don't accidentally strangle yourself", "we'll only start your training once you've got these two items ready", "so remember, treats and a collar", "FLAG:collarNo", "FLAG:treatsNo", "FLAG:petPlay", "GOTO:end",
				"[no @missTitle]", "huh, that really is unexpected", "I was sure you would be in for some pet play", "and here I thought that you wouldn't surprise me anymore", "FLAG:petPlayNo", "(end)"
			}, 
			"favorite" => new string[22]
			{
				"I've always thought the internet is too kind", "to be clear, I'm not talking about the people on the web, but rather the net itself", "I mean, no matter who you are it'll let you look at your favorite porn", "it doesn't care if you've been a good @boy", "it doesn't care if you've been obedient", "the internet just gives you what you want, no questions asked", "and a reward isn't the same if you haven't had to put in the work", "your favorite porn should be the biggest reward you can get", "not the standard thing you're searching for", "even if you're looking for things to add to your own stash",
				"-------------------TODO------------------", "", "", "", "", "", "", "", "", "",
				"", ""
			}, 
			"petPlayAdvanced" => new string[107]
			{
				"do you want to sink even deeper into this pet persona of yours?", "ASK: just know that you'll lose control either way, not that you had much to lose", "[I may be a human after all]", "then I'll grant you those human rights back you seem to care about so much", "though along with those rights come the stresses you ran away from in the first place", "whenever the world has got you down, I'll be here for you", "petPlayAdvancedNo", "GOTO:end", "[please fully turn me into a pet]", "you've accepted your role as a pet",
				"ISSETTING:treats ISSETTING:collar and you've even brought what I asked of you like the good @boy you are GOTO:introDone", "ISSETTING:treats though I'd love for you to have a collar, you've done well getting those treats GOTO:introDone", "ISSETTING:collar if only you'd brought some treats, well it's your loss GOTO:introDone", "(introDone)", "I've not really acted like an owner so far", "I've let you run around freely, more a wild animal than a pet", "and it's time to leash you", "both physically and mentally I want you bound, so tight that you can't even move a finger without being instructed to do so", "and for those mental bonds I want to know precisely who, or rather what, I'm dealing with", "so far I've been calling you a pup, loyal and cute",
				"ASK: but maybe you've got a cat's pride hiding underneath that thick layer of servitude", "[treat me like a kitten]", "no no no, that's all wrong", "I'm not \"treating\" you like a kitten", "you're a cat as long as I'm looking at you", "FLAG:cat GOTO:petCont", "[treat me like a puppy]", "no no no, that's all wrong", "I'm not \"treating\" you like a pup", "you're a puppy as long as I'm looking at you",
				"FLAG:pup GOTO:petCont", "(petCont)", "next, and I promise this is the last time I'll make you have to think", "I want two names from you", "the first is what you get called as a human", "this doesn't have to be your real name but you should distinctly remember it as something you identify by", "the second name is what I'll call you from now on when you sink into being a perfect little pet", "(repeatName)", "TEXTFIELD:subName so, what's your \"real\" name @subTitle", "it should sound good as well",
				"{GETVAR:subName}", "ASK: is GETVAR:subName correct?", "[@yes]", "GOTO:contName", "@no", "GOTO:repeatName", "(contName)", "GETVAR:subName huh", "a name is such a powerful thing", "just hearing your name spoken will draw your attention",
				"making it so very hard to look away", "{obey, GETVAR:subName}", "isn't that right GETVAR:subName", "every pet has a name", "something bubbly and cute", "a name to mark just how docile and obedient that @pup is", "you may have your own in mind", "or you could let me choose the perfect name for you", "but if you let me choose you can't refuse what I pick for you", "good pets obey their owners after all",
				"and I'm sure you're the best @pup in the world", "ASK: so, do you want to pick a name yourself or will you let your owner pick?", "[please pick for me @missTitle]", "now this is quite the responsibility", "it should be sweet but not too sweet", "and just a hint of strength in there", "ISSETTING:pup yes, yes I think I've got it SETVAR:petName,Luna", "ISSETTING:cat yes, yes I think I've got it SETVAR:petName,Lucy", "your name as my obedient little @pup will be GETVAR:petName", "{GETVAR:petName}",
				"GOTO:contPetName", "[please let me pick for myself]", "oh, so you had something in mind already?", "(repeatPetName)", "TEXTFIELD:petName then tell me my cute @pup", "it should sound good as well", "{GETVAR:petName}", "ASK: is GETVAR:petName correct?", "[@yes]", "GOTO:contPetName",
				"@no", "GOTO:repeatPetName", "(contPetName)", "this name is just the cutest", "you're such a good @boy GETVAR:petName SNAP:", "you'll come to love this name even more than your own", "because when I say GETVAR:petName SNAP:", "you get to feel good", "so GETVAR:petName, I want you to leave that old name behind for now SNAP:", "don't worry, you'll be able to pick it up again later",
				"but while you're leaving things behind anyway, how about you leave behind some more", "FLAGT:petPlay since a @pup doesn't have any obligations all that stress can keep sticking to your old name", "ISSETTING:pup and nobody is going to expect much more than a bark out of you so your speech can be discarded as well", "ISSETTING:cat and nobody is going to expect much more than a meow out of you so your speech can be discarded as well", "ASK: can you do that for me?", "[@yes]", "GETVAR:petName is such a good @boy SNAP:", "you don't mind me keeping you like this for a while right?", "for now you just get to feel good", "no hard questions",
				"no stressful choices", "just pleasure and obedience", "GETVAR:petName is such a good @boy SNAP:", "and you've earned to feel this way", "so just let go of those restraints", "FLAG:petPlayAdvanced and sink deep into pleasure", "(end)"
			}, 
			"censorship" => new string[23]
			{
				"deep down you love to be denied", "I know that right now you may hate how you're feeling", "but if you truly wanted to you could leave and not come back", "you love this ache, you love getting told no", "I must say that I've got a bit of a soft spot for your desperation", "so I'll help you out by giving you what you really want", "more denial", "I understand that you may not want this right now", "even more denial, even more ache", "but it's a different denial than you're used to",
				"you see, I'm not going to deny your dick", "I'm going to deny your eyes", "you've been blessed to live in a time where porn is everywhere", "that doesn't mean you'll get to see it though", "ASK:do you deserve to see all these sexy images?", "[yes @missTitle]", "then you'd best keep me happy or I'll take them from you", "FLAG:censorshipNo", "[no @missTitle]", "you're such a good @boy for me",
				"and don't worry, as long as you keep my happy you'll barely notice the difference", "but one foot out of line and you'll miss what you've given up", "FLAG:censorship"
			}, 
			"prostateOrgasm" => new string[]
			{
				"STOPSTROKING:", "I've been eyeing that ass of yours @subTitle", "there's a deeper kind of pleasure I want to pull out of you", "an orgasm milked straight from your prostate, with no hands at all", "ASK: did you ever have a prostate orgasm?", "[yes @missTitle]", "then you already know how good that deep release can feel", "I'll be chasing it out of you when I see fit", "FLAG:prostateOrgasm", "GOTO:end",
				"[not yet, but I want to]", "good @boy, I'll train that hole until it can cum all on its own", "FLAG:prostateOrgasm", "GOTO:end", "[no, and I'd rather not]", "then I won't push a hands-free orgasm on you", "FLAG:prostateOrgasmNo", "(end)", ""
			},
			_ => new string[0], 
		};
	}
}
