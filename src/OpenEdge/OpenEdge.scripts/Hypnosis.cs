using System;
using System.Linq;

namespace OpenEdge.scripts;

internal class Hypnosis : TalkBaseClass
{
	public Hypnosis(MainWindow mw)
		: base(mw)
	{
		allText = createInduction().Concat(createBody().Concat(createAwakener())).ToArray();
		allText = mw.lr.getScript("hypnosis");
		setFlag("hypnosis", temp: true);
	}

	private string[] createInduction()
	{
		return new Random().Next(100) switch
		{
			0 => new string[23]
			{
				"HYPNOSISPLUS: HYPNOSISON: STROKEOFF:", "{empty}", "empty your head", "{obey}", "slow your breathing", "EMPTY", "breathe in", "EMPTY", "hold it", "EMPTY",
				"breathe out", "EMPTY", "hold it", "EMPTY", "breathe in", "EMPTY", "hold it", "EMPTY", "breathe out", "EMPTY",
				"hold it", "EMPTY", "keep going"
			}, 
			1 => new string[17]
			{
				"HYPNOSISPLUS: HYPNOSISON: STROKEOFF:", "lets wipe those thoughts away for a minute", "keep your breathing slow", "breathing in desire", "EMPTY", "it travels down your throat", "refreshes and rejuvenates you", "breathing out resistance", "EMPTY", "hot air moving upwards as you blow it out",
				"and before you know it", "in comes more desire", "EMPTY", "breath after breath", "pooling deep inside of you", "it feels good", "the cool air is making you so relaxed"
			}, 
			2 => new string[23]
			{
				"HYPNOSISPLUS: HYPNOSISON: STROKEOFF:", "feel yourself relax", "as you slow your breathing", "stretch for me", "put tension in your arms and legs", "and release it", "match your breath to it", "stretch and suck in air", "hold that pose for a little while", "and",
				"release, breathing out as you do so", "feel your fatigue dissipating", "stretch once more", "hold", "release", "do you feel that heat", "heat in your arms", "heat in your legs", "maybe even in your fingers", "or the soles of your feet",
				"it feels nice", "it's so relaxing", "keep your breathing slow"
			}, 
			3 => new string[20]
			{
				"HYPNOSISPLUS: HYPNOSISON: STROKEOFF:", "it's time to wind down", "both your body", "and your mind", "just relax", "just breathe", "just feel the air flow", "in then out", "out then in", "forever repeating",
				"a mantra in your mind", "in then out", "out then in", "again and again", "in then out", "out then in", "you can't stop breathing", "so you can't stop sinking", "in then out", "out then in"
			}, 
			4 => new string[18]
			{
				"HYPNOSISPLUS: HYPNOSISON: STROKEOFF:", "I want you to imagine something for me", "there is a big balloon", "a balloon filled with helium", "it's constantly pulling upwards", "trying to float to the clouds", "but you're holding on to the string", "preventing it from ever leaving", "but it's such a big balloon", "it's pulling so hard",
				"and it takes so much effort", "your arm growing numb from the exertion", "the numbness travels to your shoulder", "and you can't hold on anymore", "just let the balloon float away", "up up up, into the clouds", "out of sight", "out of mind"
			}, 
			5 => new string[25]
			{
				"you can be sure I won't ever do something to harm you", "everything I do, is what you've asked me to do", "and because of this you can just", "HYPNOSISPLUS: HYPNOSISON: STROKEOFF:", "let go", "of your worries", "of your stresses", "even of your desires", "you can just relax", "and focus on your breathing",
				"slow your breath", "down to a crawl", "hold it for a while", "and release slowly", "you're safe here", "so you can just", "let go", "just rest", "you've earned it", "your breathing slowing",
				"becoming deep and languid", "as if the breaths themselves", "were covered in syrup", "a sweet treat", "dragging you down"
			}, 
			6 => new string[23]
			{
				"HYPNOSISPLUS: HYPNOSISON: STROKEOFF:", "take deep and languid breaths", "you want to sink for me don't you?", "you long to put that brain away for a little bit", "with your body sitting here mindless", "it just wants to feel good", "I want you to visualize a cage for me", "a cube of steel made up of gleaming bars", "it's sitting at the bottom of the spiral", "an open door in front of you",
				"just one step will take you inside", "I'm standing behind you", "a key sits in my hands", "I motion towards the door", "and your mind steps inside", "the door closes behind you", "followed by the click of a lock", "bars surround you", "your mind locked away", "in a cage of my making",
				"I've got the key", "trust me with it", "I'll give it back in due time"
			}, 
			7 => new string[23]
			{
				"HYPNOSISPLUS: HYPNOSISON: STROKEOFF:", "your brain is smart", "too smart for it's own good", "all those thoughts make it so hard", "and we want your mind to be soft", "soft and malleable", "so let me help you", "let me melt that mind a bit", "until it comes oozing out of your ears", "just like a putty or a slime",
				"but things tend to get stuck in them", "putty will get full of dirty things", "if your mind is like putty", "things will get stuck", "things will get mingled", "so mixed together", "that it becomes one mass", "my words and training", "the same as your own thoughts", "forever a part of you",
				"it's fine though", "you like being a bit dirty", "tainted by my color"
			}, 
			_ => createInduction(), 
		};
	}

