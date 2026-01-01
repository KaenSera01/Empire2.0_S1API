using Empire.DebtHelpers;
using Empire.NPC.Data;
using System.Collections.Generic;

namespace Empire.NPC.S1API_NPCs
{
	public class MrBadger : EmpireNPC
	{
		public override string DealerId => "mr_badger";
		public override string FirstName => "Mr.";
		public override string LastName => "Badger";
		public override string DisplayName => "Mr. Badger";
		public override int Tier => 1;

		public override List<UnlockRequirement> UnlockRequirements { get; protected set; } =
			new List<UnlockRequirement>(); // Starts unlocked

		public override List<string> DealDays { get; protected set; } =
			new List<string> { "Sunday", "Monday", "Thursday", "Friday" };

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
				Args = new List<string> { "give", "meth", "10" }
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
					RepMult = 0.002f,
					XpMult = 0.005f,

					Qualities = new List<Quality>
					{
						new Quality { Type = "poor",     DollarMult = 0f, UnlockRep = 0 },
						new Quality { Type = "standard", DollarMult = 0f, UnlockRep = 65 }
					},

					Effects = new List<Effect>
					{
						new Effect { Name = "Paranoia",   UnlockRep = 0,   Probability = 2.0f, DollarMult = 0f },
						new Effect { Name = "Focused",    UnlockRep = 38,  Probability = 1.0f, DollarMult = 0f },
						new Effect { Name = "Random",     UnlockRep = 80,  Probability = 0.3f, DollarMult = 0f },
						new Effect { Name = "Random",     UnlockRep = 105, Probability = 0.6f, DollarMult = 0f }
					}
				}
			};

		public override List<Shipping> Shippings { get; protected set; } =
			new List<Shipping>
			{
				new Shipping
				{
					Name = "Badger's Quick Courier",
					Cost = 0,
					UnlockRep = 0,
					MinAmount = 1,
					StepAmount = 1,
					MaxAmount = 10,
					DealModifier = new List<float> { 1f, 1f, 1f, 1f }
				},
				new Shipping
				{
					Name = "Badger's Rough Ride",
					Cost = 5000,
					UnlockRep = 142,
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
					"Dude! You're like, the source? Awesome! Combo said you were the man."
				},
				DealStart = new List<string>
				{
					"Alright, I need {amount} of {quality} {product} with required effects: {effects} and optional effects: {optionalEffects}. Don't mess this up.",
					"Listen, deliver {amount} {quality} {product} that includes required effects: {effects} and optional effects: {optionalEffects}.",
					"Get {amount} {quality} {product} featuring required effects: {effects} and optional effects: {optionalEffects} immediately."
				},
				Accept = new List<string>
				{
					"Awesome, let’s do this!",
					"Sweet, I’m totally in!"
				},
				Incomplete = new List<string>
				{
					"Aw, man! That's all? I told 'em {amount} total! Bring the rest, dude!",
					"Hold up, only this much? Need {amount} for the full experience!",
					"Where's the other {amount}, bro? Beam it over!"
				},
				Expire = new List<string>
				{
					"Dude, you took longer than warp speed. The moment's passed, man.",
					"Aw, beans. Too late. We found... alternative entertainment."
				},
				Fail = new List<string>
				{
					"Whoa, bad trip, man! This is not what I ordered! You gotta fix this!",
					"Red alert! This is all wrong! Mission aborted!"
				},
				Success = new List<string>
				{
					"Righteous! This looks stellar.",
					"Awesome! To infinity... and beyond!",
					"Heh heh, nice. Party time."
				},
				Reward = new List<string>
				{
					"Here's the cash, man. You earned ${dollar}. Totally worth it.",
					"Payment beamed. {dollar} Dollars. Live long and prosper.",
					"Got the eddies. {dollar} incoming. Rock on."
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