using English_Quiz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace English_Quiz.Controllers
{
    public class LoginController : Controller
    {
        private English_QuizEntities db = new English_QuizEntities();
        // GET: Login
        public ActionResult Index()
        {
            HttpCookie cookie = Request.Cookies[ConstantData.USER_COOKIES];
            //Nhớ mật khẩu
            ViewBag.UserLogin = new LoginModel();
            ViewBag.UserLogin.username = string.Empty;
            ViewBag.UserLogin.password = string.Empty;
            if (cookie != null)
            {
                ViewBag.UserLogin.username = cookie[ConstantData.USERNAME_COOKIES].ToString();
                ViewBag.UserLogin.password = cookie[ConstantData.PASSWORD_COOKIES].ToString();
            }
            return View();
        }
        public ActionResult CheckLogin(string username, string pwd, string remember)
        {
            User obj = new User();
            obj = db.Users.FirstOrDefault(x => x.USER_NAME == username && x.IS_MANAGE == true);

            if (obj!=null)
            {
                if (checkPass(pwd, obj.PASSWORD))
                { //Đăng nhập thành công
                    Session.Add(ConstantData.USER_SESSION, obj);
                    Session.Add("UserName", obj.FULL_NAME);
                    Session.Add("Image", obj.PROFILE_IMAGE);
                    Session.Add("Role", obj.ROLE_ID);
                    if (Convert.ToBoolean(remember))
                    {
                        HttpCookie cookie = new HttpCookie(ConstantData.USER_COOKIES);
                        cookie[ConstantData.USERNAME_COOKIES] = obj.FULL_NAME;
                        cookie[ConstantData.PASSWORD_COOKIES] = pwd;
                        cookie.Expires = DateTime.Now.AddDays(100);
                        Response.Cookies.Add(cookie);
                    }
                    return Json(new { error = false, message = "Đăng nhập thành công" });
                }
                return Json(new { error = true, message = "Mật khẩu không chính xác" });
            }
            return Json(new { error = true, message = "Tài khoản không chính xác" });
        }
        private Boolean checkPass(string pass1 , string pass2)
        {
            if (toMD5.MD5Hash(pass1) == pass2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public ActionResult Logout()
        {
            //AdminBaseController.strControlerCurrent = string.Empty;
            Session[ConstantData.USER_SESSION] = null;
            //Lưu nhật kí

            return RedirectToAction("Index", "login");
        }
    }
}