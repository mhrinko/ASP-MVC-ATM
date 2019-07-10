using System.ComponentModel.DataAnnotations;

namespace ATM.DataAccess.Models
{
    public class CreditCard
    {
        public int Id { get; set; }

        [Required]
        [RegularExpression(@"^\d{4}(-\d{4}){3}$", ErrorMessage = "The card number must have the following format: 1111-1111-1111-1111")]
        [StringLength(19, MinimumLength = 19)]
        public string Number { get; set; }

        [Range(1000, 9999)]
        public int Pin { get; set; }

        [DataType(DataType.Currency)]
        public decimal Balance { get; set; }

        [Display(Name = "Is valid")]
        public bool IsValid{ get; set; }

        public override bool Equals(object obj)
        {
            var card = obj as CreditCard;
            return card != null &&
                   Id == card.Id &&
                   Number == card.Number &&
                   Pin == card.Pin &&
                   Balance == card.Balance &&
                   IsValid == card.IsValid;
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}
