using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ATM.Models;

namespace ATM.Controllers
{
    public class ActionResultsController : Controller
    {
        private readonly ATMContext _context;

        public ActionResultsController(ATMContext context)
        {
            _context = context;
        }

        // GET: ActionResults
        public async Task<IActionResult> Index()
        {
            return View(await _context.ActionResults.ToListAsync());
        }

        // GET: ActionResults/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actionResult = await _context.ActionResults
                .FirstOrDefaultAsync(m => m.Id == id);
            if (actionResult == null)
            {
                return NotFound();
            }

            return View(actionResult);
        }

        // GET: ActionResults/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ActionResults/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CreditCardId,OperationCode,TimeStamp,WithdrawalAmount")] UserActionResult actionResult)
        {
            if (ModelState.IsValid)
            {
                _context.Add(actionResult);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(actionResult);
        }

        // GET: ActionResults/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actionResult = await _context.ActionResults.FindAsync(id);
            if (actionResult == null)
            {
                return NotFound();
            }
            return View(actionResult);
        }

        // POST: ActionResults/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CreditCardId,OperationCode,TimeStamp,WithdrawalAmount")] UserActionResult actionResult)
        {
            if (id != actionResult.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(actionResult);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ActionResultExists(actionResult.Id))
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
            return View(actionResult);
        }

        // GET: ActionResults/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actionResult = await _context.ActionResults
                .FirstOrDefaultAsync(m => m.Id == id);
            if (actionResult == null)
            {
                return NotFound();
            }

            return View(actionResult);
        }

        // POST: ActionResults/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var actionResult = await _context.ActionResults.FindAsync(id);
            _context.ActionResults.Remove(actionResult);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ActionResultExists(int id)
        {
            return _context.ActionResults.Any(e => e.Id == id);
        }
    }
}
