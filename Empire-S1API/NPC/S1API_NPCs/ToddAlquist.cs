using Empire.DebtHelpers;
using Empire.NPC.Data;
using System.Collections.Generic;

namespace Empire.NPC.S1API_NPCs
{
	public class ToddAlquist : EmpireNPC
	{
		public override string DealerId => "todd_alquist";
		public override string FirstName => "Todd";
		public override string LastName => "Alquist";
		public override string DisplayName => "Todd Alquist";
		public override int Tier => 2;

		public override List<UnlockRequirement> UnlockRequirements { get; protected set; } =
			new List<UnlockRequirement>
			{
				new UnlockRequirement { Name = "Mr. Badger", MinRep = 100 }
			};

		public override List<string> DealDays { get; protected set; } =
			new List<string> { "Monday", "Tuesday", "Thursday", "Sunday" };

		public override bool CurfewDeal { get; protected set; } = true;

		public override List<List<float>> Deals { get; protected set; } =
			new List<List<float>>
			{
				new List<float> { 2f, 1.2f, 25875f, 120f },
				new List<float> { 4f, 1.0f, 30375f, 143f }
			};

		public override int RefreshCost { get; protected set; } = 500;

		public override DealerReward Reward { get; protected set; } =
			new DealerReward
			{
				unlockRep = 200,
				RepCost = 50,
				Type = "console",
				Args = new List<string> { "give", "revolver" }
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
					BaseDollar = 37,
					BaseRep = 85,
					BaseXp = 71,
					RepMult = 0.002f,
					XpMult = 0.001f,

					Qualities = new List<Quality>
					{
						new Quality { Type = "poor",     DollarMult = 0f, UnlockRep = 0 },
						new Quality { Type = "standard", DollarMult = 0f, UnlockRep = 150 }
					},

					Effects = new List<Effect>
					{
						new Effect { Name = "Zombifying", UnlockRep = 0,    Probability = 2.0f, DollarMult = 0f },
						new Effect { Name = "Random",     UnlockRep = 280,  Probability = 1.2f, DollarMult = 0f },
						new Effect { Name = "Calming",    UnlockRep = 660,  Probability = 1.0f, DollarMult = 0f },
						new Effect { Name = "Random",     UnlockRep = 880,  Probability = 0.3f, DollarMult = 0f },
						new Effect { Name = "Random",     UnlockRep = 1130, Probability = 0.6f, DollarMult = 0f }
					}
				}
			};

		public override List<Shipping> Shippings { get; protected set; } =
			new List<Shipping>
			{
				new Shipping
				{
					Name = "Todd's Free Ride",
					Cost = 0,
					UnlockRep = 0,
					MinAmount = 20,
					StepAmount = 1,
					MaxAmount = 80,
					DealModifier = new List<float> { 1f, 1f, 1f, 1f }
				},
				new Shipping
				{
					Name = "El Camino Express",
					Cost = 40000,
					UnlockRep = 480,
					MinAmount = 40,
					StepAmount = 5,
					MaxAmount = 150,
					DealModifier = new List<float> { 1.25f, 1.25f, 1.25f, 1.25f }
				},
				new Shipping
				{
					Name = "Alquist's Heavy Haul",
					Cost = 50000,
					UnlockRep = 1480,
					MinAmount = 50,
					StepAmount = 10,
					MaxAmount = 200,
					DealModifier = new List<float> { 1.5f, 1.5f, 1.5f, 1.5f }
				}
			};

		public override Dialogue EmpireDialogue { get; protected set; } =
			new Dialogue
			{
				Intro = new List<string>
				{
					"Hey... Mr. White? Or, uh, you're the supplier? Okay. Right on."
				},
				DealStart = new List<string>
				{
					"I need {amount} of {quality} {product} with required effects: {effects} and optional effects: {optionalEffects}.",
					"Deliver {amount} {quality} {product} ensuring required effects: {effects} and optional effects: {optionalEffects}.",
					"Make sure {amount} {quality} {product} comes with required effects: {effects} and optional effects: {optionalEffects}."
				},
				Accept = new List<string>
				{
					"Okay. Cool.",
					"Right on. Let's do it."
				},
				Incomplete = new List<string>
				{
					"Uh... this isn't {amount}. Did something happen?",
					"Hey, uh, we need {amount} total. This is... less.",
					"Just need the other {amount}. No worries."
				},
				Expire = new List<string>
				{
					"Shoot. Took too long. Uncle Jack's not gonna like that.",
					"Ah, man. Missed the pickup. Sorry."
				},
				Fail = new List<string>
				{
					"Uh oh. This isn't right. That's... that's not good.",
					"This isn't the blue stuff. Or... whatever it's supposed to be. Gotta fix this."
				},
				Success = new List<string>
				{
					"Okay. Looks good.",
					"Nice. Thanks.",
					"Alright. Appreciate it."
				},
				Reward = new List<string>
				{
					"Here you go. Payment. {dollar}",
					"Okay, money's here. {dollar}",
					"This is yours. Right on. {dollar}"
				}
			};

		public override Gift? Gift { get; protected set; } =
			new Gift
			{
				Cost = 2000,
				Rep = 40
			};

		public override Debt? Debt { get; protected set; } = new Debt();

		public override DebtManager? DebtManager { get; set; }
	}
}