using Empire.DebtHelpers;
using Empire.NPC.Data;
using System.Collections.Generic;

namespace Empire.NPC.S1API_NPCs
{
	public class DonEladioVuente : EmpireNPC
	{
		public override string DealerId => "don_eladio_vuente";
		public override string FirstName => "Don Eladio";
		public override string LastName => "Vuente";
		public override string DisplayName => "Don Eladio Vuente";
		public override int Tier => 4;

		public override List<UnlockRequirement> UnlockRequirements { get; protected set; } =
			new List<UnlockRequirement>
			{
				new UnlockRequirement { Name = "Gustavo Fring", MinRep = 5000 },
				new UnlockRequirement { Name = "Hector Salamanca", MinRep = 1500 }
			};

		public override List<string> DealDays { get; protected set; } =
			new List<string> { "Wednesday", "Sunday" };

		public override bool CurfewDeal { get; protected set; } = true;

		public override List<List<float>> Deals { get; protected set; } =
			new List<List<float>>
			{
				new List<float> { 4f, 1.5f, 248699f, 781f },
				new List<float> { 5f, 1.2f, 278599f, 879f },
				new List<float> { 6f, 1.0f, 308549f, 1037f }
			};

		public override int RefreshCost { get; protected set; } = 1500;

		public override DealerReward Reward { get; protected set; } =
			new DealerReward
			{
				unlockRep = 400,
				RepCost = 250,
				Type = "console",
				Args = new List<string> { "setjumpforce", "2" }
			};

		public override float RepLogBase { get; protected set; } = 6f;

		public override List<Drug> Drugs { get; protected set; } =
			new List<Drug>
			{
                // Cocaine
                new Drug
				{
					Type = "cocaine",
					UnlockRep = 0,
					BaseDollar = 134,
					BaseRep = 423,
					BaseXp = 367,
					RepMult = 0.001f,
					XpMult = 0.001f,

					Qualities = new List<Quality>
					{
						new Quality { Type = "premium",  DollarMult = 0f, UnlockRep = 0 },
						new Quality { Type = "heavenly", DollarMult = 0f, UnlockRep = 500 }
					},

					Effects = new List<Effect>
					{
						new Effect { Name = "Explosive",     UnlockRep = 0,     Probability = 2.0f, DollarMult = 0f },
						new Effect { Name = "Munchies",      UnlockRep = 950,   Probability = 2.0f, DollarMult = 0f },
						new Effect { Name = "Random",        UnlockRep = 3050,  Probability = 1.2f, DollarMult = 0f },
						new Effect { Name = "Laxative",      UnlockRep = 5550,  Probability = 1.0f, DollarMult = 0f },
						new Effect { Name = "TropicThunder", UnlockRep = 8600,  Probability = 1.0f, DollarMult = 0f },
						new Effect { Name = "Random",        UnlockRep = 10800, Probability = 0.3f, DollarMult = 0f },
						new Effect { Name = "Random",        UnlockRep = 13100, Probability = 0.6f, DollarMult = 0f }
					}
				},

                // Meth
                new Drug
				{
					Type = "meth",
					UnlockRep = 2450,
					BaseDollar = 49,
					BaseRep = 398,
					BaseXp = 383,
					RepMult = 0.002f,
					XpMult = 0.001f,

					Qualities = new List<Quality>
					{
						new Quality { Type = "premium", DollarMult = 0f, UnlockRep = 2450 }
					},

					Effects = new List<Effect>
					{
						new Effect { Name = "Zombifying", UnlockRep = 2450, Probability = 2.0f, DollarMult = 0f },
						new Effect { Name = "Glowie",     UnlockRep = 4700, Probability = 2.0f, DollarMult = 0f },
						new Effect { Name = "Random",     UnlockRep = 7600, Probability = 1.2f, DollarMult = 0f },
						new Effect { Name = "Smelly",     UnlockRep = 9650, Probability = 1.0f, DollarMult = 0f },
						new Effect { Name = "Slippery",   UnlockRep = 11900,Probability = 1.0f, DollarMult = 0f },
						new Effect { Name = "Random",     UnlockRep = 14350,Probability = 0.3f, DollarMult = 0f },
						new Effect { Name = "Random",     UnlockRep = 15620,Probability = 0.6f, DollarMult = 0f }
					}
				}
			};

