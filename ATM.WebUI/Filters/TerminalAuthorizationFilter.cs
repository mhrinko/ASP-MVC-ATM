using ATM.WebUI.Utilities;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace ATM.WebUI.Filters
{
    public class TerminalAuthorizationFilter : ActionFilterAttribute
    {
        private readonly ITerminalSessionManager _sessionManager = new BasicTerminalSessionManager();

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!_sessionManager.IsAuthorized(context.HttpContext))
            {
                throw new InvalidOperationException("Can't access this area without providing a valid number-pin combination");
            }
            base.OnActionExecuting(context);
        }
    }
}
