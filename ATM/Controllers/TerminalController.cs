using ATM.Filters;
using ATM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ATM.Controllers
{
    public class TerminalController : Controller
    {
        private ATMContext _context;
        //private ILogger _logger;

        public static readonly string SESSSION_KEY_CARD_ID = "cardId";

        public TerminalController(ATMContext context/*, ILogger logger*/)
        {
            _context = context;
            //_logger = logger;
        }

        public IActionResult Index()
        {
            return View(nameof(Card));
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
            int id;
            id = await _context.GetCardIdByNumberAsync(number);
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
            bool isValid = await _context.IsValidCardCombinationAsync(cardId, pin);
            if (isValid)
            {
                return RedirectToAction(nameof(Menu));
            }
            else
            {
                throw new InvalidCastException("No card identifier provided");
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
            var result = await _context.GetCreditCardDetailsByIdAsync(cardId);
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
            var result = await _context.WithdrawByIdAsync(cardId, withdrawalAmount);
            return RedirectToAction(nameof(WithdrawalResult), result);
        }

        [TerminalAuthorizationFilter]
        public IActionResult WithdrawalResult(WithdrawalResultViewModel viewModel)
        {
            return View(viewModel);
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [TerminalAuthorizationFilter]
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
            if (exceptionHandlerPathFeature?.Path == "/index")
            {
                errorView.ErrorMessage += " from home page";
            }
            if (exceptionHandlerPathFeature?.Path == "/index")
            {
                errorView.ErrorMessage += " from home page";
            }

            return View(errorView);
        }
    }
}