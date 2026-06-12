using OpenEdge.helper;

namespace OpenEdge.vocab;

internal class CbtTauntBase : BaseVocab
{
	public CbtTauntBase(Voc voc)
		: base(voc)
	{
		petPlay.AddRange(new string[1] { "you should never physically punish your pet, I'll make an exception for you though" });
		moodHigh.AddRange(new string[4] { "you're such a good @subTitle for suffering through this", "I love watching you hurt yourself", "it makes me wet to see how much power I hold over you", "it's sad, you were doing so well for me" });
		normal.AddRange(new string[11]
		{
			"don't start using less force just because it hurts", "do you think I'm making you do this for show? make it hurt", "you'd better redouble your efforts after this", "keep going @subTitle", "harder now @subTitle", "put some strength into it or I'm going to deny you", "bad @boys get punished", "reflect on why you're being punished", "I enjoy making you squeal, but that doesn't mean I don't have a solid reason for doing this", "some people need a hand to get the message",
			"I'll break you down to build you back up"
		});
		moodLow.AddRange(new string[9] { "you're such a little shit", "I truly couldn't care less how you feel right now", "you deserve this treatment", "you were made to be used and discarded", "you're nothing to me @subTitle", "you've forced my hand with your recent behavior", "I used to think we could have fun together, but you're in need of re-education", "how hard can it can be to obey?", "I'll beat this lesson into you as many times as necessary" });
		sph.AddRange(new string[3] { "you've got more problems than just your small dick", "a small dicked loser who can't even obey is utterly worthless", "you're going to kill me with boredom unless you actually start applying some force" });
	}
}
