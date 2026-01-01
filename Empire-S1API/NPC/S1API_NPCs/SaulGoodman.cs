using Empire.DebtHelpers;
using Empire.NPC.Data;
using System.Collections.Generic;

namespace Empire.NPC.S1API_NPCs
{
	public class SaulGoodman : EmpireNPC
	{
		public override string DealerId => "saul_goodman";
		public override string FirstName => "Saul";
		public override string LastName => "Goodman";
		public override string DisplayName => "Saul Goodman";
		public override int Tier => 2;

		public override List<UnlockRequirement> UnlockRequirements { get; protected set; } =
			new List<UnlockRequirement>
			{
				new UnlockRequirement { Name = "Skinny Pete", MinRep = 100 }
			};

		public override List<string> DealDays { get; protected set; } =
			new List<string> { "Monday", "Tuesday", "Friday", "Saturday" };

		public override bool CurfewDeal { get; protected set; } = true;

		public override List<List<float>> Deals { get; protected set; } =
			new List<List<float>>
			{
				new List<float> { 2f, 0.85f, 6076f, 20f },
				new List<float> { 3f, 0.7f, 8022f, 30f }
			};

		public override int RefreshCost { get; protected set; } = 500;

		public override DealerReward Reward { get; protected set; } =
			new DealerReward
			{
				unlockRep = 200,
				RepCost = 100,
				Type = "console",
				Args = new List<string> { "setlawintensity", "1" }
			};

		public override float RepLogBase { get; protected set; } = 4f;

		public override List<Drug> Drugs { get; protected set; } =
			new List<Drug>
			{
                // Weed
                new Drug
				{
					Type = "weed",
					UnlockRep = 0,
					BaseDollar = 17,
					BaseRep = 26,
					BaseXp = 17,
					RepMult = 0.004f,
					XpMult = 0.001f,

					Qualities = new List<Quality>
					{
						new Quality { Type = "poor",     DollarMult = 0f, UnlockRep = 0 },
						new Quality { Type = "standard", DollarMult = 0f, UnlockRep = 150 }
					},

					Effects = new List<Effect>
					{
						new Effect { Name = "Sneaky",      UnlockRep = 0,   Probability = 2.0f, DollarMult = 0f },
						new Effect { Name = "Random",      UnlockRep = 40,  Probability = 1.2f, DollarMult = 0f },
						new Effect { Name = "Laxative",    UnlockRep = 380, Probability = 1.0f, DollarMult = 0f },
						new Effect { Name = "Random",      UnlockRep = 475, Probability = 0.3f, DollarMult = 0f },
						new Effect { Name = "Random",      UnlockRep = 590, Probability = 0.6f, DollarMult = 0f }
					}
				},

                // Cocaine
                new Drug
				{
					Type = "cocaine",
					UnlockRep = 90,
					BaseDollar = 71,
					BaseRep = 17,
					BaseXp = 18,
					RepMult = 0.001f,
					XpMult = 0.001f,

					Qualities = new List<Quality>
					{
						new Quality { Type = "poor",     DollarMult = 0f, UnlockRep = 90 },
						new Quality { Type = "standard", DollarMult = 0f, UnlockRep = 345 }
					},

					Effects = new List<Effect>
					{
						new Effect { Name = "Electrifying", UnlockRep = 90,  Probability = 2.0f, DollarMult = 0f },
						new Effect { Name = "Random",       UnlockRep = 265, Probability = 1.2f, DollarMult = 0f },
						new Effect { Name = "Refreshing",   UnlockRep = 435, Probability = 1.0f, DollarMult = 0f },
						new Effect { Name = "Random",       UnlockRep = 540, Probability = 0.3f, DollarMult = 0f },
						new Effect { Name = "Random",       UnlockRep = 665, Probability = 0.6f, DollarMult = 0f }
					}
				}
			};

		public override List<Shipping> Shippings { get; protected set; } =
			new List<Shipping>
			{
				new Shipping
				{
					Name = "Legal Document Courier",
					Cost = 0,
					UnlockRep = 0,
					MinAmount = 1,
					StepAmount = 1,
					MaxAmount = 15,
					DealModifier = new List<float> { 1f, 1f, 1f, 1f }
				},
				new Shipping
				{
					Name = "Saul's Sleek Shuttle",
					Cost = 7500,
					UnlockRep = 220,
					MinAmount = 10,
					StepAmount = 5,
					MaxAmount = 30,
					DealModifier = new List<float> { 1.25f, 1.25f, 1.25f, 1.25f }
				},
				new Shipping
				{
					Name = "Goodman's Gold Standard Transport",
					Cost = 15000,
					UnlockRep = 750,
					MinAmount = 20,
					StepAmount = 10,
					MaxAmount = 50,
					DealModifier = new List<float> { 1.5f, 1.5f, 1.5f, 1.5f }
				}
			};

		public override Dialogue EmpireDialogue { get; protected set; } =
			new Dialogue
			{
				Intro = new List<string>
				{
					"Alright, let's talk turkey! Or... whatever you're slingin'. Whatcha got for ol' Saul?"
				},
				DealStart = new List<string>
				{
					"Okay, pal, I need {amount} of {quality} {product} with required effects: {effects} and optional effects: {optionalEffects}.",
					"Deliver {amount} {quality} {product} ensuring it carries required effects: {effects} and optional effects: {optionalEffects}.",
					"Make it so that {amount} {quality} {product} comes with required effects: {effects} and optional effects: {optionalEffects}."
				},
				Accept = new List<string>
				{
					"Done deal! Pleasure doin' business!",
					"Alright! Let's make some money!"
				},
				Incomplete = new List<string>
				{
					"Whoa there, Speedy Gonzales! Where's the rest? The full {amount}! My client's waiting!",
					"Hey! The deal was for {amount}! Don't nickel and dime me!",
					"This ain't gonna cut it! Need the other {amount}! Chop chop!"
				},
				Expire = new List<string>
				{
					"Time's up, pal! You blew it! Opportunity knocked, and you were nappin'!",
					"Too slow! My client went elsewhere! You snooze, you lose!"
				},
				Fail = new List<string>
				{
					"What is this garbage?! You trying to get me disbarred?! Get outta my office!",
					"This is amateur hour! You call this product?! You're fired!"
				},
				Success = new List<string>
				{
					"Alright! Looks good enough for government work... or my client.",
					"Yeah, this'll do. Nice.",
					"Perfecto! Another satisfied customer... soon."
				},
				Reward = new List<string>
				{
					"Here's your cut, minus my fee, of course. ${dollar} Don't spend it all in one place!",
					"Payment sent! {dollar} Remember who to call when you're in a jam!",
					"Cash money! Better call Saul! {dollar}"
				}
			};

		public override Gift? Gift { get; protected set; } =
			new Gift
			{
				Cost = 2000,
				Rep = 40
			};

		public override Debt? Debt { get; protected set; } = new Debt(); // No debt in JSON

		public override DebtManager? DebtManager { get; set; }
	}
}