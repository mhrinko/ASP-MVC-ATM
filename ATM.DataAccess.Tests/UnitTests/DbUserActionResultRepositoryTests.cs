using ATM.DataAccess.Data;
using ATM.DataAccess.Models;
using ATM.TestUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ATM.DataAccess.Tests.UnitTests
{
    public class DBUserActionResultRepositoryTests
    {
        private static readonly UserActionResult ACTION_NOT_IN_SEEDING_ACTIONS = new UserActionResult
        {
            Id = 999,
            CreditCardId = 999,
            OperationCode = 0,
            TimeStamp = new DateTime(2010, 10, 10, 10, 10, 10),
            WithdrawalAmount = 0M
        };

        private static readonly List<UserActionResult> ACTION_RESULTS = new List<UserActionResult>() {
            new UserActionResult
            {
                Id = 1,
                CreditCardId = 1,
                OperationCode = 0,
                TimeStamp = new DateTime(2011, 11, 11, 11, 11, 11),
                WithdrawalAmount = 0
            },
            new UserActionResult
            {
                Id = 2,
                CreditCardId = 1,
                OperationCode = 1,
                TimeStamp = new DateTime(2011, 10, 10, 9, 9, 0),
                WithdrawalAmount = 1.00M
            },
            new UserActionResult
            {
                Id = 3,
                CreditCardId = 2,
                OperationCode = 0,
                TimeStamp = new DateTime(2011, 6, 6, 9, 6, 0),
                WithdrawalAmount = 0M
            }
            };

        #region GetAllAsyncTests
        [Fact]
        public async Task GetAllAsync_ShouldReturnSeedingActions()
        {
            using (var db = new ATMContext(TestOptions.TestDbContextOptions<ATMContext>()))
            {
                // Arrange
                var expectedResults = ACTION_RESULTS;
                await db.AddRangeAsync(expectedResults);
                await db.SaveChangesAsync();
                IRepository<UserActionResult> repository = new DBUserActionResultRepository(db);

                // Act
                var result = await repository.GetAllAsync();

                // Assert
                var actualMessages = Assert.IsAssignableFrom<List<UserActionResult>>(result);
                Assert.Equal(
                    expectedResults.OrderBy(x => x.Id).Select(x => (id: x.Id, cid: x.CreditCardId, time: x.TimeStamp, operationCode: x.OperationCode, withdrawal: x.WithdrawalAmount)),
                    actualMessages.OrderBy(x => x.Id).Select(x => (id: x.Id, cid: x.CreditCardId, time: x.TimeStamp, operationCode: x.OperationCode, withdrawal: x.WithdrawalAmount)));
            }
        }
        #endregion

        #region GetByIdAsyncTests
        [Fact]
        public async Task GetBuIdAsync_ShouldReturnAAction()
        {
            using (var db = new ATMContext(TestOptions.TestDbContextOptions<ATMContext>()))
            {
                // Arrange
                var expectedAction = ACTION_RESULTS.First();
                await db.AddRangeAsync(ACTION_RESULTS);
                await db.SaveChangesAsync();
                var expectedDbAction = ACTION_RESULTS.First();
                int actionId = expectedDbAction.Id;
                IRepository<UserActionResult> repository = new DBUserActionResultRepository(db);

                // Act
                var result = await repository.GetByIdAsync(actionId);

                // Assert
                Assert.Equal(expectedDbAction, result);
            }
        }

        [Fact]
        public async Task GetBuIdAsync_ShouldReturnNoAction()
        {
            using (var db = new ATMContext(TestOptions.TestDbContextOptions<ATMContext>()))
            {
                // Arrange
                await db.AddRangeAsync(ACTION_RESULTS);
                await db.SaveChangesAsync();
                UserActionResult expectedDbAction = null;
                int actionId = db.ActionResults.OrderBy(x => x.Id).Last().Id + 1;
                IRepository<UserActionResult> repository = new DBUserActionResultRepository(db);

                // Act
                var result = await repository.GetByIdAsync(actionId);

                // Assert
                Assert.Equal(expectedDbAction, result);
            }
        }
        #endregion

        #region AddAsyncTests
        [Fact]
        public async Task AddAsync_ShouldAddANewAction()
        {
            using (var db = new ATMContext(TestOptions.TestDbContextOptions<ATMContext>()))
            {
                // Arrange
                var expectedDbAction = ACTION_NOT_IN_SEEDING_ACTIONS;
                var initActions = ACTION_RESULTS;
                if (initActions.Exists(a => a.Id == expectedDbAction.Id))
                {
                    throw new InvalidOperationException($"Seeding actions already contain the action that is not supposed to be there: {expectedDbAction}");
                }
                await db.AddRangeAsync(initActions);
                await db.SaveChangesAsync();
                IRepository<UserActionResult> repository = new DBUserActionResultRepository(db);

                var expectedActions = await repository.GetAllAsync();
                expectedActions.Add(expectedDbAction);

                // Act
                await repository.AddAsync(expectedDbAction);
                var result = await repository.GetAllAsync();

                // Assert

                var actualActions = Assert.IsAssignableFrom<List<UserActionResult>>(result);
                Assert.Equal(
                    expectedActions.OrderBy(x => x.Id).Select(x => (id: x.Id, cid: x.CreditCardId, time: x.TimeStamp, operationCode: x.OperationCode, withdrawal: x.WithdrawalAmount)),
                    actualActions.OrderBy(x => x.Id).Select(x => (id: x.Id, cid: x.CreditCardId, time: x.TimeStamp, operationCode: x.OperationCode, withdrawal: x.WithdrawalAmount)));
            }
        }

        [Fact]
        public async Task AddAsync_ShouldNotAddAAction()
        {
            using (var db = new ATMContext(TestOptions.TestDbContextOptions<ATMContext>()))
            {
                // Arrange
                var expectedAction = ACTION_RESULTS.First();
                await db.AddRangeAsync(ACTION_RESULTS);
                await db.SaveChangesAsync();
                IRepository<UserActionResult> repository = new DBUserActionResultRepository(db);
                var expectedActions = await repository.GetAllAsync();

                // Act
                Func<Task> action = async () => await repository.AddAsync(expectedAction);

                // Assert
                await Assert.ThrowsAsync<ArgumentException>(action);
            }
        }
        #endregion

        #region DeleteAsyncTests
        [Fact]
        public async Task DeleteAsync_ShouldDeleteAAction()
        {
            using (var db = new ATMContext(TestOptions.TestDbContextOptions<ATMContext>()))
            {
                // Arrange
                var initActions = ACTION_RESULTS;
                await db.AddRangeAsync(initActions);
                await db.SaveChangesAsync();
                IRepository<UserActionResult> repository = new DBUserActionResultRepository(db);

                var expectedActions = await repository.GetAllAsync();
                var actionToDelete = expectedActions.First();
                expectedActions.Remove(actionToDelete);

                // Act
                await repository.DeleteAsync(actionToDelete);
                var result = await repository.GetAllAsync();

                // Assert

                var actualActions = Assert.IsAssignableFrom<List<UserActionResult>>(result);
                Assert.Equal(
                    expectedActions.OrderBy(x => x.Id).Select(x => (id: x.Id, cid: x.CreditCardId, time: x.TimeStamp, operationCode: x.OperationCode, withdrawal: x.WithdrawalAmount)),
                    actualActions.OrderBy(x => x.Id).Select(x => (id: x.Id, cid: x.CreditCardId, time: x.TimeStamp, operationCode: x.OperationCode, withdrawal: x.WithdrawalAmount)));
            }
        }

        [Fact]
        public async Task DeleteAsync_ShouldNotDelete()
        {
            using (var db = new ATMContext(TestOptions.TestDbContextOptions<ATMContext>()))
            {
                // Arrange
                var actionToDelete = ACTION_NOT_IN_SEEDING_ACTIONS;
                var initActions = ACTION_RESULTS;
                if (initActions.Exists(a => a.Id == actionToDelete.Id))
                {
                    throw new InvalidOperationException($"Seeding cards already contain the card that is not supposed to be there: {actionToDelete}");
                }
                await db.AddRangeAsync(initActions);
                await db.SaveChangesAsync();
                IRepository<UserActionResult> repository = new DBUserActionResultRepository(db);

                // Act
                Func<Task> action = async () => await repository.DeleteAsync(actionToDelete);

                // Assert
                await Assert.ThrowsAsync<ArgumentException>(action);
            }
        }
        #endregion

        #region EditAsyncTests
        [Fact]
        public async Task EditAsync_ShouldEditACard()
        {
            using (var db = new ATMContext(TestOptions.TestDbContextOptions<ATMContext>()))
            {
                // Arrange
                var initCards = ACTION_RESULTS;
                await db.AddRangeAsync(initCards);
                await db.SaveChangesAsync();
                IRepository<UserActionResult> repository = new DBUserActionResultRepository(db);

                var expectedCards = await repository.GetAllAsync();
                var cardToEdit = expectedCards.First();
                expectedCards.Remove(cardToEdit);

                cardToEdit.WithdrawalAmount += 1.00M;
                expectedCards.Add(cardToEdit);

                // Act
                await repository.EditAsync(cardToEdit);
                var result = await repository.GetAllAsync();

                // Assert

                var actualCards = Assert.IsAssignableFrom<List<UserActionResult>>(result);
                Assert.Equal(
                    expectedCards.OrderBy(x => x.Id).Select(x => (id: x.Id, cid: x.CreditCardId, time: x.TimeStamp, operationCode: x.OperationCode, withdrawal: x.WithdrawalAmount)),
                    actualCards.OrderBy(x => x.Id).Select(x => (id: x.Id, cid: x.CreditCardId, time: x.TimeStamp, operationCode: x.OperationCode, withdrawal: x.WithdrawalAmount)));
            }
        }

        [Fact]
        public async Task EditAsync_ShouldNotEdit()
        {
            using (var db = new ATMContext(TestOptions.TestDbContextOptions<ATMContext>()))
            {
                // Arrange
                var cardToEdit = ACTION_NOT_IN_SEEDING_ACTIONS;
                var initCards = ACTION_RESULTS;
                if (initCards.Exists(cc => cc.Id == cardToEdit.Id))
                {
                    throw new InvalidOperationException($"Seeding cards already contain the card that is not supposed to be there: {cardToEdit}");
                }
                await db.AddRangeAsync(initCards);
                await db.SaveChangesAsync();
                IRepository<UserActionResult> repository = new DBUserActionResultRepository(db);

                // Act
                Func<Task> action = async () => await repository.EditAsync(cardToEdit);

                // Assert
                await Assert.ThrowsAsync<ArgumentException>(action);
            }
        }
        #endregion
    }
}
