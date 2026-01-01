using Empire.DebtHelpers;
using Empire.NPC.Data;
using System.Collections.Generic;

namespace Empire.NPC.S1API_NPCs
{
	public class Heisenberg : EmpireNPC
	{
		public override string DealerId => "heisenberg";
		public override string FirstName => "The";
		public override string LastName => "Heisenberg";
		public override string DisplayName => "The Heisenberg";
		public override int Tier => 5;

		public override List<UnlockRequirement> UnlockRequirements { get; protected set; } =
			new List<UnlockRequirement>
			{
				new UnlockRequirement { Name = "Jesse Pinkman", MinRep = 2000 },
				new UnlockRequirement { Name = "Saul Goodman", MinRep = 1500 },
				new UnlockRequirement { Name = "Gustavo Fring", MinRep = 6900 },
				new UnlockRequirement { Name = "Don Eladio Vuente", MinRep = 4000 },
				new UnlockRequirement { Name = "Lydia Rodarte-Quayle", MinRep = 2000 }
			};

		public override List<string> DealDays { get; protected set; } =
			new List<string> { "Friday" };

		public override bool CurfewDeal { get; protected set; } = true;

		public override List<List<float>> Deals { get; protected set; } =
			new List<List<float>>
			{
				new List<float> { 5f, 1.6f, 315900f, 1361f },
				new List<float> { 6f, 1.3f, 340200f, 1490f },
				new List<float> { 7f, 1.0f, 364500f, 1620f }
			};

		public override int RefreshCost { get; protected set; } = 2500;

		public override DealerReward Reward { get; protected set; } =
			new DealerReward
			{
				unlockRep = 500,
				RepCost = 300,
				Type = "console",
				Args = new List<string> { "growplants" }
			};

		public override float RepLogBase { get; protected set; } = 7f;

		public override List<Drug> Drugs { get; protected set; } =
			new List<Drug>
			{
                // Meth
                new Drug
				{
					Type = "meth",
					UnlockRep = 0,
					BaseDollar = 80,
					BaseRep = 668,
					BaseXp = 624,
					RepMult = 0.002f,
					XpMult = 0.001f,

					Qualities = new List<Quality>
					{
						new Quality { Type = "heavenly", DollarMult = 0f, UnlockRep = 0 }
					},

					Effects = new List<Effect>
					{
						new Effect { Name = "Schizophrenic", UnlockRep = 0,     Probability = 2.0f, DollarMult = 0f },
						new Effect { Name = "AntiGravity",    UnlockRep = 1600,  Probability = 2.0f, DollarMult = 0f },
						new Effect { Name = "Random",         UnlockRep = 2400,  Probability = 2.0f, DollarMult = 0f },
						new Effect { Name = "Random",         UnlockRep = 4600,  Probability = 1.2f, DollarMult = 0f },
						new Effect { Name = "Jennerising",    UnlockRep = 7400,  Probability = 1.0f, DollarMult = 0f },
						new Effect { Name = "Cyclopean",      UnlockRep = 10500, Probability = 1.0f, DollarMult = 0f },
						new Effect { Name = "Random",         UnlockRep = 12100, Probability = 0.3f, DollarMult = 0f },
						new Effect { Name = "Random",         UnlockRep = 13850, Probability = 0.6f, DollarMult = 0f }
					}
				}
			};

		public override List<Shipping> Shippings { get; protected set; } =
			new List<Shipping>
			{
				new Shipping
				{
					Name = "Heisenberg's Free Courier",
					Cost = 0,
					UnlockRep = 0,
					MinAmount = 150,
					StepAmount = 1,
					MaxAmount = 200,
					DealModifier = new List<float> { 1f, 1f, 1f, 1f }
				},
				new Shipping
				{
					Name = "Aztek Express",
					Cost = 100000,
					UnlockRep = 1600,
					MinAmount = 200,
					StepAmount = 5,
					MaxAmount = 300,
					DealModifier = new List<float> { 1.25f, 1.25f, 1.25f, 1.25f }
				},
				new Shipping
				{
					Name = "Gray Matter Contract",
					Cost = 150000,
					UnlockRep = 3600,
					MinAmount = 250,
					StepAmount = 10,
					MaxAmount = 500,
					DealModifier = new List<float> { 1.5f, 1.5f, 1.5f, 1.5f }
				},
				new Shipping
				{
					Name = "Empire Logistics",
					Cost = 200000,
					UnlockRep = 6100,
					MinAmount = 300,
					StepAmount = 20,
					MaxAmount = 600,
					DealModifier = new List<float> { 1.75f, 1.75f, 1.75f, 1.75f }
				},
				new Shipping
				{
					Name = "Heisenberg's Premium Drop",
					Cost = 300000,
					UnlockRep = 9100,
					MinAmount = 400,
					StepAmount = 20,
					MaxAmount = 800,
					DealModifier = new List<float> { 2f, 2f, 2f, 2f }
				},
				new Shipping
				{
					Name = "The One Who Knocks Delivery",
					Cost = 500000,
					UnlockRep = 15650,
					MinAmount = 500,
					StepAmount = 40,
					MaxAmount = 1000,
					DealModifier = new List<float> { 2.5f, 2.5f, 2.5f, 2.5f }
				}
			};

		public override Dialogue EmpireDialogue { get; protected set; } =
			new Dialogue
			{
				Intro = new List<string>
				{
					"I am the one who knocks."
				},
				DealStart = new List<string>
				{
					"I require {amount} of my {quality} {product} with required effects: {effects} and optional effects: {optionalEffects}. Do not disappoint me.",
					"The empire demands {amount} {quality} {product} featuring required effects: {effects} and optional effects: {optionalEffects}.",
					"Prepare {amount} {quality} {product} ensuring it has required effects: {effects} and optional effects: {optionalEffects} flawlessly."
				},
				Accept = new List<string>
				{
					"Let’s proceed.",
					"We have a deal. Excellent."
				},
				Incomplete = new List<string>
				{
					"This is not the full quantity. {amount} was the order. You are testing my patience.",
					"Incomplete. My calculations were precise. You are short {amount}. Correct this.",
					"Fulfill the entire order - {amount}. Now."
				},
				Expire = new List<string>
				{
					"Time's up. You've failed to meet the schedule. This partnership is dissolved.",
					"You've lost your window. Inefficiency is a liability I won't tolerate."
				},
				Fail = new List<string>
				{
					"This product is compromised. Substandard. You are incompetent.",
					"This is unacceptable. You have failed. Stay out of my territory."
				},
				Success = new List<string>
				{
					"Exactly as specified. Excellent.",
					"The quality is satisfactory. Proceed with distribution.",
					"Correct. Your execution is adequate."
				},
				Reward = new List<string>
				{
					"Payment rendered. Maintain this standard. {dollar}",
					"The funds are transferred. Discretion is expected. {dollar}",
					"Your compensation. Remember who is in charge. {dollar}"
				}
			};

		public override Gift? Gift { get; protected set; } =
			new Gift
			{
				Cost = 5000,
				Rep = 100
			};

		public override Debt? Debt { get; protected set; } = null;

		public override DebtManager? DebtManager { get; set; }
	}
}