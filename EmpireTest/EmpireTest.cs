using Empire.NPC.S1API_NPCs;
using NUnit.Framework;
using System.IO;

namespace EmpireTest
{
	[TestFixture]
	public class EmpireTests
	{
		[Test]
		[TestCase("combo_costco.png")]
		[TestCase("tuco.png")]
		public void Icon_ShouldLoad(string fileName)
		{
			string resourcePath = $"Empire.NPC.S1API_NPCs.Icons.{fileName}";

			var asm = typeof(EmpireNPC).Assembly;
			using Stream? stream = asm.GetManifestResourceStream(resourcePath);
			Assert.That(stream, Is.Not.Null);
		}

		//[Test]
		//public void JsonLoader_Loads_Valid_NPCs()
		//{
		//	// Act
		//	var npcs = EmpireJsonLoader.LoadAllJsonNPCs();

		//	// Assert
		//	Assert.That(npcs.Count, Is.EqualTo(1));

		//	var npc = npcs[0];

		//	Assert.That(npc.DealerId, Is.EqualTo("plague_rat"));
		//	Assert.That(npc.FirstName, Is.EqualTo("Plague"));
		//	Assert.That(npc.LastName, Is.EqualTo("Rat"));
		//	Assert.That(npc.Tier, Is.EqualTo(1));

		//	Assert.That(npc.DefaultDealDays, Is.EquivalentTo(new[] { "Monday", "Tuesday", "Wednesday", "Thursday" }));
		//	Assert.That(npc.ActiveDealDays, Is.EquivalentTo(new[] { "Monday", "Tuesday", "Wednesday", "Thursday" }));

		//	Assert.That(npc.Deals.Count, Is.EqualTo(2));
		//	Assert.That(npc.Drugs.Count, Is.EqualTo(2));
		//	Assert.That(npc.Shippings.Count, Is.EqualTo(2));

		//	Assert.That(npc.Reward.unlockRep, Is.EqualTo(100));
		//	Assert.That(npc.RefreshCost, Is.EqualTo(200));
		//	Assert.That(npc.RepLogBase, Is.EqualTo(3f));

		//	Assert.That(npc.EmpireDialogue.Intro.Count, Is.GreaterThan(0));
		//	Assert.That(npc.Gift.Cost, Is.EqualTo(1000));
		//}
	}
}
