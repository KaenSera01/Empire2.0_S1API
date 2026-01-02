using MelonLoader;
using System;
using System.IO;
using System.IO.Compression;
using UnityEngine;

namespace Empire.Utilities
{
	public static class EmpirePngLoader
	{
		public static Texture2D? LoadTexture(byte[] pngData)
		{
			using var ms = new MemoryStream(pngData);
			using var br = new BinaryReader(ms);

			// PNG signature
			byte[] signature = br.ReadBytes(8);
			if (signature.Length != 8 ||
				signature[0] != 0x89 || signature[1] != 0x50 ||
				signature[2] != 0x4E || signature[3] != 0x47 ||
				signature[4] != 0x0D || signature[5] != 0x0A ||
				signature[6] != 0x1A || signature[7] != 0x0A)
			{
				MelonLogger.Msg("Invalid PNG signature");
			}

			int width = 0;
			int height = 0;
			byte bitDepth = 0;
			byte colorType = 0;
			byte[] idat = null;

			// Read chunks
			while (br.BaseStream.Position < br.BaseStream.Length)
			{
				int length = ReadInt(br);
				string type = new string(br.ReadChars(4));
				byte[] data = br.ReadBytes(length);
				br.ReadBytes(4); // CRC

				if (type == "IHDR")
				{
					width = ReadInt(data, 0);
					height = ReadInt(data, 4);
					bitDepth = data[8];
					colorType = data[9];
				}
				else if (type == "IDAT")
				{
					idat = idat == null ? data : Combine(idat, data);
				}
				else if (type == "IEND")
				{
					break;
				}
			}

			if (idat == null)
			{
				MelonLogger.Msg("PNG missing IDAT");
				return null;
			}

			if (bitDepth != 8 || colorType != 6)
			{
				MelonLogger.Msg($"Unsupported PNG format (bitDepth={bitDepth}, colorType={colorType}), expected 8-bit RGBA");
				return null;
			}

			// Decompress IDAT (zlib)
			byte[] decompressed = DecompressZlib(idat);

			int bpp = 4; // RGBA
			int stride = width * bpp;
			Color32[] pixels = new Color32[width * height];

			int src = 0;
			int dst = 0;

			byte[] prevScanline = new byte[stride];
			byte[] currScanline = new byte[stride];

			for (int y = 0; y < height; y++)
			{
				byte filter = decompressed[src++];

				// Reconstruct current scanline
				switch (filter)
				{
					case 0: // None
						Buffer.BlockCopy(decompressed, src, currScanline, 0, stride);
						src += stride;
						break;

					case 1: // Sub
						for (int i = 0; i < stride; i++)
						{
							byte left = i >= bpp ? currScanline[i - bpp] : (byte)0;
							currScanline[i] = (byte)(decompressed[src++] + left);
						}
						break;

					case 2: // Up
						for (int i = 0; i < stride; i++)
						{
							byte up = prevScanline[i];
							currScanline[i] = (byte)(decompressed[src++] + up);
						}
						break;

					case 3: // Average
						for (int i = 0; i < stride; i++)
						{
							byte left = i >= bpp ? currScanline[i - bpp] : (byte)0;
							byte up = prevScanline[i];
							byte avg = (byte)((left + up) / 2);
							currScanline[i] = (byte)(decompressed[src++] + avg);
						}
						break;

					case 4: // Paeth
						for (int i = 0; i < stride; i++)
						{
							byte left = i >= bpp ? currScanline[i - bpp] : (byte)0;
							byte up = prevScanline[i];
							byte upLeft = i >= bpp ? prevScanline[i - bpp] : (byte)0;
							byte paeth = PaethPredictor(left, up, upLeft);
							currScanline[i] = (byte)(decompressed[src++] + paeth);
						}
						break;

					default:
						MelonLogger.Msg($"Unsupported PNG filter type: {filter}");
						break;
				}

				// Convert scanline to pixels
				for (int x = 0; x < width; x++)
				{
					int i = x * 4;
					byte r = currScanline[i];
					byte g = currScanline[i + 1];
					byte b = currScanline[i + 2];
					byte a = currScanline[i + 3];

					pixels[dst++] = new Color32(r, g, b, a);
				}

				// Swap scanline buffers
				var temp = prevScanline;
				prevScanline = currScanline;
				currScanline = temp;
			}

			// Create texture and assign pixels (bottom-left origin)
			Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);

			for (int i = 0; i < pixels.Length; i++)
			{
				int x = i % width;
				int y = i / width;
				tex.SetPixel(x, height - 1 - y, pixels[i]); // flip vertically
			}

			tex.Apply();
			return tex;
		}

		private static int ReadInt(BinaryReader br)
		{
			byte[] b = br.ReadBytes(4);
			if (BitConverter.IsLittleEndian) Array.Reverse(b);
			return BitConverter.ToInt32(b, 0);
		}

		private static int ReadInt(byte[] data, int offset)
		{
			byte[] b = new byte[4];
			Buffer.BlockCopy(data, offset, b, 0, 4);
			if (BitConverter.IsLittleEndian) Array.Reverse(b);
			return BitConverter.ToInt32(b, 0);
		}

		private static byte[] Combine(byte[] a, byte[] b)
		{
			byte[] r = new byte[a.Length + b.Length];
			Buffer.BlockCopy(a, 0, r, 0, a.Length);
			Buffer.BlockCopy(b, 0, r, a.Length, b.Length);
			return r;
		}

		private static byte[] DecompressZlib(byte[] data)
		{
			using var ms = new MemoryStream(data);
			// Skip zlib header (2 bytes)
			ms.ReadByte();
			ms.ReadByte();

			using var ds = new DeflateStream(ms, CompressionMode.Decompress);
			using var outMs = new MemoryStream();
			ds.CopyTo(outMs);
			return outMs.ToArray();
		}

		private static byte PaethPredictor(byte a, byte b, byte c)
		{
			int p = a + b - c;
			int pa = Math.Abs(p - a);
			int pb = Math.Abs(p - b);
			int pc = Math.Abs(p - c);

			if (pa <= pb && pa <= pc) return a;
			if (pb <= pc) return b;
			return c;
		}
	}
}