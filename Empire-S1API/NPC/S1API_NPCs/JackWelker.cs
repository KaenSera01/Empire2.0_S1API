using Empire.DebtHelpers;
using Empire.NPC.Data;
using System.Collections.Generic;

namespace Empire.NPC.S1API_NPCs
{
	public class JackWelker : EmpireNPC
	{
		public override string DealerId => "jack_welker";
		public override string FirstName => "Jack";
		public override string LastName => "Welker";
		public override string DisplayName => "Jack Welker";
		public override int Tier => 3;

		public override List<UnlockRequirement> UnlockRequirements { get; protected set; } =
			new List<UnlockRequirement>
			{
				new UnlockRequirement { Name = "Todd Alquist", MinRep = 1313 }
			};

		public override List<string> DealDays { get; protected set; } =
			new List<string> { "Wednesday", "Thursday", "Saturday" };

		public override bool CurfewDeal { get; protected set; } = true;

		public override List<List<float>> Deals { get; protected set; } =
			new List<List<float>>
			{
				new List<float> { 3f, 1.4f, 87516f, 398f },
				new List<float> { 4f, 1.2f, 98456f, 442f },
				new List<float> { 5f, 1.0f, 109395f, 486f }
			};

		public override int RefreshCost { get; protected set; } = 1000;

		public override DealerReward Reward { get; protected set; } =
			new DealerReward
			{
				unlockRep = 300,
				RepCost = 200,
				Type = "console",
				Args = new List<string> { "sethealth", "200" }
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
					BaseDollar = 51,
					BaseRep = 241,
					BaseXp = 211,
					RepMult = 0.002f,
					XpMult = 0.001f,

					Qualities = new List<Quality>
					{
						new Quality { Type = "standard", DollarMult = 0f, UnlockRep = 0 },
						new Quality { Type = "premium",  DollarMult = 0f, UnlockRep = 350 }
					},

					Effects = new List<Effect>
					{
						new Effect { Name = "Toxic",       UnlockRep = 0,    Probability = 2.0f, DollarMult = 0f },
						new Effect { Name = "Explosive",   UnlockRep = 670,  Probability = 2.0f, DollarMult = 0f },
						new Effect { Name = "Smelly",      UnlockRep = 1620, Probability = 1.0f, DollarMult = 0f },
						new Effect { Name = "Disorienting",UnlockRep = 2920, Probability = 1.0f, DollarMult = 0f },
						new Effect { Name = "Random",      UnlockRep = 3570, Probability = 0.3f, DollarMult = 0f },
						new Effect { Name = "Random",      UnlockRep = 4320, Probability = 0.6f, DollarMult = 0f }
					}
				}
			};

		public override List<Shipping> Shippings { get; protected set; } =
			new List<Shipping>
			{
				new Shipping
				{
					Name = "Jack's Free Compound Run",
					Cost = 0,
					UnlockRep = 0,
					MinAmount = 60,
					StepAmount = 1,
					MaxAmount = 100,
					DealModifier = new List<float> { 1f, 1f, 1f, 1f }
				},
				new Shipping
				{
					Name = "Compound Supply Run",
					Cost = 65000,
					UnlockRep = 1170,
					MinAmount = 100,
					StepAmount = 5,
					MaxAmount = 200,
					DealModifier = new List<float> { 1.25f, 1.25f, 1.25f, 1.25f }
				},
				new Shipping
				{
					Name = "Aryan Brotherhood Freight",
					Cost = 100000,
					UnlockRep = 2320,
					MinAmount = 130,
					StepAmount = 10,
					MaxAmount = 300,
					DealModifier = new List<float> { 1.5f, 1.5f, 1.5f, 1.5f }
				},
				new Shipping
				{
					Name = "Welker's Final Dispatch",
					Cost = 125000,
					UnlockRep = 5170,
					MinAmount = 160,
					StepAmount = 20,
					MaxAmount = 400,
					DealModifier = new List<float> { 1.75f, 1.75f, 1.75f, 1.75f }
				}
			};

		public override Dialogue EmpireDialogue { get; protected set; } =
			new Dialogue
			{
				Intro = new List<string>
				{
					"Well now, lookie here. Todd says you're reliable. Let's hope so."
				},
				DealStart = new List<string>
				{
					"I require {amount} of {quality} {product} with required effects: {effects} and optional effects: {optionalEffects}.",
					"Provide {amount} {quality} {product} that carries required effects: {effects} and optional effects: {optionalEffects}.",
					"Deliver {amount} {quality} {product} including required effects: {effects} and optional effects: {optionalEffects}."
				},
				Accept = new List<string>
				{
					"Good. Get it done.",
					"Alright. Don't disappoint."
				},
				Incomplete = new List<string>
				{
					"Hey! Where's the rest? Thought we had a deal for {amount}!",
					"You short? That ain't smart. Need the full {amount}.",
					"This ain't {amount}. You tryin' to pull somethin'?"
				},
				Expire = new List<string>
				{
					"Took your damn time. We handled it. Get lost.",
					"Too slow. We don't wait around. Deal's off."
				},
				Fail = new List<string>
				{
					"What is this trash? You think we're stupid? Big mistake.",
					"This ain't the quality we paid for. You got a problem now."
				},
				Success = new List<string>
				{
					"Alright. Looks good.",
					"Yeah, this'll work.",
					"Solid."
				},
				Reward = new List<string>
				{
					"Here's your money. Now beat it. {dollar}",
					"Payment. Don't come around unless you're called. {dollar}",
					"Take it. We're done here. {dollar}"
				}
			};

		public override Gift? Gift { get; protected set; } =
			new Gift
			{
				Cost = 3000,
				Rep = 60
			};

		public override Debt? Debt { get; protected set; } = null;

		public override DebtManager? DebtManager { get; set; }
	}
}