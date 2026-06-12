using NAudio.Wave;

public class LoopStream : WaveStream
{
	private WaveStream sourceStream;

	public bool EnableLooping { get; set; }

	public override WaveFormat WaveFormat => sourceStream.WaveFormat;

	public override long Length => sourceStream.Length;

	public override long Position
	{
		get
		{
			return sourceStream.Position;
		}
		set
		{
			sourceStream.Position = value;
		}
	}

	public LoopStream(WaveStream sourceStream)
	{
		this.sourceStream = sourceStream;
		EnableLooping = true;
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		int i;
		int num;
		for (i = 0; i < count; i += num)
		{
			num = sourceStream.Read(buffer, offset + i, count - i);
			if (num == 0)
			{
				if (sourceStream.Position == 0L || !EnableLooping)
				{
					break;
				}
				sourceStream.Position = 0L;
			}
		}
		return i;
	}
}
