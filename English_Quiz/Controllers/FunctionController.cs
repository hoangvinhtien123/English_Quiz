using English_Quiz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace English_Quiz.Controllers
{
    public class FunctionController : BaseController
    {
        private English_QuizEntities db = new English_QuizEntities();
        // GET: Function
        [CheckPermission(PermissionName = "QuanLyChucNang", Action = ConstantCommon.Action.View)]
        public ActionResult Index()
        {
            return View(db.Functions.ToList());
        }
    }
}