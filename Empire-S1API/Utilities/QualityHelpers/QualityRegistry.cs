using System;
using System.Collections.Generic;
using System.Linq;

namespace Empire.Utilities.QualityHelpers
{
	public static class QualityRegistry
	{
		public static readonly List<QualityInfo> Qualities = new List<QualityInfo>()
		{
			new QualityInfo { Name = "trash",     Color = "#a84545", DollarMult = 0f },
			new QualityInfo { Name = "poor",      Color = "#5bad38", DollarMult = 0f },
			new QualityInfo { Name = "standard",  Color = "#358ecd", DollarMult = 0f },
			new QualityInfo { Name = "premium",   Color = "#e93be9", DollarMult = 0.25f },
			new QualityInfo { Name = "heavenly",  Color = "#ecb522", DollarMult = 0.50f }
		};

		public static readonly Dictionary<string, QualityInfo> ByName =
			Qualities.ToDictionary(q => q.Name, q => q, StringComparer.OrdinalIgnoreCase);

		public static int GetQualityNumberSafe(string quality)
		{
			if (string.IsNullOrWhiteSpace(quality))
				return -1;

			string key = quality.Trim();

			if (!ByName.TryGetValue(key, out var info))
				return -1;

			return Qualities.IndexOf(info);
		}
	}
}