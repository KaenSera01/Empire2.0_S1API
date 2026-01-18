//using Empire.DebtHelpers;
//using Empire.NPC.Data;
//using Empire.Utilities.JSONHelpers;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Empire.NPC.S1API_NPCs
//{
//	public class RuntimeJsonEmpireNPC : JsonEmpireNPC
//	{
//		public RuntimeJsonEmpireNPC(EmpireNPCJson json) : base(json)
//		{
//		}
//	}

//	public abstract class JsonEmpireNPC : EmpireNPC
//	{
//		private readonly EmpireNPCJson data;

//		protected JsonEmpireNPC(EmpireNPCJson json) : base()
//		{
//			data = json;
//			IsCustomNPC = true;

//			// Unlocking & deal-day structure
//			UnlockRequirements = json.UnlockRequirements;
//			DefaultDealDays = json.DefaultDealDays;
//			ActiveDealDays = new List<string>(json.DefaultDealDays);
//			CurfewDeal = json.CurfewDeal;

//			// Deal parameters
//			Deals = json.Deals;
//			RefreshCost = json.RefreshCost;
//			Reward = json.Reward;
//			RepLogBase = json.RepLogBase;

//			// Inventory & shipping
//			Drugs = json.Drugs;
//			Shippings = json.Shippings;

//			// Dialogue & misc
//			EmpireDialogue = json.EmpireDialogue ?? new Dialogue();
//			Gift = json.Gift;

//			_npcSprite = EmpireResourceLoader.LoadIconFromDisk(Image);
//		}

//		public override string DealerId => data.DealerId;
//		public override string FirstName => data.FirstName;
//		public override string LastName => data.LastName;
//		public override int Tier => data.Tier;

//		public override List<UnlockRequirement> UnlockRequirements { get; protected set; }
//		public override List<string> DefaultDealDays { get; protected set; }
//		public override List<string> ActiveDealDays { get; set; }

//		public override bool CurfewDeal { get; protected set; }
//		public override List<List<float>> Deals { get; protected set; }

//		public override int RefreshCost { get; protected set; }
//		public override DealerReward Reward { get; protected set; }
//		public override float RepLogBase { get; protected set; }

//		public override List<Drug> Drugs { get; protected set; }
//		public override List<Shipping> Shippings { get; protected set; }

//		public override DebtManager? DebtManager { get; set; }
//	}
//}
