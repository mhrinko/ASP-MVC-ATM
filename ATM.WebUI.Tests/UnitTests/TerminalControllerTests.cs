using ATM.BusinessLogic.Services;
using ATM.DataAccess.Models;
using ATM.WebUI.Controllers;
using ATM.WebUI.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ATM.WebUI.Tests.UnitTests
{
    public class TerminalControllerTests
    {
        #region TestData
        private static readonly CreditCard TEST_CARD = new CreditCard
        {
            Id = 3456,
            Number = "1234-5678-7890-1234",
            Pin = 7134,
            Balance = 100.00M,
            IsValid = true
        };
        private static readonly CreditCardBalanceViewModel TEST_BALANCE_VIEW = new CreditCardBalanceViewModel
        {
            Number = TEST_CARD.Number,
            Balance = TEST_CARD.Balance
        };
        private static readonly decimal WITHDRAWAL_AMOUNT = 1.0M;
        private static readonly WithdrawalResultViewModel WITHDRAWAL_VIEW_MODEL = new WithdrawalResultViewModel()
        {
            Number = TEST_CARD.Number,
            WithdrawalAmount = WITHDRAWAL_AMOUNT,
            RemainingBalance = TEST_CARD.Balance - WITHDRAWAL_AMOUNT
        };
        #endregion

        #region CardTests
        [Fact]
        public void CardGet_ShouldClearSession()
        {
            // Arrange
            var sessionManagerMock = new Mock<ITerminalSessionManager>();
            var terminalController = new TerminalController(null, null, sessionManagerMock.Object);

            // Act
            var result = terminalController.Card();

            // Assert
            Assert.NotNull(result);
            Assert.IsAssignableFrom<ViewResult>(result);
            sessionManagerMock.Verify(p => p.ClearSession(It.IsAny<HttpContext>()), Times.Once);
        }

        [Fact]
        public async Task CardPost_ShouldAddIdToSessionAsync()
        {
            // Arrange
            var terminalServiceMock = new Mock<TerminalService>(null, null, null);
            terminalServiceMock.Setup(p => p.GetCardIdByNumberAsync(TEST_CARD.Number)).ReturnsAsync(TEST_CARD.Id);
            var sessionManagerMock = new Mock<ITerminalSessionManager>();
            var terminalController = new TerminalController(terminalServiceMock.Object, null, sessionManagerMock.Object);

            // Act
            var result = await terminalController.Card(TEST_CARD.Number);

            // Assert
            Assert.NotNull(result);
            Assert.IsAssignableFrom<RedirectToActionResult>(result);
            terminalServiceMock.Verify(p => p.GetCardIdByNumberAsync(TEST_CARD.Number), Times.Once);
            sessionManagerMock.Verify(p => p.SetSessionCardId(It.IsAny<HttpContext>(), TEST_CARD.Id), Times.Once);
        }
        #endregion

        #region PinTests
        [Fact]
        public void PinGet_ShouldUnauthorize()
        {
            // Arrange
            var sessionManagerMock = new Mock<ITerminalSessionManager>();
            var terminalController = new TerminalController(null, null, sessionManagerMock.Object);

            // Act
            var result = terminalController.Pin();

            // Assert
            Assert.NotNull(result);
            Assert.IsAssignableFrom<ViewResult>(result);
            sessionManagerMock.Verify(p => p.Unauthorize(It.IsAny<HttpContext>()), Times.Once);
        }

        [Fact]
        public async Task PinPost_ShouldGetIdCheckPinAndAuthorizeAsync()
        {
            // Arrange
            var terminalServiceMock = new Mock<TerminalService>(null, null, null);
            terminalServiceMock.Setup(p => p.IsValidCardCombinationAsync(TEST_CARD.Id, TEST_CARD.Pin)).ReturnsAsync(true);
            var sessionManagerMock = new Mock<ITerminalSessionManager>();
            sessionManagerMock.Setup(p => p.GetSessionCardId(It.IsAny<HttpContext>())).Returns(TEST_CARD.Id);
            var terminalController = new TerminalController(terminalServiceMock.Object, null, sessionManagerMock.Object);

            // Act
            var result = await terminalController.Pin(TEST_CARD.Pin);

            // Assert
            Assert.NotNull(result);
            Assert.IsAssignableFrom<RedirectToActionResult>(result);
            terminalServiceMock.Verify(p => p.IsValidCardCombinationAsync(TEST_CARD.Id, TEST_CARD.Pin), Times.Once);
            sessionManagerMock.Verify(p => p.GetSessionCardId(It.IsAny<HttpContext>()), Times.Once);
            sessionManagerMock.Verify(p => p.Authorize(It.IsAny<HttpContext>()), Times.Once);
        }

        [Fact]
        public void PinPost_ShouldGetIdCheckPinAndThrow()
        {
            // Arrange
            var terminalServiceMock = new Mock<TerminalService>(null, null, null);
            terminalServiceMock.Setup(p => p.IsValidCardCombinationAsync(TEST_CARD.Id, TEST_CARD.Pin)).ReturnsAsync(false);
            var sessionManagerMock = new Mock<ITerminalSessionManager>();
            sessionManagerMock.Setup(p => p.GetSessionCardId(It.IsAny<HttpContext>())).Returns(TEST_CARD.Id);
            var terminalController = new TerminalController(terminalServiceMock.Object, null, sessionManagerMock.Object);

            // Act
            Func<Task> action = async () => await terminalController.Pin(TEST_CARD.Pin);

            // Assert
            Assert.ThrowsAsync<InvalidOperationException>(action);
            terminalServiceMock.Verify(p => p.IsValidCardCombinationAsync(TEST_CARD.Id, TEST_CARD.Pin), Times.Once);
            sessionManagerMock.Verify(p => p.GetSessionCardId(It.IsAny<HttpContext>()), Times.Once);
        }
        #endregion

        #region BalanceTests
        [Fact]
        public async Task BalanceGet_ShouldGetIdAndReturnModelAsync()
        {
            // Arrange
            var terminalServiceMock = new Mock<TerminalService>(null, null, null);
            terminalServiceMock.Setup(p => p.GetCreditCardDetailsByIdAsync(TEST_CARD.Id)).ReturnsAsync(TEST_BALANCE_VIEW);
            var sessionManagerMock = new Mock<ITerminalSessionManager>();
            sessionManagerMock.Setup(p => p.GetSessionCardId(It.IsAny<HttpContext>())).Returns(TEST_CARD.Id);
            var terminalController = new TerminalController(terminalServiceMock.Object, null, sessionManagerMock.Object);

            // Act
            var result = await terminalController.Balance();

            // Assert
            Assert.NotNull(result);
            var viewResult = Assert.IsAssignableFrom<ViewResult>(result);
            Assert.NotNull(viewResult.Model);
            var viewModel = Assert.IsAssignableFrom<CreditCardBalanceViewModel>(viewResult.Model);
            Assert.Equal(TEST_BALANCE_VIEW, viewModel);
            sessionManagerMock.Verify(p => p.GetSessionCardId(It.IsAny<HttpContext>()), Times.Once);
        }
        #endregion

        #region WiithdrawTests
        [Fact]
        public async Task WithdrawPost_ShouldGetIdTryWithdrawAndRedirectWithResultAsync()
        {
            // Arrange
            var terminalServiceMock = new Mock<TerminalService>(null, null, null);
            terminalServiceMock.Setup(p => p.WithdrawByIdAsync(TEST_CARD.Id, WITHDRAWAL_AMOUNT)).ReturnsAsync(WITHDRAWAL_VIEW_MODEL);
            var sessionManagerMock = new Mock<ITerminalSessionManager>();
            sessionManagerMock.Setup(p => p.GetSessionCardId(It.IsAny<HttpContext>())).Returns(TEST_CARD.Id);
            var terminalController = new TerminalController(terminalServiceMock.Object, null, sessionManagerMock.Object);

            // Act
            var result = await terminalController.Withdraw(WITHDRAWAL_AMOUNT);

            // Assert
            Assert.NotNull(result);
            var viewResult = Assert.IsAssignableFrom<RedirectToActionResult>(result);
            Assert.Equal(4, viewResult.RouteValues.Count);
            Assert.Equal(WITHDRAWAL_VIEW_MODEL.Number, viewResult.RouteValues[nameof(WITHDRAWAL_VIEW_MODEL.Number)]);
            Assert.Equal(WITHDRAWAL_VIEW_MODEL.WithdrawalAmount, viewResult.RouteValues[nameof(WITHDRAWAL_VIEW_MODEL.WithdrawalAmount)]);
            Assert.Equal(WITHDRAWAL_VIEW_MODEL.RemainingBalance, viewResult.RouteValues[nameof(WITHDRAWAL_VIEW_MODEL.RemainingBalance)]);
            terminalServiceMock.Verify(p => p.WithdrawByIdAsync(TEST_CARD.Id, WITHDRAWAL_AMOUNT), Times.Once);
            sessionManagerMock.Verify(p => p.GetSessionCardId(It.IsAny<HttpContext>()), Times.Once);
        }
        #endregion
    }
}
