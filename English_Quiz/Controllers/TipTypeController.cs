using English_Quiz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace English_Quiz.Controllers
{
    public class TipTypeController : Controller
    {
        // GET: TipType
        English_QuizEntities db = new English_QuizEntities();
        [CheckPermission(PermissionName = "QuanLyMeoThi", Action = ConstantCommon.Action.View)]
        public ActionResult Index()
        {
            List<Tip_Type> tip_Types = db.Tip_Type.ToList();
            return View(tip_Types);
        }
        [CheckPermission(PermissionName = "QuanLyMeoThi", Action = ConstantCommon.Action.Add)]
        public ActionResult CreateTipType()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateTipType(Tip_Type tipType)
        {
            if (ModelState.IsValid)
            {
                db.Tip_Type.Add(tipType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }
        [CheckPermission(PermissionName = "QuanLyMeoThi", Action = ConstantCommon.Action.Edit)]
        public ActionResult EditTipType(int id)
        { 
            Tip_Type tipType = db.Tip_Type.FirstOrDefault(x => x.TIP_TYPE_ID == id);
            return View(tipType);
        }
        [HttpPost]
        public ActionResult EditTipType(Tip_Type tipType , int id)
        {
            Tip_Type oldTipType = db.Tip_Type.FirstOrDefault(x => x.TIP_TYPE_ID == id);
            if (ModelState.IsValid)
            {
                db.Entry(oldTipType).CurrentValues.SetValues(tipType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }
        public JsonResult DeleteTipType(int id)
        {
            Function function = db.Functions.FirstOrDefault(x => string.Compare(x.Form_Name, "QuanLyMeoThi", true) == 0);
            int role = int.Parse(Session["Role"].ToString());
            Permission permission = db.Permissions.FirstOrDefault(x => x.Role_Id == role && x.Function_Id == function.Id);
            if (permission.Is_Delete == true)
            {
                try
                {
                    var data = db.Tip_Type.FirstOrDefault(x => x.TIP_TYPE_ID == id);

                    if (data == null)
                    {
                        return Json(new
                        {
                            Success = false,
                            Message = "Không tìm thấy đối tượng cần xóa."
                        }, JsonRequestBehavior.AllowGet);
                    }
                    db.Tip_Type.Remove(data);
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