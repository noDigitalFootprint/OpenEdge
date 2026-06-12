using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NAudio.Wave;

namespace OpenEdge.helper;

internal class AnglePlayer
{
	private List<WaveOutEvent> sounds = new List<WaveOutEvent>();

	private List<MonoToStereoProvider16> audioData = new List<MonoToStereoProvider16>();

	private Random random = new Random();

	private List<float> volume = new List<float>();

	private List<float> angle = new List<float>();

	private List<string> soundName = new List<string>();

	private List<LoopStream> loops = new List<LoopStream>();

	public bool stopPerpetualEarlicking;

	public bool stopDroning;

	public int perpetualIntensity = 1;

	public List<int> workingOn = new List<int>();

	public double asmrVolume = 0.5;

	public AnglePlayer()
	{
		prepFileAtAngle("licking1", 0f, 0f, enableLooping: true, randomPos: true);
		prepFileAtAngle("licking2", 0.33f, 0f, enableLooping: true, randomPos: true);
		prepFileAtAngle("licking3", 0.66f, 0f, enableLooping: true, randomPos: true);
		prepFileAtAngle("licking4", 1f, 0f, enableLooping: true, randomPos: true);
		prepFileAtAngle("moaning", 0.5f, 0f, enableLooping: true, randomPos: true);
		prepFileAtAngle("tickLong", 0.5f, 0.4f);
		prepFileAtAngle("snap", 0.5f, 1f);
		prepFileAtAngle("doubleSnap", 0.5f, 1f);
		prepFileAtAngle("140HZ", 0f, 0f, enableLooping: true);
		prepFileAtAngle("146HZ", 1f, 0f, enableLooping: true);
		playSilence();
	}

	public void playSilence()
	{
		try
		{
			WaveOutEvent waveOutEvent = new WaveOutEvent();
			waveOutEvent.Init(new LoopStream(new WaveFileReader(Path.Combine(RuntimePaths.AudioDir, "silence.wav")))
			{
				EnableLooping = true
			});
			waveOutEvent.Play();
		}
		catch
		{
			Task.Delay(10000).ContinueWith(delegate
			{
				playSilence();
			});
		}
	}

	public void prepFileAtAngle(string fileName, float localAngle, float localVolume, bool enableLooping = false, bool randomPos = false)
	{
		WaveOutEvent waveOutEvent = new WaveOutEvent();
		random = new Random();
		WaveFileReader waveFileReader = new WaveFileReader(Path.Combine(RuntimePaths.AudioDir, fileName + ".wav"));
		if (randomPos)
		{
			waveFileReader.Position = waveFileReader.Length * random.Next(0, 80) / 100;
		}
		LoopStream loopStream = new LoopStream(waveFileReader);
		loopStream.EnableLooping = enableLooping;
		MonoToStereoProvider16 monoToStereoProvider = new MonoToStereoProvider16(loopStream);
		if (waveFileReader.Length < 400000)
		{
			waveOutEvent.DesiredLatency = 70;
			waveOutEvent.NumberOfBuffers = 6;
		}
		try
		{
			waveOutEvent.Init(monoToStereoProvider);
			loops.Add(loopStream);
			sounds.Add(waveOutEvent);
			audioData.Add(monoToStereoProvider);
			angle.Add(localAngle);
			soundName.Add(fileName);
			volume.Add(localVolume);
			setVolume(sounds.Count - 1, localVolume);
			setAngle(sounds.Count - 1, localAngle);
		}
		catch
		{
			Task.Delay(20000).ContinueWith(delegate
			{
				prepFileAtAngle(fileName, localAngle, localVolume, enableLooping, randomPos);
			});
		}
	}

	public void panAudios()
	{
		for (int i = 0; i < 5; i++)
		{
			panAudio(i, random.NextSingle());
		}
	}

	public int numberByName(string name)
	{
		for (int i = 0; i < soundName.Count; i++)
		{
			if (soundName[i] == name)
			{
				return i;
			}
		}
		return -1;
	}

	public float getAngle(int soundNumber)
	{
		float leftVolume = audioData[soundNumber].LeftVolume;
		float rightVolume = audioData[soundNumber].RightVolume;
		if (leftVolume + rightVolume == 0f)
		{
			return 0.5f;
		}
		if (rightVolume == 0f)
		{
			return 1f;
		}
		return leftVolume / (leftVolume + rightVolume);
	}

	public void setAngle(int soundNumber, float newAngle)
	{
		angle[soundNumber] = newAngle;
		setVolume(soundNumber, getVolume(soundNumber));
	}

	public float getVolume(int soundNumber)
	{
		try
		{
			return volume[soundNumber];
		}
		catch
		{
		}
		return 0f;
	}

	public void setVolume(int soundNumber, float newVolume)
	{
		volume[soundNumber] = newVolume;
		audioData[soundNumber].LeftVolume = newVolume * angle[soundNumber];
		audioData[soundNumber].RightVolume = newVolume * (1f - angle[soundNumber]);
	}

