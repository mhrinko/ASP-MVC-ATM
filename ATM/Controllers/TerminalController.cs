using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ATM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

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
            try
            {
                id = await _context.GetCardIdByNumberAsync(number);
            }
            catch (ArgumentException ex)
            {
                return RedirectToAction(nameof(Error), ex);
            }
            catch (InvalidOperationException ex)
            {
                return RedirectToAction(nameof(Error), ex);
            }
            HttpContext.Session.SetInt32(SESSSION_KEY_CARD_ID, id);
            return RedirectToAction(nameof(Pin));
        }

        public IActionResult Pin()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Pin(int pin)
        {
            int cardId = HttpContext.Session.GetInt32(SESSSION_KEY_CARD_ID).Value;
            bool isValid = await _context.IsValidCardCombinationAsync(cardId, pin);
            if (isValid)
            {
                return RedirectToAction(nameof(Menu));
            }
            return RedirectToAction(nameof(Error), "There is no valid card with this number and pin combination");
        }

        public IActionResult Menu()
        {
            return View();
        }

        public async Task<IActionResult> Balance()
        {
            int cardId = HttpContext.Session.GetInt32(SESSSION_KEY_CARD_ID).Value;
            var result = await _context.GetCreditCardDetailsByIdAsync(cardId);
            return View(result);
        }

        public IActionResult Withdraw()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Withdraw(decimal amount)
        {
            int cardId = HttpContext.Session.GetInt32(SESSSION_KEY_CARD_ID).Value;
            var result = await _context.WithdrawByIdAsync(cardId, amount);
            return RedirectToAction(nameof(WithdrawalResult), result);
        }

        public IActionResult WithdrawalResult(WithdrawalResultViewModel viewModel)
        {
            return View(viewModel);
        }

        public IActionResult Error(Exception ex)
        {
            //_logger.LogInformation($"Handled an error at {ex.Source} with message {ex.Message}");
            //_logger.LogInformation($"Handled an error at {ex.Source} with message {ex.Message}");
            ViewData["ErrorMessage"] = ex.Message;
            return View();
        }

        public IActionResult Error(string message)
        {
            //_logger.LogInformation($"Handled an error at {ex.Source} with message {ex.Message}");
            //_logger.LogInformation($"Handled an error at {ex.Source} with message {ex.Message}");
            ViewData["ErrorMessage"] = message;
            return View();
        }
    }
}