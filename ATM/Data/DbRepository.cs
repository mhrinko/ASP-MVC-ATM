using ATM.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ATM.Data
{
    public class DbCreditCardRepository : IRepository<CreditCard>
    {
        private ATMContext _context;

        public DbCreditCardRepository(ATMContext context)
        {
            _context = context;
        }

        #region CreditCardRepository
        public async Task AddAsync(CreditCard entity)
        {
            _context.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(CreditCard entity)
        {
            _context.CreditCards.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task EditAsync(CreditCard entity)
        {
            try
            {
                _context.Update(entity);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Exists(entity.Id))
                {
                    throw new InvalidOperationException("This card doesn't exist");
                }
                else
                {
                    throw;
                }
            }
        }

        public bool Exists(int id)
        {
            return _context.CreditCards.Any(e => e.Id == id);
        }

        public async Task<List<CreditCard>> GetAllAsync()
        {
            return await _context.CreditCards.ToListAsync();
        }

        public async Task<CreditCard> GetByIdAsync(int id)
        {
            return await _context.CreditCards.FindAsync(id);
        }
        #endregion
    }

    public class DBUserActionResultRepository : IRepository<UserActionResult>
    {
        private ATMContext _context;

        public DBUserActionResultRepository(ATMContext context)
        {
            _context = context;
        }

        #region UserActionResultRepository
        public async Task AddAsync(UserActionResult entity)
        {
            _context.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(UserActionResult entity)
        {
            _context.ActionResults.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task EditAsync(UserActionResult entity)
        {
            try
            {
                _context.Update(entity);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Exists(entity.Id))
                {
                    throw new InvalidOperationException("This action result doesn't exist");
                }
                else
                {
                    throw;
                }
            }
        }

        public bool Exists(int id)
        {
            return _context.ActionResults.Any(e => e.Id == id);
        }

        public async Task<List<UserActionResult>> GetAllAsync()
        {
            return await _context.ActionResults.ToListAsync();
        }

        public async Task<UserActionResult> GetByIdAsync(int id)
        {
            return await _context.ActionResults.FindAsync(id);
        }
        #endregion
    }
}
