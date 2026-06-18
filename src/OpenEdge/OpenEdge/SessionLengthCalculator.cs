namespace OpenEdge;

// Pure session-length arithmetic, extracted from MainWindow.startSession so it can be
// unit-tested without the WPF host. Combines the freshly rolled session length with any
// time owed from an unfinished prior session and any previously banked extraTime, clamps
// the active session to the cap, and banks the overflow into extraTime for the next session
// (instead of silently discarding it).
public static class SessionLengthCalculator
{
	public const int MaxSessionLength = 7200;

	public readonly struct Result
	{
		public Result(int activeLength, int bankedExtraTime)
		{
			ActiveLength = activeLength;
			BankedExtraTime = bankedExtraTime;
		}

		// Session length to run now, never above MaxSessionLength.
		public int ActiveLength { get; }

		// Time to persist into extraTime so it carries into the next session.
		public int BankedExtraTime { get; }
	}

	public static Result CarryOverAndBank(int owed, int rolledLength, int extraTime)
	{
		int total = owed + rolledLength + extraTime;
		if (total > MaxSessionLength)
		{
			return new Result(MaxSessionLength, total - MaxSessionLength);
		}
		return new Result(total, 0);
	}
}
