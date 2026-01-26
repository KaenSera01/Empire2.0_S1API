using System.Collections.Generic;

namespace Empire.NPC.Data
{
    public class Drug
    {
        public string Type { get; set; }
        public int UnlockRep { get; set; }
        public int BaseDollar { get; set; } // Changed from BonusDollar: now reflects JSON "base_dollar"
        public int BaseRep { get; set; } // Changed from BonusRep
        public int BaseXp { get; set; }
        public float RepMult { get; set; } // Changed to use JSON "rep_mult"
        public float XpMult { get; set; } // Changed to use JSON "xp_mult"
        public List<Quality> Qualities { get; set; }
        public List<Effect> Effects { get; set; }
    }
}