using ATM.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace ATM.Filters
{
    public class TerminalAuthorizationFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.HttpContext.Session.GetInt32(TerminalController.SESSSION_KEY_CARD_ID).HasValue)
            {
                throw new InvalidOperationException("Can't access this area with no valid card selected");
            }
            base.OnActionExecuting(context);
        }
    }
}
