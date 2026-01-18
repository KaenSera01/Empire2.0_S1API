using Newtonsoft.Json;


namespace Empire.NPC.Data
{
    public class Effect
    {
        public string Name { get; set; }
        public int UnlockRep { get; set; }
        public float Probability { get; set; }
        public float DollarMult { get; set; }
    }
}