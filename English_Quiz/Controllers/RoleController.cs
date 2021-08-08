using English_Quiz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace English_Quiz.Controllers
{
    public class RoleController : BaseController
    {
        English_QuizEntities db = new English_QuizEntities();
        [CheckPermission(PermissionName = "QuanLyVaiTro", Action = ConstantCommon.Action.View)]
        public ActionResult Index()
        {
            List<Role> role = db.Roles.ToList();
            return View(role);
        }
        [CheckPermission(PermissionName = "QuanLyVaiTro", Action = ConstantCommon.Action.Add)]
        [HttpGet]
        public ActionResult CreateRole()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateRole(Role role)
        {
            if (ModelState.IsValid)
            {
                db.Roles.Add(role);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }
        [CheckPermission(PermissionName = "QuanLyVaiTro", Action = ConstantCommon.Action.Edit)]
        [HttpGet]
        public ActionResult EditRole(int id)
        {
            Role role = db.Roles.FirstOrDefault(x => x.ROLE_ID == id);
            return View(role);
        }
        [HttpPost]
        public ActionResult EditRole(Role role, int id)
        {
            Role oldRole = db.Roles.FirstOrDefault(x => x.ROLE_ID == id);
            if (ModelState.IsValid)
            {
                db.Entry(oldRole).CurrentValues.SetValues(role);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }
        
        public JsonResult DeleteVaiTro(int id)
        {
            Function function = db.Functions.FirstOrDefault(x => string.Compare(x.Form_Name, "QuanLyVaiTro", true) == 0);
            int role = int.Parse(Session["Role"].ToString());
            Permission permission = db.Permissions.FirstOrDefault(x => x.Role_Id == role && x.Function_Id == function.Id);
            if (permission.Is_Delete == true)
            {
                try
                {
                    var data = db.Roles.FirstOrDefault(x => x.ROLE_ID == id);
                    if (data == null)
                    {
                        return Json(new
                        {
                            Success = false,
                            Message = "Không tìm thấy đối tượng cần xóa."
                        }, JsonRequestBehavior.AllowGet);
                    }
                    db.Roles.Remove(data);
                    db.SaveChanges();
                    return Json(new
                    {
                        Success = true,
                        Message = "Xóa thành công vai trò này."
                    }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
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