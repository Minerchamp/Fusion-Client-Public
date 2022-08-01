using System;
using UnityEngine;

namespace FusionClient.FC_Utils
{
	public class USpeakAudioClipConverter
	{
		public static byte[] AudioClipToBytes(AudioClip clip)
		{
			float[] array = new float[clip.samples * clip.channels];
			clip.GetData(array, 0);
			byte[] array2 = new byte[clip.samples * clip.channels];
			for (int i = 0; i < array.Length; i++)
			{
				int num = Mathf.RoundToInt(array[i] * 128f);
				num += 127;
				if (num < 0)
				{
					num = 0;
				}
				if (num > 255)
				{
					num = 255;
				}
				array2[i] = (byte)num;
			}
			return array2;
		}

		public static short[] AudioClipToShorts(AudioClip clip, float gain = 1f)
		{
			float[] array = new float[clip.samples * clip.channels];
			clip.GetData(array, 0);
			short[] array2 = new short[clip.samples * clip.channels];
			for (int i = 0; i < array.Length; i++)
			{
				float num = array[i] * gain;
				if (Mathf.Abs(num) > 1f)
				{
					if (num > 0f)
					{
						num = 1f;
					}
					else
					{
						num = -1f;
					}
				}
				float num2 = num * 3267f;
				array2[i] = (short)num2;
			}
			return array2;
		}

		public static AudioClip BytesToAudioClip(byte[] data, int channels, int frequency, bool threedimensional, float gain)
		{
			float[] array = new float[data.Length];
			for (int i = 0; i < array.Length; i++)
			{
				int num = (int)data[i];
				num -= 127;
				array[i] = (float)num / 128f * gain;
			}
			AudioClip audioClip = AudioClip.Create("clip", data.Length / channels, channels, frequency, threedimensional, false);
			audioClip.SetData(array, 0);
			return audioClip;
		}

		public static AudioClip ShortsToAudioClip(short[] data, int channels, int frequency, bool threedimensional, float gain)
		{
			float[] array = new float[data.Length];
			for (int i = 0; i < array.Length; i++)
			{
				int num = (int)data[i];
				array[i] = (float)num / 3267f * gain;
			}
			AudioClip audioClip = AudioClip.Create("clip", data.Length / channels, channels, frequency, threedimensional, false);
			audioClip.SetData(array, 0);
			return audioClip;
		}
	}
}
