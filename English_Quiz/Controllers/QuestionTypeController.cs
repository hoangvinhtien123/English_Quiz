using English_Quiz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace English_Quiz.Controllers
{
    public class QuestionTypeController : BaseController
    {
        English_QuizEntities db = new English_QuizEntities();
        [CheckPermission(PermissionName = "QuanLyCauHoi", Action = ConstantCommon.Action.View)]
        public ActionResult Index()
        {
            return View(db.Question_Type.ToList());
        }
        [CheckPermission(PermissionName = "QuanLyCauHoi", Action = ConstantCommon.Action.Add)]
        public ActionResult CreateQuestionType()
        {
            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult CreateQuestionType(Question_Type questionType)
        {
            if (ModelState.IsValid)
            {
                db.Question_Type.Add(questionType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return null;
        }
        [CheckPermission(PermissionName = "QuanLyCauHoi", Action = ConstantCommon.Action.Edit)]
        public ActionResult EditQuestionType(int id)
        {
            return View(db.Question_Type.FirstOrDefault(x => x.TYPE_ID == id));
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult EditQuestionType(int id, Question_Type questionType)
        {
            Question_Type oldQuestionType = db.Question_Type.FirstOrDefault(x => x.TYPE_ID == id);
            if (ModelState.IsValid)
            {
                db.Entry(oldQuestionType).CurrentValues.SetValues(questionType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return null;
        }
        public JsonResult DeleteQuestionType(int id)
        {
            int role = int.Parse(Session["Role"].ToString());
            Permission permission = db.Permissions.FirstOrDefault(x => x.Role_Id == role);
            if (permission.Is_Delete == true)
            {
                try
                {
                    var data = db.Question_Type.FirstOrDefault(x => x.TYPE_ID == id);

                    if (data == null)
                    {
                        return Json(new
                        {
                            Success = false,
                            Message = "Không tìm thấy đối tượng cần xóa."
                        }, JsonRequestBehavior.AllowGet);
                    }
                    db.Question_Type.Remove(data);
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