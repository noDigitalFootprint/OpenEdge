using OpenEdge.helper;

namespace OpenEdge.vocab;

internal class Cock : BaseVocab
{
	public Cock(Voc voc)
		: base(voc)
	{
		ballsBound.AddRange(new string[2] { "bound dick", "bound cock" });
		normal.AddRange(new string[6] { "cock", "dick", "horny dick", "horny cock", "desperate cock", "desperate dick" });
		sph.AddRange(new string[18]
		{
			"dicklet", "toy", "small thing", "toothpick", "tiny cock", "tiny little cock", "hilarious excuse for a dick", "joke of a dick", "hilarious excuse for a cock", "joke of a cock",
			"tiny little dick", "tiny dick", "denied little dick", "denied little cock", "poor little cock", "poor little dick", "horny little cock", "horny little dick"
		});
		quickShot.AddRange(new string[6] { "leaky dick", "weak dick", "drippy dick", "leaky cock", "weak cock", "drippy cock" });
		degrading.AddRange(new string[6] { "pathetic dick", "useless dick", "worthless dick", "pathetic cock", "useless cock", "worthless cock" });
		virgin.AddRange(new string[6] { "unused dick", "virgin dick", "celibate dick", "unused cock", "virgin cock", "celibate cock" });
		chastity.AddRange(new string[8] { "caged dick", "locked dick", "restrained dick", "strained dick", "caged cock", "locked cock", "restrained cock", "strained cock" });
	}
}
