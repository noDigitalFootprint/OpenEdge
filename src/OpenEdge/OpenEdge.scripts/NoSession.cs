using System;

namespace OpenEdge.scripts;

internal class NoSession : TalkBaseClass
{
	public NoSession(MainWindow mw)
		: base(mw)
	{
		allText = mw.lr.getScript("noSession");
	}

	private string[] createSessionIntro()
	{
		return new Random().Next(100) switch
		{
			0 => new string[5] { "you've got to give a little time to myself every so often", "I get that your @cock is begging for attention but just deal with it", "I'll be ready for you tomorrow so see you then", "FLAG:cockControl and remember, no stroking", "ENDSESSION" }, 
			1 => new string[5] { "unlike you I've got a life", "and I'm not planning on spending the majority of it telling people how to stroke", "I'll be ready for you tomorrow so see you then", "FLAG:cockControl and remember, no stroking", "ENDSESSION" }, 
			2 => new string[5] { "I really did a number on you if you're back already", "sadly it takes time to prepare our little sessions so you'll have to wait", "I'll be ready for you tomorrow so see you then", "FLAG:cockControl and remember, no stroking", "ENDSESSION" }, 
			3 => new string[4] { "I'm not in the mood for another session just yet", "check back in with me tomorrow", "FLAG:cockControl and remember, no stroking", "ENDSESSION" }, 
			_ => createSessionIntro(), 
		};
	}
}
