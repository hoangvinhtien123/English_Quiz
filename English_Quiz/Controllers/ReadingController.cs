using English_Quiz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace English_Quiz.Controllers
{
    public class ReadingController : Controller
    {
        English_QuizEntities db = new English_QuizEntities();
        [CheckPermission(PermissionName = "QuanLyCauHoi", Action = ConstantCommon.Action.View)]
        public ActionResult Index()
        {
            return View(db.Readings.ToList());
        }
        [CheckPermission(PermissionName = "QuanLyCauHoi", Action = ConstantCommon.Action.Add)]
        public ActionResult CreateReading()
        {
            ViewBag.ListType = new SelectList(db.Reading_Type.ToList(), "READING_TYPE_ID", "READING_TYPE_NAME");
            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult CreateReading(Reading reading)
        {
            if (ModelState.IsValid)
            {
                db.Readings.Add(reading);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return null;
        }
        [CheckPermission(PermissionName = "QuanLyCauHoi", Action = ConstantCommon.Action.Edit)]
        public ActionResult EditReading(int id)
        {
            ViewBag.ListType = new SelectList(db.Reading_Type.ToList(), "READING_TYPE_ID", "READING_TYPE_NAME");
            return View(db.Readings.FirstOrDefault(x => x.READING_ID == id));
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult EditQuestionType(int id, Reading reading)
        {
            Reading oldReading = db.Readings.FirstOrDefault(x => x.READING_ID == id);
            if (ModelState.IsValid)
            {
                db.Entry(oldReading).CurrentValues.SetValues(reading);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return null;
        }
        public JsonResult DeleteReading(int id)
        {
            int role = int.Parse(Session["Role"].ToString());
            Permission permission = db.Permissions.FirstOrDefault(x => x.Role_Id == role);
            if (permission.Is_Delete == true)
            {
                try
                {
                    var data = db.Readings.FirstOrDefault(x => x.READING_ID == id);

                    if (data == null)
                    {
                        return Json(new
                        {
                            Success = false,
                            Message = "Không tìm thấy đối tượng cần xóa."
                        }, JsonRequestBehavior.AllowGet);
                    }
                    db.Readings.Remove(data);
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
                        Message = "Không thể xóa đối tượng này. Vì sẽ ảnh hưởng đến dữ liệu khác." + e.Message
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