using Empire.DebtHelpers;
using Empire.NPC.Data;
using System.Collections.Generic;

namespace Empire.NPC.S1API_NPCs
{
	public class ComboCostco : EmpireNPC
	{
		public override string DealerId => "combo_costco";
		public override string FirstName => "Combo";
		public override string LastName => "Costco";
		public override string DisplayName => "Combo Costco";
		public override int Tier => 1;

		// No unlock requirements in JSON
		public override List<UnlockRequirement> UnlockRequirements { get; protected set; } =
			new List<UnlockRequirement>();

		public override List<string> DealDays { get; protected set; } =
			new List<string> { "Monday", "Tuesday", "Wednesday", "Thursday" };

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
				Args = new List<string> { "give", "cocaine", "5" }
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
					RepMult = 0.001f,
					XpMult = 0.001f,
					Qualities = new List<Quality>
					{
						new Quality { Type = "poor",     DollarMult = 0f, UnlockRep = 0 },
						new Quality { Type = "standard", DollarMult = 0f, UnlockRep = 58 }
					},
					Effects = new List<Effect>
					{
						new Effect { Name = "Energizing", UnlockRep = 0,   Probability = 2.0f, DollarMult = 0f },
						new Effect { Name = "Euphoric",   UnlockRep = 33,  Probability = 1.0f, DollarMult = 0f },
						new Effect { Name = "Random",     UnlockRep = 78,  Probability = 0.3f, DollarMult = 0f },
						new Effect { Name = "Random",     UnlockRep = 100, Probability = 0.6f, DollarMult = 0f }
					}
				}
			};

		public override List<Shipping> Shippings { get; protected set; } =
			new List<Shipping>
			{
				new Shipping
				{
					Name = "BMX Brigade",
					Cost = 0,
					UnlockRep = 0,
					MinAmount = 1,
					StepAmount = 1,
					MaxAmount = 10,
					DealModifier = new List<float> { 1f, 1f, 1f, 1f }	
				},
				new Shipping
				{
					Name = "Uncle Nelson's Loaner",
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
					"Yo, word on the street is you're the supplier. Uncle Nelsons kid, yeah? You holdin'?"
				},
				DealStart = new List<string>
				{
					"Alright, check it, I need {amount} of {quality} {product} with required effects: {effects} and optional effects: {optionalEffects}. Got it?",
					"Look, my people need {amount} of {quality} {product}. Ensure it has required effects: {effects} and optional effects: {optionalEffects}. You strapped?",
					"Yo, listen up, {amount} {quality} {product}. It must deliver required effects: {effects} along with optional effects: {optionalEffects}. Don't waste my time."
				},
				Accept = new List<string>
				{
					"Deal’s on, man!",
					"Cool, let’s roll with it!"
				},
				Incomplete = new List<string>
				{
					"Nah man, this ain't it. Need the full {amount}. Step it up.",
					"Yo, where's the rest? Need {amount} total, bro.",
					"This ain't gonna cut it. Bring the remaining {amount}."
				},
				Expire = new List<string>
				{
					"Too slow, man. My corner's dry now. Deal's off.",
					"You took your sweet time, huh? Lost the sale. We're done."
				},
				Fail = new List<string>
				{
					"What the hell is this? You trying to get me popped? Get outta here!",
					"You messed up BAD. This ain't gonna fly. Lose my number."
				},
				Success = new List<string>
				{
					"Alright, looks good. Solid.",
					"Yeah, this is the stuff. Nice.",
					"Finally. This'll move."
				},
				Reward = new List<string>
				{
					"Here's your cut of ${dollar}, man. Keep it coming.",
					"Cash is here -> ${dollar}. Good business.",
					"Payment of ${dollar} sent. Don't get sloppy."
				}
			};

		public override Gift? Gift { get; protected set; } =
			new Gift
			{
				Cost = 1000,
				Rep = 20
			};

		public override DebtManager? DebtManager { get; set; }

		public override Debt? Debt { get; protected set; } =
			new Debt();

	}
}