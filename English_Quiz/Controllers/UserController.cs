using English_Quiz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HashPassword;
namespace English_Quiz.Controllers
{
    public class UserController : BaseController
    {
        English_QuizEntities db = new English_QuizEntities();
        // GET: Admin
        [CheckPermission(PermissionName = "QuanLyNguoiDung", Action = ConstantCommon.Action.View)]
        public ActionResult Index()
        {
            return View(db.Users.ToList());
        }
        [CheckPermission(PermissionName = "QuanLyNguoiDung", Action = ConstantCommon.Action.Add)]
        [HttpGet]
        public ActionResult CreateUser()
        {
           
            ViewBag.ListRole = new SelectList(db.Roles.ToList(), "ROLE_ID", "ROLE_NAME");
            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult CreateUser(User user, HttpPostedFileBase fUpload)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (fUpload != null)
                    {
                        fUpload.SaveAs(Server.MapPath($"~/Content/img/User/{fUpload.FileName}"));
                        user.PROFILE_IMAGE = fUpload.FileName;
                    }
                    else
                    {
                        user.PROFILE_IMAGE = "default_user_image.png";
                    }
                    user.PASSWORD = toMD5.MD5Hash(user.PASSWORD);
                    db.Users.Add(user);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    ViewBag.errorMsg = "Thêm mới người dùng bị lỗi";
                    ViewBag.ListRole = new SelectList(db.Roles.ToList(), "ROLE_ID", "ROLE_NAME");
                    return View();
                }
                
            }
            return null;
        }
        [CheckPermission(PermissionName = "QuanLyNguoiDung", Action = ConstantCommon.Action.Edit)]
        public ActionResult EditUser(int UserId)
        {
            User user = db.Users.FirstOrDefault(x => x.USER_ID == UserId);
            ViewBag.ListRole = new SelectList(db.Roles.ToList(), "ROLE_ID", "ROLE_NAME");
            return View(user);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult EditUser(User user ,int UserId, HttpPostedFileBase fUpload)
        {
            User oldUser = db.Users.FirstOrDefault(x => x.USER_ID == UserId);
            if (ModelState.IsValid)
            {
                try
                {
                    if (fUpload != null)
                    {
                        fUpload.SaveAs(Server.MapPath($"~/Content/img/User/{fUpload.FileName}"));
                        user.PROFILE_IMAGE = fUpload.FileName;
                    }
                    else
                    {
                        user.PROFILE_IMAGE = oldUser.PROFILE_IMAGE;
                    }
                    if (user.PASSWORD !=oldUser.PASSWORD)
                    {
                        user.PASSWORD = toMD5.MD5Hash(user.PASSWORD);
                    }
                    db.Entry(oldUser).CurrentValues.SetValues(user);
                    db.SaveChanges();
                    return RedirectToAction("index");
                }
                catch (Exception e)
                {
                    ViewBag.errorMsg = "Cập nhật người dùng bị lỗi";
                    ViewBag.ListRole = new SelectList(db.Roles.ToList(), "ROLE_ID", "ROLE_NAME");
                    return View();
                }
                
            }
            return null;
        }
       
        public JsonResult delete(int id)
        {
            Function function = db.Functions.FirstOrDefault(x => string.Compare(x.Form_Name, "QuanLyNguoiDung", true) == 0);
            int role = int.Parse(Session["Role"].ToString());
            Permission permission = db.Permissions.FirstOrDefault(x => x.Role_Id == role && x.Function_Id == function.Id);
            if (permission.Is_Delete == true)
            {
                try
                {
                    var data = db.Users.FirstOrDefault(x => x.USER_ID == id);
                    if (data == null)
                    {
                        return Json(new
                        {
                            Success = false,
                            Message = "Không tìm thấy đối tượng cần xóa."
                        }, JsonRequestBehavior.AllowGet);
                    }
                    db.Users.Remove(data);
                    db.SaveChanges();
                    return Json(new
                    {
                        Success = true,
                        Message = "Xóa thành công"
                    }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception e)
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "Không thể xóa đối tượng này. Vì sẽ ảnh hưởng đến dữ liệu khác."
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new
                {
                    Success = false,
                    Message = "Không có quyền xóa !"
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}