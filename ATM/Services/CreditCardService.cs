using ATM.Data;
using ATM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ATM.Services
{
    public class CreditCardService
    {
        public IRepository<CreditCard> _creditCardRepository;

        public CreditCardService(IRepository<CreditCard> creditCardRepository)
        {
            _creditCardRepository = creditCardRepository;
        }

        public async Task<List<CreditCard>> GetAllCreditCards()
        {
            return await _creditCardRepository.GetAllAsync();
        }

        public async Task<CreditCard> GetCreditCardById(int id)
        {
            return await _creditCardRepository.GetByIdAsync(id);
        }

        public async Task AddCreditCard(CreditCard creditCard)
        {
            await _creditCardRepository.AddAsync(creditCard);
        }

        public async Task EditCreditCard(CreditCard creditCard)
        {
            await _creditCardRepository.EditAsync(creditCard);
        }

        public async Task DeleteCreditCardById(int id)
        {
            CreditCard creditCard = await GetCreditCardById(id);
            if (creditCard != null)
            {
                await _creditCardRepository.DeleteAsync(creditCard);
            }
        }

        public bool CreditCardExists(int id)
        {
            return _creditCardRepository.Exists(id);
        }
    }
}
