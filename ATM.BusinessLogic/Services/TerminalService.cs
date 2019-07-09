using ATM.DataAccess;
using ATM.DataAccess.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ATM.BusinessLogic.Services
{
    public class TerminalService
    {
        private IRepository<CreditCard> _creditCardRepository;
        private IRepository<UserActionResult> _actionResultRepository;
        private ILogger _logger;

        public TerminalService(IRepository<CreditCard> creditCardRepository, IRepository<UserActionResult> actionResultRepository, ILogger<TerminalService> logger)
        {
            _creditCardRepository = creditCardRepository;
            _actionResultRepository = actionResultRepository;
            _logger = logger;
        }

        public async Task<int> GetCardIdByNumberAsync(string number)
        {
            var creditCards = await _creditCardRepository.GetAllAsync();
            CreditCard card;
            try
            {
                card = creditCards.Find(x => x.Number == number);
            }
            catch (Exception)
            {
                throw new ArgumentException("Card with this number doesn't exist");
            }
            if (card == null)
            {
                throw new ArgumentException("Card with this number doesn't exist");
            }
            if (!card.IsValid)
            {
                throw new InvalidOperationException("Card is invalid");
            }
            return card.Id;
        }

        public async Task<bool> IsValidCardCombinationAsync(int? id, int pin)
        {
            if (!id.HasValue)
            {
                throw new ArgumentNullException("No card identifier provided");
            }
            var cards = await _creditCardRepository.GetAllAsync();
            CreditCard cc = null;
            try
            {
                cc = cards.Find(x => x.Id == id && x.Pin == pin && x.IsValid);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Caught {ex.GetType()} in {nameof(IsValidCardCombinationAsync)}", ex);
            }
            return cc != null;
        }

        public async Task<CreditCardBalanceViewModel> GetCreditCardDetailsByIdAsync(int? id)
        {
            if (!id.HasValue)
            {
                throw new ArgumentNullException("No card identifier provided");
            }
            var cards = await _creditCardRepository.GetAllAsync();
            CreditCard card;
            try
            {
                card = cards.Find(x => x.Id == id);
            }
            catch (Exception)
            {
                throw new ArgumentException("Card with this ID doesn't exist");
            }
            if (card == null)
            {
                throw new ArgumentException("Card with this ID doesn't exist");
            }
            if (!card.IsValid)
            {
                throw new InvalidOperationException("Card is invalid");
            }
            await _actionResultRepository.AddAsync(new UserActionResult { CreditCardId = card.Id, OperationCode = 0, TimeStamp = DateTime.Now, WithdrawalAmount = 0 });
            return new CreditCardBalanceViewModel { Number = card.Number, Balance = card.Balance };
        }

        public async Task<WithdrawalResultViewModel> WithdrawByIdAsync(int? id, decimal amount)
        {
            if (!id.HasValue)
            {
                throw new ArgumentNullException("No card identifier provided");
            }
            var cards = await _creditCardRepository.GetAllAsync();
            CreditCard card;
            try
            {
                card = cards.Find(x => x.Id == id);
            }
            catch (Exception)
            {
                throw new ArgumentException("Card with this ID doesn't exist");
            }
            if (card == null)
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
            // Should be a transaction
            card.Balance -= amount;
            await _creditCardRepository.EditAsync(card);
            await _actionResultRepository.AddAsync(new UserActionResult { CreditCardId = card.Id, OperationCode = 1, TimeStamp = DateTime.Now, WithdrawalAmount = amount });
            return new WithdrawalResultViewModel { Number = card.Number, RemainingBalance = card.Balance, WithdrawalAmount = amount };
        }
    }
}
