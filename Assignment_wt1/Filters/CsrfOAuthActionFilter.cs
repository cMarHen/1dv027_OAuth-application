using Assignment_wt1.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Assignment_wt1.Filters
{
    public class CsrfOAuthActionFilter : IAsyncActionFilter
    {
        private readonly ISessionHandler _sessionHandler;

        public CsrfOAuthActionFilter(ISessionHandler sessionHandler)
        {
            _sessionHandler = sessionHandler;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            try
            {
                var stateFromRequest = context.HttpContext.Request.Query["state"].ToString();
                var stateFromSession = _sessionHandler.GetSession("oauth_state");

                if (stateFromRequest != stateFromSession)
                {
                    _sessionHandler.ClearSession();
                    context.Result = new RedirectResult("/Error"); // TODO: Send BadRequest
                    return;
                }

                await next();
            }
            catch (Exception)
            {
                context.Result = new RedirectResult("/Error"); // TODO: Send BadRequest
            }

        }
    }
}
