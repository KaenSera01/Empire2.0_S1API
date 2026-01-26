namespace Empire.NPC.Data
{
    public class Effect
    {
        public string Name { get; set; }
        public int UnlockRep { get; set; }
        public float Probability { get; set; }
        public float DollarMult { get; set; }
        //public bool TakeFromList { get; set; } // Change to use JSON "take_from_list"
    }
}