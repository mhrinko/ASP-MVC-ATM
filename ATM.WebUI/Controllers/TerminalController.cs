using ATM.BusinessLogic.Services;
using ATM.DataAccess.Models;
using ATM.WebUI.Filters;
using ATM.WebUI.Models;
using ATM.WebUI.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ATM.WebUI.Controllers
{
    public class TerminalController : Controller
    {
        private TerminalService _terminalService;
        private ILogger _logger;
        private ITerminalSessionManager _sessionManager;

        public TerminalController(TerminalService terminalService, ILogger<TerminalController> logger, ITerminalSessionManager sessionManager)
        {
            _terminalService = terminalService;
            _logger = logger;
            _sessionManager = sessionManager;
        }

        public IActionResult Index()
        {
            return RedirectToAction(nameof(Card));
        }

        public IActionResult Card()
        {
            _sessionManager.ClearSession(HttpContext);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Card(string number)
        {
            int id = await _terminalService.GetCardIdByNumberAsync(number);
            _sessionManager.SetSessionCardId(HttpContext, id);
            return RedirectToAction(nameof(Pin));
        }

        [TerminalIdFilter]
        public IActionResult Pin()
        {
            _sessionManager.Unauthorize(HttpContext);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [TerminalIdFilter]
        public async Task<IActionResult> Pin(int pin)
        {
            int cardId = _sessionManager.GetSessionCardId(HttpContext).Value;
            bool isValid = await _terminalService.IsValidCardCombinationAsync(cardId, pin);
            if (isValid)
            {
                _sessionManager.Authorize(HttpContext);
                return RedirectToAction(nameof(Menu));
            }
            else
            {
                throw new InvalidOperationException("No valid card found with this number-pin combination");
            }
        }

        [TerminalAuthorizationFilter]
        [TerminalIdFilter]
        public IActionResult Menu()
        {
            return View();
        }

        [TerminalAuthorizationFilter]
        [TerminalIdFilter]
        public async Task<IActionResult> Balance()
        {
            int cardId = _sessionManager.GetSessionCardId(HttpContext).Value;
            var result = await _terminalService.GetCreditCardDetailsByIdAsync(cardId);
            return View(result);
        }

        [TerminalAuthorizationFilter]
        [TerminalIdFilter]
        public IActionResult Withdraw()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [TerminalAuthorizationFilter]
        [TerminalIdFilter]
        public async Task<IActionResult> Withdraw(decimal withdrawalAmount)
        {
            int cardId = _sessionManager.GetSessionCardId(HttpContext).Value;
            var result = await _terminalService.WithdrawByIdAsync(cardId, withdrawalAmount);
            return RedirectToAction(nameof(WithdrawalResult), result);
        }

        [TerminalAuthorizationFilter]
        [TerminalIdFilter]
        public IActionResult WithdrawalResult(WithdrawalResultViewModel viewModel)
        {
            return View(viewModel);
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            ErrorViewModel errorView = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                Referrer = HttpContext.Request.Headers["referer"]
            };
            var exceptionHandlerPathFeature =
                HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            errorView.ErrorMessage = exceptionHandlerPathFeature?.Error.Message;

            string logMessage = "";
            if (errorView.ShowRequestId)
            {
                logMessage += $"Request ID: {errorView.RequestId}";
            }
            if (errorView.ShowReferrer)
            {
                logMessage += $"Referrer: {errorView.Referrer}";
            }
            if (errorView.ShowErrorMessage)
            {
                logMessage += $"Request ID: {errorView.ShowErrorMessage}";
            }

            return View(errorView);
        }
    }
}