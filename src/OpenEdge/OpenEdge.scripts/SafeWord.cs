namespace OpenEdge.scripts;

internal class SafeWord : TalkBaseClass
{
	public SafeWord(MainWindow mw)
		: base(mw)
	{
		allText = mw.lr.getScript("safeWord");
	}

	private string[] safeWord()
	{
		return new string[19]
		{
			"STOPSTROKING:", "HYPNOSISOFF:", "ASK:I'm so sorry to hear that, do you need a second?", "[I want to end the session]", "then we'll do just that, always remember that anything I say is roleplay here", "I've got no interest in causing you long lasting negative feelings", "so drink something and I hope I'll see you again soon", "ENDSESSION", "[I'm fine]", "I'm glad to hear it",
			"GOTO:preEnd", "[I want a break]", "you don't have to force yourself", "get yourself something to drink and stretch yourself out a bit", "ASK:just give me a call when you're ready", "[I'm ready]", "(preEnd)", "let's get back to it then", "(end)"
		};
	}
}
