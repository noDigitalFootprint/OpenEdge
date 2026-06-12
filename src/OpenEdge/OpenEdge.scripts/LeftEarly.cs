namespace OpenEdge.scripts;

internal class LeftEarly : TalkBaseClass
{
	public LeftEarly(MainWindow mw)
		: base(mw)
	{
		mw.changeMoodBy(-20);
		createSessionIntro();
		setFlag("FLAGT:sessionIntro", temp: true);
		allText = mw.lr.getScript("leftEarly");
	}

	private string[] createSessionIntro()
	{
		incrementFlag("leftEarly");
		return getMyScript() switch
		{
			0 => new string[4] { "what the fuck are you doing?", "I'm not planning out full sessions just for you to quit in the middle", "don't you dare do that again", "you will sit there and take the punishment that I dish out" }, 
			1 => new string[4] { "did you think you could just leave early?", "I'm not your on call dominatrix", "I own you", "and it seems you don't fully get that" }, 
			2 => new string[4] { "you can't just leave like that @subTitle", "it's really fucking annoying when I clear out my schedule for a session and you bail on me", "it makes these sessions less enjoyable for me", "and it would be in your best interest to keep me happy" }, 
			3 => new string[7] { "so, I'm sure you've thought up some kind of an excuse as to why you were allowed to leave early", "but to be honest I don't care", "your task is simple to understand isn't it?", "you are to sit and suffer, that's it", "that's all you have to do", "and somehow you've convinced yourself that this is difficult", "well let me hear your pathetic excuse then" }, 
			_ => createSessionIntro(), 
		};
	}

	private string[] createSessionEnd()
	{
		return new string[23]
		{
			"ASK: why did you leave the session early?", "[I was bored]", "you're going to wish you hadn't said that @subTitle", "ISSETTING:string BINDBALLS", "CBTEXTREME:", "GOTO:end", "[our connection stalled]", "and you didn't think to call me back?", "that's the exact same thing as leaving early", "next time I expect you to get back in contact with me within ten minutes",
			"GOTO:end", "[I thought we were done]", "you don't get to think", "you just get to suffer", "I'll break off the connection when I'm done with you", "all you have to do is wait a little bit, I'm sure even you can do that much", "GOTO:end", "(end)", "ORGASMDENIED:", "PUNISHMENT:",
			"I'm adding the time you skipped out on to the session", "and you won't be cumming today @subTitle", "ISSETTING:humiliation maybe this'll teach you that my time is more precious than yours"
		};
	}
}
