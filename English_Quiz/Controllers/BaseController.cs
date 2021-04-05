using English_Quiz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace English_Quiz.Controllers
{
    public class BaseController : Controller
    {
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var session = Session[ConstantData.USER_SESSION];
            var controller = filterContext.Controller.ControllerContext;
            if (session == null)
            {
                filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new { Controller = "login", Action = "Index" }));
            }
            base.OnActionExecuted(filterContext);
        }
    }
}