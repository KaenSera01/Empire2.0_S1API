using System;
using System.Linq;
using UnityEngine;
using MelonLoader;
using S1API.Money;
using S1API.Storages;
using S1API.DeadDrops;
using S1API.Quests;
using S1API.Products;
using S1API.Saveables;
using System.Collections.Generic;
using S1API.Console;
using S1API.GameTime;
using S1API.Utils;
using S1API.Quests.Constants;
using Empire.Phone;
using Empire.NPC;
using Empire.NPC.S1API_NPCs;
using Empire.Quest.Data;
using S1Quest = S1API.Quests.Quest;
using Empire.Utilities.QualityHelpers;
using Empire.NPC.Data.Enums;
using Empire.Debug;

namespace Empire.Quest
{
    public class QuestDelivery : S1Quest
	{
        [SaveableField("DeliveryData")]
        public DeliverySaveData Data = new DeliverySaveData();

        public EmpireNPC buyer;
        private DeadDropInstance deliveryDrop;
        private StorageInstance? subscribedStorage;
        public QuestEntry deliveryEntry;
        public QuestEntry rewardEntry;
        public static bool QuestActive = false;
        public static QuestDelivery? Active { get; internal set; }

        public QuestEntry GetDeliveryEntry() => deliveryEntry;
        public QuestEntry GetRewardEntry() => rewardEntry;
        public void ForceCancel()
        {
            DebugLogger.Log("🚫 QuestDelivery.ForceCancel() called.");

            GiveReward("Failed");
            if (deliveryEntry != null && deliveryEntry.State != QuestState.Completed)
                deliveryEntry.SetState(QuestState.Expired);
            if (rewardEntry != null && rewardEntry.State != QuestState.Completed)
                rewardEntry.SetState(QuestState.Failed);
            QuestActive = false;
            Active = null; // 👈 Reset after cancel
            Fail();            
        }

        public void Cleanup()
        {
            DebugLogger.Log("🚫 QuestDelivery.Cleanup() called.");
            CleanupSubscriptions();
            DebugLogger.Log("Calling");
            Fail();
            QuestActive = false;
            Active = null; // 👈 Reset after cancel
        }

        private void ExpireCountdown()
        {
            
            // Reduce the quest time by 1 day
            DebugLogger.Log($"ExpireCountdown called. DealTime before: {Data.DealTime}");
            Data.DealTime -= 1;
            DebugLogger.Log($"DealTime after: {Data.DealTime}");
            // Update the delivery entry description with the new time
            deliveryEntry.Title = $"{Data.Task} at the {Colorize(deliveryDrop.Name, DropNameColor)}. Expiry: {Colorize(Data.DealTime.ToString(), DealTimeColor)} Days";


            // Check if the quest time has expired and rewardEntry is not active
            if (Data.DealTime <= 0 && rewardEntry.State!=QuestState.Active)
            {
                // If the quest time has expired, fail the quest
                
                GiveReward("Expired");
                if (deliveryEntry != null && deliveryEntry.State != QuestState.Completed)
                    deliveryEntry.SetState(QuestState.Expired);
                if (rewardEntry != null && rewardEntry.State != QuestState.Completed)
                    rewardEntry.SetState(QuestState.Expired);
                QuestActive = false;
                Active = null; // 👈 Reset after cancel
                Expire();
                
            }
        }

        protected override Sprite? QuestIcon
        {
            get
            {
                //Use static image setting from PhoneApp Accept Quest
                //return ImageUtils.LoadImage(Data.QuestImage ?? Path.Combine(MelonEnvironment.ModsDirectory, "Empire", "EmpireIcon_quest.png"));
				return EmpireResourceLoader.LoadEmbeddedIcon(Data.QuestImage ?? "EmpireIcon_quest.png");

			}
        }

        protected override void OnLoaded()
        {
            DebugLogger.Log("Quest OnLoaded called.");
            base.OnLoaded();
            MelonCoroutines.Start(WaitForBuyerAndLoad());
            
            DebugLogger.Log($"Quest OnLoaded() done.");
        }

