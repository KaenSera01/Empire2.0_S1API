using System;
using MelonLoader;
using S1API.Internal.Abstraction;
using S1API.Saveables;
using S1API.GameTime;
using Empire.EmpireSetup.Data;
using Empire.Debug;

namespace Empire.EmpireSetup
{
    public class EmpireSaveData : Saveable
    {
        [SaveableField("empire_save_data")]
        public GlobalSaveData SaveData = new GlobalSaveData();

        public EmpireSaveData()
        {
            DebugLogger.Log("Empire Save Data Constructor");
        }

        protected override void OnLoaded()
        {
            DebugLogger.Log("Empire Save Data Loaded");
            GeneralSetup.EmpSaveData = this;
            GeneralSetup.UncCalls(); // TODO - shift to proper flow
            TimeManager.OnDayPass -= GeneralSetup.ResetPlayerStats;
            TimeManager.OnDayPass += GeneralSetup.ResetPlayerStats; // TODO - shift to proper flow
        }

        protected override void OnCreated()
        {
            DebugLogger.Log("Empire Save Data Created");
            GeneralSetup.EmpSaveData = this;
            GeneralSetup.UncCalls(); // TODO - shift to proper flow
            TimeManager.OnDayPass -= GeneralSetup.ResetPlayerStats;
            TimeManager.OnDayPass += GeneralSetup.ResetPlayerStats; // TODO - shift to proper flow
        }


    }
}