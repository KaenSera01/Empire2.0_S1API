using Empire.DebtHelpers;
using Empire.NPC.Data;
using Empire.NPC.Data.Enums;
using Empire.NPC.SaveData;
using Empire.Reward;
using Empire.Utilities;
using Empire.Utilities.EffectHelpers;
using Empire.Utilities.QualityHelpers;
using MelonLoader;
using S1API.Entities;
using S1API.Utils;
using S1API.Saveables;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Empire.NPC.S1API_NPCs
{
    public abstract class EmpireNPC : S1API.Entities.NPC
    {
        public override bool IsPhysical => false;
		public virtual bool IsInitialized { get; set; } = false;
		public virtual bool IsUnlocked { get; set; } = false;
		public abstract string DealerId { get; }
		public new abstract string FirstName { get;		 }
		public new abstract string LastName { get; }
		public virtual string DisplayName => $"{FirstName} {LastName}";
		public virtual string Image => $"{DealerId}.png";
		public abstract int Tier { get; }
		public abstract List<UnlockRequirement> UnlockRequirements { get; protected set; }
		public abstract List<string> DealDays { get; protected set; }
		public abstract bool CurfewDeal { get; protected set; }
		public abstract List<List<float>> Deals { get; protected set; } //	Each inner list: { dealTime, dealTimeMultipler, dollarPenalty, RepPenalty }
		public abstract int RefreshCost { get; protected set; }
		public abstract DealerReward Reward { get; protected set; }
		public abstract float RepLogBase { get; protected set; }
		public abstract List<Drug> Drugs { get; protected set; }
		public abstract List<Shipping> Shippings { get; protected set; }
		public virtual Dialogue EmpireDialogue { get; protected set; } = new Dialogue();
		public abstract DebtManager? DebtManager { get; set; }
		public RewardManager? RewardManager { get; protected set; }
		public virtual Gift? Gift { get; protected set; }
		public virtual Debt? Debt { get; protected set; }


		[SaveableField("DealerSaveData")]
		protected DealerSaveData _DealerData = new DealerSaveData();
		public DealerSaveData DealerSaveData => _DealerData;

		protected Sprite? _npcSprite;		

		protected virtual void OnEmpireCreated() { }
		protected virtual void OnEmpireLoaded() { }

		public virtual Sprite? GetNPCSprite()
		{
			if (_npcSprite != null)
				return _npcSprite;

			try
			{
				_npcSprite = EmpireResourceLoader.LoadEmbeddedIcon(Image) ?? EmpireResourceLoader.LoadEmbeddedIcon("fallback-dealer.png");
			}
			catch (Exception e)
			{
				MelonLogger.Error($"❌ Error loading NPC sprite for {Image}: {e}");
				_npcSprite = EmpireResourceLoader.LoadEmbeddedIcon("fallback-dealer.png");
			}

			return _npcSprite;
		}

		protected override void ConfigurePrefab(NPCPrefabBuilder builder)
		{
			builder.WithIdentity(DealerId, FirstName, LastName)
				   .WithIcon(GetNPCSprite());
		}

	protected override void OnCreated()
	{
		base.OnCreated();
		OnEmpireCreated();
		MelonLogger.Msg($"🆕 Created Empire NPC: {DisplayName} (ID: {DealerId})");

		Contacts.RegisterEmpireNPC(this);
		MelonLogger.Msg($"Registered Empire NPC '{DisplayName}' with Contacts.");
		
		// Send intro message in OnCreated instead of OnLoaded
		// This ensures it works both on first creation AND after save/load
		if (!DealerSaveData.IsInitialized)
		{
			SendTextMessage("Hi, this is just a message to let you know I'm here, and I'm watching you.  Don't screw up.  See you on the way up.");
			DealerSaveData.IsInitialized = true;
		}
	}

	protected override void OnLoaded()
	{
		base.OnLoaded();
		OnEmpireLoaded();

		// Intro message moved to OnCreated to fix save/load issues
		MelonLogger.Msg($"📂 Loaded Empire NPC: {DisplayName} (ID: {DealerId})");			
	}

		public void IncreaseCompletedDeals(int amount)
		{
			_DealerData.DealsCompleted += amount;
			MelonLogger.Msg($"✅ {DisplayName}'s completed deals increased by {amount}. Total Completed Deals: {DealerSaveData.DealsCompleted}");
		}
		public void GiveReputation(int amount)
		{
			_DealerData.Reputation += amount;
			//if reputation <1 make it 1
			if (_DealerData.Reputation < 1)
			{
				_DealerData.Reputation = 1;
			}
			MelonLogger.Msg($"✅ {DisplayName}'s reputation increased by {amount}. New Reputation: {DealerSaveData.Reputation}");
		}

		//Send the message to the player using the phone app or return the message string only if returnMessage is true
		public string? SendCustomMessage(string message, string product = "", int amount = 0, string quality = "", List<string>? necessaryEffects = null, List<string>? optionalEffects = null, int dollar = 0, bool returnMessage = false, int index = -1)
		{
			// Use case-insensitive property lookup to avoid simple casing mismatches
			//var prop = Dialogue.GetType().GetProperty(messageType, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
			//List<string>? messages = prop?.GetValue(Dialogue) as List<string>;

			// If messages is null or empty, log and fallback
			//if (messages == null || messages.Count == 0)
			//{
			//	MelonLogger.Msg($"❌ Message type '{messageType}' not found in Dialogue or contains no lines.");
			//	if (returnMessage)
			//	{
			//		return messageType;
			//	}
			//	else
			//	{
			//		SendTextMessage($"Message Failed - no message found for {messageType}");    //	keep it for debugging purposes
			//		return null;
			//	}
			//}

			string line = message;
			// Safe random selection (messages.Count > 0 guaranteed here)
			//if (index < 0 || index >= messages.Count)
			//{
			//	line = messages[RandomUtils.RangeInt(0, messages.Count)];
			//}
			//else
			//{
			//	line = messages[index];
			//}

			string qualityColor = "#FFFFFF"; // fallback

			var info = quality?.GetQuality();
			if (info != null)
				qualityColor = info.Color;

			string formatted = line
				.Replace("{product}", $"<color=#FF0004>{product}</color>")
				.Replace("{amount}", $"<color=#FF0004>{amount}x</color>")
				.Replace("{quality}", $"<color={qualityColor}>{(string.IsNullOrWhiteSpace(quality) ? "unknown" : quality)}</color>")
				.Replace("{dollar}", $"<color=#FF0004>{dollar}</color>");

			if (necessaryEffects != null && necessaryEffects.Count > 0)
			{
				string requiredEffects = string.Join(", ", necessaryEffects.Select(e => $"<color=#FF0004>{e}</color>"));
				formatted = formatted.Replace("{effects}", requiredEffects);
			}
			else
			{
				formatted = formatted.Replace("{effects}", "none");
			}

			string effects = (optionalEffects != null && optionalEffects.Count > 0)
				? string.Join(", ", optionalEffects.Select(e => $"<color=#00FFFF>{e}</color>"))
				: "none";

			formatted = formatted.Replace("{optionalEffects}", effects);

			//if (messageType.Equals("accept", StringComparison.OrdinalIgnoreCase))
			//{
			//	if (CurfewDeal)
			//		formatted += "\nRemember that we only accept packages under cover of night.";
			//	else
			//		formatted += "\nWe'll take that delivery any time!";
			//}

			if (returnMessage)
			{
				return formatted;
			}
			else
			{
				SendTextMessage(formatted);
				return null;
			}
		}

		//	DialogueType enum overload to avoid reflection
		public string? SendCustomMessage(
			DialogueType type,
			string product = "",
			int amount = 0,
			string quality = "",
			List<string>? necessaryEffects = null,
			List<string>? optionalEffects = null,
			int dollar = 0,
			bool returnMessage = false,
			int index = -1)
		{
			List<string>? messages = GetDialogueLines(type);

			// Check FIRST, log SECOND - Fix for null/empty list access bug
			if (messages == null || messages.Count == 0)
			{
				MelonLogger.Warning($"❌ DialogueType '{type}' has no lines for {DisplayName}.");
				if (returnMessage)
					return type.ToString();

				SendTextMessage($"[{DisplayName}]: Message unavailable ({type})");
				return null;
			}

			MelonLogger.Msg($"Retrieved {messages.Count} dialogue lines for {type} from {DisplayName}");

			// Pick a line
			string line = (index < 0 || index >= messages.Count)
				? messages[RandomUtils.RangeInt(0, messages.Count)]
				: messages[index];

			// Formatting
			string qualityColor = quality?.GetQuality()?.Color ?? "#FFFFFF";

			string formatted = line
				.Replace("{product}", $"<color=#FF0004>{product}</color>")
				.Replace("{amount}", $"<color=#FF0004>{amount}x</color>")
				.Replace("{quality}", $"<color={qualityColor}>{(string.IsNullOrWhiteSpace(quality) ? "unknown" : quality)}</color>")
				.Replace("{dollar}", $"<color=#FF0004>{dollar}</color>");

			if (necessaryEffects != null && necessaryEffects.Count > 0)
			{
				string requiredEffects = string.Join(", ", necessaryEffects.Select(e => $"<color=#FF0004>{e}</color>"));
				formatted = formatted.Replace("{effects}", requiredEffects);
			}
			else
			{
				formatted = formatted.Replace("{effects}", "none");
			}

			string effects = (optionalEffects != null && optionalEffects.Count > 0)
				? string.Join(", ", optionalEffects.Select(e => $"<color=#00FFFF>{e}</color>"))
				: "none";

			formatted = formatted.Replace("{optionalEffects}", effects);

			// Special case for Accept
			if (type == DialogueType.Accept)
			{
				if (CurfewDeal)
					formatted += "\nRemember that we only accept packages under cover of night.";
				else
					formatted += "\nWe'll take that delivery any time!";
			}

			MelonLogger.Msg($"Sending? returnMessage={returnMessage}, formatted='{formatted}'");

			if (returnMessage)
				return formatted;

			SendTextMessage(formatted);
			return null;
		}

		/// <summary>
		/// Safely sends a text message with validation to prevent empty/null messages.
		/// </summary>
		/// <param name="message">The message to send</param>
		/// <param name="fallbackContext">Context information for logging if message is invalid</param>
		protected void SendTextMessageSafe(string? message, string fallbackContext = "")
		{
			if (string.IsNullOrWhiteSpace(message))
			{
				MelonLogger.Warning($"[{DisplayName}] Attempted to send empty message. Context: {fallbackContext}");
				return;
			}
			SendTextMessage(message);
		}

		public string GetDrugUnlockInfo()
		{
			var info = new System.Text.StringBuilder();

			info.AppendLine(
				$"<b>Dealer: {DisplayName}</b> " +
				$"(Reputation: <color=#FFFFFF>{DealerSaveData.Reputation}</color>)");

			foreach (var drug in Drugs)
			{
				info.AppendLine($"<b>• {drug.Type}</b> - {Status(drug.UnlockRep)}");

				// Qualities
				if (drug.Qualities?.Count > 0)
				{
					info.AppendLine("  Qualities:");

					foreach (var quality in drug.Qualities)
					{
						var qInfo = quality.Type.GetQuality();
						float effective = quality.DollarMult + (qInfo?.DollarMult ?? 0f);

						info.AppendLine(
							$"    - {quality.Type} " +
							$"(x<color=#00FFFF>{effective:F2}</color>) : {Status(quality.UnlockRep)}");
					}
				}

				// Effects
				if (drug.Effects?.Count > 0)
				{
					info.AppendLine("  Effects:");

					foreach (var effect in drug.Effects)
					{
						var eInfo = effect.Name.GetEffect();
						float effective = effect.DollarMult + (eInfo?.DollarMult ?? 0f);

						info.AppendLine(
							$"    - {effect.Name} " +
							$"(Prob <color=#FFA500>{effect.Probability:F2}</color>, " +
							$"x<color=#00FFFF>{effective:F2}</color>) : {Status(effect.UnlockRep)}");
					}
				}

				info.AppendLine();
			}

			return info.ToString();
		}

		private string Status(int unlockRep)
		{
			return unlockRep <= DealerSaveData.Reputation ? "<color=#00FF00>Unlocked</color>" : $"<color=#FF4500>Locked (Unlock at: {unlockRep})</color>";
		}

		//A method that upgrades ShippingTier to the next available shipping option
		public bool UpgradeShipping()
		{
			if (_DealerData.ShippingTier < Shippings.Count - 1)
			{
				_DealerData.ShippingTier++;
				MelonLogger.Msg($"✅ Shipping upgraded to tier {_DealerData.ShippingTier}.");
				return true;
			}
			else
			{
				MelonLogger.Msg($"⚠️ Shipping already at max tier {_DealerData.ShippingTier}.");
				return false;
			}
		}

		//A method to check if the new reputation unlocks any new drug, quality or effects for the dealer 
		public void UnlockDrug()
		{
			// Initialize the drug list
			var drugList = Drugs ?? new List<Drug>();

			// Filter drugs based on the current reputation and use null-coalescing for qualities and effects
			var validDrugs = drugList
				.Where(d => d.UnlockRep <= _DealerData.Reputation) // Unlock drugs with UnlockRep <= current reputation
				.Select(d => new Drug
				{
					Type = d.Type,
					UnlockRep = d.UnlockRep,
					BaseDollar = d.BaseDollar,
					BaseRep = d.BaseRep,
					BaseXp = d.BaseXp,
					RepMult = d.RepMult,
					XpMult = d.XpMult,
					// Unlock qualities where UnlockRep <= current reputation safely
					Qualities = (d.Qualities?.Where(q => q.UnlockRep <= DealerSaveData.Reputation).ToList()) ?? new List<Quality>(),
					// Unlock effects where UnlockRep <= current reputation safely
					Effects = (d.Effects?.Where(e => e.UnlockRep <= DealerSaveData.Reputation).ToList()) ?? new List<Effect>()
				})
				.ToList();

			// Update the DealerSaveData with the unlocked drugs
			DealerSaveData.UnlockedDrugs = validDrugs;

			// Log the unlocked drugs for debugging
			MelonLogger.Msg($"   Found {validDrugs.Count} drug(s) unlocked for dealer '{DisplayName}' at rep {DealerSaveData.Reputation}.");
			foreach (var drug in validDrugs)
			{
				MelonLogger.Msg($"      Drug: {drug.Type} | UnlockRep: {drug.UnlockRep}");
				MelonLogger.Msg($"         Unlocked Qualities: {string.Join(", ", drug.Qualities.Select(q => q.Type))}");
				MelonLogger.Msg($"         Unlocked Effects: {string.Join(", ", drug.Effects.Select(e => e.Name))}");
			}
		}

		private List<string>? GetDialogueLines(DialogueType type)
		{
			var d = EmpireDialogue; // your buyer’s Dialogue object

			if (d == null)
			{
				MelonLogger.Msg($"❌ EmpireDialogue is null for dealer {DisplayName}.");
				return null;
			}

			MelonLogger.Msg($"[GetDialogueLines] Getting dialogue lines for {type} from dealer {DisplayName}.");

			return type switch
			{
				DialogueType.Intro => d.Intro,
				DialogueType.DealStart => d.DealStart,
				DialogueType.Accept => d.Accept,
				DialogueType.Incomplete => d.Incomplete,
				DialogueType.Expire => d.Expire,
				DialogueType.Fail => d.Fail,
				DialogueType.Success => d.Success,
				DialogueType.Reward => d.Reward,
				_ => null
			};
		}
	}
}
