namespace OpenEdge.scripts;

internal class HypnosisMantra : TalkBaseClass
{
	public HypnosisMantra(MainWindow mw)
		: base(mw)
	{
		allText = mw.lr.getScript("hypnosisMantra");
		setFlag("mantra", temp: true);
	}

	public new string Talk()
	{
		location++;
		string sentence = allText[(location - 1) % allText.Length];
		sentence = CheckWords(sentence);
		sentence = sentence.Trim();
		string text = "";
		string[] array = sentence.Split(" ");
		foreach (string text2 in array)
		{
			string text3 = interpetText(text2.Trim(), sentence);
			if (text3 == "flagFailed")
			{
				text = "";
				break;
			}
			text = text + text3 + " ";
		}
		return text.Trim();
	}

	private string[] createBody()
	{
		switch (getMyScript())
		{
		case 0:
			if (mw.getTFlag("collar") && mw.getTFlag("collar") && mw.isSettingEnabled("petPlay"))
			{
				return new string[4] { "collar goes on", "mind goes off", "pets don't think", "pets just sink" };
			}
			break;
		case 1:
			return new string[4] { "pink clouds", "pop pop pop", "thoughts away", "when I drop" };
		case 2:
			return new string[4] { "I need permission, to edge and stroke", "my pleasure, she may revoke", "I must obey her every word", "or suffer the wrath I have incurred" };
		case 3:
			return new string[4] { "it feels good to obey", "obedience is pleasure", "pleasure is obedience", "I love feeling good" };
		case 4:
			if (mw.isSettingEnabled("petPlay") && mw.isSettingEnabled("treats"))
			{
				return new string[4] { "training makes me happy", "I'm proud of my obedience", "my owner loves me", "I'm a good @pup" };
			}
			break;
		case 5:
			return new string[4] { "I should obey", "I should stroke", "I should edge", "I need to cum" };
		case 6:
			if (mw.isSettingEnabled("humiliation"))
			{
				return new string[4] { "I get hard when I get bullied", "and that's just pathetic", "I love getting degraded", "like a whimpy little slut" };
			}
			break;
		case 7:
			return new string[4] { "I'm a good @boy", "I want to stay good", "I want to be praised", "I want to be loved" };
		case 8:
			if (mw.isSettingEnabled("petPlay"))
			{
				return new string[4] { "I love to obey", "obeying is easy", "it's easy to obey", "obeying makes me hard" };
			}
			break;
		case 9:
			if (int.Parse(mw.getVar("denied")) > 2)
			{
				return new string[4] { "I'm so needy", "I need to touch myself", "I need to cum", "I need permission" };
			}
			break;
		case 10:
			if (int.Parse(mw.getVar("denied")) > 2)
			{
				return new string[4] { "I can't take it anymore", "I have to cum", "please let me cum", "I need permission" };
			}
			break;
		}
		return createBody();
	}
}
