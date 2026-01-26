using System.Collections.Generic;

namespace Empire.NPC.Data
{
    public class Shipping
    {
        public string Name { get; set; }
        public int Cost { get; set; }
        public int UnlockRep { get; set; }
        public int MinAmount { get; set; }
        public int StepAmount { get; set; }
        public int MaxAmount { get; set; }
        public List<float> DealModifier { get; set; }
    }
}