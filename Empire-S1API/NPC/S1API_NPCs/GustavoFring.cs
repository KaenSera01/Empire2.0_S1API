using Empire.DebtHelpers;
using Empire.NPC.Data;
using System.Collections.Generic;

namespace Empire.NPC.S1API_NPCs
{
	public class GustavoFring : EmpireNPC
	{
		public override string DealerId => "gustavo_fring";
		public override string FirstName => "Gustavo";
		public override string LastName => "Fring";
		public override string DisplayName => "Gustavo Fring";
		public override int Tier => 4;

		public override List<UnlockRequirement> UnlockRequirements { get; protected set; } =
			new List<UnlockRequirement>
			{
				new UnlockRequirement { Name = "Saul Goodman", MinRep = 1000 },
				new UnlockRequirement { Name = "Mike Ehrmantraut", MinRep = 1500 },
				new UnlockRequirement { Name = "Jesse Pinkman", MinRep = 1313 }
			};

		public override List<string> DealDays { get; protected set; } =
			new List<string> { "Monday", "Wednesday" };

		public override bool CurfewDeal { get; protected set; } = true;

		public override List<List<float>> Deals { get; protected set; } =
			new List<List<float>>
			{
				new List<float> { 3f, 1.3f, 159744f, 458f },
				new List<float> { 4f, 1.15f, 172032f, 515f },
				new List<float> { 5f, 0.95f, 204320f, 573f }
			};

		public override int RefreshCost { get; protected set; } = 1050;

		public override DealerReward Reward { get; protected set; } =
			new DealerReward
			{
				unlockRep = 400,
				RepCost = 250,
				Type = "console",
				Args = new List<string> { "setmovespeed", "2" }
			};

		public override float RepLogBase { get; protected set; } = 6f;

		public override List<Drug> Drugs { get; protected set; } =
			new List<Drug>
			{
                // Meth
                new Drug
				{
					Type = "meth",
					UnlockRep = 0,
					BaseDollar = 68,
					BaseRep = 260,
					BaseXp = 215,
					RepMult = 0.002f,
					XpMult = 0.001f,

					Qualities = new List<Quality>
					{
						new Quality { Type = "premium",  DollarMult = 0f, UnlockRep = 0 },
						new Quality { Type = "heavenly", DollarMult = 0f, UnlockRep = 300 }
					},

					Effects = new List<Effect>
					{
						new Effect { Name = "Focused",    UnlockRep = 0,    Probability = 2.0f, DollarMult = 0f },
						new Effect { Name = "Toxic",      UnlockRep = 580,  Probability = 2.0f, DollarMult = 0f },
						new Effect { Name = "Random",     UnlockRep = 1300, Probability = 1.2f, DollarMult = 0f },
						new Effect { Name = "Calming",    UnlockRep = 2630, Probability = 1.0f, DollarMult = 0f },
						new Effect { Name = "Sedating",   UnlockRep = 4130, Probability = 1.0f, DollarMult = 0f },
						new Effect { Name = "Random",     UnlockRep = 5200, Probability = 0.3f, DollarMult = 0f },
						new Effect { Name = "Random",     UnlockRep = 6400, Probability = 0.6f, DollarMult = 0f }
					}
				},

                // Cocaine
                new Drug
				{
					Type = "cocaine",
					UnlockRep = 1750,
					BaseDollar = 104,
					BaseRep = 220,
					BaseXp = 224,
					RepMult = 0.001f,
					XpMult = 0.001f,

					Qualities = new List<Quality>
					{
						new Quality { Type = "premium", DollarMult = 0f, UnlockRep = 1750 }
					},

					Effects = new List<Effect>
					{
						new Effect { Name = "Paranoia",   UnlockRep = 1750, Probability = 2.0f, DollarMult = 0f },
						new Effect { Name = "Euphoric",   UnlockRep = 3050, Probability = 2.0f, DollarMult = 0f },
						new Effect { Name = "Random",     UnlockRep = 4650, Probability = 1.2f, DollarMult = 0f },
						new Effect { Name = "Sneaky",     UnlockRep = 5780, Probability = 1.0f, DollarMult = 0f },
						new Effect { Name = "BrightEyed", UnlockRep = 7050, Probability = 1.0f, DollarMult = 0f },
						new Effect { Name = "Random",     UnlockRep = 7730, Probability = 0.3f, DollarMult = 0f },
						new Effect { Name = "Random",     UnlockRep = 8430, Probability = 0.3f, DollarMult = 0f }
					}
				}
			};

