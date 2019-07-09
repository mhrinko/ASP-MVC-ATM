using ATM.DataAccess.Data;
using ATM.DataAccess.Models;
using ATM.DataAccess.Tests.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ATM.DataAccess.Tests.UnitTests
{
    public class DbCreditCardRepositoryTests
    {
        private static readonly CreditCard CARD_NOT_IN_SEEDING_CARDS = new CreditCard { Id = 999, Number = "1234-5678-2345-6789", Pin = 1234, Balance = 1278.00M, IsValid = true };

        #region GetAllAsyncTests

        [Fact]
        public async Task GetAllAsync_ShouldReturnSeedingCards()
        {
            using (var db = new ATMContext(TestOptions.TestDbContextOptions()))
            {
                // Arrange
                var expectedCards = ATMContext.GetSeedingCards();
                await db.AddRangeAsync(expectedCards);
                await db.SaveChangesAsync();
                IRepository<CreditCard> repository = new DbCreditCardRepository(db);

                // Act
                var result = await repository.GetAllAsync();

                // Assert
                var actualCards = Assert.IsAssignableFrom<List<CreditCard>>(result);
                Assert.Equal(
                    expectedCards.OrderBy(x => x.Id).Select(x => (id : x.Id, number : x.Number, pin : x.Pin, balance : x.Balance, isValid : x.IsValid)),
                    actualCards.OrderBy(x => x.Id).Select(x => (id: x.Id, number: x.Number, pin: x.Pin, balance: x.Balance, isValid: x.IsValid)));
            }
        }
        #endregion

        #region GetByIdAsyncTests
        [Fact]
        public async Task GetBuIdAsync_ShouldReturnACard()
        {
            using (var db = new ATMContext(TestOptions.TestDbContextOptions()))
            {
                // Arrange
                var expectedCard = ATMContext.GetSeedingCards().First();
                await db.AddRangeAsync(ATMContext.GetSeedingCards());
                await db.SaveChangesAsync();
                var expectedDbCard = await db.CreditCards.Where(x => x.Number == expectedCard.Number).FirstAsync();
                int cardId = expectedDbCard.Id;
                IRepository<CreditCard> repository = new DbCreditCardRepository(db);

                // Act
                var result = await repository.GetByIdAsync(cardId);

                // Assert
                Assert.Equal(expectedDbCard, result);
            }
        }

        [Fact]
        public async Task GetBuIdAsync_ShouldReturnNoCard()
        {
            using (var db = new ATMContext(TestOptions.TestDbContextOptions()))
            {
                // Arrange
                await db.AddRangeAsync(ATMContext.GetSeedingCards());
                await db.SaveChangesAsync();
                CreditCard expectedDbCard = null;
                int cardId = db.CreditCards.OrderBy(x => x.Id).Last().Id + 1;
                IRepository<CreditCard> repository = new DbCreditCardRepository(db);

                // Act
                var result = await repository.GetByIdAsync(cardId);

                // Assert
                Assert.Equal(expectedDbCard, result);
            }
        }
        #endregion

        #region AddAsyncTests
        [Fact]
        public async Task AddAsync_ShouldAddANewCard()
        {
            using (var db = new ATMContext(TestOptions.TestDbContextOptions()))
            {
                // Arrange
                var expectedDbCard = CARD_NOT_IN_SEEDING_CARDS;
                var initCards = ATMContext.GetSeedingCards();
                if (initCards.Exists(cc => cc.Id == expectedDbCard.Id || cc.Number == expectedDbCard.Number))
                {
                    throw new InvalidOperationException($"Seeding cards already contain the card that is not supposed to be there: {expectedDbCard}");
                }
                await db.AddRangeAsync(initCards);
                await db.SaveChangesAsync();
                IRepository<CreditCard> repository = new DbCreditCardRepository(db);

                var expectedCards = await repository.GetAllAsync();
                expectedCards.Add(expectedDbCard);

                // Act
                await repository.AddAsync(expectedDbCard);
                var result = await repository.GetAllAsync();

                // Assert

                var actualCards = Assert.IsAssignableFrom<List<CreditCard>>(result);
                Assert.Equal(
                    expectedCards.OrderBy(x => x.Id).Select(x => (id: x.Id, number: x.Number, pin: x.Pin, balance: x.Balance, isValid: x.IsValid)),
                    actualCards.OrderBy(x => x.Id).Select(x => (id: x.Id, number: x.Number, pin: x.Pin, balance: x.Balance, isValid: x.IsValid)));
            }
        }

        [Fact]
        public async Task AddAsync_ShouldNotAddACard()
        {
            using (var db = new ATMContext(TestOptions.TestDbContextOptions()))
            {
                // Arrange
                var expectedCard = ATMContext.GetSeedingCards().First();
                await db.AddRangeAsync(ATMContext.GetSeedingCards());
                await db.SaveChangesAsync();
                var expectedDbCard = await db.CreditCards.Where(x => x.Number == expectedCard.Number).FirstAsync();
                IRepository<CreditCard> repository = new DbCreditCardRepository(db);
                var expectedCards = await repository.GetAllAsync();

                // Act
                Func<Task> action = async () => await repository.AddAsync(expectedDbCard);

                // Assert
                await Assert.ThrowsAsync<ArgumentException>(action);
            }
        }
        #endregion

        #region DeleteAsyncTests
        [Fact]
        public async Task DeleteAsync_ShouldDeleteACard()
        {
            using (var db = new ATMContext(TestOptions.TestDbContextOptions()))
            {
                // Arrange
                var initCards = ATMContext.GetSeedingCards();
                await db.AddRangeAsync(initCards);
                await db.SaveChangesAsync();
                IRepository<CreditCard> repository = new DbCreditCardRepository(db);

                var expectedCards = await repository.GetAllAsync();
                var cardToDelete = expectedCards.First();
                expectedCards.Remove(cardToDelete);

                // Act
                await repository.DeleteAsync(cardToDelete);
                var result = await repository.GetAllAsync();

                // Assert

                var actualCards = Assert.IsAssignableFrom<List<CreditCard>>(result);
                Assert.Equal(
                    expectedCards.OrderBy(x => x.Id).Select(x => (id: x.Id, number: x.Number, pin: x.Pin, balance: x.Balance, isValid: x.IsValid)),
                    actualCards.OrderBy(x => x.Id).Select(x => (id: x.Id, number: x.Number, pin: x.Pin, balance: x.Balance, isValid: x.IsValid)));
            }
        }

        [Fact]
        public async Task DeleteAsync_ShouldNotDelete()
        {
            using (var db = new ATMContext(TestOptions.TestDbContextOptions()))
            {
                // Arrange
                var cardToDelete = CARD_NOT_IN_SEEDING_CARDS;
                var initCards = ATMContext.GetSeedingCards();
                if (initCards.Exists(cc => cc.Id == cardToDelete.Id || cc.Number == cardToDelete.Number))
                {
                    throw new InvalidOperationException($"Seeding cards already contain the card that is not supposed to be there: {cardToDelete}");
                }
                await db.AddRangeAsync(initCards);
                await db.SaveChangesAsync();
                IRepository<CreditCard> repository = new DbCreditCardRepository(db);

                // Act
                Func<Task> action = async () => await repository.DeleteAsync(cardToDelete);

                // Assert
                await Assert.ThrowsAsync<ArgumentException>(action);
            }
        }
        #endregion

        #region EditAsyncTests
        [Fact]
        public async Task EditAsync_ShouldEditACard()
        {
            using (var db = new ATMContext(TestOptions.TestDbContextOptions()))
            {
                // Arrange
                var initCards = ATMContext.GetSeedingCards();
                await db.AddRangeAsync(initCards);
                await db.SaveChangesAsync();
                IRepository<CreditCard> repository = new DbCreditCardRepository(db);

                var expectedCards = await repository.GetAllAsync();
                var cardToEdit = expectedCards.First();
                expectedCards.Remove(cardToEdit);

                cardToEdit.Balance -= 1.00M;
                expectedCards.Add(cardToEdit);

                // Act
                await repository.EditAsync(cardToEdit);
                var result = await repository.GetAllAsync();

                // Assert

                var actualCards = Assert.IsAssignableFrom<List<CreditCard>>(result);
                Assert.Equal(
                    expectedCards.OrderBy(x => x.Id).Select(x => (id: x.Id, number: x.Number, pin: x.Pin, balance: x.Balance, isValid: x.IsValid)),
                    actualCards.OrderBy(x => x.Id).Select(x => (id: x.Id, number: x.Number, pin: x.Pin, balance: x.Balance, isValid: x.IsValid)));
            }
        }

        [Fact]
        public async Task EditAsync_ShouldNotEdit()
        {
            using (var db = new ATMContext(TestOptions.TestDbContextOptions()))
            {
                // Arrange
                var cardToEdit = CARD_NOT_IN_SEEDING_CARDS;
                var initCards = ATMContext.GetSeedingCards();
                if (initCards.Exists(cc => cc.Id == cardToEdit.Id || cc.Number == cardToEdit.Number))
                {
                    throw new InvalidOperationException($"Seeding cards already contain the card that is not supposed to be there: {cardToEdit}");
                }
                await db.AddRangeAsync(initCards);
                await db.SaveChangesAsync();
                IRepository<CreditCard> repository = new DbCreditCardRepository(db);

                // Act
                Func<Task> action = async () => await repository.EditAsync(cardToEdit);

                // Assert
                await Assert.ThrowsAsync<ArgumentException>(action);
            }
        }
        #endregion
    }
}
