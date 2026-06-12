namespace OpenEdge.scripts;

internal class StrokeStories : TalkBaseClass
{
	public StrokeStories(MainWindow mw)
		: base(mw)
	{
		allText = mw.lr.getScript("strokeStories");
	}
}
