using System.Collections.Generic;
using System.Linq;

namespace OpenEdge.helper;

internal class BaseVocab
{
	public Voc voc;

	private MainWindow mw;

	protected List<string> normal = new List<string>();

	protected List<string> degrading = new List<string>();

	protected List<string> sph = new List<string>();

	protected List<string> chastity = new List<string>();

	protected List<string> quickShot = new List<string>();

	protected List<string> virgin = new List<string>();

	protected List<string> cuck = new List<string>();

	protected List<string> petPlay = new List<string>();

	protected List<string> humiliation = new List<string>();

	protected List<string> gayHumiliation = new List<string>();

	protected List<string> cei = new List<string>();

	protected List<string> feet = new List<string>();

	protected List<string> gay = new List<string>();

	protected List<string> sissy = new List<string>();

	protected List<string> kneeling = new List<string>();

	protected List<string> pronounsHim = new List<string>();

	protected List<string> pronounsHer = new List<string>();

	protected List<string> pronounsThey = new List<string>();

	protected List<string> moodHigh = new List<string>();

	protected List<string> moodLow = new List<string>();

	protected List<string> orgasmDenied = new List<string>();

	protected List<string> longSinceOrgasm = new List<string>();

	protected List<string> shortSinceOrgasm = new List<string>();

	protected List<string> treats = new List<string>();

	protected List<string> censoredView = new List<string>();

	protected List<string> strokeSlow = new List<string>();

	protected List<string> strokeFast = new List<string>();

	protected List<string> plug = new List<string>();

	protected List<string> ballsBound = new List<string>();

	protected List<string> collar = new List<string>();

	protected List<string> clamp = new List<string>();

	protected List<string> findom = new List<string>();

	protected List<string> removed = new List<string>();

	protected List<string> pup = new List<string>();

	protected List<string> cat = new List<string>();

	protected List<string> strokeType = new List<string>();

	public string[][] tagTeases = new string[0][];

	public BaseVocab(Voc voc)
	{
		this.voc = voc;
		mw = voc.mw;
	}

	public List<string> getVocabList()
	{
		List<string> list = new List<string>();
		foreach (string item in getList())
		{
			list.Add(getWords(item));
		}
		return list;
	}

