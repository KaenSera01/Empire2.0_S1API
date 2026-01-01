using Empire.DebtHelpers;
using Empire.NPC.Data;
using System.Collections.Generic;

namespace Empire.NPC.S1API_NPCs
{
	public class TheCousins : EmpireNPC
	{
		public override string DealerId => "the_cousins";
		public override string FirstName => "The";
		public override string LastName => "Cousins";
		public override string DisplayName => "The Cousins";
		public override int Tier => 2;

		public override List<UnlockRequirement> UnlockRequirements { get; protected set; } =
			new List<UnlockRequirement>
			{
				new UnlockRequirement { Name = "Hector Salamanca", MinRep = 1000 }
			};

		public override List<string> DealDays { get; protected set; } =
			new List<string> { "Monday", "Wednesday", "Friday", "Sunday" };

		public override bool CurfewDeal { get; protected set; } = true;

		public override List<List<float>> Deals { get; protected set; } =
			new List<List<float>>
			{
				new List<float> { 3f, 1.5f, 93044f, 270f },
				new List<float> { 5f, 1.2f, 113731f, 350f }
			};

		public override int RefreshCost { get; protected set; } = 500;

		public override DealerReward Reward { get; protected set; } =
			new DealerReward
			{
				unlockRep = 200,
				RepCost = 40,
				Type = "console",
				Args = new List<string> { "teleport", "church" }
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
					BaseDollar = 75,
					BaseRep = 181,
					BaseXp = 153,
					RepMult = 0.001f,
					XpMult = 0.001f,

					Qualities = new List<Quality>
					{
						new Quality { Type = "poor",     DollarMult = 0f, UnlockRep = 0 },
						new Quality { Type = "standard", DollarMult = 0f, UnlockRep = 250 }
					},

					Effects = new List<Effect>
					{
						new Effect { Name = "Toxic",      UnlockRep = 0,    Probability = 2.0f, DollarMult = 0f },
						new Effect { Name = "Random",     UnlockRep = 490,  Probability = 1.2f, DollarMult = 0f },
						new Effect { Name = "Focused",    UnlockRep = 1420, Probability = 1.0f, DollarMult = 0f },
						new Effect { Name = "Random",     UnlockRep = 2590, Probability = 0.3f, DollarMult = 0f },
						new Effect { Name = "Random",     UnlockRep = 3470, Probability = 0.6f, DollarMult = 0f }
					}
				},

                // Meth
                new Drug
				{
					Type = "meth",
					UnlockRep = 1140,
					BaseDollar = 40,
					BaseRep = 168,
					BaseXp = 161,
					RepMult = 0.002f,
					XpMult = 0.001f,

					Qualities = new List<Quality>
					{
						new Quality { Type = "poor",     DollarMult = 0f, UnlockRep = 1140 },
						new Quality { Type = "standard", DollarMult = 0f, UnlockRep = 1820 }
					},

					Effects = new List<Effect>
					{
						new Effect { Name = "Toxic",       UnlockRep = 1140, Probability = 2.0f, DollarMult = 0f },
						new Effect { Name = "Random",      UnlockRep = 1820, Probability = 1.2f, DollarMult = 0f },
						new Effect { Name = "Energizing",  UnlockRep = 0,    Probability = 1.0f, DollarMult = 0f },
						new Effect { Name = "Random",      UnlockRep = 0,    Probability = 0.3f, DollarMult = 0f },
						new Effect { Name = "Random",      UnlockRep = 0,    Probability = 0.6f, DollarMult = 0f }
					}
				}
			};

		public override List<Shipping> Shippings { get; protected set; } =
			new List<Shipping>
			{
				new Shipping
				{
					Name = "Cousins' Free Run",
					Cost = 0,
					UnlockRep = 0,
					MinAmount = 10,
					StepAmount = 1,
					MaxAmount = 50,
					DealModifier = new List<float> { 1f, 1f, 1f, 1f }
				},
				new Shipping
				{
					Name = "Silver Axe Transport",
					Cost = 30000,
					UnlockRep = 790,
					MinAmount = 30,
					StepAmount = 5,
					MaxAmount = 90,
					DealModifier = new List<float> { 1.25f, 1.25f, 1.25f, 1.25f }
				},
				new Shipping
				{
					Name = "Cross-Border Cattle Truck",
					Cost = 100000,
					UnlockRep = 5090,
					MinAmount = 50,
					StepAmount = 20,
					MaxAmount = 250,
					DealModifier = new List<float> { 1.5f, 1.5f, 1.5f, 1.5f }
				}
			};

		public override Dialogue EmpireDialogue { get; protected set; } =
			new Dialogue
			{
				Intro = new List<string>
				{
					"..."
				},
				DealStart = new List<string>
				{
					"I require {amount} of {quality} {product} with required effects: {effects} and optional effects: {optionalEffects}.",
					"Ensure delivery of {amount} {quality} {product} that carries required effects: {effects} and optional effects: {optionalEffects}.",
					"Provide {amount} {quality} {product} with the required effects: {effects} and optional effects: {optionalEffects}."
				},
				Accept = new List<string>
				{
					"*Nod*",
					"..."
				},
				Incomplete = new List<string>
				{
					"*Stare intensifies* ... {amount} ...",
					"*Glares* ... More ... {amount} ...",
					"*Tilts head* ... {amount} ..."
				},
				Expire = new List<string>
				{
					"*Turns and walks away*",
					"..."
				},
				Fail = new List<string>
				{
					"*Draws weapon*",
					"*Cracks knuckles*",
					"..."
				},
				Success = new List<string>
				{
					"*Takes package, nods*",
					"...",
					"Bueno."
				},
				Reward = new List<string>
				{
					"*Slides money forward* ({dollar})",
					"*Points to money* ({dollar})",
					"... ({dollar})"
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
				TotalDebt = 100000,
				InterestRate = 0.1f,
				DayMultiple = 10,
				DayExponent = 2.0f,
				ProductBonus = 2.0f
			};

		public override DebtManager? DebtManager { get; set; }
	}
}