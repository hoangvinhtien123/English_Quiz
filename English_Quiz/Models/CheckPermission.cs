using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace English_Quiz.Models
{
    public class CheckPermission : AuthorizeAttribute
    {
        public string PermissionName { get; set; }
        public ConstantCommon.Action Action;
        public bool isLogin = false;

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var _context = new English_QuizEntities();
            User objUser = null;
            objUser = HttpContext.Current.Session[ConstantData.USER_SESSION] as User;
            if (objUser == null)
            {
                isLogin = false;
                return false;
            }
            else
            {
                isLogin = true;
            }
            //Vai trò

            int RoleId = (int)objUser.ROLE_ID;
            Function CNPM = null;
            try
            {
                //Lấy chức năng
                CNPM = _context.Functions.FirstOrDefault(x => string.Compare(x.Form_Name, PermissionName, true) == 0);
                //Kiểm tra chức năng và vai trò
                if (CNPM != null && !string.IsNullOrEmpty(RoleId + ""))
                {
                    Permission objChucNang = null;
                    objChucNang = _context.Permissions.Where(p => (int)p.Role_Id == RoleId && p.Function_Id == CNPM.Id).First();
                    if (objChucNang != null)
                    {
                        if (CheckAction(objChucNang))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (isLogin)
            {
                filterContext.Result = new ViewResult()
                {
                    ViewName = "~/Views/Login/NoPermission.cshtml"
                };
            }
            else
            {
                
                filterContext.Result = new ViewResult()
                {
                    ViewName = "~/Views/Login/Index.cshtml",

                };
            }
        }

        public bool CheckAction(Permission obj)
        {
            switch (Action)
            {
                case ConstantCommon.Action.View:
                    {
                        if (obj.Is_View == true)
                        {
                            return true;
                        }
                    }
                    break;

                case ConstantCommon.Action.Delete:
                    {
                        if (obj.Is_Delete == true)
                        {
                            return true;
                        }
                    }
                    break;

                case ConstantCommon.Action.Edit:
                    {
                        if (obj.Is_Edit == true)
                        {
                            return true;
                        }
                    }
                    break;

                case ConstantCommon.Action.Add:
                    {
                        if (obj.Is_Add == true)
                        {
                            return true;
                        }
                    }
                    break;
                default:
                    return false;
            }
            return false;
        }
    }
}