using English_Quiz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace English_Quiz.Controllers
{
    public class QuizTypeController : Controller
    {
        English_QuizEntities db = new English_QuizEntities();
        [CheckPermission(PermissionName = "QuanLyCauHoi", Action = ConstantCommon.Action.View)]
        public ActionResult Index()
        {
            return View(db.Quiz_Type.ToList());
        }
        [CheckPermission(PermissionName = "QuanLyCauHoi", Action = ConstantCommon.Action.Add)]
        [HttpGet]
        public ActionResult CreateQuizType()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateQuizType(Quiz_Type quiz_Type)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Quiz_Type.Add(quiz_Type);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch(Exception e)
                {
                    ViewBag.errorMsg = "Thêm mới loại đề bị lỗi";
                    return View();
                }
                
            }
            return null;
        }
        [CheckPermission(PermissionName = "QuanLyCauHoi", Action = ConstantCommon.Action.Edit)]
        [HttpGet]
        public ActionResult EditQuizType(string quizTypeId)
        {
            Quiz_Type quiz_Type = db.Quiz_Type.FirstOrDefault(x => x.QUIZ_TYPE_ID == quizTypeId);
            return View(quiz_Type);
        }
        [HttpPost]
        public ActionResult EditQuizType(string quizTypeId , Quiz_Type quiz_Type)
        {
            Quiz_Type oldQuizType = db.Quiz_Type.FirstOrDefault(x => x.QUIZ_TYPE_ID == quizTypeId);
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(oldQuizType).CurrentValues.SetValues(quiz_Type);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    ViewBag.errorMsg = "Cập nhật loại đề bị lỗi";
                    return View();
                }
                
            }
            return null;
        }
        public JsonResult DeleteQuizType(string id)
        {
            Function function = db.Functions.FirstOrDefault(x => string.Compare(x.Form_Name, "QuanLyCauHoi", true) == 0);
            int role = int.Parse(Session["Role"].ToString());
            Permission permission = db.Permissions.FirstOrDefault(x => x.Role_Id == role && x.Function_Id == function.Id);
            if (permission.Is_Delete == true)
            {
                try
                {

                    var data = db.Quiz_Type.FirstOrDefault(x => x.QUIZ_TYPE_ID == id);
                    
                    if (data == null)
                    {
                        return Json(new
                        {
                            Success = false,
                            Message = "Không tìm thấy đối tượng cần xóa."
                        }, JsonRequestBehavior.AllowGet);
                    }
                    db.Quiz_Type.Remove(data);
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