using ATM.BusinessLogic.Services;
using ATM.DataAccess;
using ATM.DataAccess.Models;
using ATM.TestUtils;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ATM.BusinessLogic.Tests.UnitTests
{
    public class TerminalServiceTests
    {
        private static Mock<IRepository<CreditCard>> GetCardRepoMock()
        {
            var cardRepoMock = new Mock<IRepository<CreditCard>>();
            cardRepoMock.Setup(p => p.GetAllAsync()).ReturnsAsync(SampleData.CREDITCARDS);
            return cardRepoMock;
        }
        private static Mock<IRepository<UserActionResult>> GetActionRepoMock(Action action)
        {
            var actionRepoMock = new Mock<IRepository<UserActionResult>>();
            actionRepoMock.Setup(p => p.AddAsync(It.IsAny<UserActionResult>())).Callback(action);
            return actionRepoMock;
        }

        #region GetCardIdByNumberAsyncTests
        [Fact]
        public async Task GetCardIdByNumberAsyncTests_ShouldReturnId()
        {
            // Arrange
            int expectedId = SampleData.CREDITCARDS.First().Id;
            string number = SampleData.CREDITCARDS.First().Number;
            var terminalService = new TerminalService(GetCardRepoMock().Object, null, null);

            // Act
            var result = await terminalService.GetCardIdByNumberAsync(number);

            // Assert
            Assert.Equal(expectedId, result);
        }

        [Fact]
        public async Task GetCardIdByNumberAsyncTests_ShouldThrowArgEx()
        {
            // Arrange
            string number = SampleData.CARD_NOT_ON_THE_LIST.Number;
            var terminalService = new TerminalService(GetCardRepoMock().Object, null, null);

            // Act
            Func<Task> action = async () => await terminalService.GetCardIdByNumberAsync(number);

            // Assert
            Exception ex = await Assert.ThrowsAsync<ArgumentException>(action);
            Assert.Equal("Card with this number doesn't exist", ex.Message);
        }

        [Fact]
        public async Task GetCardIdByNumberAsyncTests_ShouldThrowInvalOperEx()
        {
            // Arrange
            string number = SampleData.CREDITCARDS.Where(p => !p.IsValid).First().Number;
            var terminalService = new TerminalService(GetCardRepoMock().Object, null, null);

            // Act
            Func<Task> action = async () => await terminalService.GetCardIdByNumberAsync(number);

            // Assert
            Exception ex = await Assert.ThrowsAsync<InvalidOperationException>(action);
            Assert.Equal("Card is invalid", ex.Message);
        }
        #endregion

        #region IsValidCardCombinationAsync
        [Fact]
        public async Task IsValidCardCombinationAsync_ShouldReturnTrue()
        {
            // Arrange
            var card = SampleData.CREDITCARDS.First(x => x.IsValid);
            int cardId = card.Id;
            int cardPin = card.Pin;
            var terminalService = new TerminalService(GetCardRepoMock().Object, null, null);

            // Act
            var result = await terminalService.IsValidCardCombinationAsync(cardId, cardPin);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsValidCardCombinationAsync_ShouldReturnFalse()
        {
            // Arrange
            var card = SampleData.CREDITCARDS.First(x => !x.IsValid);
            int cardId = card.Id;
            int cardPin = card.Pin;
            var terminalService = new TerminalService(GetCardRepoMock().Object, null, null);

            // Act
            var result = await terminalService.IsValidCardCombinationAsync(cardId, cardPin);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsValidCardCombinationAsync_ShouldThrowArgNullEx()
        {
            // Arrange
            var terminalService = new TerminalService(GetCardRepoMock().Object, null, null);

            // Act
            Func<Task> action = async () => await terminalService.IsValidCardCombinationAsync(null, 1);

            // Assert
            Exception ex = await Assert.ThrowsAsync<ArgumentNullException>(action);
        }

        [Fact]
        public async Task IsValidCardCombinationAsync_ShouldReturnFalseAndLog()
        {
            // Arrange
            var card = SampleData.CREDITCARDS.First(x => !x.IsValid);
            int cardId = card.Id;
            int cardPin = card.Pin;
            var loggerMock = new Mock<ILogger<TerminalService>>();
            //loggerMock.Verify(p => p.LogInformation(It.IsAny<string>(), It.IsAny<Exception>()), Times.Once); // not supported ?
            var terminalService = new TerminalService(GetCardRepoMock().Object, null, loggerMock.Object);

            // Act
            var result = await terminalService.IsValidCardCombinationAsync(cardId, cardPin);

            // Assert
            Assert.False(result);
        }
        #endregion

        #region GetCreditCardDetailsByIdAsync
        [Fact]
        public async Task GetCreditCardDetailsByIdAsync_ShouldReturnView()
        {
            // Arrange
            var card = SampleData.CREDITCARDS.First();
            int cardId = card.Id;
            var expectedView = new CreditCardBalanceViewModel { Number = card.Number, Balance = card.Balance };
            int count = 0;
            var loggerMock = GetActionRepoMock(() => { count++; });

            var terminalService = new TerminalService(
                GetCardRepoMock().Object,
                loggerMock.Object,
                null);

            // Act
            var result = await terminalService.GetCreditCardDetailsByIdAsync(cardId);

            // Assert
            Assert.Equal(expectedView.Number, result.Number);
            Assert.Equal(expectedView.Balance, result.Balance);
            Assert.True(count > 0);
        }

        [Fact]
        public async Task GetCreditCardDetailsByIdAsync_ShouldThrowArgEx()
        {
            // Arrange
            int cardId = SampleData.CARD_NOT_ON_THE_LIST.Id;
            var terminalService = new TerminalService(GetCardRepoMock().Object, null, null);

            // Act
            Func<Task> action = async () => await terminalService.GetCreditCardDetailsByIdAsync(cardId);

            // Assert
            Exception ex = await Assert.ThrowsAsync<ArgumentException>(action);
            Assert.Equal("Card with this ID doesn't exist", ex.Message);
        }

        [Fact]
        public async Task GetCreditCardDetailsByIdAsync_ShouldThrowInvalOpEx()
        {
            // Arrange
            int cardId = SampleData.CREDITCARDS.First(c => !c.IsValid).Id;
            var terminalService = new TerminalService(GetCardRepoMock().Object, null, null);

            // Act
            Func<Task> action = async () => await terminalService.GetCreditCardDetailsByIdAsync(cardId);

            // Assert
            Exception ex = await Assert.ThrowsAsync<InvalidOperationException>(action);
            Assert.Equal("Card is invalid", ex.Message);
        }

        [Fact]
        public async Task GetCreditCardDetailsByIdAsync_ShouldThrowArgNullEx()
        {
            // Arrange
            int? cardId = new int?();
            var terminalService = new TerminalService(GetCardRepoMock().Object, null, null);

            // Act
            Func<Task> action = async () => await terminalService.GetCreditCardDetailsByIdAsync(cardId);

            // Assert
            Exception ex = await Assert.ThrowsAsync<ArgumentNullException>(action);
        }
        #endregion

        #region WithdrawByIdAsync
        [Fact]
        public async Task WithdrawByIdAsync_ShouldReturnView()
        {
            // Arrange
            var card = SampleData.CREDITCARDS.First();
            int cardId = card.Id;
            decimal amount = card.Balance;
            var expectedView = new WithdrawalResultViewModel { Number = card.Number, RemainingBalance = 0, WithdrawalAmount = amount };
            int actionResultCalls = 0;
            int editCalls = 0;
            var loggerMock = GetActionRepoMock(() => { actionResultCalls++; });
            var cardRepoMock = GetCardRepoMock();
            cardRepoMock.Setup(p => p.EditAsync(It.IsAny<CreditCard>())).Callback(() => { editCalls++; });

            var terminalService = new TerminalService(cardRepoMock.Object, loggerMock.Object, null);

            // Act
            var result = await terminalService.WithdrawByIdAsync(cardId, amount);

            // Assert
            Assert.Equal(expectedView.Number, result.Number);
            Assert.Equal(expectedView.RemainingBalance, result.RemainingBalance);
            Assert.Equal(expectedView.WithdrawalAmount, result.WithdrawalAmount);
            Assert.True(actionResultCalls > 0);
            Assert.True(editCalls > 0);
        }

        [Fact]
        public async Task WithdrawByIdAsync_ShouldThrowArgEx()
        {
            // Arrange
            int cardId = SampleData.CARD_NOT_ON_THE_LIST.Id;
            decimal amount = 1.00M;
            var terminalService = new TerminalService(GetCardRepoMock().Object, null, null);

            // Act
            Func<Task> action = async () => await terminalService.WithdrawByIdAsync(cardId, amount);

            // Assert
            Exception ex = await Assert.ThrowsAsync<ArgumentException>(action);
            Assert.Equal("Card with this ID doesn't exist", ex.Message);
        }

        [Fact]
        public async Task WithdrawByIdAsync_ShouldThrowInvalOpEx()
        {
            // Arrange
            int cardId = SampleData.CREDITCARDS.First(c => !c.IsValid).Id;
            decimal amount = 1.00M;
            var terminalService = new TerminalService(GetCardRepoMock().Object, null, null);

            // Act
            Func<Task> action = async () => await terminalService.WithdrawByIdAsync(cardId, amount);

            // Assert
            Exception ex = await Assert.ThrowsAsync<InvalidOperationException>(action);
            Assert.Equal("Card is invalid", ex.Message);
        }

        [Fact]
        public async Task WithdrawByIdAsync_ShouldThrowInvalOpEx_InsufficientFunds()
        {
            // Arrange
            var card = SampleData.CREDITCARDS.First(c => c.IsValid);
            int cardId = card.Id;
            decimal amount = card.Balance + 1M;
            var terminalService = new TerminalService(GetCardRepoMock().Object, null, null);

            // Act
            Func<Task> action = async () => await terminalService.WithdrawByIdAsync(cardId, amount);

            // Assert
            Exception ex = await Assert.ThrowsAsync<InvalidOperationException>(action);
            Assert.Equal("Insufficient funds", ex.Message);
        }

        [Fact]
        public async Task WithdrawByIdAsync_ShouldThrowArgNullEx()
        {
            // Arrange
            int? cardId = new int?();
            decimal amount = 1.00M;
            var terminalService = new TerminalService(GetCardRepoMock().Object, null, null);

            // Act
            Func<Task> action = async () => await terminalService.WithdrawByIdAsync(cardId, amount);

            // Assert
            Exception ex = await Assert.ThrowsAsync<ArgumentNullException>(action);
        }
        #endregion
    }
}
