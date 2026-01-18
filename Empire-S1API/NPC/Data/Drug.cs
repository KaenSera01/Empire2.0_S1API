using System.Collections.Generic;
using Newtonsoft.Json;


namespace Empire.NPC.Data
{
    public class Drug
    {
        public string Type { get; set; }
        public int UnlockRep { get; set; }
        public int BaseDollar { get; set; }
        public int BaseRep { get; set; }
        public int BaseXp { get; set; }
        public float RepMult { get; set; }
        public float XpMult { get; set; }
        public List<Quality> Qualities { get; set; }
        public List<Effect> Effects { get; set; }
    }
}