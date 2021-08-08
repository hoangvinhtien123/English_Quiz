using English_Quiz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace English_Quiz.Controllers
{
    public class ReadingTypeController : BaseController
    {
        // GET: ReadingType
        English_QuizEntities db = new English_QuizEntities();
        [CheckPermission(PermissionName = "QuanLyCauHoi", Action = ConstantCommon.Action.View)]
        public ActionResult Index()
        {
            return View(db.Reading_Type.ToList());
        }
        [CheckPermission(PermissionName = "QuanLyCauHoi", Action = ConstantCommon.Action.Add)]
        [HttpGet]
        public ActionResult CreateReadingType()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateReadingType(Reading_Type reading_type)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Reading_Type.Add(reading_type);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    ViewBag.errorMsg = "Thêm mới loại bài đọc bị lỗi";
                    return View();
                }
                
            }
            return null;
        }
        [CheckPermission(PermissionName = "QuanLyCauHoi", Action = ConstantCommon.Action.Edit)]
        [HttpGet]
        public ActionResult EditReadingType(int readingTypeId)
        {
            Reading_Type reading_type = db.Reading_Type.FirstOrDefault(x => x.READING_TYPE_ID == readingTypeId);
            return View(reading_type);
        }
        [HttpPost]
        public ActionResult EditReadingType(int readingTypeId, Reading_Type reading_type)
        {
            Reading_Type oldReadingType = db.Reading_Type.FirstOrDefault(x => x.READING_TYPE_ID == readingTypeId);
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(oldReadingType).CurrentValues.SetValues(reading_type);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                } 
                catch (Exception e)
                {
                    ViewBag.errorMsg = "Cập nhật loại bài đọc bị lỗi";
                    return View();
                }
                
            }
            return null;
        }
        public JsonResult DeleteReadingType(int id)
        {
            Function function = db.Functions.FirstOrDefault(x => string.Compare(x.Form_Name, "QuanLyCauHoi", true) == 0);
            int role = int.Parse(Session["Role"].ToString());
            Permission permission = db.Permissions.FirstOrDefault(x => x.Role_Id == role && x.Function_Id == function.Id);
            if (permission.Is_Delete == true)
            {
                try
                {

                    var data = db.Reading_Type.FirstOrDefault(x => x.READING_TYPE_ID == id);

                    if (data == null)
                    {
                        return Json(new
                        {
                            Success = false,
                            Message = "Không tìm thấy đối tượng cần xóa."
                        }, JsonRequestBehavior.AllowGet);
                    }
                    db.Reading_Type.Remove(data);
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