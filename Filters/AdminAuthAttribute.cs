using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CloudCompare.Filters
{
    // Redirects to the login page when there is no admin session
    public class AdminAuthAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (string.IsNullOrEmpty(context.HttpContext.Session.GetString("Admin")))
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
            }
        }
    }
}