        private System.Collections.IEnumerator WaitForBuyerAndLoad()
        {
            float timeout = 5f;
            float waited = 0f;
            DebugLogger.Log("Quest-WaitForBuyerAndLoad-Waiting for buyer to be initialized...");
            // while (Contacts.Buyers == null OR For all key value pairs in Contacts.Buyers, check if the value.IsInitialized is false for at least one of them OR waited < timeqout)
            while (!Contacts.IsInitialized && waited < timeout)
            {
                waited += Time.deltaTime;
                yield return null; // wait 1 frame
            }
            if (!Contacts.IsInitialized)
            {
                DebugLogger.LogWarning("⚠️ Buyer NPCs still not initialized after timeout. Skipping status sync.");
                yield break;
            }
        }

        protected override void OnCreated()
        {
            DebugLogger.Log("Quest OnCreated called.");
            base.OnCreated();
            DebugLogger.Log($"QuestOnCreated() done.");
            
            // Ensure any previously active QuestDelivery cleans up its subscriptions
            if (Active != null && Active != this)
                Active.CleanupSubscriptions();

            buyer = Contacts.GetBuyer(Data.DealerName);
            ConsoleHelper.SetLawIntensity((float)2*buyer.Tier);// TODO - Expose Deal Heat thru JSON
            QuestActive = true;
            Active = this;
            TimeManager.OnDayPass -= ExpireCountdown; // Remove any previous subscription
            TimeManager.OnDayPass += ExpireCountdown; // Add a single subscription
            if (!Data.Initialized)
            {
                var drops = DeadDropManager.All?.ToList();
                if (drops == null || drops.Count < 1)
                {
                    DebugLogger.LogError("❌ Not enough dead drops to assign delivery/reward.");
                    return;
                }

                deliveryDrop = drops[RandomUtils.RangeInt(0, drops.Count)];
                Data.DeliveryDropGUID = deliveryDrop.GUID;
                Data.Initialized = true;
            }
            else
            {
                deliveryDrop = DeadDropManager.All.FirstOrDefault(d => d.GUID == Data.DeliveryDropGUID);
            }
            deliveryEntry = AddEntry($"{Data.Task} at the {Colorize(deliveryDrop.Name, DropNameColor)}. Expiry: {Colorize(Data.DealTime.ToString(), DealTimeColor)} Days");
            deliveryEntry.POIPosition = deliveryDrop.Position;
            deliveryEntry.Begin();

            rewardEntry = AddEntry($"Wait for the payment to arrive.");
            DebugLogger.Log("📦 Setting rewardEntry state to Inactive.");
            rewardEntry.SetState(QuestState.Inactive);

            // Ensure we don't add duplicate subscriptions. Remove any existing handlers first.
            if (deliveryDrop?.Storage != null)
            {
                DebugLogger.Log($"Subscribing CheckDelivery for quest {Data.DealerName} on storage {deliveryDrop.GUID}");
                subscribedStorage = deliveryDrop.Storage;
                subscribedStorage.OnClosed -= CheckDelivery;
                subscribedStorage.OnClosed += CheckDelivery;
            }
            else
            {
                DebugLogger.LogWarning("⚠️ deliveryDrop.Storage is null when attempting to subscribe to OnClosed.");
            }

            DebugLogger.Log("📦 QuestDelivery started with drop locations assigned.");
        }

        private uint PackageAmount(string packaging)
        {
            // Return the amount based on the packaging type - UPDATABLE
            return packaging switch
            {
                "Brick" => 20,
                "Jar" => 5,
                "Baggie" => 1,
                _ => 0,
            };
        }

        /// <summary>
        /// Safely sends a message from the buyer NPC with null checks and error handling.
        /// </summary>
        private void SendBuyerMessage(DialogueType type, string context = "")
        {
            if (buyer == null)
            {
                DebugLogger.LogWarning($"Cannot send {type} message: buyer is null. Context: {context}");
                return;
            }
            
            try
            {
                buyer.SendCustomMessage(type, Data.ProductID, (int)Data.RequiredAmount, 
                    Data.Quality, Data.NecessaryEffects, Data.OptionalEffects, Data.Reward);
            }
            catch (Exception ex)
            {
                DebugLogger.LogError($"Failed to send {type} message from {buyer.DisplayName}: {ex.Message}");
            }
        }

