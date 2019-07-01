using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ATM.Models
{
    public class ATMContext : DbContext
    {
        public ATMContext (DbContextOptions<ATMContext> options)
            : base(options)
        {
        }

        public virtual DbSet<CreditCard> CreditCards { get; set; }
        public virtual DbSet<UserActionResult> ActionResults { get; set; }

        public async virtual Task<int> GetCardIdByNumberAsync(string number)
        {
            CreditCard card;
            try
            {
                card = await CreditCards.Where(x => x.Number == number).FirstAsync();
            }
            catch (Exception)
            {
                throw new ArgumentException("Card with this number doesn't exist");
            }
            if (!card.IsValid)
            {
                throw new InvalidOperationException("Card is invalid");
            }
            return card.Id;
        }

        public async virtual Task<bool> IsValidCardCombinationAsync(int id, int pin)
        {
            CreditCard cc = await CreditCards.Where(x => x.Id == id && x.Pin == pin && x.IsValid).FirstAsync();
            return cc != null;
        }

        public async virtual Task<CreditCard> GetCreditCardByIdAsync(int id)
        {
            CreditCard card;
            try
            {
                card = await CreditCards.Where(x => x.Id == id).FirstAsync();
            }
            catch (Exception)
            {
                throw new ArgumentException("Card with this ID doesn't exist");
            }
            if (!card.IsValid)
            {
                throw new InvalidOperationException("Card is invalid");
            }
            return card;
        }

        public async virtual Task<CreditCardBalanceViewModel> GetCreditCardDetailsByIdAsync(int id)
        {
            CreditCard card;
            try
            {
                card = await CreditCards.Where(x => x.Id == id).FirstAsync();
            }
            catch (Exception)
            {
                throw new ArgumentException("Card with this ID doesn't exist");
            }
            if (!card.IsValid)
            {
                throw new InvalidOperationException("Card is invalid");
            }
            ActionResults.Add(new UserActionResult { CreditCardId = id, OperationCode = 0, TimeStamp = DateTime.Now, WithdrawalAmount = 0 });
            await SaveChangesAsync();
            return new CreditCardBalanceViewModel { Number = card.Number, Balance = card.Balance };
        }

        public async virtual Task<WithdrawalResultViewModel> WithdrawByIdAsync(int id, decimal amount)
        {
            CreditCard card;
            try
            {
                card = await CreditCards.Where(x => x.Id == id).FirstAsync();
            }
            catch (Exception)
            {
                throw new ArgumentException("Card with this ID doesn't exist");
            }
            if (!card.IsValid)
            {
                throw new InvalidOperationException("Card is invalid");
            }
            if (card.Balance < amount)
            {
                throw new InvalidOperationException("Insufficient funds");
            }
            if (amount <= 0)
            {
                throw new InvalidOperationException($"Invalid withdrawal amount: {amount}, must be above zero");
            }

            card.Balance -= amount;
            UserActionResult actionResult = new UserActionResult { CreditCardId = card.Id, OperationCode = 1, TimeStamp = DateTime.Now, WithdrawalAmount = amount };
            ActionResults.Add(actionResult);
            await SaveChangesAsync();

            return new WithdrawalResultViewModel { Number = card.Number, RemainingBalance = card.Balance, WithdrawalAmount = amount };
        }

        public static List<CreditCard> GetSeedingCards()
        {
            return new List<CreditCard>()
            {
                new CreditCard { Number = "1111-1111-1111-1111", Pin = 1111, Balance = 1000, IsValid = true },
                new CreditCard { Number = "2222-2222-2222-2222", Pin = 2222, Balance = 500, IsValid = false },
                new CreditCard { Number = "1234-1234-1234-1234", Pin = 1234, Balance = 2000, IsValid = true }
            };
        }
    }
}
