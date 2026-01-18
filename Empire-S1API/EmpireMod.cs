using Core.DebugHandler;
using Empire;
using Empire.NPC;
using Empire.Phone;
using MelonLoader;
using S1API.GameTime;

[assembly: MelonInfo(typeof(EmpireMod), "Empire (Forked by Kaen01)", "2.0", "Aracor")]
[assembly: MelonGame("TVGS", "Schedule I")]

namespace Empire
{
    public class EmpireMod : MelonMod
    {
		public static MelonPreferences_Category Config;
		public static MelonPreferences_Entry<bool> RandomizeDealDays;
		public static MelonPreferences_Entry<int> Tier1MinDays;
		public static MelonPreferences_Entry<int> Tier2MinDays;
		public static MelonPreferences_Entry<int> Tier3MinDays;
		public static MelonPreferences_Entry<int> Tier4MinDays;
		public static MelonPreferences_Entry<int> Tier5MinDays;
        public static MelonPreferences_Entry<bool> DisableNecessaryEffects;
        public static MelonPreferences_Entry<float> ExceedQualityBonus;
        public static MelonPreferences_Entry<bool> NoRefreshCost;

		private bool _jsonNpcsInstantiated = false;

		public override void OnInitializeMelon()
        {
            base.OnInitializeMelon();

			TimeManager.OnDayPass -= EmpirePhoneApp.DetermineDealDaysStatic;
			TimeManager.OnDayPass += EmpirePhoneApp.DetermineDealDaysStatic;

			Config = MelonPreferences.CreateCategory("Empire (Forked by Kaen01)", "Empire 2.0 Mod Configuration");

            ExceedQualityBonus = Config.CreateEntry<float>(
                "ExceedQualityBonus",
                0.15f,
                "Exceed Quality Bonus Multiplier",
                "Multiplier applied to the reward for exceeding the requested quality in Empire quests. Default is 0.15 (15%), same as the base game."
            );
            DebugLogger.Log("✅ Loaded setting: ExceedQualityBonus = " + ExceedQualityBonus.Value);

            DisableNecessaryEffects = Config.CreateEntry<bool>(
                         "DisableNecessaryEffects",
                         false,
                         "Disable Necessary Effects",
                         "If enabled, necessary effects for Empire quests will be disabled, making them easier to complete."
                     );
            DebugLogger.Log("✅ Loaded setting: DisableNecessaryEffects = " + DisableNecessaryEffects.Value);

            NoRefreshCost = Config.CreateEntry<bool>(
                "NoRefreshCost",
                false,
                "No Refresh Cost",
                "If enabled, refreshing Empire quests will not cost any money."
            );
            DebugLogger.Log("✅ Loaded setting: NoRefreshCost = " + NoRefreshCost.Value);

			RandomizeDealDays = Config.CreateEntry<bool>(
                "RandomizeDealDays", 
                false,
                "Randomize Deal Days", 
                "If enabled, the days on which deals are available will be randomized each week. Must be true for the MinDays (below) to be used."
            );
            DebugLogger.Log("✅ Loaded setting: RandomizeDealDays = " + RandomizeDealDays.Value);

			Tier1MinDays = Config.CreateEntry<int>(
                "Tier1MinDays", 
                4, 
                "Tier 1 Minimum Deal Days",
				"Minimum number of days per week Tier 1 dealers can have deals. Max = Min + 2"
			);
            DebugLogger.Log("✅ Loaded setting: Tier1MinDays = " + Tier1MinDays.Value);

			Tier2MinDays = Config.CreateEntry<int>(
                "Tier2MinDays", 
                3, 
                "Tier 2 Minimum Deal Days",
				"Minimum number of days per week Tier 2 dealers can have deals. Max = Min + 2"
			);
            DebugLogger.Log("✅ Loaded setting: Tier2MinDays = " + Tier2MinDays.Value);

			Tier3MinDays = Config.CreateEntry<int>(
                "Tier3MinDays", 
                2, 
                "Tier 3 Minimum Deal Days",
				"Minimum number of days per week Tier 3 dealers can have deals. Max = Min + 2"
			);
            DebugLogger.Log("✅ Loaded setting: Tier3MinDays = " + Tier3MinDays.Value);

			Tier4MinDays = Config.CreateEntry<int>(
                "Tier4MinDays", 
                1, 
                "Tier 4 Minimum Deal Days",
				"Minimum number of days per week Tier 4 dealers can have deals. Max = Min + 2"
			);
            DebugLogger.Log("✅ Loaded setting: Tier4MinDays = " + Tier4MinDays.Value);

			Tier5MinDays = Config.CreateEntry<int>(
                "Tier5MinDays", 
                1, 
                "Tier 5 Minimum Deal Days",
				"Minimum number of days per week Tier 5 dealers can have deals. Max = Min + 2"
			);
            DebugLogger.Log("✅ Loaded setting: Tier5MinDays = " + Tier5MinDays.Value);

			MelonPreferences.Save();

            MelonLogger.Msg("✅ Empire Mod initialized!");
			DebugLogger.Log("📦 Loading Empire JSON NPC definitions...");
			//EmpireJsonLoader.LoadAllNpcDtos();
		}

		public override void OnSceneWasUnloaded(int buildIndex, string sceneName)
        {
            if (sceneName == "Main")
            {
                DebugLogger.Log("🧹 Resetting Empire static state after Main scene unload");
				if (EmpirePhoneApp.Instance != null) 
                    EmpirePhoneApp.Reset();

                Contacts.Reset();
				TimeManager.OnDayPass -= EmpirePhoneApp.DetermineDealDaysStatic;
			}
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
   //         if (sceneName == "Main")
   //         {
			//	if (!_jsonNpcsInstantiated)
			//	{
			//		_jsonNpcsInstantiated = true;

			//		DebugLogger.Log("🔧 Instantiating JSON NPCs now that S1API + FishNet are ready...");
			//		EmpireJsonLoader.InstantiateAllJsonNpcs();
			//	}
			//}
		}

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            if (sceneName == "Main")
            {
				// Also reset on initialization to be safe
				DebugLogger.Log("🧹 Resetting Empire static state after Main scene initialization");
                if (EmpirePhoneApp.Instance != null)
                    EmpirePhoneApp.Reset();
                
                Contacts.Reset();
				TimeManager.OnDayPass -= EmpirePhoneApp.DetermineDealDaysStatic;
				TimeManager.OnDayPass += EmpirePhoneApp.DetermineDealDaysStatic;
			}
        }

        public override void OnApplicationQuit()
        {
            base.OnApplicationQuit();

			TimeManager.OnDayPass -= EmpirePhoneApp.DetermineDealDaysStatic;
		}
	}
}