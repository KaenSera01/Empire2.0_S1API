using System;
using MelonLoader;
using S1API.GameTime;
using S1API.Money;
using S1API.Entities;
using Empire.NPC.S1API_NPCs;
using Core.DebugHandler;

namespace Empire.DebtHelpers
{
    public class DebtManager
    {
        EmpireNPC buyer;
        public bool paidthisweek = false;
        //paythisweek is set to true
        private void SetPayThisWeek()
        {
            paidthisweek = false;
        }

        public DebtManager(EmpireNPC buyer)
        {
            this.buyer = buyer;
            if (buyer.DealerSaveData.DebtRemaining > 0)
            {
                TimeManager.OnWeekPass += DebtPayment; // Subscribe to the week passed event
                //set paidthisweek to false at the start of each week
                TimeManager.OnWeekPass += SetPayThisWeek;
            }
            if (buyer.Debt.TotalDebt == buyer.DealerSaveData.DebtRemaining)
                SendDebtMessage(0,"initial"); // Send the initial debt message

            CheckIfPaidThisWeek();
        }

        // create a method that uses TimeManager.ElapsedDays which returns integer and finds the nearest highest int (including it) divisible by 7
        public static int GetNearestWeek(int days)
        {
            // Find the nearest week (multiple of 7) that is greater than or equal to days
            int weeks = (int)Math.Ceiling((days+1) / 7.0);
            return weeks * 7;
        }
        public void SendDebtMessage(int paymentAmount, string source)
        {
            float debtPayable = buyer.Debt.DayMultiple * (float)Math.Pow(GetNearestWeek(TimeManager.ElapsedDays), buyer.Debt.DayExponent);
            //If DebtRemaining<debtPayable, set debtPayable to DebtRemaining
            if (debtPayable > buyer.DealerSaveData.DebtRemaining)
            {
                debtPayable = buyer.DealerSaveData.DebtRemaining;
            }
            if (paymentAmount > 0)
            {
                buyer.SendCustomMessage($"You have paid ${paymentAmount} towards your debt.");
            }
            if (buyer.DealerSaveData.DebtRemaining <= 0)
            {
                buyer.SendCustomMessage($"You have no debt remaining after this {source}. Congrats!");
            }
            else if (debtPayable > buyer.DealerSaveData.DebtPaidThisWeek)
            {
                buyer.SendCustomMessage($"You owe us ${buyer.DealerSaveData.DebtRemaining}. You have paid ${buyer.DealerSaveData.DebtPaidThisWeek} this week so far. We will take ${debtPayable - buyer.DealerSaveData.DebtPaidThisWeek} at the beginning of next week or equivalent in products with a bonus multiple of {buyer.Debt.ProductBonus}. If you don't pay, we will take your life.");
            }
            else
            {
                buyer.SendCustomMessage($"You owe us ${buyer.DealerSaveData.DebtRemaining}. You have paid ${buyer.DealerSaveData.DebtPaidThisWeek} this week so far. Congrats! This is more than this week's quota. Rest of business this week will be in cash.");
            }

            // Send a message to the player about the debt status
            // This is a placeholder for the actual message sending method
            DebugLogger.Log($"Debt status for {buyer.DisplayName}: {buyer.DealerSaveData.DebtRemaining}");
        }

        // Method to check if debtpayable>debt paid this week, if so set paidthisweek to true
        public void CheckIfPaidThisWeek()
        {
            float debtPayable = buyer.Debt.DayMultiple * (float)Math.Pow(GetNearestWeek(TimeManager.ElapsedDays), buyer.Debt.DayExponent);
            if (debtPayable > buyer.DealerSaveData.DebtPaidThisWeek)
            {
                paidthisweek = false;
            }
            else
            {
                paidthisweek = true;
            }
        }


        private void DebtPayment()
        {
            if (buyer.DealerSaveData.DebtRemaining <= 0)
            {
                TimeManager.OnWeekPass -= DebtPayment; // Unsubscribe if debt is paid off
                return;
            }
            float paymentAmount = CalculateWeeklyPayment();
            buyer.DealerSaveData.DebtRemaining -= paymentAmount; // Deduct the payment amount from the dealer's remaining debt
            if (Money.GetCashBalance() < paymentAmount)
            {
                // If the player doesn't have enough cash, kill the player
                Player.Local.Kill(); // This is a placeholder for the actual kill player method
            }
            Money.ChangeCashBalance(-(int)paymentAmount); // Deduct the payment amount from the player's cash balance
            buyer.DealerSaveData.DebtRemaining = (int)(buyer.DealerSaveData.DebtRemaining * (1 + buyer.Debt.InterestRate)); // Apply interest to the remaining debt
            buyer.DealerSaveData.DebtPaidThisWeek = 0; // Reset the amount paid this week
            //buyer.SendCustomMessage($"You have paid ${paymentAmount} towards your debt.");
            SendDebtMessage((int)paymentAmount,"weekly payment"); // Send the updated debt message
            CheckIfPaidThisWeek();
        }

        private float CalculateWeeklyPayment()
        {
            // Calculate the amount payable based on the DayMultiple, DayExponent and elapsed days as multiple*day**exponent
            float dayMultiple = buyer.Debt.DayMultiple;
            float dayExponent = buyer.Debt.DayExponent;
            float elapsedDays = TimeManager.ElapsedDays;

            float amountPayable = dayMultiple * (float)Math.Pow(elapsedDays, dayExponent);

            amountPayable -= buyer.DealerSaveData.DebtPaidThisWeek;
            if (amountPayable < 0)
            {
                amountPayable = 0; // Ensure the payment is not negative
            }
            // Calculate the weekly payment based on the debt parameters
            //Ensure the amount payable does not exceed the remaining debt
            if (amountPayable > buyer.DealerSaveData.DebtRemaining)
            {
                amountPayable = buyer.DealerSaveData.DebtRemaining;
            }
            //Log the calculation details
            //DebugLogger.Log($"Calculating weekly payment for {buyer.DisplayName}: DayMultiple={dayMultiple}, DayExponent={dayExponent}, ElapsedDays={elapsedDays}, InitialAmount={dayMultiple * (float)Math.Pow(elapsedDays, dayExponent)}, AmountPaidThisWeek={buyer.DealerSaveData.DebtPaidThisWeek}, AmountPayable={amountPayable}");
            
            return amountPayable;
        }
    }

}