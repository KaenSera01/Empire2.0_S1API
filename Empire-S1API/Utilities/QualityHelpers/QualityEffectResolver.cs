using Empire.NPC.Data;
using Empire.Utilities.EffectHelpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Empire.Utilities.QualityHelpers
{
	public static class QualityEffectResolver
	{
		public static (string name, float mult, string color) ResolveQuality(Quality q)
		{
			var info = q.Type.GetQuality(); // registry lookup

			string name = info?.Name ?? q.Type;
			float mult = info?.DollarMult ?? 0f;
			string color = info?.Color ?? "#FFFFFF";

			return (name, mult, color);
		}

		public static string ResolveRandomEffectName(IEnumerable<string> exclude)
		{
			var pool = EffectRegistry.Effects
				.Where(e =>
					!e.Name.Equals("Random", StringComparison.OrdinalIgnoreCase) &&
					!exclude.Contains(e.Name, StringComparer.OrdinalIgnoreCase))
				.ToList();

			if (pool.Count == 0)
				return "";

			return pool[UnityEngine.Random.Range(0, pool.Count)].Name;
		}

		public static (string name, float mult, string color) ResolveEffect(string effectName, float baseMult)
		{
			var info = effectName.GetEffect();

			string name = info?.Name ?? effectName;
			float mult = baseMult + (info?.DollarMult ?? 0f);
			string color = "#FFFFFF";

			return (name, mult, color);
		}
	}
}
