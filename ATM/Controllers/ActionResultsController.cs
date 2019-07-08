using ATM.Models;
using ATM.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ATM.Controllers
{
    public class ActionResultsController : Controller
    {
        private readonly ActionResultService _actionResultService;

        public ActionResultsController(ActionResultService actionResultService)
        {
            _actionResultService = actionResultService;
        }

        // GET: ActionResults
        public async Task<IActionResult> Index()
        {
            return View(await _actionResultService.GetAllActionResults());
        }

        // GET: ActionResults/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actionResult = await _actionResultService.GetActionResultById(id.Value);
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
                await _actionResultService.EditActionResult(actionResult);
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

            var actionResult = await _actionResultService.GetActionResultById(id.Value);
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
                    await _actionResultService.EditActionResult(actionResult);
                }
                catch (InvalidOperationException)
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

            var actionResult = await _actionResultService.GetActionResultById(id.Value);
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
            if (ActionResultExists(id))
            {
                await _actionResultService.DeleteActionResultById(id); 
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ActionResultExists(int id)
        {
            return _actionResultService.ActionResultExists(id);
        }
    }
}
