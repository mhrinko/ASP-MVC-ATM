using Microsoft.AspNetCore.Http;

namespace ATM.WebUI.Utilities
{
    public class BasicTerminalSessionManager : ITerminalSessionManager
    {
        private static readonly string SESSSION_KEY_CARD_ID = "cardId";
        private static readonly string SESSSION_KEY_AUTHORIZED = "isAuthorized";

        public void ClearSession(HttpContext httpContext)
        {
            httpContext.Session.Clear();
        }

        public int? GetSessionCardId(HttpContext httpContext)
        {
            return httpContext.Session.GetInt32(SESSSION_KEY_CARD_ID);
        }

        public void SetSessionCardId(HttpContext httpContext, int id)
        {
            httpContext.Session.SetInt32(SESSSION_KEY_CARD_ID, id);
        }

        public void Authorize(HttpContext httpContext)
        {
            httpContext.Session.SetInt32(SESSSION_KEY_AUTHORIZED, 1);
        }

        public void Unauthorize(HttpContext httpContext)
        {
            httpContext.Session.SetInt32(SESSSION_KEY_AUTHORIZED, 0);
        }

        public bool IsAuthorized(HttpContext httpContext)
        {
            int? val = httpContext.Session.GetInt32(SESSSION_KEY_AUTHORIZED);
            return val != null && val.HasValue && val == 1;
        }
    }
}
