namespace Empire.DebtHelpers    
{
    public class Debt
    {
        public float TotalDebt { get; set; } = 0f;
        public float InterestRate { get; set; } = 0f;
        public float DayMultiple { get; set; } = 0f;
        public float DayExponent { get; set; } = 0f;
        public float ProductBonus { get; set; } = 0f;
    }
}