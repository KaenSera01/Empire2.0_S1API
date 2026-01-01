using Empire.DebtHelpers;
using Empire.NPC.Data;
using System.Collections.Generic;

namespace Empire.NPC.S1API_NPCs
{
	public class TucoSalamanca : EmpireNPC
	{
		public override string DealerId => "tuco_salamanca";
		public override string FirstName => "Tuco";
		public override string LastName => "Salamanca";
		public override string DisplayName => "Tuco Salamanca";
		public override int Tier => 3;

		public override List<UnlockRequirement> UnlockRequirements { get; protected set; } =
			new List<UnlockRequirement>
			{
				new UnlockRequirement { Name = "Jesse Pinkman", MinRep = 777 },
				new UnlockRequirement { Name = "Hector Salamanca", MinRep = 777 }
			};

		public override List<string> DealDays { get; protected set; } =
			new List<string> { "Tuesday", "Friday", "Sunday" };

		public override bool CurfewDeal { get; protected set; } = true;

		public override List<List<float>> Deals { get; protected set; } =
			new List<List<float>>
			{
				new List<float> { 2f, 1.15f, 40634f, 122f },
				new List<float> { 3f, 0.9f, 46838f, 135f },
				new List<float> { 4f, 0.8f, 55043f, 149f }
			};

		public override int RefreshCost { get; protected set; } = 1000;

		public override DealerReward Reward { get; protected set; } =
			new DealerReward
			{
				unlockRep = 300,
				RepCost = 100,
				Type = "console",
				Args = new List<string> { "clearwanted" }
			};

		public override float RepLogBase { get; protected set; } = 5f;

		public override List<Drug> Drugs { get; protected set; } =
			new List<Drug>
			{
                // Meth
                new Drug
				{
					Type = "meth",
					UnlockRep = 0,
					BaseDollar = 43,
					BaseRep = 88,
					BaseXp = 62,
					RepMult = 0.002f,
					XpMult = 0.001f,

					Qualities = new List<Quality>
					{
						new Quality { Type = "standard", DollarMult = 0f, UnlockRep = 0 },
						new Quality { Type = "premium",  DollarMult = 0f, UnlockRep = 100 }
					},

					Effects = new List<Effect>
					{
						new Effect { Name = "Schizophrenic", UnlockRep = 0,    Probability = 2.0f, DollarMult = 0f },
						new Effect { Name = "Athletic",       UnlockRep = 190,  Probability = 2.0f, DollarMult = 0f },
						new Effect { Name = "Jennerising",    UnlockRep = 460,  Probability = 1.0f, DollarMult = 0f },
						new Effect { Name = "AntiGravity",    UnlockRep = 1100, Probability = 1.0f, DollarMult = 0f },
						new Effect { Name = "Random",         UnlockRep = 1400, Probability = 0.3f, DollarMult = 0f },
						new Effect { Name = "Random",         UnlockRep = 1700, Probability = 0.6f, DollarMult = 0f }
					}
				},

                // Cocaine
                new Drug
				{
					Type = "cocaine",
					UnlockRep = 640,
					BaseDollar = 98,
					BaseRep = 69,
					BaseXp = 66,
					RepMult = 0.001f,
					XpMult = 0.001f,

					Qualities = new List<Quality>
					{
						new Quality { Type = "standard", DollarMult = 0f, UnlockRep = 640 },
						new Quality { Type = "premium",  DollarMult = 0f, UnlockRep = 1000 }
					},

					Effects = new List<Effect>
					{
						new Effect { Name = "Spicy",      UnlockRep = 640,  Probability = 2.0f, DollarMult = 0f },
						new Effect { Name = "Cyclopean",  UnlockRep = 1270, Probability = 2.0f, DollarMult = 0f },
						new Effect { Name = "LongFaced",  UnlockRep = 1590, Probability = 1.0f, DollarMult = 0f },
						new Effect { Name = "Shrinking",  UnlockRep = 1920, Probability = 1.0f, DollarMult = 0f },
						new Effect { Name = "Random",     UnlockRep = 2070, Probability = 0.3f, DollarMult = 0f },
						new Effect { Name = "Random",     UnlockRep = 2310, Probability = 0.6f, DollarMult = 0f }
					}
				}
			};