		public override List<Shipping> Shippings { get; protected set; } =
			new List<Shipping>
			{
				new Shipping
				{
					Name = "Gus's Free Frozen Freight",
					Cost = 0,
					UnlockRep = 0,
					MinAmount = 50,
					StepAmount = 1,
					MaxAmount = 100,
					DealModifier = new List<float> { 1f, 1f, 1f, 1f }
				},
				new Shipping
				{
					Name = "Pollos Refrigerated Trucks",
					Cost = 40000,
					UnlockRep = 980,
					MinAmount = 60,
					StepAmount = 5,
					MaxAmount = 160,
					DealModifier = new List<float> { 1.25f, 1.25f, 1.25f, 1.25f }
				},
				new Shipping
				{
					Name = "Madrigal Air Freight",
					Cost = 60000,
					UnlockRep = 2250,
					MinAmount = 80,
					StepAmount = 10,
					MaxAmount = 200,
					DealModifier = new List<float> { 1.5f, 1.5f, 1.5f, 1.5f }
				},
				new Shipping
				{
					Name = "Superlab Logistics",
					Cost = 90000,
					UnlockRep = 3650,
					MinAmount = 120,
					StepAmount = 20,
					MaxAmount = 240,
					DealModifier = new List<float> { 1.75f, 1.75f, 1.75f, 1.75f }
				},
				new Shipping
				{
					Name = "Fring's Final Express",
					Cost = 125000,
					UnlockRep = 9180,
					MinAmount = 200,
					StepAmount = 20,
					MaxAmount = 400,
					DealModifier = new List<float> { 2f, 2f, 2f, 2f }
				}
			};

		public override Dialogue EmpireDialogue { get; protected set; } =
			new Dialogue
			{
				Intro = new List<string>
				{
					"Welcome. I trust your journey was uneventful. Shall we discuss the terms?"
				},
				DealStart = new List<string>
				{
					"I require {amount} of {quality} {product} with required effects: {effects} and optional effects: {optionalEffects}. Precision is key.",
					"Provide {amount} {quality} {product} ensuring it carries required effects: {effects} and optional effects: {optionalEffects}.",
					"Deliver {amount} of {quality} {product} that meets the criteria: required effects {effects} and optional effects {optionalEffects}."
				},
				Accept = new List<string>
				{
					"Proceed.",
					"This is acceptable, move forward."
				},
				Incomplete = new List<string>
				{
					"This quantity is insufficient. The agreement was for {amount}. Rectify this immediately.",
					"There appears to be a discrepancy. I require the remaining {amount}. Do not test my patience.",
					"The full order is {amount}. This is incomplete. Fulfill your obligation."
				},
				Expire = new List<string>
				{
					"Punctuality is a mark of professionalism. You have failed to meet the deadline. This arrangement is concluded.",
					"Time is a resource I do not waste. Your delay is unacceptable. We are finished here."
				},
				Fail = new List<string>
				{
					"This product is substandard. It does not meet Los Pollos Hermanos' standards... or mine. This is a grave error.",
					"You have proven unreliable. Such incompetence cannot be tolerated in my organization. Goodbye."
				},
				Success = new List<string>
				{
					"The product appears satisfactory. Well done.",
					"Acceptable. Your adherence to standards is noted.",
					"Good. This meets expectations."
				},
				Reward = new List<string>
				{
					"Payment, as agreed. Maintain this level of quality. {dollar}",
					"The transaction is complete. Efficiency is rewarded. {dollar}",
					"Your compensation. Remember the importance of discretion. {dollar}"
				}
			};

		public override Gift? Gift { get; protected set; } =
			new Gift
			{
				Cost = 4000,
				Rep = 80
			};

		public override Debt? Debt { get; protected set; } = null;

		public override DebtManager? DebtManager { get; set; }
	}
}