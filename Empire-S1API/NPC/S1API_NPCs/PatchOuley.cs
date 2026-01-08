using Empire.DebtHelpers;
using Empire.NPC.Data;
using System.Collections.Generic;

namespace Empire.NPC.S1API_NPCs
{
	public class PatchOuley : EmpireNPC
	{
		public override string DealerId => "patch_ouley";
		public override string FirstName => "Patch";
		public override string LastName => "Ouley";
		public override string DisplayName => "Patch Ouley";
		public override int Tier => 1;

		// No unlock requirements in JSON
		public override List<UnlockRequirement> UnlockRequirements { get; protected set; } =
			new List<UnlockRequirement>();

		public override List<string> DefaultDealDays { get; protected set; } =
			new List<string> { "Sunday", "Tuesday", "Wednesday", "Thursday", "Saturday" };

		public override List<string> ActiveDealDays { get; set; } =
			new List<string> { "Sunday", "Tuesday", "Wednesday", "Thursday", "Saturday" };

		public override bool CurfewDeal { get; protected set; } = false;

		//	Each Deals inner list: { dealTime, dealTimeMultipler, dollarPenalty, RepPenalty }
		public override List<List<float>> Deals { get; protected set; } =
			new List<List<float>>
			{
				new List<float> { 1f, 0.85f, 614f, 4f },
				new List<float> { 2f, 0.7f, 800f, 8f }
			};

		public override int RefreshCost { get; protected set; } = 200;

		public override DealerReward Reward { get; protected set; } =
			new DealerReward
			{
				unlockRep = 100,
				RepCost = 70,
				Type = "console",
				Args = new List<string> { "give", "shroom", "20" }
			};

		public override float RepLogBase { get; protected set; } = 3f;

		public override List<Drug> Drugs { get; protected set; } =
			new List<Drug>
			{
				new Drug
				{
					Type = "shrooms",
					UnlockRep = 0,
					BaseDollar = 14,
					BaseRep = 14,
					BaseXp = 9,
					RepMult = 0.001f,
					XpMult = 0.001f,
					Qualities = new List<Quality>
					{
						new Quality { Type = "standard", DollarMult = 0f, UnlockRep = 0 },
						new Quality { Type = "premium",  DollarMult = 0f, UnlockRep = 150 }
					},
					Effects = new List<Effect>
					{
						new Effect { Name = "ThoughtProvoking", UnlockRep = 0,   Probability = 2.0f, DollarMult = 0f },
						new Effect { Name = "Anti-Gravity",     UnlockRep = 75,  Probability = 1.0f, DollarMult = 0f },
						new Effect { Name = "Random",           UnlockRep = 150,  Probability = 0.3f, DollarMult = 0f },
						new Effect { Name = "Glowing",          UnlockRep = 200,  Probability = 0.6f, DollarMult = 0f }
					}
				},
				new Drug
				{
					Type = "weed",
					UnlockRep = 50,
					BaseDollar = 10,
					BaseRep = 9,
					BaseXp = 6,
					RepMult = 0.001f,
					XpMult = 0.001f,
					Qualities = new List<Quality>
					{
						new Quality { Type = "poor",     DollarMult = 0f, UnlockRep = 50 },
						new Quality { Type = "standard", DollarMult = 0f, UnlockRep = 100 }
					},
					Effects = new List<Effect>
					{
						new Effect { Name = "Sedating",   UnlockRep = 150,   Probability = 2.0f, DollarMult = 0f },
						new Effect { Name = "Euphoric",   UnlockRep = 125,  Probability = 1.0f, DollarMult = 0f },
						new Effect { Name = "Random",     UnlockRep = 175,  Probability = 0.3f, DollarMult = 0f },
						new Effect { Name = "Glowing",     UnlockRep = 300, Probability = 0.6f, DollarMult = 0f }
					}
				}
			};

		public override List<Shipping> Shippings { get; protected set; } =
			new List<Shipping>
			{
				new Shipping
				{
					Name = "Flower Power",
					Cost = 0,
					UnlockRep = 0,
					MinAmount = 2,
					StepAmount = 1,
					MaxAmount = 10,
					DealModifier = new List<float> { 1f, 1f, 1f, 1f }
				},
				new Shipping
				{
					Name = "Moonbeams and Peace",
					Cost = 5000,
					UnlockRep = 135,
					MinAmount = 5,
					StepAmount = 5,
					MaxAmount = 40,
					DealModifier = new List<float> { 1.25f, 1.25f, 1.25f, 1.25f }
				},
				new Shipping
				{
					Name = "The Moon's Blessed",
					Cost = 45000,
					UnlockRep = 325,
					MinAmount = 20,
					StepAmount = 10,
					MaxAmount = 100,
					DealModifier = new List<float> { 1.5f, 1.5f, 1.5f, 1.5f }
				},
				new Shipping
				{
					Name = "All Children of Peace",
					Cost = 100000,
					UnlockRep = 800,
					MinAmount = 40,
					StepAmount = 20,
					MaxAmount = 500,
					DealModifier = new List<float> { 1.75f, 1.75f, 1.75f, 1.75f }
				}
			};

		public override Dialogue EmpireDialogue { get; protected set; } =
			new Dialogue
			{
				Intro = new List<string>
				{
					"Greetings, child of peace. May love and the moon guide your path. The children of Peace will come to you for enlightenment soon."
				},
				DealStart = new List<string>
				{
					"The Children of Peace need {amount} of {quality} {product} with required effects: {effects} and optional effects: {optionalEffects}. Bring us Peace.",
					"Would you bring the Children of Peace {amount} of {quality} {product}? It should have these required effects: {effects} and optional effects: {optionalEffects}, and include Peace and Love.",
					"We teeter on the precipice of Peace, and we need {amount} {quality} {product} to bring her Love to fruition. Please include required effects: {effects} along with optional effects: {optionalEffects}."
				},
				Accept = new List<string>
				{
					"We seek Peace. And your contribution to her Children",
					"Yes! Love and Peace. Bring Her grace to us!"
				},
				Incomplete = new List<string>
				{
					"There is not enough Love and Peace here - {amount} is not enough. More Love and Peace!",
					"The moonbeams are too dim. Brighten them with the full amount of {amount}, child of Peace.",
					"Would you grace the Children of Peace with the full portion of {amount}? We thank you."
				},
				Expire = new List<string>
				{
					"The sun rises, the moon retreats. Peace and Love strive for another day. But this day is over.",
					"Dude, my high is gone. And I have no Love or Peace. Get it together, man."
				},
				Fail = new List<string>
				{
					"You cannot fail Peace or her children like this. We must strive for her radiance.",
					"No love or peace for the children. And no earthly gains for you."
				},
				Success = new List<string>
				{
					"The moon brightens. Her children rejoice!",
					"Love and Peace to you, my child.",
					"The Children sing your praises and revel in Peace."
				},
				Reward = new List<string>
				{
					"Peace has granted you ${dollar} in earthly recompense. She urges you to seek higher planes.",
					"The Children of Peace have gathered an offering for you of ${dollar} in return for your assistance in achieving a higher plane.",
					"The moonbeams have illuminated this pile of ${dollar} to be given to you. Go in peace."
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