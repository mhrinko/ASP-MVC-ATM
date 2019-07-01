using System;
using System.ComponentModel.DataAnnotations;

namespace ATM.Models
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
    }
}
