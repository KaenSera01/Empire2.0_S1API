using Empire.NPC.S1API_NPCs;
using Empire.Utilities;
using System.IO;
using UnityEngine;

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

	private static string IconResourcePath(string fileName)
		=> $"Empire.NPC.S1API_NPCs.Icons.{fileName}";
}