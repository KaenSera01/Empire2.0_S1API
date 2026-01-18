using Empire.NPC.Data;
using Empire.NPC.S1API_NPCs;
using Empire.Utilities.ListHelpers;
using Empire.Utilities.QualityHelpers;
using S1API.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Empire.Utilities.QuestHelpers
{ 
	public class QuestBuilder
	{
		private readonly EmpireNPC buyer;
		private readonly Drug unlockedDrug;
		private readonly Shipping shipping;

		public QuestBuilder(EmpireNPC buyer, Drug unlockedDrug)
		{
			this.buyer = buyer;
			this.unlockedDrug = unlockedDrug;
			this.shipping = buyer.Shippings[buyer.DealerSaveData.ShippingTier];
		}

		// --- AMOUNT CALCULATION --------------------------------------------------

		public int CalculateAmount()
		{
			int min = shipping.MinAmount;
			int max = shipping.MaxAmount;

			double logResult = 0;
			if (buyer.RepLogBase > 1)
			{
				logResult = Math.Log(buyer.DealerSaveData.Reputation + 1, buyer.RepLogBase);
				logResult = logResult < 4 ? 0 : logResult - 4;
			}

			int steps = (max - min) / shipping.StepAmount;
			int randomStep = steps > 0 ? RandomUtils.RangeInt(0, steps + 1) : 0;
			randomStep = (int)(randomStep * (1 + logResult));

			return min + randomStep * shipping.StepAmount;
		}

		// --- QUALITY SELECTION ---------------------------------------------------

		public (string name, float mult, string color) SelectQuality()
		{
			var q = unlockedDrug.Qualities.RandomElement();
			return QualityEffectResolver.ResolveQuality(q);
		}

		// --- EFFECT SELECTION ----------------------------------------------------

		public (
			List<string> necessary, List<float> necessaryMult,
			List<string> optional, List<float> optionalMult,
			float tempMult11, float tempMult21
		) SelectEffects(float randomNum1)
		{
			var necessary = new List<string>();
			var necessaryMult = new List<float>();
			var optional = new List<string>();
			var optionalMult = new List<float>();

			float temp11 = 1f;
			float temp21 = 1f;

			bool noNecessary = EmpireMod.DisableNecessaryEffects.Value;

			foreach (var e in unlockedDrug.Effects)
			{
				bool isNecessary =
					e.Probability > 1f &&
					UnityEngine.Random.value < (e.Probability - 1f) &&
					!noNecessary;

				bool isOptional =
					(e.Probability > 0f && e.Probability <= 1f &&
					 UnityEngine.Random.value < e.Probability)
					|| noNecessary;

				if (!isNecessary && !isOptional)
					continue;

				string effectName = e.Name;

				if (effectName == "Random")
					effectName = QualityEffectResolver.ResolveRandomEffectName(
						necessary.Concat(optional)
					);

				if (string.IsNullOrEmpty(effectName))
					continue;

				var (name, mult, _) = QualityEffectResolver.ResolveEffect(effectName, e.DollarMult);
				float scaled = mult * randomNum1;

				if (isNecessary)
				{
					necessary.Add(name);
					necessaryMult.Add(scaled);
					temp11 += scaled;
					temp21 += scaled;
				}
				else
				{
					optional.Add(name);
					optionalMult.Add(scaled);
					temp21 += scaled;
				}
			}

			return (necessary, necessaryMult, optional, optionalMult, temp11, temp21);
		}
	}
}
