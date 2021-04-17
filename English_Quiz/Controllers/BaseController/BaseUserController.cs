using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace English_Quiz.Controllers
{
    public class BaseUserController : Controller
    {
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            
            var controller = filterContext.Controller.ControllerContext;
            if(controller.RouteData.Values["action"].ToString() != "UserLogin")
            {
                Session["ActionName"] = controller.RouteData.Values["action"].ToString();
            }
            base.OnActionExecuted(filterContext); 
        }
    }
}