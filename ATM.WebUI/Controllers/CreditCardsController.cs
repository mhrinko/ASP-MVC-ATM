using ATM.BusinessLogic.Services;
using ATM.DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ATM.WebUI.Controllers
{
    public class CreditCardsController : Controller
    {
        private readonly CreditCardService _creditCardService;

        public CreditCardsController(CreditCardService creditCardService)
        {
            _creditCardService = creditCardService;
        }

        // GET: CreditCards
        public async Task<IActionResult> Index()
        {
            return View(await _creditCardService.GetAllCreditCards());
        }

        // GET: CreditCards/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var creditCard = await _creditCardService.GetCreditCardById(id.Value);
            if (creditCard == null)
            {
                return NotFound();
            }

            return View(creditCard);
        }

        // GET: CreditCards/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CreditCards/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Number,Pin,Balance,IsValid")] CreditCard creditCard)
        {
            if (ModelState.IsValid)
            {
                await _creditCardService.AddCreditCard(creditCard);
                return RedirectToAction(nameof(Index));
            }
            return View(creditCard);
        }

        // GET: CreditCards/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var creditCard = await _creditCardService.GetCreditCardById(id.Value);
            if (creditCard == null)
            {
                return NotFound();
            }
            return View(creditCard);
        }

        // POST: CreditCards/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Number,Pin,Balance,IsValid")] CreditCard creditCard)
        {
            if (id != creditCard.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _creditCardService.EditCreditCard(creditCard);
                }
                catch (InvalidOperationException)
                {
                    if (!CreditCardExists(creditCard.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(creditCard);
        }

        // GET: CreditCards/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var creditCard = await _creditCardService.GetCreditCardById(id.Value);
            if (creditCard == null)
            {
                return NotFound();
            }

            return View(creditCard);
        }

        // POST: CreditCards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (CreditCardExists(id))
            {
                await _creditCardService.DeleteCreditCardById(id);
            }
            return RedirectToAction(nameof(Index));
        }

        private bool CreditCardExists(int id)
        {
            return _creditCardService.CreditCardExists(id);
        }
    }
}
