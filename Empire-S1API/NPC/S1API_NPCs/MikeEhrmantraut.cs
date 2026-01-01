using Empire.DebtHelpers;
using Empire.NPC.Data;
using System.Collections.Generic;

namespace Empire.NPC.S1API_NPCs
{
	public class MikeEhrmantraut : EmpireNPC
	{
		public override string DealerId => "mike_ehrmantraut";
		public override string FirstName => "Mike";
		public override string LastName => "Ehrmantraut";
		public override string DisplayName => "Mike Ehrmantraut";
		public override int Tier => 2;

		public override List<UnlockRequirement> UnlockRequirements { get; protected set; } =
			new List<UnlockRequirement>
			{
				new UnlockRequirement { Name = "Saul Goodman", MinRep = 250 }
			};

		public override List<string> DealDays { get; protected set; } =
			new List<string> { "Tuesday", "Wednesday", "Thursday", "Sunday" };

		public override bool CurfewDeal { get; protected set; } = true;

		public override List<List<float>> Deals { get; protected set; } =
			new List<List<float>>
			{
				new List<float> { 2f, 0.85f, 22842f, 65f },
				new List<float> { 4f, 0.7f, 33036f, 88f }
			};

		public override int RefreshCost { get; protected set; } = 500;

		public override DealerReward Reward { get; protected set; } =
			new DealerReward
			{
				unlockRep = 200,
				RepCost = 50,
				Type = "console",
				Args = new List<string> { "save" }
			};

		public override float RepLogBase { get; protected set; } = 4f;

		public override List<Drug> Drugs { get; protected set; } =
			new List<Drug>
			{
                // Meth
                new Drug
				{
					Type = "meth",
					UnlockRep = 0,
					BaseDollar = 40,
					BaseRep = 50,
					BaseXp = 39,
					RepMult = 0.002f,
					XpMult = 0.001f,

					Qualities = new List<Quality>
					{
						new Quality { Type = "poor",     DollarMult = 0f, UnlockRep = 0 },
						new Quality { Type = "standard", DollarMult = 0f, UnlockRep = 275 }
					},

					Effects = new List<Effect>
					{
						new Effect { Name = "Focused",     UnlockRep = 0,   Probability = 2.0f, DollarMult = 0f },
						new Effect { Name = "Random",      UnlockRep = 75,  Probability = 1.2f, DollarMult = 0f },
						new Effect { Name = "Smelly",      UnlockRep = 705, Probability = 1.0f, DollarMult = 0f },
						new Effect { Name = "Random",      UnlockRep = 890, Probability = 0.3f, DollarMult = 0f },
						new Effect { Name = "Random",      UnlockRep = 1115,Probability = 0.6f, DollarMult = 0f }
					}
				},

                // Cocaine
                new Drug
				{
					Type = "cocaine",
					UnlockRep = 165,
					BaseDollar = 74,
					BaseRep = 65,
					BaseXp = 41,
					RepMult = 0.001f,
					XpMult = 0.001f,

					Qualities = new List<Quality>
					{
						new Quality { Type = "poor",     DollarMult = 0f, UnlockRep = 165 },
						new Quality { Type = "standard", DollarMult = 0f, UnlockRep = 635 }
					},

					Effects = new List<Effect>
					{
						new Effect { Name = "Seizure",     UnlockRep = 165, Probability = 2.0f, DollarMult = 0f },
						new Effect { Name = "Random",      UnlockRep = 485, Probability = 1.2f, DollarMult = 0f },
						new Effect { Name = "Balding",     UnlockRep = 805, Probability = 1.0f, DollarMult = 0f },
						new Effect { Name = "Random",      UnlockRep = 1010,Probability = 0.3f, DollarMult = 0f },
						new Effect { Name = "Random",      UnlockRep = 1255,Probability = 0.6f, DollarMult = 0f }
					}
				}
			};

		public override List<Shipping> Shippings { get; protected set; } =
			new List<Shipping>
			{
				new Shipping
				{
					Name = "Mike's No-Nonsense Pickup",
					Cost = 0,
					UnlockRep = 0,
					MinAmount = 10,
					StepAmount = 1,
					MaxAmount = 30,
					DealModifier = new List<float> { 1f, 1f, 1f, 1f }
				},
				new Shipping
				{
					Name = "Ehrmantraut's Steady Ride",
					Cost = 20000,
					UnlockRep = 405,
					MinAmount = 20,
					StepAmount = 5,
					MaxAmount = 50,
					DealModifier = new List<float> { 1.25f, 1.25f, 1.25f, 1.25f }
				},
				new Shipping
				{
					Name = "Mike's Final Warning Haul",
					Cost = 35000,
					UnlockRep = 1425,
					MinAmount = 30,
					StepAmount = 10,
					MaxAmount = 90,
					DealModifier = new List<float> { 1.5f, 1.5f, 1.5f, 1.5f }
				}
			};

		public override Dialogue EmpireDialogue { get; protected set; } =
			new Dialogue
			{
				Intro = new List<string>
				{
					"You got somethin' for me? Let's make this professional."
				},
				DealStart = new List<string>
				{
					"I require {amount} of {quality} {product} with required effects: {effects} and optional effects: {optionalEffects}.",
					"Get me {amount} {quality} {product} that includes required effects: {effects} and optional effects: {optionalEffects}.",
					"Ensure {amount} {quality} {product} delivers required effects: {effects} and optional effects: {optionalEffects}."
				},
				Accept = new List<string>
				{
					"Good. Stick to the plan.",
					"Alright. Don't deviate."
				},
				Incomplete = new List<string>
				{
					"This isn't the full count. Where's the rest? {amount} was the number.",
					"Don't play stupid. You're short {amount}. Fix it.",
					"The job requires {amount}. This is partial. Get the rest."
				},
				Expire = new List<string>
				{
					"You missed the window. That's sloppy. Job's off.",
					"Too late. Lack of punctuality creates loose ends. We're done."
				},
				Fail = new List<string>
				{
					"This isn't what was agreed upon. Poor quality. Amateur mistake.",
					"You compromised the operation. That's unacceptable. Get lost."
				},
				Success = new List<string>
				{
					"Looks right. Professional.",
					"Okay. Meets spec.",
					"Solid work."
				},
				Reward = new List<string>
				{
					"Here's the payment. Earned it. {dollar}",
					"Funds transferred. Keep this level of competence. {dollar}",
					"Your cut. No half measures. {dollar}"
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