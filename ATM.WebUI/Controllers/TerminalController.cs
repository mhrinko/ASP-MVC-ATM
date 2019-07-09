using ATM.BusinessLogic.Services;
using ATM.DataAccess.Models;
using ATM.WebUI.Filters;
using ATM.WebUI.Models;
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

        public static readonly string SESSSION_KEY_CARD_ID = "cardId";

        public TerminalController(TerminalService terminalService, ILogger<TerminalController> logger)
        {
            _terminalService = terminalService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return RedirectToAction(nameof(Card));
        }

        public IActionResult Card()
        {
            HttpContext.Session.Clear();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Card(string number)
        {
            int id = await _terminalService.GetCardIdByNumberAsync(number);
            HttpContext.Session.SetInt32(SESSSION_KEY_CARD_ID, id);
            return RedirectToAction(nameof(Pin));
        }

        [TerminalAuthorizationFilter]
        public IActionResult Pin()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [TerminalAuthorizationFilter]
        public async Task<IActionResult> Pin(int pin)
        {
            int cardId = HttpContext.Session.GetInt32(SESSSION_KEY_CARD_ID).Value;
            bool isValid = await _terminalService.IsValidCardCombinationAsync(cardId, pin);
            if (isValid)
            {
                return RedirectToAction(nameof(Menu));
            }
            else
            {
                throw new InvalidOperationException("No valid card found with this number-pin combination");
            }
        }

        [TerminalAuthorizationFilter]
        public IActionResult Menu()
        {
            return View();
        }

        [TerminalAuthorizationFilter]
        public async Task<IActionResult> Balance()
        {
            int cardId = HttpContext.Session.GetInt32(SESSSION_KEY_CARD_ID).Value;
            var result = await _terminalService.GetCreditCardDetailsByIdAsync(cardId);
            return View(result);
        }

        [TerminalAuthorizationFilter]
        public IActionResult Withdraw()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [TerminalAuthorizationFilter]
        public async Task<IActionResult> Withdraw(decimal withdrawalAmount)
        {
            int cardId = HttpContext.Session.GetInt32(SESSSION_KEY_CARD_ID).Value;
            var result = await _terminalService.WithdrawByIdAsync(cardId, withdrawalAmount);
            return RedirectToAction(nameof(WithdrawalResult), result);
        }

        [TerminalAuthorizationFilter]
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