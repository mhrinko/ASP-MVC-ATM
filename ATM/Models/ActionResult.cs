using System;
using System.ComponentModel.DataAnnotations;

namespace ATM.Models
{
    public class UserActionResult
    {
        [Display(Name = "Result record ID")]
        public int Id { get; set; }

        [Display(Name = "Credit card ID")]
        public int CreditCardId { get; set; }

        [Display(Name = "Operation code")]
        public int OperationCode { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Operation timestamp")]
        public DateTime TimeStamp { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Withdrawal amount")]
        public decimal WithdrawalAmount { get; set; }
    }
}
