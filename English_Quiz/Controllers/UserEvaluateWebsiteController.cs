using English_Quiz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace English_Quiz.Controllers
{
    public class UserEvaluateWebsiteController : Controller
    {
        English_QuizEntities db = new English_QuizEntities();
        [CheckPermission(PermissionName = "QuanLyDanhGia", Action = ConstantCommon.Action.View)]
        public ActionResult Index()
        {
            return View(db.User_Evaluate_Website.ToList());
        }
        public JsonResult approvedReview(string id)
        {
            Function function = db.Functions.FirstOrDefault(x => string.Compare(x.Form_Name, "QuanLyCauHoi", true) == 0);
            int role = int.Parse(Session["Role"].ToString());
            Permission permission = db.Permissions.FirstOrDefault(x => x.Role_Id == role && x.Function_Id == function.Id);
            if (permission.Is_Edit== false)
            {
                return Json(new
                {
                    Success = false,
                    Message = "Không có quyền cập nhật"
                }, JsonRequestBehavior.AllowGet);
            }
            Guid prKey = Guid.Parse(id);
            try
            {
                var data = db.User_Evaluate_Website.FirstOrDefault(x => x.PR_KEY == prKey);

                if (data == null)
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "Không tìm thấy đối tượng cần duyệt"
                    }, JsonRequestBehavior.AllowGet);
                }
                User_Evaluate_Website evaluate = new User_Evaluate_Website();
                evaluate.ACTIVE = true;
                evaluate.USER_CONTENT_EVALUATE = data.USER_CONTENT_EVALUATE;
                evaluate.USER_JOB = data.USER_JOB;
                evaluate.PR_KEY = data.PR_KEY;
                evaluate.USER_NAME_EVALUATE = data.USER_NAME_EVALUATE;
                db.Entry(data).CurrentValues.SetValues(evaluate);
                db.SaveChanges();
                return Json(new
                {
                    Success = true,
                    Message = "Duyệt thành công"
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new
                {
                    Success = false,
                    Message = "Lỗi! Không thể duyệt đối tượng này."
                }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult unActiveReview(string id)
        {
            Function function = db.Functions.FirstOrDefault(x => string.Compare(x.Form_Name, "QuanLyCauHoi", true) == 0);
            int role = int.Parse(Session["Role"].ToString());
            Permission permission = db.Permissions.FirstOrDefault(x => x.Role_Id == role && x.Function_Id == function.Id);
            if (permission.Is_Edit == false)
            {
                return Json(new
                {
                    Success = false,
                    Message = "Không có quyền cập nhật"
                }, JsonRequestBehavior.AllowGet);
            }
            Guid prKey = Guid.Parse(id);
            try
            {
                var data = db.User_Evaluate_Website.FirstOrDefault(x => x.PR_KEY == prKey);

                if (data == null)
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "Không tìm thấy đối tượng cần hủy duyệt"
                    }, JsonRequestBehavior.AllowGet);
                }
                User_Evaluate_Website evaluate = new User_Evaluate_Website();
                evaluate.ACTIVE = false;
                evaluate.USER_CONTENT_EVALUATE = data.USER_CONTENT_EVALUATE;
                evaluate.USER_JOB = data.USER_JOB;
                evaluate.PR_KEY = data.PR_KEY;
                evaluate.USER_NAME_EVALUATE = data.USER_NAME_EVALUATE;
                db.Entry(data).CurrentValues.SetValues(evaluate);
                db.SaveChanges();
                return Json(new
                {
                    Success = true,
                    Message = "Hủy duyệt thành công"
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new
                {
                    Success = false,
                    Message = "Lỗi! Không thể hủy duyệt đối tượng này."
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}