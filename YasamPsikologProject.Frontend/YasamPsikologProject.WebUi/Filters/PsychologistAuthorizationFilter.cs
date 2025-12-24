using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using YasamPsikologProject.WebUi.Helpers;

namespace YasamPsikologProject.WebUi.Filters
{
    /// <summary>
    /// Psikolog yetkisi kontrolü için Action Filter
    /// </summary>
    public class PsychologistAuthorizationFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            // TEMPORARILY DISABLED FOR TESTING
            // All authorization checks are bypassed
            
            //var session = context.HttpContext.Session;

            //// Session kontrolü
            //if (!session.IsAuthenticated())
            //{
            //    context.Result = new RedirectToActionResult("Login", "Account", null);
            //    return;
            //}

            //// Psikolog rolü kontrolü
            //if (!session.IsPsychologist())
            //{
            //    context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
            //    return;
            //}

            //// Psikolog ID kontrolü
            //if (!session.GetPsychologistId().HasValue)
            //{
            //    context.Result = new RedirectToActionResult("Login", "Account", null);
            //    return;
            //}
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Action tamamlandıktan sonra yapılacak işlemler
        }
    }
}
