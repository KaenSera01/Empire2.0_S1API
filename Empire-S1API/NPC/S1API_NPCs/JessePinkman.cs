using Empire.DebtHelpers;
using Empire.NPC.Data;
using System.Collections.Generic;

namespace Empire.NPC.S1API_NPCs
{
	public class JessePinkman : EmpireNPC
	{
		public override string DealerId => "jesse_pinkman";
		public override string FirstName => "Jesse";
		public override string LastName => "Pinkman";
		public override string DisplayName => "Jesse Pinkman";
		public override int Tier => 3;

		public override List<UnlockRequirement> UnlockRequirements { get; protected set; } =
			new List<UnlockRequirement>
			{
				new UnlockRequirement { Name = "Skinny Pete",                 MinRep = 200 },
				new UnlockRequirement { Name = "Domingo 'Krazy-8' Molina",    MinRep = 690 },
				new UnlockRequirement { Name = "Mr. Badger",                  MinRep = 200 },
				new UnlockRequirement { Name = "Combo Costco",                MinRep = 200 }
			};

		public override List<string> DealDays { get; protected set; } =
			new List<string> { "Tuesday", "Thursday", "Friday" };

		public override bool CurfewDeal { get; protected set; } = true;

		public override List<List<float>> Deals { get; protected set; } =
			new List<List<float>>
			{
				new List<float> { 2f, 1.0f, 13816f, 75f },
				new List<float> { 3f, 0.9f, 15941f, 83f },
				new List<float> { 4f, 0.8f, 18067f, 91f }
			};

		public override int RefreshCost { get; protected set; } = 1000;

		public override DealerReward Reward { get; protected set; } =
			new DealerReward
			{
				unlockRep = 300,
				RepCost = 200,
				Type = "console",
				Args = new List<string> { "setquality", "heavenly" }
			};

		public override float RepLogBase { get; protected set; } = 5f;

		public override List<Drug> Drugs { get; protected set; } =
			new List<Drug>
			{
                // Weed
                new Drug
				{
					Type = "weed",
					UnlockRep = 0,
					BaseDollar = 37,
					BaseRep = 80,
					BaseXp = 39,
					RepMult = 0.002f,
					XpMult = 0.001f,

					Qualities = new List<Quality>
					{
						new Quality { Type = "standard", DollarMult = 0f, UnlockRep = 0 },
						new Quality { Type = "premium",  DollarMult = 0f, UnlockRep = 210 },
						new Quality { Type = "heavenly", DollarMult = 0f, UnlockRep = 1800 }
					},

					Effects = new List<Effect>
					{
						new Effect { Name = "Paranoia",        UnlockRep = 0,    Probability = 2.0f, DollarMult = 0f },
						new Effect { Name = "ThoughtProvoking", UnlockRep = 60,   Probability = 2.0f, DollarMult = 0f },
						new Effect { Name = "Foggy",            UnlockRep = 375,  Probability = 1.0f, DollarMult = 0f },
						new Effect { Name = "Slippery",         UnlockRep = 690,  Probability = 1.0f, DollarMult = 0f },
						new Effect { Name = "Random",           UnlockRep = 885,  Probability = 0.3f, DollarMult = 0f },
						new Effect { Name = "Random",           UnlockRep = 1090, Probability = 0.6f, DollarMult = 0f }
					}
				},

                // Meth
                new Drug
				{
					Type = "meth",
					UnlockRep = 120,
					BaseDollar = 48,
					BaseRep = 60,
					BaseXp = 41,
					RepMult = 0.004f,
					XpMult = 0.001f,

					Qualities = new List<Quality>
					{
						new Quality { Type = "standard", DollarMult = 0f, UnlockRep = 130 },
						new Quality { Type = "premium",  DollarMult = 0f, UnlockRep = 485 }
					},

					Effects = new List<Effect>
					{
						new Effect { Name = "Toxic",        UnlockRep = 130,  Probability = 2.0f, DollarMult = 0f },
						new Effect { Name = "Explosive",    UnlockRep = 780,  Probability = 2.0f, DollarMult = 0f },
						new Effect { Name = "BrightEyed",   UnlockRep = 1010, Probability = 1.0f, DollarMult = 0f },
						new Effect { Name = "Disorienting", UnlockRep = 1230, Probability = 1.0f, DollarMult = 0f },
						new Effect { Name = "Random",       UnlockRep = 1340, Probability = 0.3f, DollarMult = 0f },
						new Effect { Name = "Random",       UnlockRep = 1490, Probability = 0.6f, DollarMult = 0f }
					}
				}
			};