        /// <summary>
        /// Safely sends a custom string message from the buyer NPC with null checks.
        /// </summary>
        private void SendBuyerCustomMessage(string message, string context = "")
        {
            if (buyer == null)
            {
                DebugLogger.LogWarning($"Cannot send custom message: buyer is null. Context: {context}");
                return;
            }
            
            if (string.IsNullOrWhiteSpace(message))
            {
                DebugLogger.LogWarning($"Cannot send empty message from {buyer.DisplayName}. Context: {context}");
                return;
            }
            
            try
            {
                buyer.SendCustomMessage(message);
            }
            catch (Exception ex)
            {
                DebugLogger.LogError($"Failed to send custom message from {buyer.DisplayName}: {ex.Message}");
            }
        }

        private void CheckDelivery()
        {
            if (!QuestActive || Active != this)
            {
                DebugLogger.Log("CheckDelivery ignored: quest not active or not current.");
                // Proactively detach in case cleanup did not run yet for this instance
                if (subscribedStorage != null)
                {
                    subscribedStorage.OnClosed -= CheckDelivery;
                    subscribedStorage = null;
                }
                if (deliveryDrop?.Storage != null)
                {
                    deliveryDrop.Storage.OnClosed -= CheckDelivery;
                }
                return;
            }
            DebugLogger.Log("CheckDelivery called.");
            // Add null checks
            if (deliveryDrop?.Storage?.Slots == null)
            {
                DebugLogger.LogError("❌ Storage or slots are null in CheckDelivery");
                return;
            }
            DebugLogger.Log($"Expecting ProductID: {Data.ProductID}, RequiredAmount: {Data.RequiredAmount}");
            // If buyer.CurfewDeal is true and TimeManager.IsNight is false, return and Log
            if (buyer != null && buyer.CurfewDeal && !TimeManager.IsNight)
            {
                DebugLogger.Log("❌ Curfew deal is true but it is not night. Cannot deliver.");
                //ToDO - Shift to JSON
                SendBuyerCustomMessage("Deliveries only after Curfew.", "curfew check");
                return;
            }

            foreach (var slot in deliveryDrop.Storage.Slots)
            {
                // Add null check for slot
                if (slot?.ItemInstance == null)
                {
                    DebugLogger.LogWarning("⚠️ Encountered null slot or item instance, skipping...");
                    continue;
                }
                bool isProductInstance = slot.ItemInstance is ProductInstance;
                // Add null check and safe cast
                var item = slot.ItemInstance as ProductInstance;
                if (item == null)
                {
                    SendBuyerCustomMessage("This is not even a product...", "invalid product instance");
                    DebugLogger.LogWarning("⚠️ Item is not a ProductInstance, skipping...");
                    continue;
                }
                DebugLogger.Log($"Slot: {item.Definition?.Category} - {slot.Quantity} package - {item.Definition?.Name} ");
                string slotProductID = isProductInstance ? item.Definition?.Name : "null";
                string packaging = isProductInstance ? item.AppliedPackaging?.Name : "null";
                int quantity = slot.Quantity;
                // Add null check for Data.NecessaryEffects
                //if (!EmpireMod.DisableNecessaryEffects.Value)
                //{
                    if (Data?.NecessaryEffects == null)
                    {
                        DebugLogger.LogError("❌ NecessaryEffects is null");
                        return;
                    }
                //}
                ProductDefinition productDef = ProductManager.DiscoveredProducts.FirstOrDefault(p => p.ID == item?.Definition.ID);
                var productType = GetProductType(productDef);

                if (productType != Data.ProductID)
                {
                    DebugLogger.LogError($"❌ Product type mismatch: {productType} != {Data.ProductID}");
                    SendBuyerCustomMessage("This is not the drug type I ordered.", "product type mismatch");
                    continue;
                }

                var props = productDef.Properties; 
                if (productDef is WeedDefinition weed)
                    props = weed.GetProperties();
                else if (productDef is MethDefinition meth)
                    props = meth.GetProperties();
                else if (productDef is CocaineDefinition coke)
                    props = coke.GetProperties();
                else if (productDef is ShroomDefinition shroom)
                    props = shroom.GetProperties();

                DebugLogger.Log($"count : {props.Count}");
                var properties = new List<string>();
                if (props.Count > 0)
                {
                    for (int i = 0; i < props.Count; i++)
                    {
                        var prop = props[i];
                        properties.Add(prop.name.Trim().ToLower());
                    }
                }
                DebugLogger.Log($"Item Properties: {string.Join(", ", properties)}");
                // Melonlogger the Data.NecessaryEffects and OptionalEffects
                DebugLogger.Log($"NecessaryEffects: {string.Join(", ", Data.NecessaryEffects)}");
                DebugLogger.Log($"OptionalEffects: {string.Join(", ", Data.OptionalEffects)}");

                if (!Data.NecessaryEffects.All(effect => properties.Contains(effect.Trim().ToLower())))
                {
                    DebugLogger.LogError($"❌ Effect type mismatch"); 
                    //ToDO - Shift to JSON
                    SendBuyerCustomMessage("All the required necessary effects are not present.", "missing required effects");
                    continue;
                }
                var quality = item?.Quality ?? 0;
                DebugLogger.Log($"Quality: {quality}");
                // convert the quality enum to a lower trim quality string
                string qualityString = quality.ToString().ToLower().Trim();
                int qualityNumber = GetQualityNumber(qualityString);
                // Check if the quality is within the required range after converting quality enum to string.trim.lower
                var exceedQualityNumber = qualityNumber - GetQualityNumber(Data.Quality);
                float exceedQualityBonusMult = 0f;

                if (exceedQualityNumber < 0)
                {
                    DebugLogger.LogError($"❌ Quality mismatch: {quality} < {GetQualityNumber(Data.Quality)} or {quality} > {GetQualityNumber(Data.Quality)}");
                    //ToDO - Shift to JSON
                    SendBuyerCustomMessage("The quality of the product is worse than what I ordered.", "quality too low");
                    continue;
                }
                else
                {
					// Log quality match or exceeds
					exceedQualityBonusMult = exceedQualityNumber * EmpireMod.ExceedQualityBonus.Value;
                    DebugLogger.Log($"✅ Quality match or exceeds. ExceedQualityNumber: {exceedQualityNumber}, BonusMult: {exceedQualityBonusMult}");
				}
				
                if (isProductInstance)
                {
                    uint total = (uint)(quantity * PackageAmount(packaging));
                    if (total <= Data.RequiredAmount)
                    {
                        slot.AddQuantity(-quantity);
                        UpdateReward(total, productDef, properties, exceedQualityBonusMult);
                        Data.RequiredAmount -= total;
                        DebugLogger.Log($"✅ Delivered {total}x {slotProductID} to the stash. Remaining: {Data.RequiredAmount}. Reward now: {Data.Reward}");
                    }
                    else
                    {
                        //FLOOR of the negative of the division to get the number of packages to remove
                        int toRemove = (int)Math.Ceiling((float)Data.RequiredAmount / PackageAmount(packaging));
                        toRemove = Math.Min(toRemove, slot.Quantity);
                        slot.AddQuantity(-toRemove);
                        UpdateReward(Data.RequiredAmount, productDef, properties, exceedQualityBonusMult);
                        Data.RequiredAmount = 0;
                        DebugLogger.Log($"✅ Delivered {total}x {slotProductID} to the stash. Remaining: {Data.RequiredAmount}. Reward now: {Data.Reward}");
                        break;
                    }
                }
            }
            if (Data.RequiredAmount <= 0 && deliveryEntry.State==QuestState.Active)
            {
                SendBuyerMessage(DialogueType.Success, "delivery complete");
                DebugLogger.Log("❌ No required amount to deliver. Quest done.");
                
                deliveryEntry.Complete();
                rewardEntry.SetState(QuestState.Active);
                MelonCoroutines.Start(DelayedReward("Completed"));

            }
            else if (Data.RequiredAmount > 0)
            {
                SendBuyerMessage(DialogueType.Incomplete, "partial delivery");
                DebugLogger.Log($"Continue delivery. Remaining amount: {Data.RequiredAmount}");
            }
        }

