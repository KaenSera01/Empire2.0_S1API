using Empire.DebtHelpers;
using Empire.NPC.Data;
using MelonLoader;
using System.Collections.Generic;

namespace Empire.NPC.S1API_NPCs
{
	public class SkinnyPete : EmpireNPC
	{
		public override string DealerId => "skinny_pete";
		public override string FirstName => "Skinny";
		public override string LastName => "Pete";
		public override string DisplayName => "Skinny Pete";
		public override int Tier => 1;

		public override List<UnlockRequirement> UnlockRequirements { get; protected set; } =
			new List<UnlockRequirement>(); // Starts unlocked

		public override List<string> DealDays { get; protected set; } =
			new List<string> { "Monday", "Wednesday", "Friday", "Saturday" };

		public override bool CurfewDeal { get; protected set; } = false;

		public override List<List<float>> Deals { get; protected set; } =
			new List<List<float>>
			{
				new List<float> { 1f, 0.85f, 941f, 8f },
				new List<float> { 2f, 0.7f, 1112f, 12f }
			};

		public override int RefreshCost { get; protected set; } = 200;

		public override DealerReward Reward { get; protected set; } =
			new DealerReward
			{
				unlockRep = 100,
				RepCost = 40,
				Type = "console",
				Args = new List<string> { "give", "ogkush", "20" }
			};

		public override float RepLogBase { get; protected set; } = 3f;

		public override List<Drug> Drugs { get; protected set; } =
			new List<Drug>
			{
				new Drug
				{
					Type = "weed",
					UnlockRep = 0,
					BaseDollar = 10,
					BaseRep = 9,
					BaseXp = 6,
					RepMult = 0.004f,
					XpMult = 0.005f,

					Qualities = new List<Quality>
					{
						new Quality { Type = "poor",     DollarMult = 0f, UnlockRep = 0 },
						new Quality { Type = "standard", DollarMult = 0f, UnlockRep = 60 }
					},

					Effects = new List<Effect>
					{
						new Effect { Name = "Munchies",   UnlockRep = 0,   Probability = 2.0f, DollarMult = 0f },
						new Effect { Name = "Calming",    UnlockRep = 35,  Probability = 1.0f, DollarMult = 0f },
						new Effect { Name = "Random",     UnlockRep = 88,  Probability = 0.3f, DollarMult = 0f },
						new Effect { Name = "Random",     UnlockRep = 110, Probability = 0.6f, DollarMult = 0f }
					}
				}
			};

		public override List<Shipping> Shippings { get; protected set; } =
			new List<Shipping>
			{
				new Shipping
				{
					Name = "Skinny's Slick Delivery",
					Cost = 0,
					UnlockRep = 0,
					MinAmount = 1,
					StepAmount = 1,
					MaxAmount = 10,
					DealModifier = new List<float> { 1f, 1f, 1f, 1f }
				},
				new Shipping
				{
					Name = "Pete's Chopper Express",
					Cost = 5000,
					UnlockRep = 143,
					MinAmount = 5,
					StepAmount = 5,
					MaxAmount = 20,
					DealModifier = new List<float> { 1.25f, 1.25f, 1.25f, 1.25f }
				}
			};

		public override Dialogue EmpireDialogue { get; protected set; } =
			new Dialogue
			{
				Intro = new List<string>
				{
					"Yo, what's up? Heard about you from my bros. You got the stash?"
				},
				DealStart = new List<string>
				{
					"Yo, I require {amount} of {quality} {product} with required effects: {effects} and optional effects: {optionalEffects}.",
					"Make sure {amount} {quality} {product} comes with required effects: {effects} and optional effects: {optionalEffects}.",
					"Provide {amount} {quality} {product} featuring required effects: {effects} and optional effects: {optionalEffects}."
				},
				Accept = new List<string>
				{
					"Sweet, let’s roll, man!",
					"I’m down, let’s do it!"
				},
				Incomplete = new List<string>
				{
					"Yo, hold up. Where's the rest? Short by {amount}. Can't be doin' that.",
					"Nah, man, need the full {amount}. This ain't the whole symphony.",
					"This ain't the encore, man. Need the other {amount}."
				},
				Expire = new List<string>
				{
					"Time’s up, man, you’re out.",
					"Missed it, deal’s off."
				},
				Fail = new List<string>
				{
					"Dude, you blew it big time!",
					"That’s a total bust, yo!"
				},
				Success = new List<string>
				{
					"Word. Looks righteous.",
					"Yeah, that's the melody. Nice.",
					"Solid, man. Real solid."
				},
				Reward = new List<string>
				{
					"A'ight, here's the paper. ${dollar} straight up.",
					"Payday, yo. {dollar} in your pocket. Keep it tight.",
					"Money's there. {dollar}. Respect."
				}
			};

		public override Gift? Gift { get; protected set; } =
			new Gift
			{
				Cost = 1000,
				Rep = 20
			};

		public override Debt? Debt { get; protected set; } =
			new Debt(); // No debt in JSON

		public override DebtManager? DebtManager { get; set; }
	}
}