using Empire.Utilities.EffectHelpers;
using System.Collections.Generic;
using System.Linq;

namespace Empire.Reward
{
	public class RewardBreakdown
	{
		public float BaseValue { get; set; }
		public float QualityBonus { get; set; }
		public float DealTimeBonus { get; set; }
		public float ExceedQualityBonus { get; set; }
		public List<EffectBreakdown> NecessaryEffects { get; set; } = new List<EffectBreakdown>();
		public List<EffectBreakdown> OptionalEffects { get; set; } = new List<EffectBreakdown>();
		public float EffectsTotalBonus => NecessaryEffects.Sum(e => e.Value) + OptionalEffects.Sum(e => e.Value);
		public float FinalReward => BaseValue + QualityBonus + DealTimeBonus + EffectsTotalBonus;
	}
}
