using MelonLoader;
using S1API.Internal.Abstraction;
using S1API.Saveables;
using S1API.GameTime;
using Empire.GeneralSetup.Data;
using Core.DebugHandler;

namespace Empire.GeneralSetup
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
            GeneralSetup.EmpireSaveData = this;
            GeneralSetup.UncCalls(); // TODO - shift to proper flow
            TimeManager.OnDayPass -= GeneralSetup.ResetPlayerStats;
            TimeManager.OnDayPass += GeneralSetup.ResetPlayerStats; // TODO - shift to proper flow
        }

        protected override void OnCreated()
        {
            DebugLogger.Log("Empire Save Data Created");
            GeneralSetup.EmpireSaveData = this;
            GeneralSetup.UncCalls(); // TODO - shift to proper flow
            TimeManager.OnDayPass -= GeneralSetup.ResetPlayerStats;
            TimeManager.OnDayPass += GeneralSetup.ResetPlayerStats; // TODO - shift to proper flow
        }


    }
}