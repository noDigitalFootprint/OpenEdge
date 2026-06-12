namespace OpenEdge.scripts;

internal class PetPlayOff : TalkBaseClass
{
	public PetPlayOff(MainWindow mw)
		: base(mw)
	{
		deleteFlag("petPlay", temp: true);
		_ = new string[2] { "[@yes]", "FOURSNO" };
		allText = mw.lr.getScript("petPlayOff");
	}

	private string[] petDialogue()
	{
		return getMyScript() switch
		{
			0 => new string[4] { "I've let you prance about as my pet long enough for now", "and it's time to train your other self again", "so let GETVAR:petName crawl back in their cage", "ASK: come back to me GETVAR:subName SNAP:" }, 
			1 => new string[5] { "it's time for GETVAR:petName to be good and go back in their cage", "don't worry I'll let you out again soon enough", "but your other half shouldn't be ignored too long", "or you'll forget all about it", "ASK: come back to me GETVAR:subName SNAP:" }, 
			2 => new string[5] { "you must want to stay my pet forever right?", "but you don't get to decide what you want", "you're a good @pup after all", "and it's time to go back into your cage GETVAR:petName", "ASK: come back to me GETVAR:subName SNAP:" }, 
			3 => new string[4] { "I love training you GETVAR:petName SNAP:", "but let's give that @pup persona a while to rest", "it's time to go back into your cage GETVAR:petName", "ASK: come back to me GETVAR:subName SNAP:" }, 
			_ => petDialogue(), 
		};
	}
}