	private string[] createAwakener()
	{
		if (new Random().Next(100) == 0)
		{
			return new string[19]
			{
				"it's time to come back", "I'm going to count you up", "and once I reach five you'll be awake and alert again", "back to the real world", "HYPNOSISMIN:", "one", "your field of view widening", "HYPNOSISMIN:", "two", "your surroundings coming back to you",
				"HYPNOSISMIN:", "three", "blink your eyes", "HYPNOSISMIN:", "four", "stretching out your arms and legs", "HYPNOSISMIN:", "five", "HYPNOSISOFF: STROKEOFF: SNAPDOUBLE: wide awake now"
			};
		}
		return createAwakener();
	}

	private string[] createBody()
	{
		switch (getMyScript())
		{
		case 0:
			return new string[107]
			{
				"stare at the", "pretty spiral", "{stare}", "round", "{stare}", " and ", "{stare}", "round", " and ", "round",
				"spinning", "winding", "eyes glazed over", "as you", "{drop}", "SNAP: DROP HYPNOSISPLUS: SUBLIMINALAUDIOON: STROKEON:", "deep into trance", "{down}", "down down down", "{deeper}",
				"deeper deeper deeper", "resistance dropping", "dropping like your mind", "{lower}", "lower lower lower", "{pleasure}", "SNAP: pleasure HYPNOSISPLUS:", "drop down into trance", "{can't resist}", "you could resist if you wanted to",
				"but you're not going to do that", "you'll sit there", "{glazed eyes}", "with glazed over eyes", "{obey}", "mouth open", "{drool}", "thinking is so hard", "{drool}", "you don't want to think",
				"you just want to feel", "{pleasure}", "SNAP: pleasure HYPNOSISPLUS:", "EMPTY", "{pleasure}", "SNAP: obey HYPNOSISPLUS:", "EMPTY", "{pleasure}", "SNAP: pleasure HYPNOSISPLUS:", "EMPTY",
				"{pleasure}", "SNAP: submit HYPNOSISPLUS:", "EMPTY", "{pleasure}", "SNAP: pleasure HYPNOSISPLUS:", "EMPTY", "{pleasure}", "SNAP: follow HYPNOSISPLUS:", "EMPTY", "you can feel so",
				"{pleasure}", "SNAP: good HYPNOSISPLUS:", "EMPTY", "give in to", "{pleasure}", "SNAP: pleasure HYPNOSISPLUS:", "EMPTY", "{edge}", "SNAP: edge for me HYPNOSISPLUS:", "EMPTY",
				"{pleasure}", "SNAP: pleasure", "EMPTY", "so deep", "{pleasure}", "SNAP: pleasure", "EMPTY", "can't think", "{pleasure}", "SNAP: pleasure",
				"EMPTY", "can't look away", "EMPTY", "{pleasure}", "SNAP: pleasure", "EMPTY", "stare at the spiral", "EMPTY", "{pleasure}", "SNAP: pleasure",
				"{pleasure}", "SNAP: pleasure", "EMPTY", "feels good", "{pleasure}", "SNAP: pleasure", "EMPTY", "SNAP: happy", "{pleasure}", "SNAP: pleasure",
				"SNAP: fulfilled", "{pleasure}", "SNAP: pleasure", "you look so cute", "{pleasure}", "SNAP: pleasure", "a blank face suits you"
			};
		case 1:
			if (mw.getTFlag("petPlay"))
			{
				return new string[130]
				{
					"feel yourself sink", "sink deep into a warm bed", "it's comfortable", "it feels like home", "it feels safe", "you feel so sleepy", "so drowsy", "so droopy", "lay down on that warm bed", "you'll do as your @missTitle commands",
					"and I'm telling you to", "{drop}", "SNAP: DROP HYPNOSISPLUS: SUBLIMINALAUDIOON: STROKEON:", "don't worry, I'm watching over you", "I'm next to you", "{I want to be close}", "you want to be close", "{I want to snuggle}", "you want to snuggle", "{I want to be pet}",
					"you want to be pet", "you are so cute", "you should be a", "{pleasure}", "SNAP: good pet HYPNOSISPLUS:", "EMPTY", "you want to be a", "{pleasure}", "SNAP: good pet", "EMPTY",
					"you need to be a", "{pleasure}", "SNAP: good pet  HYPNOSISPLUS:", "EMPTY", "you are going to be a", "{pleasure}", "SNAP: good pet", "EMPTY", "{collared}", "you want to be collared",
					"{pleasure}", "SNAP: good pet  HYPNOSISPLUS:", "EMPTY", "{trained}", "you want to be trained", "{pleasure}", "SNAP: good pet", "EMPTY", "{obey}", "nothing makes you happier than obeying",
					"{pleasure}", "SNAP: good pet  HYPNOSISPLUS:", "EMPTY", "{cute}", "looking cute for your owner", "{pleasure}", "SNAP: good pet", "EMPTY", "{obey}", "acting obedient like a",
					"{pleasure}", "SNAP: good pet HYPNOSISPLUS:", "EMPTY", "{collar}", "you love your collar", "{pleasure}", "SNAP: good pet", "EMPTY", "{edge}", "rub your @cock and edge",
					"{pleasure}", "SNAP: HYPNOSISPLUS: good pets don't cum", "EMPTY", "{pleasure}", "SNAP: good pets edge", "EMPTY", "{pleasure}", "SNAP: are you a good pet?", "EMPTY", "{pleasure}",
					"SNAP: HYPNOSISPLUS: do you want to be a good pet?", "EMPTY", "{pleasure}", "SNAP: do you have to be a good pet?", "EMPTY", "ISSETTING:cat GOTO:catCont", "bark for me", "{pleasure}", "SNAP: HYPNOSISPLUS: be a good pup and bark", "EMPTY",
					"{pleasure}", "SNAP: bark", "{pleasure}", "SNAP: bark", "{pleasure}", "SNAP: bark", "{pleasure}", "SNAP: bark", "{pleasure}", "SNAP: bark",
					"{pleasure}", "SNAP: bark", "GOTO:petEnd", "(catCont)", "meow for me", "{pleasure}", "SNAP: HYPNOSISPLUS: be a good kitten and meow", "EMPTY", "{pleasure}", "SNAP: meow",
					"{pleasure}", "SNAP: meow", "{pleasure}", "SNAP: meow", "{pleasure}", "SNAP: meow", "{pleasure}", "SNAP: meow", "{pleasure}", "SNAP: meow",
					"(petEnd)", "you've made me so proud", "you're such a ", "{pleasure}", "SNAP: good pet", "you love this", "ISSETTING:cat you love to meow", "ISFLAG:dog you love to bark", "you love being a pet", "and you love your owner"
				};
			}
			break;
		case 2:
			return new string[73]
			{
				"you wanted this", "you asked for this", "so let me take you", "{down}", "SNAP: DOWN HYPNOSISPLUS: SUBLIMINALAUDIOON: STROKEON:", "", "{deeper}", "SNAP: down deeper down HYPNOSISPLUS:", "", "{more}",
				"SNAP: down another level HYPNOSISPLUS:", "", "{down}", "SNAP: you can go down HYPNOSISPLUS:", "", "{down}", "SNAP: you're going down HYPNOSISPLUS:", "", "{more}", "SNAP: down another level HYPNOSISPLUS:",
				"", "{deeper}", "SNAP: down deeper down HYPNOSISPLUS:", "", "{deeper}", "SNAP: so deep now HYPNOSISPLUS:", "", "{down}", "SNAP: so down now HYPNOSISPLUS:", "",
				"{down}", "SNAP: you will go down further", "", "{falling}", "SNAP: always falling", "", "{faster}", "SNAP: faster and faster", "", "{down}",
				"SNAP: plummeting down", "", "{down}", "SNAP: down, down, down", "", "{open}", "SNAP: you're open", "{change}", "SNAP: open to change", "{easy}",
				"SNAP: you're easy", "{bend}", "SNAP: easy to bend", "{simple}", "SNAP: you're simple", "{mold}", "SNAP: simple to mold", "", "you're comfortable", "comfortable with the idea of",
				"{change}", "SNAP: change", "EMPTY", "comfortable with the idea of", "handing over control", "to give the power to alter", "to adjust and steer", "you can't handle the responsibility", "the stress of consequence", "so my little hypnoslut",
				"hand it over to me", "let me change you", "trust me with your senses"
			};
		case 3:
			return new string[54]
			{
				"you're such a good @boy for me", "{I want to obey}", "and good @boys want to obey", "{I want to drop}", "good @boys want to drop", "{yes @missTitle}", "are you a good @boy?", "{drop}", "SNAP: then DROP HYPNOSISPLUS: SUBLIMINALAUDIOON: STROKEON:", "{falling}",
				"feel yourself falling", "falling forward", "falling under my control", "faster and faster you fall", "sucked into my influence", "{drop}", "SNAP: as you DROP HYPNOSISPLUS:", "{pleasure}", "SNAP: good @boy", "you like that",
				"you like getting praised", "it's only fair", "{pleasure}", "SNAP: you're so good at holding back HYPNOSISPLUS:", "{pleasure}", "SNAP: you're so good at obeying HYPNOSISPLUS:", "because you are a", "{pleasure}", "SNAP: good @boy HYPNOSISPLUS:", "{pleasure}",
				"SNAP: good @boy", "{pleasure}", "SNAP: good @boy HYPNOSISPLUS:", "you want to cum", "but you need permission", "precum is oozing out", "making your cock", "sticky and needy", "desperate for release", "but you can't",
				"{pleasure}", "SNAP: you're a good @boy HYPNOSISPLUS:", "{pleasure}", "SNAP: edge, my good @boy HYPNOSISPLUS:", "stay there", "{pleasure}", "SNAP: you're such a good @boy", "{pleasure}", "SNAP: you're so precious to me", "{pleasure}",
				"SNAP: you're everything I wish for", "{pleasure}", "SNAP: good @boy", "SNAP: let the edge go my good @boy STROKEOFF:"
			};
		case 4:
			if (int.Parse(mw.getVar("illegalCum")) > 0)
			{
				return new string[106]
				{
					"you've been bad before", "having an orgasm without permission", "and that won't do", "so I'm just going to erase the want", "the want for an orgasm", "the need for release", "all of that will be gone", "along with any other thoughts", "{stare}", "so just stare deep",
					"{deep}", "deep into the spiral", "you don't have to catch every word", "some will simply move past you", "too quick for your mind to follow", "too fleeting for your brain to comprehend", "but the words will still reach you", "your subconscious mind will catch them", "whether you like it or not", "{stare}",
					"so you can just stare", "you don't have to focus your eyes", "you just have to sit there", "SNAP: and absorb your programming HYPNOSISPLUS: SUBLIMINALAUDIOON: STROKEON:", "{I've been bad}", "I've been bad", "{I've disappointed my @missTitle}", "I've disappointed my @missTitle", "{I want to be good}", "I want to be good",
					"{I want to be praised}", "I want to be praised", "{if I obey I get praised}", "if I obey I get praised", "{I should obey}", "I should obey", "{it feels good to get praised}", "it feels good to get praised", "{it feels good to obey}", "it feels good to obey",
					"{my @missTitle knows what's best}", "my @missTitle knows what's best", "{so I can just follow}", "so I can just follow", "{it's so easy to follow}", "it's so easy to follow", "{all I do is comply}", "all I do is comply", "{all I want is to submit}", "all I want is to submit",
					"{but my brain is weak}", "but my brain is weak", "{I can't control myself}", "I can't control myself", "{I need my @missTitle}", "I need my @missTitle", "{I need her control}", "I need her control", "{I need her dominance}", "I need her dominance",
					"{and she is telling me to}", "and she is telling me to", "{DROP}", "SNAP: DROP HYPNOSISPLUS:", "{I want more of this}", "SNAP: I want more of this", "{I love this}", "SNAP: I love this", "{I love getting trained}", "SNAP: I love getting trained",
					"{I love getting programmed}", "SNAP: I love getting programmed", "{I want more pleasure}", "I want more pleasure", "{I want more training}", "I want more training", "{never enough}", "never enough", "{always more}", "always more",
					"{more pleasure}", "more pleasure", "{more obedience}", "more obedience", "{I want to obey}", "I want to obey", "{so I can get praised}", "so I can get praised", "{so I can feel good}", "so I can feel good",
					"{I must resist orgasm}", "I must resist orgasm", "{I hate cumming without permission}", "I hate cumming without permission", "{I'm scared of spurting too soon}", "I'm scared of spurting too soon", "{it's hard to cum without permission}", "it's hard to cum without permission", "{I want permission}", "I want permission",
					"{I need permission}", "I need permission", "{my @missTitle can give me permission}", "my @missTitle can give me permission", "{so I should obey}", "so I should obey"
				};
			}
			break;
		case 5:
			return new string[63]
			{
				"servitude is simple", "SNAP: all you have to do is drop your defenses HYPNOSISPLUS: SUBLIMINALAUDIOON: STROKEON:", "lower your guard just a little", "and I'll slip through the cracks", "time after time you'll let me in", "just a little at first", "your curiosity urging you onward", "this idea of me will grow inside of you", "the idea that I can grant you pleasure", "the idea that I can make you feel good",
				"people can't fight pleasure", "not when it feels so good to just", "{give in}", "SNAP: give in HYPNOSISPLUS:", "{give in}", "SNAP: give in and let me make you feel good HYPNOSISPLUS:", "{give in}", "SNAP: give in and let me help you HYPNOSISPLUS:", "you'll get used to the feeling", "the feeling of handing over control",
				"and those sturdy defenses of yours", "will have cracks running over the walls", "{drop}", "SNAP: every time you drop HYPNOSISPLUS:", "{submit}", "SNAP: every time you submit HYPNOSISPLUS:", "{pleasure}", "SNAP: every time you feel pleasure HYPNOSISPLUS:", "I worm my way further inside", "widening those cracks in your defenses",
				"until those walls", "crumble", "{down}", "SNAP: down", "but that's what you want right?", "you want to feel good", "{pleasure}", "SNAP: you want to feel pleasure", "and all you have to do to get it", "is sit",
				"and stare", "drinking in my words", "my control", "so I can grant you", "{pleasure}", "SNAP: pleasure", "{pleasure}", "SNAP: all I want to do is grant you pleasure", "and you want to feel good", "we're forming pathways in your mind",
				"simple but powerful routines", "{obey}", "SNAP: that while you obey", "you'll feel good", "{pleasure}", "SNAP: you'll feel pleasure", "and all you have to do is", "{obey}", "obey", "{obey}",
				"SNAP: obey and be rewarded", "revel in servitude", "and feel bliss"
			};
		case 6:
			if (mw.getTFlag("petPlay"))
			{
				return new string[70]
				{
					"you love getting praise", "you love getting called a", "{good @boy}", "good @boy", "{good @boy GETVAR:petName}", "good @boy GETVAR:petName SNAP:", "do you want praise?", "then you'd better earn it", "focus on the spiral", "and let my words slip by",
					"be a good @puppy", "SNAP: and drop HYPNOSISPLUS: SUBLIMINALAUDIOON: STROKEON:", "{good @boy GETVAR:petName}", "good @boy GETVAR:petName SNAP:", "you're doing so well", "just like that", "sink @pup", "deeper and deeper", "tumbling down", "{good @boy GETVAR:petName}",
					"good @boy GETVAR:petName SNAP:", "let go of your human name", "you don't have to remember it", "I'll remember it for you", "{pleasure}", "for now you're GETVAR:petName SNAP:", "my good @boy", "my good @pup", "you can just forget", "you can just obey",
					"{pleasure}", "GETVAR:petName SNAP:", "focus", "obey", "a @pup can't read", "so just stare", "eyes feeling tired", "glazing over", "my words enter your head", "slipping through the gaps",
					"{pleasure}", "you're my good @boy", "I want the best for you", "trust me", "{drop GETVAR:petName }", "and drop GETVAR:petName HYPNOSISPLUS: SNAP:", "you go deep", "deeper than normal", "deeper than usual", "tumbling",
					"eyes flickering", "you're deeper now HYPNOSISPLUS:", "deeper than normal", "deeper than usual", "falling endlessly", "{good @boy GETVAR:petName}", "good @boy GETVAR:petName SNAP: HYPNOSISPLUS:", "{pleasure}", "GETVAR:petName feels good SNAP: HYPNOSISPLUS:", "{pleasure}",
					"GETVAR:petName feels safe SNAP: HYPNOSISPLUS:", "{pleasure}", "GETVAR:petName feels warm SNAP: HYPNOSISPLUS:", "it's fine to forget", "I will keep track", "you've forgotten something", "I'll remember it for you", "so just let it go", "{pleasure}", "SNAP: good @boy GETVAR:petName"
				};
			}
			break;
		}
		return createBody();
	}
}
