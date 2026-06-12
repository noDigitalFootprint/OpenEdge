using System;
using System.Collections.Generic;
using System.Linq;
using OpenEdge.vocab;

namespace OpenEdge.helper;

internal class Voc
{
	private string[] allSaidFrases = new string[10];

	private int frasesPos;

	public TagTease tagTease;

	public MainWindow mw;

	public Voc(MainWindow mw)
	{
		this.mw = mw;
		tagTease = new TagTease(this);
	}

	public string getVoc(string vocabName)
	{
		string text = "";
		vocabName = vocabName.Trim();
		text = mw.lr.getVocab(vocabName);
		if (text != "")
		{
			return text.Trim();
		}
		return vocabName;
	}

	public string sifonList(List<string> fullList)
	{
		List<string> list = fullList.ToList();
		string[] array = allSaidFrases;
		foreach (string text in array)
		{
			if (Enumerable.Contains(allSaidFrases, text))
			{
				list.Remove(text);
			}
		}
		string text2 = "";
		Random random = new Random();
		text2 = ((list.Count <= 0) ? "" : list[random.Next(list.Count)]);
		allSaidFrases[frasesPos % 10] = text2;
		frasesPos++;
		return text2;
	}
}
