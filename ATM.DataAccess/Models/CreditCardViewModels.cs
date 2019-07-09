using System;
using System.ComponentModel.DataAnnotations;

namespace ATM.DataAccess.Models
{
    public class CreditCardBalanceViewModel
    {
        [Required]
        [RegularExpression(@"^\d{4}(-\d{4}){3}$")]
        public string Number { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Operation timestamp")]
        public DateTime CurrentTime { get => DateTime.Now; }

        [DataType(DataType.Currency)]
        public decimal Balance { get; set; }

        public override bool Equals(object obj)
        {
            var model = obj as CreditCardBalanceViewModel;
            return model != null &&
                   Number == model.Number &&
                   CurrentTime == model.CurrentTime &&
                   Balance == model.Balance;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Number, CurrentTime, Balance);
        }
    }

    public class WithdrawalResultViewModel
    {
        [Required]
        [RegularExpression(@"^\d{4}(-\d{4}){3}$")]
        public string Number { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Operation timestamp")]
        public DateTime CurrentTime { get => DateTime.Now; }

        [DataType(DataType.Currency)]
        [Display(Name = "Withdrawal amount")]
        public decimal WithdrawalAmount { get; set; }
        [DataType(DataType.Currency)]
        [Display(Name = "Remaining balance")]
        public decimal RemainingBalance { get; set; }

        public override bool Equals(object obj)
        {
            var model = obj as WithdrawalResultViewModel;
            return model != null &&
                   Number == model.Number &&
                   CurrentTime == model.CurrentTime &&
                   WithdrawalAmount == model.WithdrawalAmount &&
                   RemainingBalance == model.RemainingBalance;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Number, CurrentTime, WithdrawalAmount, RemainingBalance);
        }
    }
}
