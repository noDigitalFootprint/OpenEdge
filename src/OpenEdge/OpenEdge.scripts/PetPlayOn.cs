namespace OpenEdge.scripts;

internal class PetPlayOn : TalkBaseClass
{
	public PetPlayOn(MainWindow mw)
		: base(mw)
	{
		_ = new string[2] { "COLLAR", "FLAGT:petPlay FLAGT:petPlayDone" };
		_ = new string[2] { "[@yes]", "FOURS" };
		allText = mw.lr.getScript("petPlayOn");
	}

	private string[] petDialogue()
	{
		return getMyScript() switch
		{
			0 => new string[4] { "I'll let you drop back into your pet persona for a bit GETVAR:subName", "as always you can leave your worries at the door", "and embrace being my @pup for a bit", "ASK: come and play GETVAR:petName! SNAP:" }, 
			1 => new string[4] { "you look like you need to relax for a bit GETVAR:subName", "so come and be my pet for a bit", "and relax that tired brain of yours", "ASK: come and play GETVAR:petName! SNAP:" }, 
			2 => new string[5] { "I want to train your obedience some more GETVAR:subName", "so I'm going to feed that petplay fetish of yours", "until it stops being a fetish", "and turns into an addiction", "ASK: come and play GETVAR:petName! SNAP:" }, 
			3 => new string[5] { "what do you think will happen when someone figures out your other name GETVAR:subName?", "will you turn into a pet when it's called out?", "unable to resist as my conditioning takes control", "dragging your mind down", "ASK: and inviting GETVAR:petName to come and play SNAP:" }, 
			4 => new string[9] { "I may just keep you in your pet mode from now on GETVAR:subName", "you'd be reduced to a panting and whimpering mess", "so intensely in heat that you start humping whatever you can get your little paws on", "but not today", "today I want to give you yet another taste of heaven", "the pleasure that assaults you from every angle imaginable", "your brain slowly getting fried by the waves of dopamine washing over you", "and just for a second you forget about GETVAR:subName", "ASK: come and play GETVAR:petName! SNAP:" }, 
			5 => new string[6] { "I love that blank look on your face as you drop back into your role as my pet", "GETVAR:subName has worries and responsibilities", "and seeing those fall off your shoulders before my eyes is amazing", "now let them go once more", "and show me just how empty that head of yours can get", "ASK: come and play GETVAR:petName! SNAP:" }, 
			_ => petDialogue(), 
		};
	}
}
