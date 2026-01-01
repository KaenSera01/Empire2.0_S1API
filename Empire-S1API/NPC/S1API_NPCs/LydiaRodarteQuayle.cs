using Empire.DebtHelpers;
using Empire.NPC.Data;
using System.Collections.Generic;

namespace Empire.NPC.S1API_NPCs
{
	public class LydiaRodarteQuayle : EmpireNPC
	{
		public override string DealerId => "lydia_rodarte_quayle";
		public override string FirstName => "Lydia";
		public override string LastName => "Rodarte-Quayle";
		public override string DisplayName => "Lydia Rodarte-Quayle";
		public override int Tier => 4;

		public override List<UnlockRequirement> UnlockRequirements { get; protected set; } =
			new List<UnlockRequirement>
			{
				new UnlockRequirement { Name = "Gustavo Fring", MinRep = 4000 },
				new UnlockRequirement { Name = "Todd Alquist", MinRep = 1313 }
			};

		public override List<string> DealDays { get; protected set; } =
			new List<string> { "Tuesday", "Saturday" };

		public override bool CurfewDeal { get; protected set; } = true;

		public override List<List<float>> Deals { get; protected set; } =
			new List<List<float>>
			{
				new List<float> { 3f, 1.2f, 69069f, 322f },
				new List<float> { 4f, 1.12f, 74382f, 362f },
				new List<float> { 5f, 1.04f, 79695f, 403f }
			};

		public override int RefreshCost { get; protected set; } = 1500;

		public override DealerReward Reward { get; protected set; } =
			new DealerReward
			{
				unlockRep = 400,
				RepCost = 250,
				Type = "console",
				Args = new List<string> { "setstaminareserve", "200" }
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
					BaseDollar = 54,
					BaseRep = 181,
					BaseXp = 155,
					RepMult = 0.002f,
					XpMult = 0.001f,

					Qualities = new List<Quality>
					{
						new Quality { Type = "premium", DollarMult = 0f, UnlockRep = 0 }
					},

					Effects = new List<Effect>
					{
						new Effect { Name = "Paranoia",    UnlockRep = 0,    Probability = 2.0f, DollarMult = 0f },
						new Effect { Name = "Focused",     UnlockRep = 220,  Probability = 2.0f, DollarMult = 0f },
						new Effect { Name = "Random",      UnlockRep = 770,  Probability = 1.2f, DollarMult = 0f },
						new Effect { Name = "Refreshing",  UnlockRep = 1520, Probability = 1.0f, DollarMult = 0f },
						new Effect { Name = "Sneaky",      UnlockRep = 2470, Probability = 1.0f, DollarMult = 0f },
						new Effect { Name = "Random",      UnlockRep = 2950, Probability = 0.3f, DollarMult = 0f },
						new Effect { Name = "Random",      UnlockRep = 3470, Probability = 0.6f, DollarMult = 0f }
					}
				}
			};

		public override List<Shipping> Shippings { get; protected set; } =
			new List<Shipping>
			{
				new Shipping
				{
					Name = "Lydia's Secure Freight",
					Cost = 0,
					UnlockRep = 0,
					MinAmount = 60,
					StepAmount = 1,
					MaxAmount = 100,
					DealModifier = new List<float> { 1f, 1f, 1f, 1f }
				},
				new Shipping
				{
					Name = "Madrigal Global Logistics",
					Cost = 40000,
					UnlockRep = 520,
					MinAmount = 75,
					StepAmount = 5,
					MaxAmount = 150,
					DealModifier = new List<float> { 1.25f, 1.25f, 1.25f, 1.25f }
				},
				new Shipping
				{
					Name = "Stevia Sweetener Containers",
					Cost = 50000,
					UnlockRep = 1170,
					MinAmount = 90,
					StepAmount = 10,
					MaxAmount = 150,
					DealModifier = new List<float> { 1.5f, 1.5f, 1.5f, 1.5f }
				},
				new Shipping
				{
					Name = "Trans-Pacific Cargo Lines",
					Cost = 60000,
					UnlockRep = 2020,
					MinAmount = 120,
					StepAmount = 20,
					MaxAmount = 200,
					DealModifier = new List<float> { 1.75f, 1.75f, 1.75f, 1.75f }
				},
				new Shipping
				{
					Name = "Quayle's Express Delivery",
					Cost = 80000,
					UnlockRep = 4050,
					MinAmount = 140,
					StepAmount = 20,
					MaxAmount = 260,
					DealModifier = new List<float> { 2f, 2f, 2f, 2f }
				}
			};

		public override Dialogue EmpireDialogue { get; protected set; } =
			new Dialogue
			{
				Intro = new List<string>
				{
					"Okay, hi. Let's make this quick. Is everything secure? Are you sure?"
				},
				DealStart = new List<string>
				{
					"I require {amount} of {quality} {product} with required effects: {effects} and optional effects: {optionalEffects}. Ensure perfection.",
					"Deliver {amount} {quality} {product} that includes required effects: {effects} and optional effects: {optionalEffects}.",
					"Provide {amount} {quality} {product} featuring required effects: {effects} and optional effects: {optionalEffects}."
				},
				Accept = new List<string>
				{
					"Good, let’s move forward.",
					"This meets my needs, proceed."
				},
				Incomplete = new List<string>
				{
					"No, no, no! This isn't right! It was supposed to be {amount}! Where's the rest? This is a disaster!",
					"This is only part of the shipment! I need {amount} total! My contacts will not be happy!",
					"Incomplete! The full order was {amount}! Find the rest, please! Urgently!"
				},
				Expire = new List<string>
				{
					"You're too late! The window closed! This is going to cause so many problems!",
					"No! The timing was critical! You missed it! I have to go."
				},
				Fail = new List<string>
				{
					"This is wrong! Contaminated! Compromised! Get it away from me! This is a nightmare!",
					"You've ruined everything! Do you know the consequences? This is unacceptable!"
				},
				Success = new List<string>
				{
					"Okay... okay, it looks right. Is it sealed properly?",
					"Thank god. Appears correct. Let's move it.",
					"Finally. Just... get it loaded."
				},
				Reward = new List<string>
				{
					"The payment is processed. Just... be careful. {dollar}",
					"Funds transferred. Ensure traceability is zero. {dollar}",
					"Okay, money sent. Don't call me again unless necessary. {dollar}"
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