using English_Quiz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace English_Quiz.Controllers
{
    public class TipController : BaseController
    {
        English_QuizEntities db = new English_QuizEntities();
        [CheckPermission(PermissionName = "QuanLyMeoThi", Action = ConstantCommon.Action.View)]
        public ActionResult Index()
        {
            List<Tip> tip = db.Tips.ToList();
            return View(tip);
        }
        [CheckPermission(PermissionName = "QuanLyMeoThi", Action = ConstantCommon.Action.Add)]
        [HttpGet]
        public ActionResult CreateTip()
        {
            ViewBag.ListType = new SelectList(db.Tip_Type.ToList(), "TIP_TYPE_ID", "TIP_TYPE_NAME");
            return View();
        }
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult CreateTip(Tip tip)
        {
            if (ModelState.IsValid)
            {
                db.Tips.Add(tip);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }
        [CheckPermission(PermissionName = "QuanLyMeoThi", Action = ConstantCommon.Action.Edit)]
        [HttpGet]
        public ActionResult EditTip(int id)
        {
            ViewBag.ListType = new SelectList(db.Tip_Type.ToList(), "TIP_TYPE_ID", "TIP_TYPE_NAME");
            Tip tip = db.Tips.FirstOrDefault(x => x.TIP_ID == id);
            return View(tip);
        }
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult EditTip(Tip tip, int id)
        {
            Tip oldTip = db.Tips.FirstOrDefault(x => x.TIP_ID == id);
            if (ModelState.IsValid)
            {
                db.Entry(oldTip).CurrentValues.SetValues(tip);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }
        public JsonResult DeleteTip(int id)
        {
            Function function = db.Functions.FirstOrDefault(x => string.Compare(x.Form_Name, "QuanLyMeoThi", true) == 0);
            int role = int.Parse(Session["Role"].ToString());
            Permission permission = db.Permissions.FirstOrDefault(x => x.Role_Id == role && x.Function_Id == function.Id);
            if (permission.Is_Delete == true)
            {
                try
                {
                    var data = db.Tips.FirstOrDefault(x => x.TIP_ID == id);

                    if (data == null)
                    {
                        return Json(new
                        {
                            Success = false,
                            Message = "Không tìm thấy đối tượng cần xóa."
                        }, JsonRequestBehavior.AllowGet);
                    }
                    db.Tips.Remove(data);
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