		public override List<Shipping> Shippings { get; protected set; } =
			new List<Shipping>
			{
				new Shipping
				{
					Name = "Pinkman's Free Run",
					Cost = 0,
					UnlockRep = 0,
					MinAmount = 5,
					StepAmount = 1,
					MaxAmount = 25,
					DealModifier = new List<float> { 1f, 1f, 1f, 1f }
				},
				new Shipping
				{
					Name = "Crystal Clear Courier",
					Cost = 10000,
					UnlockRep = 310,
					MinAmount = 10,
					StepAmount = 5,
					MaxAmount = 40,
					DealModifier = new List<float> { 1.25f, 1.25f, 1.25f, 1.25f }
				},
				new Shipping
				{
					Name = "Slab Runner Van",
					Cost = 20000,
					UnlockRep = 615,
					MinAmount = 20,
					StepAmount = 10,
					MaxAmount = 60,
					DealModifier = new List<float> { 1.5f, 1.5f, 1.5f, 1.5f }
				},
				new Shipping
				{
					Name = "Blue Magic Heavy Haul",
					Cost = 40000,
					UnlockRep = 1645,
					MinAmount = 40,
					StepAmount = 20,
					MaxAmount = 100,
					DealModifier = new List<float> { 1.75f, 1.75f, 1.75f, 1.75f }
				}
			};

		public override Dialogue EmpireDialogue { get; protected set; } =
			new Dialogue
			{
				Intro = new List<string>
				{
					"Yo! Mr. White sent you? Heard about you from my crew. You got the blue?"
				},
				DealStart = new List<string>
				{
					"Alright yo, I need {amount} of {quality} {product} with required effects: {effects} and optional effects: {optionalEffects}.",
					"Get {amount} of {quality} {product} that delivers required effects: {effects} and optional effects: {optionalEffects}.",
					"Listen up, {amount} {quality} {product} must include required effects: {effects} along with optional effects: {optionalEffects}."
				},
				Accept = new List<string>
				{
					"Yeah, science, bitch!",
					"Let’s do this, yo!"
				},
				Incomplete = new List<string>
				{
					"Yo, what the hell? This ain't {amount}! Bring the rest, man!",
					"Seriously? Where's the other {amount}? Don't be a d-bag!",
					"Dude! {amount} total! You shorting me? Not cool, bitch!"
				},
				Expire = new List<string>
				{
					"Yo, time's up! Where you been? Deal's dead, man.",
					"Way too slow, bitch! Lost the window. I'm out."
				},
				Fail = new List<string>
				{
					"What is this crap?! You trying to pull a fast one? Get lost!",
					"This is messed up, yo! Total garbage! You're fired!"
				},
				Success = new List<string>
				{
					"Yeah! That's the stuff! Science, bitch!",
					"Alright! Lookin' good, yo.",
					"Hell yeah. This'll fly."
				},
				Reward = new List<string>
				{
					"Here's the skrilla, yo. ${dollar} Keep it real.",
					"Boom! Payment, bitch! {dollar}",
					"Money's wired. Yeah, magnets! {dollar}"
				}
			};

		public override Gift? Gift { get; protected set; } =
			new Gift
			{
				Cost = 3000,
				Rep = 60
			};

		public override Debt? Debt { get; protected set; } =
			new Debt(); // No debt in JSON

		public override DebtManager? DebtManager { get; set; }
	}
}