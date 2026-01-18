using System.Collections.Generic;
using System.Linq;

namespace Empire.Utilities.EffectHelpers
{
    public static class EffectRegistry
	{
		//	refactored DollarMult to be the % increase in value the effect provides, not 1f - DollarMult
		public static readonly List<EffectInfo> Effects = new List<EffectInfo>()
		{
			new EffectInfo { Name = "AntiGravity", DollarMult = 0.54f },
			new EffectInfo { Name = "Athletic", DollarMult = 0.32f },
			new EffectInfo { Name = "Balding", DollarMult = 0.30f },
			new EffectInfo { Name = "BrightEyed", DollarMult = 0.40f },
			new EffectInfo { Name = "Calming", DollarMult = 0.10f },
			new EffectInfo { Name = "CalorieDense", DollarMult = 0.28f },
			new EffectInfo { Name = "Cyclopean", DollarMult = 0.56f },
			new EffectInfo { Name = "Disorienting", DollarMult = 0.10f },
			new EffectInfo { Name = "Electrifying", DollarMult = 0.50f },
			new EffectInfo { Name = "Energizing", DollarMult = 0.22f },
			new EffectInfo { Name = "Euphoric", DollarMult = 0.42f },
			new EffectInfo { Name = "Explosive", DollarMult = 0.40f },
			new EffectInfo { Name = "Focused", DollarMult = 0.62f },
			new EffectInfo { Name = "Foggy", DollarMult = 0.36f },
			new EffectInfo { Name = "Gingeritis", DollarMult = 0.20f },
			new EffectInfo { Name = "Glowie", DollarMult = 0.62f },
			new EffectInfo { Name = "Jennerising", DollarMult = 0.58f },
			new EffectInfo { Name = "Laxative", DollarMult = 0.20f },
			new EffectInfo { Name = "LongFaced", DollarMult = 0.52f },
			new EffectInfo { Name = "Munchies", DollarMult = 0.12f },
			new EffectInfo { Name = "Paranoia", DollarMult = 0.20f },
			new EffectInfo { Name = "Refreshing", DollarMult = 0.15f },
			new EffectInfo { Name = "Schizophrenic", DollarMult = 0.24f },
			new EffectInfo { Name = "Sedating", DollarMult = 0.26f },
			new EffectInfo { Name = "Seizure", DollarMult = 0.20f },
			new EffectInfo { Name = "Shrinking", DollarMult = 0.40f },
			new EffectInfo { Name = "Slippery", DollarMult = 0.66f },
			new EffectInfo { Name = "Smelly", DollarMult = 0.20f },
			new EffectInfo { Name = "Sneaky", DollarMult = 0.24f },
			new EffectInfo { Name = "Spicy", DollarMult = 0.36f },
			new EffectInfo { Name = "ThoughtProvoking", DollarMult = 0.44f },
			new EffectInfo { Name = "Toxic", DollarMult = 0.20f },
			new EffectInfo { Name = "TropicThunder", DollarMult = 0.46f },
			new EffectInfo { Name = "Zombifying", DollarMult = 0.58f },
			new EffectInfo { Name = "Random", DollarMult = 0.00f }
		};

		public static readonly Dictionary<string, EffectInfo> ByName =
			Effects.ToDictionary(e => e.Name, e => e);
	}
}