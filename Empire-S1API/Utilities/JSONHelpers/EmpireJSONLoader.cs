//using Empire.NPC;
//using Empire.NPC.S1API_NPCs;
//using MelonLoader;
//using MelonLoader.Utils;
//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.IO;

//namespace Empire.Utilities.JSONHelpers
//{
//	public static class EmpireJsonLoader
//	{
//		private static readonly string NpcFolder =
//			Path.Combine(MelonEnvironment.ModsDirectory, "Empire", "NPCs");

//		// Phase 1: store DTOs until S1API is ready
//		public static readonly List<EmpireNPCJson> LoadedNpcDtos = new List<EmpireNPCJson>();

//		// -----------------------------
//		// PHASE 1: Load + validate JSON
//		// -----------------------------
//		public static void LoadAllNpcDtos()
//		{
//			LoadedNpcDtos.Clear();

//			if (!Directory.Exists(NpcFolder))
//			{
//				DebugLogger.LogWarning($"Empire NPC folder not found: {NpcFolder}");
//				return;
//			}

//			var jsonFiles = Directory.GetFiles(NpcFolder, "*.json", SearchOption.AllDirectories);

//			foreach (var file in jsonFiles)
//				TryLoadDto(file);
//		}

//		private static void TryLoadDto(string file)
//		{
//			try
//			{
//				string jsonText = File.ReadAllText(file);
//				var dto = JsonConvert.DeserializeObject<EmpireNPCJson>(jsonText);

//				if (!Validate(dto, file))
//					return;

//				LoadedNpcDtos.Add(dto);
//				DebugLogger.Log($"✔ Loaded NPC JSON DTO: {dto.DealerId}");
//			}
//			catch (Exception ex)
//			{
//				DebugLogger.LogError($"❌ Failed to load NPC JSON '{Path.GetFileName(file)}': {ex.Message}");
//			}
//		}

//		// Multi-error validation
//		private static bool Validate(EmpireNPCJson json, string file)
//		{
//			var errors = new List<string>();

//			if (json == null)
//			{
//				DebugLogger.LogError($"❌ NPC JSON '{Path.GetFileName(file)}' failed: Deserialization returned null");
//				return false;
//			}

//			if (string.IsNullOrWhiteSpace(json.DealerId))
//				errors.Add("Missing DealerId");

//			if (string.IsNullOrWhiteSpace(json.FirstName))
//				errors.Add("Missing FirstName");

//			if (string.IsNullOrWhiteSpace(json.LastName))
//				errors.Add("Missing LastName");

//			if (json.Tier <= 0)
//				errors.Add("Tier must be >= 1");

//			if (json.DefaultDealDays == null || json.DefaultDealDays.Count == 0)
//				errors.Add("DefaultDealDays is required and cannot be empty");

//			if (json.Drugs == null || json.Drugs.Count == 0)
//				errors.Add("Drugs list is required and cannot be empty");

//			if (json.Deals == null || json.Deals.Count == 0)
//				errors.Add("Deals list is required and cannot be empty");

//			if (json.Shippings == null || json.Shippings.Count == 0)
//				errors.Add("Shippings list is required and cannot be empty");

//			if (errors.Count == 0)
//				return true;

//			DebugLogger.LogError($"❌ NPC JSON '{Path.GetFileName(file)}' failed validation:");
//			foreach (var err in errors)
//				DebugLogger.LogError($"   • {err}");

//			return false;
//		}

//		// -----------------------------
//		// PHASE 2: Instantiate NPCs
//		// -----------------------------
//		public static void InstantiateAllJsonNpcs()
//		{
//			foreach (var dto in LoadedNpcDtos)
//			{
//				try
//				{
//					var npc = new RuntimeJsonEmpireNPC(dto);
//					Contacts.RegisterEmpireNPC(npc);

//					DebugLogger.Log($"✔ Registered JSON NPC: {npc.DisplayName} ({npc.DealerId})");
//				}
//				catch (Exception ex)
//				{
//					DebugLogger.LogError($"❌ Failed to instantiate JSON NPC '{dto.DealerId}': {ex.Message}");
//				}
//			}
//		}
//	}
//}
