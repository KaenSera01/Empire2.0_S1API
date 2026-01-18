using Core.DebugHandler;
using Empire.DebtHelpers;
using Empire.NPC.Data.Enums;
using Empire.NPC.S1API_NPCs;
using Empire.Phone;
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
                DebugLogger.LogError("❌ AllEmpireNPCs is null.");
            }
            else
            {
				DebugLogger.Log($"AllEmpireNPCs count: {AllEmpireNPCs.Count}");
            }

			if (npc == null)
			{
				DebugLogger.LogError("❌ Attempted to register a null EmpireNPC.");
				return;
			}

			if (string.IsNullOrEmpty(npc.DealerId))
			{
				DebugLogger.LogError("❌ EmpireNPC has no DealerId.");
				return;
			}

			// Prevent duplicate registration
			if (Buyers.ContainsKey(npc.DealerId))
			{
				DebugLogger.LogWarning($"⚠️ Empire NPC already registered: {npc.DealerId}");
				return;
			}

			// Register
			Buyers[npc.DealerId] = npc;
			BuyersByDisplayName[npc.DisplayName] = npc;
            npc.IsInitialized = true;
			// Note: Do NOT set DealerSaveData.IsInitialized here - it's used to track if intro was sent in OnLoaded()
			// The proper intro dialogue is sent in UpdateCoroutine based on IntroDone flag

			DebugLogger.Log($"✅ Registered Empire NPC: {npc.DealerId}; Initialized: {npc.IsInitialized}");

            if (AllEmpireNPCs != null)
            {
				if (Buyers.Count >= AllEmpireNPCs.Count)    //  >= just in case
				{
					IsInitialized = true;
					MelonLogger.Msg($"🎉 All Empire 2.0 NPCs registered ({Buyers.Count}/{AllEmpireNPCs.Count}). Initialization complete.");
					Contacts.Update();
					DebugLogger.Log("🔄 Contacts Update called after all NPCs registered.");
                    EmpirePhoneApp.DetermineDealDaysStatic();
                    DebugLogger.Log("📱 EmpirePhoneApp.DetermineDealDaysStatic() called.");
				}
			}
            else
            {
                DebugLogger.Log($"AllEmpireNPCs null.  Something borked.");
            }
		}

		public static EmpireNPC? GetBuyer(string dealerName)
        {
			BuyersByDisplayName.TryGetValue(dealerName, out var buyer);
            DebugLogger.Log($"🔍 GetBuyer called for dealerName: {dealerName}, Found: {buyer != null}");

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
            DebugLogger.Log("🧹 Empire Contacts state reset complete");
        }

        /// <summary>
        /// Process all buyers synchronously (unlock, UnlockDrug) and start async intro message sending.
        /// This is called when all NPCs are registered.
        /// </summary>
        public static void Update()
        {
            DebugLogger.Log("🔄 Contacts.Update() - Starting synchronous buyer processing...");
            
            // Process buyers synchronously - this is critical for LoadQuests to work
            ProcessBuyersSynchronously();
            
            // Mark as fully processed so LoadQuests can run
            AreContactsFullyProcessed = true;
            DebugLogger.Log("✅ Contacts fully processed - buyers unlocked and drugs unlocked.");
            
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
            DebugLogger.Log($"📋 Processing {Buyers.Count} buyers synchronously...");
            
            foreach (var buyer in Buyers.Values)
            {
                DebugLogger.Log("Buyer type: " + buyer.GetType().Name);

				try
                {
                    bool canUnlock = !buyer.IsUnlocked &&
                                     (buyer.UnlockRequirements == null ||
                                     !buyer.UnlockRequirements.Any() ||
                                     buyer.UnlockRequirements.All(req =>
                                         GetBuyer(req.Name)?.DealerSaveData.Reputation >= req.MinRep));

                    DebugLogger.Log($"Buyer: {buyer.DisplayName}, CanUnlock: {canUnlock}");

                    if (canUnlock)
                    {
                        DebugLogger.Log($"✅ Dealer {buyer.DisplayName} unlock requirements met.");
                        
                        if (!buyer.IsUnlocked)
                        {
                            buyer.IsUnlocked = true;
                            DebugLogger.Log($"✅ Dealer {buyer.DisplayName} is now unlocked.");

							DebugLogger.Log($"Buyer Debt Info: Debt is null: {buyer.Debt == null}, buyer.Debt.TotalDebt: {buyer.Debt?.TotalDebt}, DealerSaveData.DebtRemaining: {buyer.DealerSaveData.DebtRemaining}");
							if (buyer.Debt?.TotalDebt > 0 && !buyer.DealerSaveData.DebtInitialized)
                            {
                                buyer.DealerSaveData.DebtRemaining = buyer.Debt.TotalDebt;
                                buyer.DealerSaveData.DebtPaidThisWeek = 0;
                                buyer.DealerSaveData.DebtInitialized = true;
								DebugLogger.Log($"💰 Dealer {buyer.DisplayName} has existing debt: {buyer.Debt.TotalDebt}");
                            }

                            EmpirePhoneApp.DetermineDealDaysStatic(buyer);
						}

                        if (buyer.Debt != null && buyer.Debt.TotalDebt > 0 && buyer.DealerSaveData.DebtRemaining > 0)
                        {
                            buyer.DebtManager = new DebtManager(buyer);
                            DebugLogger.Log($"💰 Dealer {buyer.DisplayName} has debt: {buyer.Debt.TotalDebt}");
                        }

                        if (!buyer.IsInitialized)
                        {
                            buyer.IsInitialized = true;
                            DebugLogger.Log($"✅ Initialized dealer: {buyer.DisplayName}");
                        }
                        
                        // Critical: Unlock drugs so quests can be generated
                        buyer.UnlockDrug();
                    }
                    else
                    {
                        DebugLogger.Log($"🔒 Dealer {buyer.DisplayName} is locked (unlock requirements not met)");
                    }
                }
                catch (Exception ex)
                {
                    DebugLogger.LogError($"❌ Error processing buyer {buyer.DisplayName}: {ex}");
                }
            }
            
            DebugLogger.Log($"📋 Finished synchronous processing of {Buyers.Count} buyers.");
        }

        /// <summary>
        /// Async coroutine to send intro messages after CustomNpcsReady is true.
        /// This runs separately from the synchronous processing.
        /// </summary>
        private static System.Collections.IEnumerator SendIntroMessagesCoroutine()
        {
            DebugLogger.Log("📨 SendIntroMessagesCoroutine started - waiting for CustomNpcsReady...");
            
            // Wait for S1API custom NPCs to be ready for messaging
            bool customNpcsReadyInitial = false;
            try
            {
                customNpcsReadyInitial = S1API.Entities.NPC.CustomNpcsReady;
                DebugLogger.Log($"⏳ CustomNpcsReady initial value: {customNpcsReadyInitial}");
            }
            catch (System.Exception ex)
            {
                DebugLogger.LogError($"❌ Failed to access NPC.CustomNpcsReady: {ex.Message}");
                _isUpdateCoroutineRunning = false;
                yield break;
            }
            
            while (!S1API.Entities.NPC.CustomNpcsReady)
            {
                yield return null;
            }
            DebugLogger.Log("✅ S1API CustomNpcsReady - Now sending intro messages...");

			MelonCoroutines.Start(RefreshAllMessagingIconsDelayed());

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
                            DebugLogger.Log($"📨 Dealer {buyer.DisplayName} intro sent.");
                            buyer.DealerSaveData.IntroDone = true;
                        }
                        catch (Exception ex)
                        {
                            DebugLogger.LogError($"❌ Failed to send intro to {buyer.DisplayName}: {ex}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DebugLogger.LogError($"❌ Unexpected error during intro messages: {ex}");
            }
            finally
            {
                _isUpdateCoroutineRunning = false;
                DebugLogger.Log("📨 SendIntroMessagesCoroutine completed.");
            }
        }

		private static System.Collections.IEnumerator RefreshAllMessagingIconsDelayed()
		{
			yield return new UnityEngine.WaitForSeconds(5f);

			DebugLogger.Log("Refreshing messaging icons for all Empire NPCs...");

			foreach (var buyer in Buyers.Values)
			{
				try
				{
					var sprite = buyer.GetNPCSprite();
					if (sprite != null)
					{
						buyer.Icon = sprite;
						buyer.RefreshMessagingIcons();
					}
				}
				catch (Exception ex)
				{
					DebugLogger.LogWarning($"Failed to refresh icon for {buyer.DisplayName}: {ex.Message}");
				}
			}

			DebugLogger.Log("Messaging icons refresh complete.");
		}
	}
}