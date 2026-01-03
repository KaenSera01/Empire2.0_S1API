using Empire.DebtHelpers;
using Empire.NPC.Data.Enums;
using Empire.NPC.S1API_NPCs;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Empire.NPC
{
    public static class Contacts
    {
        public static Dictionary<string, EmpireNPC> Buyers { get; set; } = new Dictionary<string, EmpireNPC>(); // Key: DealerId, Value: EmpireNPC Buyer
		public static Dictionary<string, EmpireNPC> BuyersByDisplayName { get; set; } = new Dictionary<string, EmpireNPC>(); // Key: DealerId, Value: EmpireNPC Buyer
		public static bool IsInitialized { get; set; } = false;
        /// <summary>
        /// True after UpdateCoroutine has finished processing all buyers (unlocking, intros, UnlockDrug calls complete).
        /// Use this to wait before calling LoadQuests to avoid race conditions.
        /// </summary>
        public static bool AreContactsFullyProcessed { get; private set; } = false;

        private static bool _isUpdateCoroutineRunning = false;

		public static readonly List<Type> AllEmpireNPCs = typeof(EmpireNPC)
	        .Assembly
	        .GetTypes()
	        .Where(t => t.IsSubclassOf(typeof(EmpireNPC)) && !t.IsAbstract)
	        .ToList();

		public static void RegisterEmpireNPC(EmpireNPC npc)
		{
            if (AllEmpireNPCs == null)
            {
                MelonLogger.Error("❌ AllEmpireNPCs is null.");
            }
            else
            {
                MelonLogger.Msg($"AllEmpireNPCs count: {AllEmpireNPCs.Count}");
            }

			if (npc == null)
			{
				MelonLogger.Error("❌ Attempted to register a null EmpireNPC.");
				return;
			}

			if (string.IsNullOrEmpty(npc.DealerId))
			{
				MelonLogger.Error("❌ EmpireNPC has no DealerId.");
				return;
			}

			// Prevent duplicate registration
			if (Buyers.ContainsKey(npc.DealerId))
			{
				MelonLogger.Warning($"⚠️ Empire NPC already registered: {npc.DealerId}");
				return;
			}

			// Register
			Buyers[npc.DealerId] = npc;
			BuyersByDisplayName[npc.DisplayName] = npc;
            npc.IsInitialized = true;
			// Note: Do NOT set DealerSaveData.IsInitialized here - it's used to track if intro was sent in OnLoaded()
			// The proper intro dialogue is sent in UpdateCoroutine based on IntroDone flag

			MelonLogger.Msg($"✅ Registered Empire NPC: {npc.DealerId}; Initialized: {npc.IsInitialized}");

            if (AllEmpireNPCs != null)
            {
				if (Buyers.Count >= AllEmpireNPCs.Count)    //  >= just in case
				{
					IsInitialized = true;
					MelonLogger.Msg($"🎉 All Empire NPCs registered ({Buyers.Count}/{AllEmpireNPCs.Count}). Initialization complete.");
					Contacts.Update();
					MelonLogger.Msg("🔄 Contacts Update called after all NPCs registered.");
				}
			}
            else
            {
                MelonLogger.Msg($"AllEmpireNPCs null.  Something borked.");
            }
		}

		public static EmpireNPC? GetBuyer(string dealerName)
        {
			BuyersByDisplayName.TryGetValue(dealerName, out var buyer);
            MelonLogger.Msg($"🔍 GetBuyer called for dealerName: {dealerName}, Found: {buyer != null}");

            return buyer;
        }

        /// <summary>
        /// Reset Contacts static state between scene loads to avoid leaking over the previous session.
        /// </summary>
        public static void Reset()
        {
            Buyers.Clear();
            BuyersByDisplayName.Clear();
            IsInitialized = false;
            AreContactsFullyProcessed = false;
            //IsUnlocked = false;
            //BlackmarketBuyer.dealerDataIndex = 0;
            // Reset the dealer field to force re-initialization
            //BlackmarketBuyer.dealer = null;
            _isUpdateCoroutineRunning = false; // Allow coroutine to be restarted
            MelonLogger.Msg("🧹 Empire Contacts state reset complete");
        }

        /// <summary>
        /// Process all buyers synchronously (unlock, UnlockDrug) and start async intro message sending.
        /// This is called when all NPCs are registered.
        /// </summary>
        public static void Update()
        {
            MelonLogger.Msg("🔄 Contacts.Update() - Starting synchronous buyer processing...");
            
            // Process buyers synchronously - this is critical for LoadQuests to work
            ProcessBuyersSynchronously();
            
            // Mark as fully processed so LoadQuests can run
            AreContactsFullyProcessed = true;
            MelonLogger.Msg("✅ Contacts fully processed - buyers unlocked and drugs unlocked.");
            
            // Start async coroutine for sending intro messages (requires CustomNpcsReady)
            if (!_isUpdateCoroutineRunning)
            {
                _isUpdateCoroutineRunning = true;
                MelonCoroutines.Start(SendIntroMessagesCoroutine());
            }
        }
        
        /// <summary>
        /// Synchronously process all buyers: check unlock requirements, set IsUnlocked, call UnlockDrug.
        /// This runs immediately so LoadQuests has the data it needs.
        /// </summary>
        private static void ProcessBuyersSynchronously()
        {
            MelonLogger.Msg($"📋 Processing {Buyers.Count} buyers synchronously...");
            
            foreach (var buyer in Buyers.Values)
            {
                MelonLogger.Msg("Buyer type: " + buyer.GetType().Name);

				try
                {
                    bool canUnlock = !buyer.IsUnlocked &&
                                     (buyer.UnlockRequirements == null ||
                                     !buyer.UnlockRequirements.Any() ||
                                     buyer.UnlockRequirements.All(req =>
                                         GetBuyer(req.Name)?.DealerSaveData.Reputation >= req.MinRep));

                    MelonLogger.Msg($"Buyer: {buyer.DisplayName}, CanUnlock: {canUnlock}");

                    if (canUnlock)
                    {
                        MelonLogger.Msg($"✅ Dealer {buyer.DisplayName} unlock requirements met.");
                        
                        if (!buyer.IsUnlocked)
                        {
                            buyer.IsUnlocked = true;
                            MelonLogger.Msg($"✅ Dealer {buyer.DisplayName} is now unlocked.");

							MelonLogger.Msg($"Buyer Debt Info: Debt is null: {buyer.Debt == null}, buyer.Debt.TotalDebt: {buyer.Debt?.TotalDebt}, DealerSaveData.DebtRemaining: {buyer.DealerSaveData.DebtRemaining}");
							if (buyer.Debt?.TotalDebt > 0 && !buyer.DealerSaveData.DebtInitialized)
                            {
                                buyer.DealerSaveData.DebtRemaining = buyer.Debt.TotalDebt;
                                buyer.DealerSaveData.DebtPaidThisWeek = 0;
                                buyer.DealerSaveData.DebtInitialized = true;
								MelonLogger.Msg($"💰 Dealer {buyer.DisplayName} has existing debt: {buyer.Debt.TotalDebt}");
                            }
                        }

                        if (buyer.Debt != null && buyer.Debt.TotalDebt > 0 && buyer.DealerSaveData.DebtRemaining > 0)
                        {
                            buyer.DebtManager = new DebtManager(buyer);
                            MelonLogger.Msg($"💰 Dealer {buyer.DisplayName} has debt: {buyer.Debt.TotalDebt}");
                        }

                        if (!buyer.IsInitialized)
                        {
                            buyer.IsInitialized = true;
                            MelonLogger.Msg($"✅ Initialized dealer: {buyer.DisplayName}");
                        }
                        
                        // Critical: Unlock drugs so quests can be generated
                        buyer.UnlockDrug();
                    }
                    else
                    {
                        MelonLogger.Msg($"🔒 Dealer {buyer.DisplayName} is locked (unlock requirements not met)");
                    }
                }
                catch (Exception ex)
                {
                    MelonLogger.Error($"❌ Error processing buyer {buyer.DisplayName}: {ex}");
                }
            }
            
            MelonLogger.Msg($"📋 Finished synchronous processing of {Buyers.Count} buyers.");
        }

        /// <summary>
        /// Async coroutine to send intro messages after CustomNpcsReady is true.
        /// This runs separately from the synchronous processing.
        /// </summary>
        private static System.Collections.IEnumerator SendIntroMessagesCoroutine()
        {
            MelonLogger.Msg("📨 SendIntroMessagesCoroutine started - waiting for CustomNpcsReady...");
            
            // Wait for S1API custom NPCs to be ready for messaging
            bool customNpcsReadyInitial = false;
            try
            {
                customNpcsReadyInitial = S1API.Entities.NPC.CustomNpcsReady;
                MelonLogger.Msg($"⏳ CustomNpcsReady initial value: {customNpcsReadyInitial}");
            }
            catch (System.Exception ex)
            {
                MelonLogger.Error($"❌ Failed to access NPC.CustomNpcsReady: {ex.Message}");
                _isUpdateCoroutineRunning = false;
                yield break;
            }
            
            while (!S1API.Entities.NPC.CustomNpcsReady)
            {
                yield return null;
            }
            MelonLogger.Msg("✅ S1API CustomNpcsReady - Now sending intro messages...");
            
            try
            {
                foreach (var buyer in Buyers.Values)
                {
                    // Only send intro to unlocked buyers who haven't received it yet
                    if (buyer.IsUnlocked && buyer.DealerSaveData.IntroDone == false)
                    {
                        try
                        {
                            buyer.SendCustomMessage(DialogueType.Intro);
                            MelonLogger.Msg($"📨 Dealer {buyer.DisplayName} intro sent.");
                            buyer.DealerSaveData.IntroDone = true;
                        }
                        catch (Exception ex)
                        {
                            MelonLogger.Error($"❌ Failed to send intro to {buyer.DisplayName}: {ex}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"❌ Unexpected error during intro messages: {ex}");
            }
            finally
            {
                _isUpdateCoroutineRunning = false;
                MelonLogger.Msg("📨 SendIntroMessagesCoroutine completed.");
            }
        }
    }
}