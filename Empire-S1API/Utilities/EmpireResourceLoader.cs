using Core.DebugHandler;
using Empire.NPC.S1API_NPCs;
using MelonLoader;
using MelonLoader.Utils;
using System;
using System.IO;
using UnityEngine;

namespace Empire.Utilities
{
	public static class EmpireResourceLoader
	{
		public static Sprite? LoadEmbeddedIcon(string fileName)
		{
			var asm = typeof(EmpireNPC).Assembly;
			string resourcePath = IconResourcePath(fileName);

			using Stream? stream = asm.GetManifestResourceStream(resourcePath);
			if (stream == null) return null;

			byte[] buffer = new byte[stream.Length];
			stream.Read(buffer, 0, buffer.Length);

			Texture2D tex = EmpirePngLoader.LoadTexture(buffer);

			return Sprite.Create(
				tex,
				new Rect(0, 0, tex.width, tex.height),
				new Vector2(0.5f, 0.5f)
			);
		}

		public static Sprite? LoadIconFromDisk(string imageName)
		{
			string iconFolder = Path.Combine(MelonEnvironment.ModsDirectory, "Empire", "Icons");
			string path = Path.Combine(iconFolder, imageName);

			if (!File.Exists(path))
				return null;

			try
			{
				byte[] bytes = File.ReadAllBytes(path);
				Texture2D? tex = EmpirePngLoader.LoadTexture(bytes);

				if (tex != null)
				{
					return Sprite.Create(
						tex,
						new Rect(0, 0, tex.width, tex.height),
						new Vector2(0.5f, 0.5f)
					);
				}
				else
					return null;
			}
			catch (Exception ex)
			{
				DebugLogger.LogError($"Failed to load icon {imageName}: {ex.Message}");
				return null;
			}
		}

		private static string IconResourcePath(string fileName)
			=> $"Empire.NPC.S1API_NPCs.Icons.{fileName}";
	}
}