        private string? GetProductType(ProductDefinition? productDef)
        {
            if (productDef is WeedDefinition)
            {
                return "weed";
            }
            else if (productDef is MethDefinition)
            {
                return "meth";
            }
            else if (productDef is CocaineDefinition)
            {
                return "cocaine";
            }
            else if (productDef is ShroomDefinition)  //  not implemented yet
            {
                return "shrooms";
            }
            else
            {
                return null;
            }
        }

		//A method that checks type of a product quality. return quality number. Takes arg as Data.quality string and returns Contacts.QualitiesDollarMult index where the key is the quality string.
		//TODO - UPDATABLE
		private int GetQualityNumber(string quality)
		{
			if (string.IsNullOrWhiteSpace(quality))
			{
				DebugLogger.LogError("❌ Quality is null or empty.");
				return -1;
			}

			// Normalize input
			string key = quality.Trim();

			// Try lookup in registry
			if (!QualityRegistry.ByName.TryGetValue(key, out var info))
			{
				DebugLogger.LogError($"❌ Quality not found: {quality}");
				return -1;
			}

			// Return index in the ordered list
			return QualityRegistry.Qualities.IndexOf(info);
		}


		private void UpdateReward(uint total, ProductDefinition? productDef, List<string> properties, float exceedQualityBonusMult)
        {
            // Check if productDef is null or not a ProductDefinition
            if (productDef == null)
            {
                DebugLogger.LogError("❌ Product definition is null or not a ProductDefinition. Reward calculation skipped.");
                return;
            }
            var qualityMult = Data.QualityMult;
            var requiredQuality = GetQualityNumber(Data.Quality);
            // Sum of all in Data.NecessaryEffectMult
            float EffectsSum = Data.NecessaryEffectMult.Sum();
            //  Add Data.OptionalEffectMult[index] to EffectsSum if key is present in properties for Data.OptionalEffects[index]
            for (int i = 0; i < Data.OptionalEffects.Count; i++)
            {
                if (properties.Contains(Data.OptionalEffects[i]))
                {
                    EffectsSum += Data.OptionalEffectMult[i];
                }
            }
            Data.Reward += (int)(total * productDef.MarketValue * (1 + qualityMult) * Data.DealTimeMult * (1 + EffectsSum) * (1 + exceedQualityBonusMult));
            DebugLogger.Log($"   Reward updated: amountProvided: {total}, total reward: {Data.Reward} - Price: {productDef.MarketValue}, Quality: {qualityMult} and EffectsSum: {EffectsSum}, DealTimeMult: {Data.DealTimeMult}, exceedQualityBonusMult: {exceedQualityBonusMult}.");
        }


