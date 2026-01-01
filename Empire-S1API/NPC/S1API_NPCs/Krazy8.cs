using Empire.DebtHelpers;
using Empire.NPC.Data;
using MelonLoader;
using System;
using System.Collections.Generic;

namespace Empire.NPC.S1API_NPCs
{
	public class Krazy8 : EmpireNPC
	{
		public override string DealerId => "krazy8";
		public override string FirstName => "Domingo";
		public override string LastName => "Molina";
		public override string DisplayName => "Domingo 'Krazy-8' Molina";
		public override int Tier => 2;

		public override List<UnlockRequirement> UnlockRequirements { get; protected set; } =
			new List<UnlockRequirement>
			{
				new UnlockRequirement
				{
					Name = "Combo Costco",
					MinRep = 100
				}
			};

		public override List<string> DealDays { get; protected set; } =
			new List<string> { "Monday", "Wednesday", "Friday", "Saturday" };

		public override bool CurfewDeal { get; protected set; } = false;

		public override List<List<float>> Deals { get; protected set; } =
			new List<List<float>>
			{
				new List<float> { 2f, 0.85f, 8186f, 50f },
				new List<float> { 4f, 0.7f, 10783f, 75f }
			};

		public override int RefreshCost { get; protected set; } = 500;

		public override DealerReward Reward { get; protected set; } =
			new DealerReward
			{
				unlockRep = 200,
				RepCost = 100,
				Type = "console",
				Args = new List<string> { "spawnvehicle", "shitbox" }
			};

		public override float RepLogBase { get; protected set; } = 4f;

		public override List<Drug> Drugs { get; protected set; } =
			new List<Drug>
			{
				new Drug
				{
					Type = "weed",
					UnlockRep = 0,
					BaseDollar = 17,
					BaseRep = 42,
					BaseXp = 31,
					RepMult = 0.002f,
					XpMult = 0.001f,
					Qualities = new List<Quality>
					{
						new Quality { Type = "poor",     DollarMult = 0f, UnlockRep = 0 },
						new Quality { Type = "standard", DollarMult = 0f, UnlockRep = 70 }
					},
					Effects = new List<Effect>
					{
						new Effect { Name = "Gingeritis",     UnlockRep = 0,   Probability = 2.0f, DollarMult = 0f },
						new Effect { Name = "Random",         UnlockRep = 130, Probability = 1.2f, DollarMult = 0f },
						new Effect { Name = "CalorieDense",   UnlockRep = 375, Probability = 1.0f, DollarMult = 0f }
					}
				}
			};

		public override List<Shipping> Shippings { get; protected set; } =
			new List<Shipping>
			{
				new Shipping
				{
					Name = "Krazy's Street Courier",
					Cost = 0,
					UnlockRep = 0,
					MinAmount = 1,
					StepAmount = 1,
					MaxAmount = 10,
					DealModifier = new List<float> { 1f, 1f, 1f, 1f }
				},
				new Shipping
				{
					Name = "Molina's Muscle Van",
					Cost = 5000,
					UnlockRep = 135,
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
					"Yo! You Combo’s guy? Heard you’re moving weight. Let’s see what you got."
				},
				DealStart = new List<string>
				{
					"I need {amount} of {quality} {product} with required effects: {effects} and optional effects: {optionalEffects}.",
					"Bring me {amount} {quality} {product}. Required effects: {effects}, optional: {optionalEffects}.",
					"Get me {amount} {quality} {product} with {effects} and maybe {optionalEffects}."
				},
				Accept = new List<string>
				{
					"Alright, let’s do this.",
					"Cool. I’m in."
				},
				Incomplete = new List<string>
				{
					"Yo, this ain’t the full {amount}. Don’t short me.",
					"Need {amount} total. You’re missing some.",
					"Where’s the rest? I asked for {amount}."
				},
				Expire = new List<string>
				{
					"Too late. Deal’s off.",
					"You missed the window. I’m out."
				},
				Fail = new List<string>
				{
					"This is trash. You trying to get me busted?",
					"Nah, this won’t fly. Get lost."
				},
				Success = new List<string>
				{
					"Looks good. Nice work.",
					"Yeah, this’ll move.",
					"Solid. I’ll take it."
				},
				Reward = new List<string>
				{
					"Here’s your cut. ${dollar}. Keep it coming.",
					"Payment sent. ${dollar}. Good hustle.",
					"You earned it. ${dollar}. Respect."
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
				TotalDebt = 10000,
				InterestRate = 0.1f,
				DayMultiple = 10,
				DayExponent = 2.0f,
				ProductBonus = 2.0f
			};

		public override DebtManager? DebtManager { get; set; }
	}
}