		public override List<Shipping> Shippings { get; protected set; } =
			new List<Shipping>
			{
				new Shipping
				{
					Name = "Eladio's Airlift",
					Cost = 0,
					UnlockRep = 0,
					MinAmount = 100,
					StepAmount = 1,
					MaxAmount = 150,
					DealModifier = new List<float> { 1f, 1f, 1f, 1f }
				},
				new Shipping
				{
					Name = "Cartel Helicopter Fleet",
					Cost = 80000,
					UnlockRep = 1650,
					MinAmount = 120,
					StepAmount = 5,
					MaxAmount = 300,
					DealModifier = new List<float> { 1.25f, 1.25f, 1.25f, 1.25f }
				},
				new Shipping
				{
					Name = "Narco Sub Armada",
					Cost = 100000,
					UnlockRep = 3950,
					MinAmount = 150,
					StepAmount = 5,
					MaxAmount = 350,
					DealModifier = new List<float> { 1.5f, 1.5f, 1.5f, 1.5f }
				},
				new Shipping
				{
					Name = "Federales Escort Service",
					Cost = 150000,
					UnlockRep = 6650,
					MinAmount = 200,
					StepAmount = 10,
					MaxAmount = 500,
					DealModifier = new List<float> { 1.75f, 1.75f, 1.75f, 1.75f }
				},
				new Shipping
				{
					Name = "Eladio's VIP Convoy",
					Cost = 170000,
					UnlockRep = 16895,
					MinAmount = 240,
					StepAmount = 20,
					MaxAmount = 600,
					DealModifier = new List<float> { 2f, 2f, 2f, 2f }
				}
			};

		public override Dialogue EmpireDialogue { get; protected set; } =
			new Dialogue
			{
				Intro = new List<string>
				{
					"*Chuckles* Look what the cat dragged in. You bring tribute... or trouble?"
				},
				DealStart = new List<string>
				{
					"Salud! I require {amount} of {quality} {product} with required effects: {effects} and optional effects: {optionalEffects}. Impress me.",
					"Bring {amount} {quality} {product} ensuring it has required effects: {effects} and optional effects: {optionalEffects}.",
					"I need {amount} {quality} {product} that delivers required effects: {effects} along with optional effects: {optionalEffects}."
				},
				Accept = new List<string>
				{
					"You may proceed.",
					"Good enough, for now."
				},
				Incomplete = new List<string>
				{
					"Is this a joke? {amount}? You think Eladio deals in scraps? Bring the full amount!",
					"You trying to insult me? Where is the other {amount}? Huh?",
					"This is not what we agreed. {amount} total. Or you pay the price."
				},
				Expire = new List<string>
				{
					"My patience is not infinite. You are too slow. The moment has passed.",
					"You kept me waiting? *Scoffs* Go away. Opportunity missed."
				},
				Fail = new List<string>
				{
					"This... *spits*... garbage? You DARE bring this to my home?! Get him out of my sight!",
					"Betrayal! This is not the quality I command! You will regret this insult."
				},
				Success = new List<string>
				{
					"*Chuckles* Excellent. This will liven up the fiesta.",
					"Ah, very good. You may live another day.",
					"Acceptable. Pour me a drink."
				},
				Reward = new List<string>
				{
					"Your payment. Now, enjoy the party... from a distance. {dollar}",
					"Here. A token for your troubles. Now leave. {dollar}",
					"Money. As promised. Do not presume this makes us friends. {dollar}"
				}
			};

		public override Gift? Gift { get; protected set; } =
			new Gift
			{
				Cost = 4000,
				Rep = 80
			};

		public override Debt? Debt { get; protected set; } =
			new Debt
			{
				TotalDebt = 1000000,
				InterestRate = 0.1f,
				DayMultiple = 10,
				DayExponent = 2.0f,
				ProductBonus = 2.0f
			};

		public override DebtManager? DebtManager { get; set; }
	}
}