	public void perpetualEarLicking()
	{
		_ = asmrVolume;
		_ = 0.0;
		float num = random.NextSingle();
		int[] array = new int[4]
		{
			numberByName("licking1"),
			numberByName("licking2"),
			numberByName("licking3"),
			numberByName("licking4")
		};
		for (int i = 0; i < array.Length; i++)
		{
			panAudio(array[i], (num + angle[array[i]]) % 1f, 20000 / perpetualIntensity);
			animateSoundVolume(array[i], (float)((double)(random.NextSingle() / 5f * (float)perpetualIntensity) * asmrVolume), 20000 / perpetualIntensity);
			if (stopPerpetualEarlicking)
			{
				stopSound(array[i]);
			}
			else
			{
				playSound(array[i], asmrVolume / 5.0);
			}
		}
		if (stopPerpetualEarlicking)
		{
			stopPerpetualEarlicking = false;
			perpetualIntensity = 1;
		}
		else
		{
			Task.Delay(24000 / perpetualIntensity).ContinueWith(delegate
			{
				perpetualEarLicking();
			});
		}
	}

	public void setDroningVolume()
	{
		animateSoundVolume(numberByName("140HZ"), (float)((double)perpetualIntensity / 5.0 * asmrVolume), 6000);
		animateSoundVolume(numberByName("146HZ"), (float)((double)perpetualIntensity / 5.0 * asmrVolume), 6000);
		if (stopDroning)
		{
			stopDroning = false;
			perpetualIntensity = 1;
		}
		else
		{
			Task.Delay(8000).ContinueWith(delegate
			{
				setDroningVolume();
			});
		}
	}

	public void setMoaning(string soundName)
	{
		int soundNumber = numberByName(soundName);
		panAudio(soundNumber, random.NextSingle());
		playSound(soundNumber, asmrVolume);
		animateSoundVolume(soundNumber, (float)((double)(random.NextSingle() / 2f + 0.5f) * asmrVolume));
		Thread.Sleep(10000 + random.Next(30000));
		animateSoundVolume(soundNumber, 0f);
		Thread.Sleep(5000 + random.Next(10000));
		stopSound(soundNumber);
	}

	public void playSound(string name, double volume = -1.0)
	{
		playSound(numberByName(name), volume);
	}

	private void playSound(int soundNumber, double volume = -1.0)
	{
		if (soundNumber != -1)
		{
			if (!loops[soundNumber].EnableLooping)
			{
				loops[soundNumber].Position = 0L;
			}
			if (volume != -1.0)
			{
				setVolume(soundNumber, (float)volume);
			}
			sounds[soundNumber].Play();
		}
	}

	public void stopSound(string name)
	{
		stopSound(numberByName(name));
	}

	private void stopSound(int soundNumber)
	{
		if (soundNumber != -1)
		{
			sounds[soundNumber].Stop();
		}
	}

	public void stopSounds()
	{
		for (int i = 0; i < sounds.Count(); i++)
		{
			animateSoundVolume(i, 0f);
		}
		Task.Delay(200).ContinueWith(delegate
		{
			foreach (WaveOutEvent sound in sounds)
			{
				sound.Stop();
			}
		});
	}

	public void playSounds(int from, int to, float volume = -1f)
	{
		for (int i = from; i < to; i++)
		{
			if (volume == -1f)
			{
				float num = random.NextSingle() + 0.2f;
				if (num > 1f)
				{
					num = 1f;
				}
				animateSoundVolume(i, num);
			}
			else
			{
				animateSoundVolume(i, volume);
			}
			sounds[i].Play();
		}
	}

	public void panAudio(int soundNumber, float newAngle, int time = 5000)
	{
		try
		{
			int sleep = 200;
			int num = time / sleep;
			if (num > 0)
			{
				float num2 = newAngle - angle[soundNumber];
				setAngle(soundNumber, angle[soundNumber] + num2 / (float)num);
				Task.Delay(sleep).ContinueWith(delegate
				{
					panAudio(soundNumber, newAngle, time - sleep);
				});
			}
			else
			{
				setAngle(soundNumber, newAngle);
			}
		}
		catch
		{
		}
	}

	public void animateSoundVolume(int soundNumber, float volumeGoal, int time = 5000)
	{
		int sleep = 200;
		int num = time / sleep;
		if (num > 0)
		{
			float num2 = volumeGoal - getVolume(soundNumber);
			float num3 = getVolume(soundNumber) + num2 / (float)num;
			if (num3 > 1f)
			{
				num3 = 1f;
			}
			setVolume(soundNumber, num3);
			Task.Delay(sleep).ContinueWith(delegate
			{
				animateSoundVolume(soundNumber, volumeGoal, time - sleep);
			});
		}
		else
		{
			setVolume(soundNumber, volumeGoal);
		}
	}
}
