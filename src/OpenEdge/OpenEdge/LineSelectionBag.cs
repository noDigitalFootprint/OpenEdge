using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenEdge;

public sealed class LineSelectionBag
{
	private sealed class BagState
	{
		public string Signature { get; set; } = "";
		public Queue<int> Remaining { get; } = new Queue<int>();
		public int LastIndex { get; set; } = -1;
	}

	private readonly Dictionary<string, BagState> states = new Dictionary<string, BagState>(StringComparer.OrdinalIgnoreCase);
	private readonly Random random;

	public LineSelectionBag()
		: this(new Random())
	{
	}

	public LineSelectionBag(Random random)
	{
		this.random = random ?? new Random();
	}

	public int NextIndex(string key, IReadOnlyList<string> options)
	{
		if (options == null || options.Count == 0)
		{
			return -1;
		}
		if (options.Count == 1)
		{
			return 0;
		}
		key = string.IsNullOrWhiteSpace(key) ? "default" : key;
		string signature = BuildSignature(options);
		if (!states.TryGetValue(key, out BagState state))
		{
			state = new BagState();
			states[key] = state;
		}
		if (!string.Equals(state.Signature, signature, StringComparison.Ordinal) || state.Remaining.Count == 0)
		{
			Refill(state, signature, options.Count);
		}
		int index = state.Remaining.Dequeue();
		state.LastIndex = index;
		return index;
	}

	private void Refill(BagState state, string signature, int count)
	{
		state.Signature = signature;
		List<int> indices = Enumerable.Range(0, count).ToList();
		for (int i = indices.Count - 1; i > 0; i--)
		{
			int swapIndex = random.Next(i + 1);
			(indices[i], indices[swapIndex]) = (indices[swapIndex], indices[i]);
		}
		if (indices.Count > 1 && indices[0] == state.LastIndex)
		{
			int swapIndex = random.Next(1, indices.Count);
			(indices[0], indices[swapIndex]) = (indices[swapIndex], indices[0]);
		}
		state.Remaining.Clear();
		foreach (int index in indices)
		{
			state.Remaining.Enqueue(index);
		}
	}

	private static string BuildSignature(IReadOnlyList<string> options)
	{
		unchecked
		{
			int hash = 17;
			foreach (string option in options)
			{
				hash = hash * 31 + (option ?? "").GetHashCode(StringComparison.Ordinal);
			}
			return options.Count + ":" + hash;
		}
	}
}
