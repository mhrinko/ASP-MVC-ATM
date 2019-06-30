using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ATM.Models
{
    public class CreditCard
    {
        public int Id { get; set; }

        [Required]
        [RegularExpression(@"^\d{4}(-\d{4}){3}$")]
        [StringLength(19, MinimumLength = 19)]
        public string Number { get; set; }

        [Range(1000, 9999)]
        public int Pin { get; set; }

        [DataType(DataType.Currency)]
        public decimal Balance { get; set; }

        [Display(Name = "Is valid")]
        public bool IsValid{ get; set; }
    }
}
