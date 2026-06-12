using System;
using System.IO;

namespace OpenEdge;

public static class ImageFileSafety
{
	private const long MaxDecodeFileBytes = 150L * 1024L * 1024L;
	private const int MaxDimension = 20000;
	private const long MaxPixelCount = 120L * 1000L * 1000L;

	public static bool IsSafeForWpfDecode(string fullPath, out string reason)
	{
		reason = "";
		if (string.IsNullOrWhiteSpace(fullPath) || !File.Exists(fullPath))
		{
			reason = "file missing";
			return false;
		}
		FileInfo file = new FileInfo(fullPath);
		if (file.Length <= 0)
		{
			reason = "empty file";
			return false;
		}
		if (file.Length > MaxDecodeFileBytes)
		{
			reason = "file too large for preview decode: " + file.Length + " bytes";
			return false;
		}
		string extension = Path.GetExtension(fullPath).ToLowerInvariant();
		if (extension == ".jpg" || extension == ".jpeg")
		{
			if (!HasJpegEndMarker(fullPath))
			{
				reason = "jpeg missing end marker";
				return false;
			}
			if (!TryReadJpegDimensions(fullPath, out int width, out int height))
			{
				reason = "jpeg dimensions unavailable";
				return false;
			}
			return AreDimensionsSafe(width, height, out reason);
		}
		if (extension == ".png" && TryReadPngDimensions(fullPath, out int pngWidth, out int pngHeight))
		{
			return AreDimensionsSafe(pngWidth, pngHeight, out reason);
		}
		return true;
	}

	private static bool AreDimensionsSafe(int width, int height, out string reason)
	{
		reason = "";
		if (width <= 0 || height <= 0)
		{
			reason = "invalid dimensions";
			return false;
		}
		if (width > MaxDimension || height > MaxDimension || (long)width * height > MaxPixelCount)
		{
			reason = "image dimensions too large: " + width + "x" + height;
			return false;
		}
		return true;
	}

	private static bool HasJpegEndMarker(string fullPath)
	{
		using FileStream stream = File.OpenRead(fullPath);
		if (stream.Length < 2)
		{
			return false;
		}
		int readSize = (int)Math.Min(4096, stream.Length);
		stream.Position = stream.Length - readSize;
		byte[] buffer = new byte[readSize];
		stream.Read(buffer, 0, buffer.Length);
		for (int i = buffer.Length - 2; i >= 0; i--)
		{
			if (buffer[i] == 0xFF && buffer[i + 1] == 0xD9)
			{
				return true;
			}
		}
		return false;
	}

	private static bool TryReadJpegDimensions(string fullPath, out int width, out int height)
	{
		width = 0;
		height = 0;
		using FileStream stream = File.OpenRead(fullPath);
		while (stream.Position < stream.Length - 1)
		{
			if (stream.ReadByte() != 0xFF)
			{
				continue;
			}
			int marker;
			do
			{
				marker = stream.ReadByte();
			}
			while (marker == 0xFF);
			if (marker < 0 || marker == 0xD9 || marker == 0xDA)
			{
				return false;
			}
			if (marker == 0xD8 || marker == 0x01 || (marker >= 0xD0 && marker <= 0xD7))
			{
				continue;
			}
			int length = ReadBigEndianUInt16(stream);
			if (length < 2 || stream.Position + length - 2 > stream.Length)
			{
				return false;
			}
			if ((marker >= 0xC0 && marker <= 0xC3) || (marker >= 0xC5 && marker <= 0xC7) || (marker >= 0xC9 && marker <= 0xCB) || (marker >= 0xCD && marker <= 0xCF))
			{
				stream.ReadByte();
				height = ReadBigEndianUInt16(stream);
				width = ReadBigEndianUInt16(stream);
				return width > 0 && height > 0;
			}
			stream.Position += length - 2;
		}
		return false;
	}

	private static bool TryReadPngDimensions(string fullPath, out int width, out int height)
	{
		width = 0;
		height = 0;
		using FileStream stream = File.OpenRead(fullPath);
		byte[] header = new byte[24];
		if (stream.Read(header, 0, header.Length) != header.Length)
		{
			return false;
		}
		byte[] signature = new byte[8] { 137, 80, 78, 71, 13, 10, 26, 10 };
		for (int i = 0; i < signature.Length; i++)
		{
			if (header[i] != signature[i]) return false;
		}
		width = (header[16] << 24) | (header[17] << 16) | (header[18] << 8) | header[19];
		height = (header[20] << 24) | (header[21] << 16) | (header[22] << 8) | header[23];
		return width > 0 && height > 0;
	}

	private static int ReadBigEndianUInt16(Stream stream)
	{
		int high = stream.ReadByte();
		int low = stream.ReadByte();
		if (high < 0 || low < 0)
		{
			return -1;
		}
		return (high << 8) | low;
	}
}
