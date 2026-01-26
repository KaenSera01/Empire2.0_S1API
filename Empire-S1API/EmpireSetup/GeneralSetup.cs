using Empire.Debug;
using Empire.PhoneCalls;
using MelonLoader;
using MelonLoader.Utils;
using Newtonsoft.Json;
using S1API.Entities;
using S1API.Entities.NPCs;
using S1API.Law;
using S1API.Logging;
using S1API.PhoneCalls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Empire.EmpireSetup
{
    public static class GeneralSetup
    {
        public static EmpireSaveData EmpSaveData { get; set; }
        // Basic intro to mod and debt mechanics
        public static void UncCalls()
        {
			//DebugLogger.Log("Unc Calls Method Triggered.");
			//Log if intro has been done
			DebugLogger.Log($"Unc Calls: {EmpSaveData.SaveData.UncNelsonCartelIntroDone}");
            if (!EmpSaveData.SaveData.UncNelsonCartelIntroDone)
            {
                EmpSaveData.SaveData.UncNelsonCartelIntroDone = true;

                // Queue the intro as a phone call with staged dialogue
                var caller = S1API.Entities.NPC.Get<UncleNelson>() as UncleNelson;

                if (caller != null)
                {
                    var call = new UncleNelsonIntroCall(caller);
                    CallManager.QueueCall(call);
                }
            }
        }
        public static void ResetPlayerStats()
        {
            DebugLogger.Log("Resetting Player Stats.");
            S1API.Console.ConsoleHelper.SetPlayerJumpMultiplier(1f);
            S1API.Console.ConsoleHelper.SetPlayerMoveSpeedMultiplier(1f);
            S1API.Console.ConsoleHelper.SetPlayerHealth(100f);
            S1API.Console.ConsoleHelper.SetPlayerEnergyLevel(100f);
            //S1API.Console.ConsoleHelper.SetLawIntensity(1f);
            DebugLogger.Log("Player Stats Reset.");
        }

        public static void Reset()  //  call only on scene unload
		{
            EmpSaveData = new EmpireSaveData();
        }
    }
}
