using Core.DebugHandler;
using Empire.Quest.Data;
using Empire.Utilities.EffectHelpers;
using Empire.Utilities.QualityHelpers;
using MelonLoader;
using S1API.Products;
using System;
using System.Collections.Generic;

namespace Empire.Reward
{
	public static class RewardCalculator
	{
		public static RewardBreakdown CalculateForDelivery(
			DeliverySaveData quest,
			ProductDefinition product,
			uint deliveredAmount,
			List<string> properties,
			Quality deliveredQualityEnum
		)
		{
			if (quest == null)
			{
				DebugLogger.LogError("RewardCalculator: quest is null.");
				return new RewardBreakdown();
			}

			if (product == null)
			{
				DebugLogger.LogError($"RewardCalculator: product is null for ProductID={quest.ProductID}.");
				return new RewardBreakdown();
			}

			if (properties == null)
			{
				DebugLogger.LogWarning("RewardCalculator: properties list is null, using empty list.");
				properties = new List<string>();
			}

			if (quest.NecessaryEffects == null || quest.NecessaryEffectMult == null)
			{
				DebugLogger.LogWarning("RewardCalculator: NecessaryEffects or NecessaryEffectMult is null, treating as empty.");
				quest.NecessaryEffects ??= new List<string>();
				quest.NecessaryEffectMult ??= new List<float>();
			}

			if (quest.OptionalEffects == null || quest.OptionalEffectMult == null)
			{
				DebugLogger.LogWarning("RewardCalculator: OptionalEffects or OptionalEffectMult is null, treating as empty.");
				quest.OptionalEffects ??= new List<string>();
				quest.OptionalEffectMult ??= new List<float>();
			}

			var breakdown = new RewardBreakdown();

			// Base value
			breakdown.BaseValue = quest.Reward * deliveredAmount;   //	changed from product.MarketValue, .Reward = BaseDollar * RequiredAmount

			DebugLogger.Log($"RewardCalculator: Delivered Amount={deliveredAmount}, Base Reward per Unit={quest.Reward / deliveredAmount}, Base Value={breakdown.BaseValue}");

			// Quality bonus
			string deliveredQualityName = deliveredQualityEnum.ToString().ToLower().Trim();
			int requestedTier = QualityRegistry.GetQualityNumberSafe(quest.Quality);
			int deliveredTier = QualityRegistry.GetQualityNumberSafe(deliveredQualityName);
			int tiersExceeded = deliveredTier - requestedTier; //	quest fails if delivered < requested, so this is always >= 0

			float configuredQualityBonus = EmpireMod.ExceedQualityBonus.Value;

			float qualityMult = (tiersExceeded * EmpireMod.ExceedQualityBonus.Value); //quest.QualityMult;
			breakdown.ExceedQualityBonus = breakdown.BaseValue * qualityMult;

			DebugLogger.Log($"RewardCalculator: Delivered Quality='{deliveredQualityName}' (Tier {deliveredTier}), Requested Quality='{quest.Quality}' (Tier {requestedTier}), Quality Mult={qualityMult}, Quality Bonus={breakdown.QualityBonus}");

			// Deal time bonus
			breakdown.DealTimeBonus = breakdown.BaseValue * (quest.DealTimeMult - 1f);
			DebugLogger.Log($"RewardCalculator: Deal Time Mult={quest.DealTimeMult}, Deal Time Bonus={breakdown.DealTimeBonus}");

			// Quality bonus -- currently only for premium and heavenly quality requests
			breakdown.QualityBonus = breakdown.BaseValue * quest.QualityMult;

			// Clone lists
			var necessaryList = new List<string>(quest.NecessaryEffects);
			var necessaryMult = new List<float>(quest.NecessaryEffectMult);

			var optionalList = new List<string>(quest.OptionalEffects);
			var optionalMult = new List<float>(quest.OptionalEffectMult);

			// Merge necessary → optional if config says so
			bool disableNecessary = false;
			try
			{
				disableNecessary = EmpireMod.DisableNecessaryEffects.Value;
			}
			catch (Exception ex)
			{
				DebugLogger.LogError($"RewardCalculator: Error reading DisableNecessaryEffects: {ex}");
			}

			if (disableNecessary)
			{
				for (int i = 0; i < necessaryList.Count; i++)
				{
					optionalList.Insert(i, necessaryList[i]);
					optionalMult.Insert(i, necessaryMult[i]);
				}

				necessaryList.Clear();
				necessaryMult.Clear();
			}

			// Necessary breakdown
			for (int i = 0; i < necessaryList.Count; i++)
			{
				string effectName = necessaryList[i];
				float mult = i < necessaryMult.Count ? necessaryMult[i] : 0f;

				float value = breakdown.BaseValue * mult;

				breakdown.NecessaryEffects.Add(new EffectBreakdown
				{
					Name = effectName,
					Multiplier = mult,
					Value = value,
					IsNecessary = true
				});
			}

			// Optional breakdown
			for (int i = 0; i < optionalList.Count; i++)
			{
				string effectName = optionalList[i];
				float mult = i < optionalMult.Count ? optionalMult[i] : 0f;

				if (!properties.Contains(effectName.Trim().ToLower()))
					continue;

				float value = breakdown.BaseValue * mult;

				breakdown.OptionalEffects.Add(new EffectBreakdown
				{
					Name = effectName,
					Multiplier = mult,
					Value = value,
					IsNecessary = false
				});
			}

			DebugLogger.Log($"RewardCalculator: Final Reward={breakdown.FinalReward} -> BaseValue: {breakdown.BaseValue} + QualityBonus: {breakdown.QualityBonus} + DealTimeBonus: {breakdown.DealTimeBonus} + EffectsTotalBonus: {breakdown.EffectsTotalBonus}");
			DebugLogger.Log($"RewardCalculator: Calculated reward: {breakdown.BaseValue + breakdown.QualityBonus + breakdown.DealTimeBonus + breakdown.EffectsTotalBonus}");

			return breakdown;
		}

		public static RewardBreakdown CalculatePreview(
			DeliverySaveData quest,
			ProductDefinition product,
			uint fullAmount,
			List<string> assumedProperties,
			Quality assumedQualityEnum
		)
		{
			// Same logic as delivery, but using fullAmount and assumed quality/properties.
			return CalculateForDelivery(quest, product, fullAmount, assumedProperties, assumedQualityEnum);
		}		
	}
}