	public List<string> getList()
	{
		List<string> list = new List<string>();
		list.AddRange(normal);
		if (mw.getFlag("degrading"))
		{
			list.AddRange(degrading);
		}
		if (mw.getFlag("sph"))
		{
			list.AddRange(sph);
		}
		if (mw.getFlag("chastity"))
		{
			list.AddRange(chastity);
		}
		if (mw.getFlag("quickshot"))
		{
			list.AddRange(quickShot);
		}
		if (mw.isSettingEnabled("virgin"))
		{
			list.AddRange(virgin);
		}
		if (mw.isSettingEnabled("cuck"))
		{
			list.AddRange(cuck);
		}
		if (mw.isSettingEnabled("petPlay"))
		{
			list.AddRange(petPlay);
		}
		if (mw.isSettingEnabled("humiliation"))
		{
			list.AddRange(humiliation);
		}
		if (mw.isSettingEnabled("gayHumiliation"))
		{
			list.AddRange(gayHumiliation);
		}
		if (mw.isSettingEnabled("cei"))
		{
			list.AddRange(cei);
		}
		if (mw.isSettingEnabled("feet"))
		{
			list.AddRange(feet);
		}
		if (mw.isSettingEnabled("gay"))
		{
			list.AddRange(gay);
		}
		if (mw.isSettingEnabled("sissy"))
		{
			list.AddRange(sissy);
		}
		if (mw.getTFlag("kneeling"))
		{
			list.AddRange(kneeling);
		}
		if (int.Parse(mw.getVar("denied")) > 3)
		{
			list.AddRange(longSinceOrgasm);
		}
		if (int.Parse(mw.getVar("denied")) < 2)
		{
			list.AddRange(shortSinceOrgasm);
		}
		if (mw.getTFlag("plug"))
		{
			list.AddRange(plug);
		}
		if (mw.getTFlag("collar"))
		{
			list.AddRange(collar);
		}
		if (mw.getTFlag("clothesPins"))
		{
			list.AddRange(clamp);
		}
		if (mw.isSettingEnabled("findom"))
		{
			list.AddRange(findom);
		}
		if (mw.getFlag("removed"))
		{
			list.AddRange(removed);
		}
		if (mw.getFlag("pup") && mw.getTFlag("petPlay"))
		{
			list.AddRange(pup);
		}
		if (mw.getFlag("cat") && mw.getTFlag("petPlay"))
		{
			list.AddRange(cat);
		}
		if (mw.getTFlag("ballsBound"))
		{
			list.AddRange(ballsBound);
		}
		if (mw.isSettingEnabled("treats") && mw.mood > 70)
		{
			list.AddRange(treats);
		}
		if (mw.orgasmDenied)
		{
			list.AddRange(orgasmDenied);
		}
		if (mw.stroking && !mw.edgeAllowed)
		{
			if (mw.bpm < 60f)
			{
				list.AddRange(strokeSlow);
			}
			else if (mw.bpm > 180f)
			{
				list.AddRange(strokeFast);
			}
		}
		switch (mw.pronouns)
		{
		case 0:
			list.AddRange(pronounsHim);
			break;
		case 1:
			list.AddRange(pronounsHer);
			break;
		case 2:
			list.AddRange(pronounsThey);
			break;
		}
		int mood = mw.mood;
		if (mood <= 70)
		{
			if (mood < 30)
			{
				list.AddRange(moodLow);
			}
		}
		else
		{
			list.AddRange(moodHigh);
		}
		if (mw.currentScript != null && mw.secWindow.censorMode != 0)
		{
			list.AddRange(censoredView);
		}
		for (int i = 0; i < tagTeases.Length / 2; i++)
		{
			int num = i * 2;
			bool flag = true;
			string[] array = tagTeases[num];
			for (mood = 0; mood < array.Length; mood++)
			{
				string text;
				if ((text = array[mood]).StartsWith("@"))
				{
					text = text.Split("@")[1];
					if (text.StartsWith("!"))
					{
						text = text.Split("!")[1];
						text = "!temp\\" + text;
					}
					else
					{
						text = "temp\\" + text;
					}
				}
				if (text.StartsWith("!"))
				{
					text = text.Split("!")[1];
					if (char.IsUpper(text, 0) && mw.currentTags.Contains(text))
					{
						flag = false;
						break;
					}
					if (mw.getFlag(text))
					{
						flag = false;
						break;
					}
				}
				else if ((char.IsUpper(text, 0) && !mw.currentTags.Contains(text)) || (!char.IsUpper(text, 0) && !mw.getFlag(text)))
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				list.AddRange(tagTeases[num + 1]);
			}
		}
		return list;
	}

	private string getWords(string currentVoc)
	{
		string[] array = currentVoc.Split(' ');
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = array[i].Trim();
			array[i] = array[i].Trim();
			string word = getWord(array[i]);
			_ = word == "";
			array[i] = word;
			array[i] += " ";
		}
		currentVoc = "";
		string[] array2 = array;
		foreach (string text in array2)
		{
			currentVoc += text;
		}
		return currentVoc.Trim();
	}

	public string getWord(string word)
	{
		if (word.StartsWith("@"))
		{
			string text = "";
			word = word.Replace("@", "");
			if (word.EndsWith(","))
			{
				word = word.Replace(',', ' ');
				text = ",";
			}
			if (word.EndsWith("."))
			{
				word = word.Replace('.', ' ');
				text = ".";
			}
			if (word.EndsWith("?"))
			{
				word = word.Replace('?', ' ');
				text = "?";
			}
			if (word.EndsWith("!"))
			{
				word = word.Replace('!', ' ');
				text = "!";
			}
			if (word.EndsWith("*"))
			{
				word = word.Replace('*', ' ');
				text = "*";
			}
			if (word.EndsWith("\""))
			{
				word = word.Replace("\"", " ");
				text = "\"";
			}
			word = word.Trim();
			return voc.getVoc(word) + text;
		}
		return word;
	}
}
