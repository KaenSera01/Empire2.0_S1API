using Empire.DebtHelpers;
using Empire.NPC.Data;
using System.Collections.Generic;

namespace Empire.NPC.S1API_NPCs
{
	public class HectorSalamanca : EmpireNPC
	{
		public override string DealerId => "hector_salamanca";
		public override string FirstName => "Hector";
		public override string LastName => "Salamanca";
		public override string DisplayName => "Hector Salamanca";
		public override int Tier => 2;

		public override List<UnlockRequirement> UnlockRequirements { get; protected set; } =
			new List<UnlockRequirement>
			{
				new UnlockRequirement { Name = "Domingo 'Krazy-8' Molina", MinRep = 420 }
			};

		public override List<string> DealDays { get; protected set; } =
			new List<string> { "Monday", "Wednesday", "Friday", "Sunday" };

		public override bool CurfewDeal { get; protected set; } = true;

		public override List<List<float>> Deals { get; protected set; } =
			new List<List<float>>
			{
				new List<float> { 2f, 0.9f, 32111f, 88f },
				new List<float> { 4f, 0.7f, 37695f, 117f }
			};

		public override int RefreshCost { get; protected set; } = 500;

		public override DealerReward Reward { get; protected set; } =
			new DealerReward
			{
				unlockRep = 200,
				RepCost = 50,
				Type = "console",
				Args = new List<string> { "lowerwanted" }
			};

		public override float RepLogBase { get; protected set; } = 4f;

		public override List<Drug> Drugs { get; protected set; } =
			new List<Drug>
			{
                // Cocaine
                new Drug
				{
					Type = "cocaine",
					UnlockRep = 0,
					BaseDollar = 77,
					BaseRep = 60,
					BaseXp = 57,
					RepMult = 0.001f,
					XpMult = 0.001f,

					Qualities = new List<Quality>
					{
						new Quality { Type = "poor",     DollarMult = 0f, UnlockRep = 0 },
						new Quality { Type = "standard", DollarMult = 0f, UnlockRep = 120 }
					},

					Effects = new List<Effect>
					{
						new Effect { Name = "Explosive",   UnlockRep = 0,    Probability = 2.0f, DollarMult = 0f },
						new Effect { Name = "Random",      UnlockRep = 230,  Probability = 1.2f, DollarMult = 0f },
						new Effect { Name = "Paranoia",    UnlockRep = 690,  Probability = 1.0f, DollarMult = 0f },
						new Effect { Name = "Random",      UnlockRep = 1270, Probability = 0.3f, DollarMult = 0f },
						new Effect { Name = "Random",      UnlockRep = 1710, Probability = 0.6f, DollarMult = 0f }
					}
				},

                // Meth
                new Drug
				{
					Type = "meth",
					UnlockRep = 560,
					BaseDollar = 42,
					BaseRep = 70,
					BaseXp = 60,
					RepMult = 0.002f,
					XpMult = 0.001f,

					Qualities = new List<Quality>
					{
						new Quality { Type = "poor",     DollarMult = 0f, UnlockRep = 560 },
						new Quality { Type = "standard", DollarMult = 0f, UnlockRep = 890 }
					},

					Effects = new List<Effect>
					{
						new Effect { Name = "Seizure",      UnlockRep = 560,  Probability = 2.0f, DollarMult = 0f },
						new Effect { Name = "Random",       UnlockRep = 1050, Probability = 1.2f, DollarMult = 0f },
						new Effect { Name = "Disorienting", UnlockRep = 1460, Probability = 1.0f, DollarMult = 0f },
						new Effect { Name = "Random",       UnlockRep = 1940, Probability = 0.3f, DollarMult = 0f },
						new Effect { Name = "Random",       UnlockRep = 2210, Probability = 0.6f, DollarMult = 0f }
					}
				}
			};

		public override List<Shipping> Shippings { get; protected set; } =
			new List<Shipping>
			{
				new Shipping
				{
					Name = "Hector's Free Haul",
					Cost = 0,
					UnlockRep = 0,
					MinAmount = 20,
					StepAmount = 1,
					MaxAmount = 40,
					DealModifier = new List<float> { 1f, 1f, 1f, 1f }
				},
				new Shipping
				{
					Name = "Regalo Helado Trucks",
					Cost = 20000,
					UnlockRep = 380,
					MinAmount = 40,
					StepAmount = 5,
					MaxAmount = 80,
					DealModifier = new List<float> { 1.25f, 1.25f, 1.25f, 1.25f }
				},
				new Shipping
				{
					Name = "Casa Tranquila Deliveries",
					Cost = 40000,
					UnlockRep = 2500,
					MinAmount = 50,
					StepAmount = 10,
					MaxAmount = 150,
					DealModifier = new List<float> { 1.5f, 1.5f, 1.5f, 1.5f }
				}
			};

		public override Dialogue EmpireDialogue { get; protected set; } =
			new Dialogue
			{
				Intro = new List<string>
				{
					"*Ding ding ding*"
				},
				DealStart = new List<string>
				{
					"*Ding* ({amount}) *Ding* ({quality}) *Ding* ({product}) *Ding* ({effects}) {optionalEffects}?",
					"*Ding ding* ({amount}) *Ding* ({quality}) *Ding* ({product}). *Ding ding ding* ({effects}) {optionalEffects}!",
					"*Ding* ({amount}) *Ding* ({product})... *Ding ding* ({effects}) {optionalEffects}!"
				},
				Accept = new List<string>
				{
					"*Ding*",
					"*Ding ding*"
				},
				Incomplete = new List<string>
				{
					"*Ding ding ding ding ding*! ({amount}!)",
					"*Angry dinging* ({amount}!)",
					"*Frantic dinging*"
				},
				Expire = new List<string>
				{
					"*Slow, deliberate ding...*",
					"*Silence... then one sharp DING*"
				},
				Fail = new List<string>
				{
					"*Violent, repeated dinging*",
					"DA BOSS CAN SOCK ME!",
					"*Spits* *Ding ding ding*"
				},
				Success = new List<string>
				{
					"*Nods slowly* *Ding*",
					"*Satisfied ding*",
					"*One calm ding*"
				},
				Reward = new List<string>
				{
					"*Points with bell* *Ding* ({dollar})",
					"*Bell tap towards money* ({dollar})",
					"*Ding* (Take it) ({dollar})"
				}
			};

		public override Gift? Gift { get; protected set; } =
			new Gift
			{
				Cost = 2000,
				Rep = 40
			};

		public override Debt? Debt { get; protected set; } =
			new Debt
			{
				TotalDebt = 50000,
				InterestRate = 0.1f,
				DayMultiple = 10,
				DayExponent = 2.0f,
				ProductBonus = 2.0f
			};

		public override DebtManager? DebtManager { get; set; }
	}
}