		public override List<Shipping> Shippings { get; protected set; } =
			new List<Shipping>
			{
				new Shipping
				{
					Name = "Tuco's Free Hustle",
					Cost = 0,
					UnlockRep = 0,
					MinAmount = 20,
					StepAmount = 1,
					MaxAmount = 40,
					DealModifier = new List<float> { 1f, 1f, 1f, 1f }
				},
				new Shipping
				{
					Name = "Abuela's Underground Tunnel",
					Cost = 20000,
					UnlockRep = 340,
					MinAmount = 30,
					StepAmount = 5,
					MaxAmount = 80,
					DealModifier = new List<float> { 1.25f, 1.25f, 1.25f, 1.25f }
				},
				new Shipping
				{
					Name = "Salamanca Muscle Convoy",
					Cost = 25000,
					UnlockRep = 840,
					MinAmount = 40,
					StepAmount = 10,
					MaxAmount = 100,
					DealModifier = new List<float> { 1.5f, 1.5f, 1.5f, 1.5f }
				},
				new Shipping
				{
					Name = "Tuco's VIP Charter",
					Cost = 40000,
					UnlockRep = 2560,
					MinAmount = 80,
					StepAmount = 20,
					MaxAmount = 160,
					DealModifier = new List<float> { 1.75f, 1.75f, 1.75f, 1.75f }
				}
			};

		public override Dialogue EmpireDialogue { get; protected set; } =
			new Dialogue
			{
				Intro = new List<string>
				{
					"WHO ARE YOU?! You got the stuff or you wasting my time?! Talk!"
				},
				DealStart = new List<string>
				{
					"Yo, I require {amount} of {quality} {product} with required effects: {effects} and optional effects: {optionalEffects}.",
					"Deliver {amount} {quality} {product} that includes required effects: {effects} and optional effects: {optionalEffects}.",
					"Make sure {amount} {quality} {product} provides required effects: {effects} along with optional effects: {optionalEffects}."
				},
				Accept = new List<string>
				{
					"You’re in, for now.",
					"Deal’s on, don’t screw it up!"
				},
				Incomplete = new List<string>
				{
					"WHAT IS THIS?! HALF?! ARE YOU STUPID?! BRING ME THE REST! {amount} MORE! NOW!",
					"YOU THINK THIS IS A GAME?! {amount} TOTAL! WHERE IS IT?!",
					"SHORT?! YOU THINK YOU CAN SHORT ME?! YOU CRAZY?! GET THE OTHER {amount}!"
				},
				Expire = new List<string>
				{
					"TOO SLOW! YOU THINK I GOT ALL DAY?! GET OUT! YOU'RE DONE!",
					"WHAT ARE YOU, A TURTLE?! DEALS OFF! GO! NOW!"
				},
				Fail = new List<string>
				{
					"THIS IS SHIT! YOU TRYING TO POISON ME?! *snorts* YOU'RE DEAD!",
					"YOU FUCKED UP! YOU FUCKED UP! NOBODY FUCKS ME OVER!"
				},
				Success = new List<string>
				{
					"Alright! Finally! Looks tight!",
					"*Snorts* YEAH! THAT'S THE STUFF!",
					"Heh heh. Good. REAL GOOD."
				},
				Reward = new List<string>
				{
					"Here's your money! Now GET OUT! {dollar}",
					"Take it! Don't look at me! {dollar}",
					"Yeah, yeah, payment. Scram! {dollar}"
				}
			};

		public override Gift? Gift { get; protected set; } =
			new Gift
			{
				Cost = 3000,
				Rep = 60
			};

		public override Debt? Debt { get; protected set; } =
			new Debt
			{
				TotalDebt = 250000,
				InterestRate = 0.1f,
				DayMultiple = 10,
				DayExponent = 2.0f,
				ProductBonus = 2.0f
			};

		public override DebtManager? DebtManager { get; set; }
	}
}