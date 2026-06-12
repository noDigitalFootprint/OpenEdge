using OpenEdge.helper;

namespace OpenEdge.vocab;

internal class CbtNippleTaunt : CbtTauntBase
{
	public CbtNippleTaunt(Voc voc)
		: base(voc)
	{
		moodHigh.AddRange(new string[2] { "has anyone ever told you that you've got the most delicious little nipples?", "you can be proud of your cute nipples" });
		normal.AddRange(new string[7] { "will your nipples grow if we keep playing with them like this?", "pinch harder slut", "you said that you wanted this so pinch harder", "hold it just like that", "does it hurt yet?", "soon your nipples will noticeably stick out even when you're not playing with them", "from now on you should watch out for pokies in your shirt" });
		moodLow.AddRange(new string[3] { "I want to hear those nipps scream", "you're just a bitch in heat aren't you?", "moan while your tits get abused" });
	}
}
