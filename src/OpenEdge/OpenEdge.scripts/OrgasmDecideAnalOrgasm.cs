namespace OpenEdge.scripts;

internal class OrgasmDecideAnalOrgasm : InteruptTalk
{
	public OrgasmDecideAnalOrgasm(MainWindow mw, TalkBaseClass homeTalk)
		: base(mw, homeTalk)
	{
		allText = mw.lr.getScript("orgasmDecideAnalOrgasm");
	}
}
