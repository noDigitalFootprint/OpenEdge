namespace OpenEdge.scripts;

internal class Disobedience : TalkBaseClass
{
	public Disobedience(MainWindow mw)
		: base(mw)
	{
		allText = mw.lr.getScript("disobedience");
	}

	private string[] setPunishment()
	{
		switch (getMyScript())
		{
		case 0:
			return new string[5] { "oh, so you don't want to?", "that's really too bad", "I guess I'll just have to break you some other way then", "just remember, you could have chosen the easy way out", "PUNISHMENT:" };
		case 1:
			if (!mw.isSettingEnabled("petPlay"))
			{
				return new string[3] { "you're such a fucking brat", "I'm going to enjoy breaking you", "PUNISHMENT:" };
			}
			break;
		case 2:
			return new string[6] { "brats will be brats I guess", "you and your combative nature have to make everything difficult", "but don't you worry", "I'll make you conform to my wishes", "and then we won't have this tension between us anymore", "PUNISHMENT:" };
		case 3:
			if (mw.isSettingEnabled("petPlay"))
			{
				return new string[6] { "you can kiss those treats goodbye for now", "this is hardly the behavior I'd expect from such a good @boy", "you want to be the best @pup you can be right?", "don't worry, just follow along and you won't even notice how far you've fallen", "first of all, I have to teach you who's in control here", "PUNISHMENT:" };
			}
			break;
		case 4:
			if (mw.isSettingEnabled("humiliation"))
			{
				return new string[6] { "is this the most you can resist?", "it's pretty pathetic that this is all your disobedience amounts to", "just stop resisting your conditioning", "and we can have some fun together", "for now though, I'll just re-educate you on your position", "PUNISHMENT:" };
			}
			break;
		case 5:
			if (mw.getTFlag("censor"))
			{
				return new string[5] { "are the censors making you frustrated?", "don't worry, with time you'll come to love them", "whether you want to or not", "but even if you're having a hard time I'm not going to ignore your disobedience", "PUNISHMENT:" };
			}
			break;
		}
		return setPunishment();
	}
}
