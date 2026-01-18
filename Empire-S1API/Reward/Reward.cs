using System;
using System.Linq;
using MelonLoader;
using S1API.GameTime;
using S1API.Console;
using UnityEngine;
using Console = S1API.Console;
using Empire.NPC.S1API_NPCs;
using Core.DebugHandler;

namespace Empire.Reward
{

    public class RewardManager
    {
        EmpireNPC buyer;
        // NEW: Store dealer reward info from the buyer's associated dealer
        public bool isRewardAvailable = false;

        public RewardManager(EmpireNPC buyer)
        {
            this.buyer = buyer;
            isRewardAvailable = true;
            TimeManager.OnDayPass -= SetRewardAvailable;
            TimeManager.OnDayPass += SetRewardAvailable;
        }

        public void SetRewardAvailable()
        {
            isRewardAvailable = true;
        }
        public string GetRewardType()
        {
            if (buyer.Reward?.Args == null || buyer.Reward.Args.Count == 0)
                return "No special favors available from this contact right now.";

            string command = buyer.Reward.Args[0].ToLower();

            switch (command)
            {
                // Camera & UI Controls
                case "freecam": return "Contact can provide surveillance equipment";
                case "hidefps": return "Contact offering to modify your HUD display";

                // Game State
                case "settime": return "Contact knows when to make things happen";
                case "settimescale": return "Contact can help time move differently";

                // Inventory & Money
                case "give":
                case "additemtoinventory": return "Contact has special items to offer";
                case "clearinventory": return "Contact can help you clean house";
                case "changecash": return "Contact offering some quick cash";
                case "changebalance": return "Contact can adjust your digital assets";
                case "addxp": return "Contact willing to share street knowledge";

                // Vehicle
                case "spawnvehicle": return "Contact has connections at the chop shop";

                // Player Abilities
                case "setmovespeed": return "Contact knows ways to enhance your mobility";
                case "setjumpforce": return "Contact has experimental movement tech";
                case "setstaminareserve": return "Contact offering stamina boosters";
                case "teleport": return "Contact knows some shortcuts through the city";

                // Property & Business
                case "setowned": return "Contact can arrange property ownership papers";
                case "packageproduct": return "Contact can help with product packaging";
                case "addemployee": return "Contact knows reliable workers";
                case "setdiscovered": return "Contact can reveal hidden locations";
                case "growplants": return "Contact knows ways to accelerate plant growth";

                // Law Enforcement
                case "raisewanted":
                case "lowerwanted":
                case "clearwanted": return "Contact has influence with law enforcement";
                case "setlawintensity": return "Contact can adjust police patrol intensity";

                // Health & Status
                case "sethealth": return "Contact knows a back-alley medic";
                case "setquality": return "Contact can improve product quality";

                // Relationships & Quests
                case "setquestentrystate": return "Contact can influence ongoing situations";
                case "setemotion": return "Contact knows how to affect people's moods";
                case "setrelationship": return "Contact can influence relationships";
                case "setunlocked": return "Contact can unlock new opportunities";

                // Environment
                case "cleartrash": return "Contact knows clean-up specialists";

                // System Controls
                case "clearbinds": return "Contact offering to adjust your controls";
                case "enable": return "Contact can toggle certain operations";
                case "endtutorial": return "Contact can end your training period";
                case "disablenpcasset": return "Contact can make certain people scarce";

                default:
                    return $"Contact has an undefined favor to offer";
            }

        }
        public void GiveReward()
        {
            
            if (!isRewardAvailable)
            {
                DebugLogger.LogError("Reward is not available right now.");
                return;
            }
            if (buyer.Reward == null || buyer.Reward.Args == null || buyer.Reward.Args.Count == 0 || string.IsNullOrEmpty(buyer.Reward.Type))
            {
                DebugLogger.LogError("No reward available from this contact.");
                return;
            }
            if (buyer.Reward.unlockRep > 0 && buyer.DealerSaveData.Reputation < buyer.Reward.unlockRep)
            {
                DebugLogger.LogError($"Insufficient reputation to claim reward. Required: {buyer.Reward.unlockRep}, Current: {buyer.DealerSaveData.Reputation}");
                return;
            }
            DebugLogger.Log($"Claiming reward: {buyer.Reward.Type} with args: {string.Join(", ", buyer.Reward.Args)}");
            // Deduct reputation cost from current reputation
            buyer.DealerSaveData.Reputation -= buyer.Reward.RepCost;
            isRewardAvailable = false;
            // Start coroutine to execute reward after 10 seconds
            MelonCoroutines.Start(ExecuteRewardAfterDelay());
        }

        private System.Collections.IEnumerator ExecuteRewardAfterDelay()
        {
            DebugLogger.Log("Reward execution will start in 10 seconds...");
            yield return new WaitForSeconds(10); // Wait for 10 seconds

            string rewardType = buyer.Reward.Type.ToLower();
            if (rewardType == "console")
            {
                if (buyer.Reward.Args != null && buyer.Reward.Args.Count > 0)
                {
                    string commandLine = string.Join(" ", buyer.Reward.Args);
                    // Split by "&&"
                    string[] commands = commandLine.Split(new string[] { "&&" }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string rawCmd in commands)
                    {
                        string cmd = rawCmd.Trim();
                        var args = cmd.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
                        int bindonceIdx = args.IndexOf("bindonce");
                        if (bindonceIdx >= 0 && bindonceIdx < args.Count - 1)
                        {
                            string key = args[bindonceIdx + 1];
                            // Replace "bindonce" with "bind"
                            args[bindonceIdx] = "bind";
                            string filteredCmd = string.Join(" ", args);
                            DebugLogger.Log($"Executing console command: {filteredCmd}");
                            ConsoleHelper.Submit(filteredCmd);
                            MelonCoroutines.Start(WaitForBindOnceKey(key));
                        }
                        else
                        {
                            DebugLogger.Log($"Executing console command: {cmd}");
                            ConsoleHelper.Submit(cmd);
                        }
                    }
                }
            }

            DebugLogger.Log("Reward executed successfully.");
        }

        private System.Collections.IEnumerator WaitForBindOnceKey(string key)
        {
            if (!Enum.TryParse<KeyCode>(key, true, out var keyCode))
            {
                DebugLogger.LogError($"Invalid key specified for bindonce: {key}");
                yield break;
            }
            DebugLogger.Log($"Waiting for key press: {keyCode}");
            while (!UnityEngine.Input.GetKeyDown(keyCode))
                yield return null;
            DebugLogger.Log($"Key {keyCode} pressed, unbinding...");
            Console.ConsoleHelper.Submit($"unbind {key}");
        }
    }

}