using System.Collections.Generic;

namespace Empire.NPC.Data
{
    public class DealerReward
    {
        public int unlockRep { get; set; }
        public int RepCost { get; set; }
        public string Type { get; set; }
        public List<string> Args { get; set; }
    }
}