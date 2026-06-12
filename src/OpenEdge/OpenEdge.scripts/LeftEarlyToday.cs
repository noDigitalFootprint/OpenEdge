namespace OpenEdge.scripts;

internal class LeftEarlyToday : TalkBaseClass
{
	public LeftEarlyToday(MainWindow mw)
		: base(mw)
	{
		mw.changeMoodBy(-10);
		allText = mw.lr.getScript("leftEarlyToday");
		incrementFlag("leftEarly");
		setFlag("leftEarlyToday", temp: true);
		setFlag("FLAGT:sessionIntro", temp: true);
	}

	private string[] createSessionIntro()
	{
		return getMyScript() switch
		{
			0 => new string[5] { "oh? and here I thought that you wouldn't be coming back today", "I expect you to get back in contact within ten minutes if our connection breaks off", "so while I'm glad to see you back here already, don't expect a standing ovation", "you won't be cumming today @subTitle, I hope you realize just how kind I'm being that this is your only punishment", "ORGASMDENIED:" }, 
			1 => new string[8] { "first you don't finish your session and then you come crawling back in no time at all", "I'll wait around for ten minutes if we get disconnected just in case it was an accident", "after that you're getting punished for leaving early", "to be honest I'm no longer in the mood", "oh don't look like that, I'll still let you stroke", "but there's no way I'm going to let you cum if I'm not really into it", "and forget about backing out now, you wouldn't bail on me again would you?", "ORGASMDENIED:" }, 
			2 => new string[5] { "so do you want to stroke your @cock or not?", "I genuinely don't know anymore", "don't expect to cum today @subTitle, you've ruined that possibility with your premature leaving", "next time get back to me within ten minutes if we get disconnected", "ORGASMDENIED:" }, 
			_ => createSessionIntro(), 
		};
	}
}