        //ToDO - Expose Wanted Level through JSON
        //Call with QuestState to be set as string - UPDATABLE
        private System.Collections.IEnumerator DelayedReward(string source)
        {
            // for buyer.Tier -1 times, raise wanted level by 1
            for (int i = 0; i < buyer.Tier - 1; i++)
            {
                ConsoleHelper.RaiseWanted();
                yield return new WaitForSeconds(1f);
            }

            yield return new WaitForSeconds(RandomUtils.RangeInt(5, 10));
            GiveReward(source);
        }

        private void CleanupSubscriptions()
        {
            // Detach storage event
            if (subscribedStorage != null)
            {
                DebugLogger.Log($"Unsubscribing CheckDelivery for quest {Data.DealerName} on storage {deliveryDrop?.GUID}");
                subscribedStorage.OnClosed -= CheckDelivery;
                subscribedStorage = null;
            }
            else if (deliveryDrop?.Storage != null)
            {
                DebugLogger.Log($"Unsubscribing CheckDelivery for quest {Data.DealerName} on storage {deliveryDrop.GUID}");
                deliveryDrop.Storage.OnClosed -= CheckDelivery;
            }

            // Detach time callback
            TimeManager.OnDayPass -= ExpireCountdown;
        }

        private void GiveReward(string source)
        {
            CleanupSubscriptions();
            if (source == "Expired")
            {
                Data.Reward = -Data.Penalties[0];
                Data.RepReward = -Data.Penalties[1];
                Money.ChangeCashBalance(Data.Reward);
                if (buyer != null)
                {
                    buyer.GiveReputation((int)Data.RepReward);
                    SendBuyerMessage(DialogueType.Expire, "quest expired");
                }
            }
            else if (source == "Failed")
            {
                Data.Reward = -Data.Penalties[0];
                Data.RepReward = -Data.Penalties[1];
                Money.ChangeCashBalance(Data.Reward);
                if (buyer != null)
                {
                    buyer.GiveReputation((int)Data.RepReward);
                    SendBuyerMessage(DialogueType.Fail, "quest failed");
                }
            }
            else if (source == "Completed")
            {
                Data.RepReward += (int)(Data.Reward * Data.RepMult);
                Data.XpReward += (int)(Data.Reward * Data.XpMult);
                if (buyer != null)
                {
                    buyer.GiveReputation((int)Data.RepReward);
                    buyer.IncreaseCompletedDeals(1);
                    buyer.UnlockDrug();
                    SendBuyerMessage(DialogueType.Reward, "quest completed");
                }
                ConsoleHelper.GiveXp(Data.XpReward);
                Contacts.Update();
                Complete();
                QuestActive = false;
                Active = null;
                //if buyer.DebtManager does not exist, log and skip debt payment
                if (buyer.DebtManager == null)
                {
                    DebugLogger.Log($"Giving total reward: ${Data.Reward}");
                    Money.ChangeCashBalance(Data.Reward);
                  
                }
                // Pay the reward or debt
                else if (buyer.DealerSaveData.DebtRemaining > 0 && !buyer.DebtManager.paidthisweek)
                {
                    // If debt remaining < reward, set it to 0 and pay the rest
                    if (buyer.DealerSaveData.DebtRemaining <= buyer.Debt.ProductBonus * Data.Reward)
                    {
                        var temp1 = Data.Reward - (int)(buyer.DealerSaveData.DebtRemaining / buyer.Debt.ProductBonus);
                        var temp2 = buyer.DealerSaveData.DebtRemaining;
                        buyer.DealerSaveData.DebtRemaining = 0;
                        DebugLogger.Log($"   Paid off debt to {buyer.DisplayName}");
                        Money.ChangeCashBalance(temp1);
                        if (buyer.DebtManager != null)
                        {
                            buyer.DebtManager.SendDebtMessage((int)temp2, "deal");
                        }
                    }
                    else
                    {
                        DebugLogger.Log($"   Paid off debt: ${Data.Reward} to {buyer.DisplayName}");
                        buyer.DealerSaveData.DebtRemaining -= buyer.Debt.ProductBonus * Data.Reward;
                        buyer.DealerSaveData.DebtPaidThisWeek += buyer.Debt.ProductBonus * Data.Reward;
                        if (buyer.DebtManager != null)
                        {
                            buyer.DebtManager.SendDebtMessage((int) (Data.Reward * buyer.Debt.ProductBonus), "deal");
                        }
                    }
                    
                    if (buyer.DebtManager != null)
                    {
                        buyer.DebtManager.CheckIfPaidThisWeek();
                    }
                }
                else
                {
					DebugLogger.Log($"Giving total reward: ${Data.Reward}");
					Money.ChangeCashBalance(Data.Reward);
                }

            }
            else
            {
                DebugLogger.LogError($"❌ Unknown source: {source}.");
                return;
            }

            DebugLogger.Log($"   Rewarded : ${Data.Reward} and Rep {Data.RepReward} and Xp (if completed) {Data.XpReward} from {Data.DealerName}");

            EmpirePhoneApp.Instance.OnQuestComplete();
            rewardEntry?.Complete();
        }

        protected override string Title =>
            !string.IsNullOrEmpty(Data?.ProductID)
                ? $"Deliver {Data.ProductID} to {Data.DealerName}"
                : "Empire Delivery";

        protected override string Description =>
            !string.IsNullOrEmpty(Data?.ProductID) && Data.RequiredAmount > 0
                ? $"{Data.Task}"
                : "Deliver the assigned product to the stash location.";

        // Add this helper method and color constants to your class (place near the top of QuestDelivery)
        private const string DropNameColor = "00FFCC"; // Teal
        private const string DealTimeColor = "FFD166"; // Warm yellow
        private string Colorize(string text, string hex) => $"<color=#{hex}>{text}</color>";

        }
}
