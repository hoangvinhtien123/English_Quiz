using English_Quiz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace English_Quiz.Controllers
{
    public class ListeningController : BaseController
    {
        English_QuizEntities db = new English_QuizEntities();
        [CheckPermission(PermissionName = "QuanLyCauHoi", Action = ConstantCommon.Action.View)]
        public ActionResult Index()
        {

            return View(db.Listenings.ToList());
        }
        public ActionResult CreateListening()
        {
            ViewBag.listType = new SelectList(db.Listening_Type.ToList(), "LISTENING_TYPE_ID", "LISTENING_TYPE_NAME_VN");
            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult CreateListening(Listening listening , HttpPostedFileBase fUpload)
        {
            if (ModelState.IsValid)
            {
                if (fUpload != null)
                {
                    fUpload.SaveAs(Server.MapPath($"~/Content/audio/{fUpload.FileName}"));
                    listening.LISTENING_FILE_NAME = fUpload.FileName;
                }
                else
                {
                    listening.LISTENING_FILE_NAME = string.Empty;
                }
                db.Listenings.Add(listening);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return null;
        }
        [CheckPermission(PermissionName = "QuanLyCauHoi", Action = ConstantCommon.Action.Edit)]
        public ActionResult EditListening(string id)
        {
            ViewBag.listType = new SelectList(db.Listening_Type.ToList(), "LISTENING_TYPE_ID", "LISTENING_TYPE_NAME_VN");
            return View(db.Listenings.FirstOrDefault(x => x.LISTENING_ID == id));
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult EditListening(string id, Listening listening , HttpPostedFileBase fUpload)
        {
            Listening oldListening = db.Listenings.FirstOrDefault(x => x.LISTENING_ID == id);
            if (ModelState.IsValid)
            {
                if (fUpload != null)
                {
                    fUpload.SaveAs(Server.MapPath($"~/Content/audio/{fUpload.FileName}"));
                    listening.LISTENING_FILE_NAME = fUpload.FileName;
                }
                else
                {
                    listening.LISTENING_FILE_NAME = oldListening.LISTENING_FILE_NAME;
                }
                db.Entry(oldListening).CurrentValues.SetValues(listening);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return null;
        }
        public JsonResult DeleteListening(string id)
        {
            int role = int.Parse(Session["Role"].ToString());
            Permission permission = db.Permissions.FirstOrDefault(x => x.Role_Id == role);
            if (permission.Is_Delete == true)
            {
                try
                {
                    var data = db.Listenings.FirstOrDefault(x => x.LISTENING_ID == id);

                    if (data == null)
                    {
                        return Json(new
                        {
                            Success = false,
                            Message = "Không tìm thấy đối tượng cần xóa."
                        }, JsonRequestBehavior.AllowGet);
                    }
                    db.Listenings.Remove(data);
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