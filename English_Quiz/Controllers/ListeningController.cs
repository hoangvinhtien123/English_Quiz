using English_Quiz.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
namespace English_Quiz.Controllers
{
    public class ListeningController : BaseController
    {
        English_QuizEntities db = new English_QuizEntities();
        [CheckPermission(PermissionName = "QuanLyCauHoi", Action = ConstantCommon.Action.View)]
        public ActionResult Index(int? ddlListeningType, int? page)
        {
            var lstListeningType = db.Listening_Type.ToList();
            ViewBag.ddlListeningType = new SelectList(lstListeningType, "LISTENING_TYPE_ID", "LISTENING_TYPE_NAME_VN");
            if (page == null) page = 1;
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            if (ddlListeningType == null)
            {
                var listListening = db.Listenings.ToList().OrderBy(x => x.LISTENING_ID);
                return View(listListening.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                int? listeningType = ddlListeningType;
                ViewBag.listeningType = listeningType;
                var listening = db.Listenings.Where(x => x.LISTENING_TYPE_ID == listeningType).ToList().OrderBy(x => x.LISTENING_ID);
                return View(listening.ToPagedList(pageNumber, pageSize));
            }
        }
        public ActionResult CreateListening()
        {
            ViewBag.listType = new SelectList(db.Listening_Type.ToList(), "LISTENING_TYPE_ID", "LISTENING_TYPE_NAME_VN");
            Listening listening = new Listening();
            int totalListening = db.Listenings.ToList().Count + 1;
            string listeningId = "LISTENING" + totalListening;
            listening.LISTENING_ID = listeningId;
            return View(listening);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult CreateListening(Listening listening, HttpPostedFileBase fUpload)
        {
            if (ModelState.IsValid)
            {
                try
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
                catch (Exception e)
                {
                    ViewBag.errorMsg = "Thêm mới bài nghe bị lỗi, lỗi là : " + e.Message;
                    ViewBag.listType = new SelectList(db.Listening_Type.ToList(), "LISTENING_TYPE_ID", "LISTENING_TYPE_NAME_VN");
                    return View();
                }
                
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
        public ActionResult EditListening(string id, Listening listening, HttpPostedFileBase fUpload)
        {
            Listening oldListening = db.Listenings.FirstOrDefault(x => x.LISTENING_ID == id);
            if (ModelState.IsValid)
            {
                try
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
                catch (Exception e)
                {
                    ViewBag.errorMsg = "Cập nhật bài nghe bị lỗi, lỗi là : " + e.Message;
                    ViewBag.listType = new SelectList(db.Listening_Type.ToList(), "LISTENING_TYPE_ID", "LISTENING_TYPE_NAME_VN");
                    return View();
                }
                
            }
            return null;
        }
        public JsonResult DeleteListening(string id)
        {
            Function function = db.Functions.FirstOrDefault(x => string.Compare(x.Form_Name, "QuanLyCauHoi", true) == 0);
            int role = int.Parse(Session["Role"].ToString());
            Permission permission = db.Permissions.FirstOrDefault(x => x.Role_Id == role && x.Function_Id == function.Id);
            if (permission.Is_Delete == true)
            {
                try
                {
                    var data = db.Listenings.FirstOrDefault(x => x.LISTENING_ID == id);
                    List<Question> questionByListeningId = null;
                    List<Answer> answerByQuestionId = null;
                    if (data == null)
                    {
                        return Json(new
                        {
                            Success = false,
                            Message = "Không tìm thấy đối tượng cần xóa."
                        }, JsonRequestBehavior.AllowGet);
                    }
                    questionByListeningId = db.Questions.Where(x => x.LISTENING_ID == id).ToList();
                    if (questionByListeningId != null)
                    {
                        var questionId = string.Empty;
                        for (int i = 0; i < questionByListeningId.Count; i++)
                        {
                            questionId = questionByListeningId[i].QUESTION_ID;
                            answerByQuestionId = db.Answers.Where(x => x.QUESTION_ID == questionId).ToList();
                            if (answerByQuestionId != null)
                            {
                                for (int j = 0; j < answerByQuestionId.Count; j++)
                                {
                                    db.Answers.Remove(answerByQuestionId[j]);
                                }
                            }
                            db.Questions.Remove(questionByListeningId[i]);
                        }
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

        public string getQuestionByListeningId()
        {
            DataSet ds = new DataSet();
            Function function = db.Functions.FirstOrDefault(x => string.Compare(x.Form_Name, "QuanLyCauHoi", true) == 0);
            int role = int.Parse(Session["Role"].ToString());
            Permission permission = db.Permissions.FirstOrDefault(x => x.Role_Id == role && x.Function_Id == function.Id);
            if (permission.Is_Add == true || permission.Is_Edit == true || permission.Is_Delete == true)
            {
                string listeningId = (Request["listeningId"] == null) ? string.Empty : Request["listeningId"].ToString();
                List<Question> lstQuestion = db.Questions.Where(x => x.LISTENING_ID == listeningId).ToList();
                DataTable questionTbl = new DataTable();
                questionTbl.Columns.Add("QUESTION_ID", typeof(string));
                questionTbl.Columns.Add("QUESTION_TEXT", typeof(string));
                questionTbl.Columns.Add("QUESTION_IMAGE", typeof(string));
                questionTbl.Columns.Add("POINT", typeof(int));
                questionTbl.Columns.Add("READING_ID", typeof(int));
                questionTbl.Columns.Add("LIST_ORDER", typeof(int));
                questionTbl.TableName = "Question";
                string path = string.Empty;
                foreach (var item in lstQuestion)
                {
                    if (item.QUESTION_IMAGE != null)
                    {
                        path = "/Content/img/Question/" + item.QUESTION_IMAGE;
                    }
                    questionTbl.Rows.Add(item.QUESTION_ID, item.QUESTION_TEXT, path, item.POINT, item.READING_ID, item.LIST_ORDER);
                    path = string.Empty;
                }
                ds.Tables.Add(questionTbl);
                int totalQuestion = db.Questions.ToList().Count + 1;
                string questionId = "TOEIC" + totalQuestion;
                DataTable countQuestion = new DataTable();
                countQuestion.Columns.Add("QUESTION_ID", typeof(string));
                countQuestion.Rows.Add(questionId);
                countQuestion.TableName = "NewQuestionId";
                ds.Tables.Add(countQuestion);
                List<Answer> ans = db.Answers.Where(x => x.QUESTION_ID == questionId).ToList();
                if (ans != null && ans.Count > 0)
                {
                    for (int i = 0; i < ans.Count; i++)
                    {
                        db.Answers.Remove(ans[i]);
                    }
                    db.SaveChanges();
                }
                return JsonConvert.SerializeObject(new
                {
                    data = ds,
                    Is_Add = permission.Is_Add,
                    Is_Edit = permission.Is_Edit,
                    Is_Delete = permission.Is_Delete
                });
            }
            else
            {
                return JsonConvert.SerializeObject(new
                {
                    False = true,
                    Message = "Không có quyền cập nhật !"
                });
            }
        }

        public string saveListeningQuestion()
        {
            string listeningId = (Request["listeningId"] == null) ? string.Empty : Request["listeningId"].ToString();
            int point = (Request["point"] == null) ? 0 : int.Parse(Request["point"].ToString());
            string questionId = (Request["questionId"] == null) ? string.Empty : Request["questionId"].ToString();
            string questionText = (Request["questionText"] == null) ? string.Empty : Request["questionText"].ToString();
            bool isNew = (Request["isNew"] == null) ? false : bool.Parse(Request["isNew"].ToString());
            int listOrder = (Request["listOrder"] == string.Empty) ? 0 : int.Parse(Request["listOrder"].ToString());
            HttpPostedFileBase file = (Request.Files.Count == 0) ? null : Request.Files[0];
            string fileName = string.Empty;
            string filePath = string.Empty;
            string path = String.Format("{0}", Server.MapPath("~/Content/img/Question"));
            if (file != null)
            {
                fileName = new FileInfo(file.FileName).Name;
                if (System.IO.File.Exists(path))
                {
                    filePath = Path.Combine(path, Path.GetFileName(fileName));
                }
                else
                {
                    System.IO.Directory.CreateDirectory(path);
                    filePath = Path.Combine(path, Path.GetFileName(fileName));
                }
            }
            if (isNew)
            {
                Question question = new Question();
                question.QUESTION_ID = questionId;
                question.LISTENING_ID = listeningId;
                question.QUESTION_TEXT = questionText;
                question.POINT = point;
                question.QUESTION_IMAGE = fileName;
                question.LIST_ORDER = listOrder;
                db.Questions.Add(question);
                db.SaveChanges();
                if (filePath != string.Empty)
                {
                    file.SaveAs(filePath);
                }
            }
            else
            {
                Question oldQuestion = db.Questions.FirstOrDefault(x => x.QUESTION_ID == questionId && x.LISTENING_ID == listeningId);
                Question newQuestion = new Question();
                newQuestion.QUESTION_ID = questionId;
                newQuestion.LISTENING_ID = listeningId;
                newQuestion.QUESTION_TEXT = questionText;
                newQuestion.POINT = point;
                newQuestion.LIST_ORDER = listOrder;
                if (fileName != string.Empty)
                {
                    newQuestion.QUESTION_IMAGE = fileName;
                }
                else
                {
                    newQuestion.QUESTION_IMAGE = oldQuestion.QUESTION_IMAGE;
                }
                db.Entry(oldQuestion).CurrentValues.SetValues(newQuestion);
                db.SaveChanges();
                if (filePath != string.Empty)
                {
                    file.SaveAs(filePath);
                }
            }
            return getQuestionByListeningId();
        }

        public JsonResult deleteListeningQuestion(string id)
        {
            int role = int.Parse(Session["Role"].ToString());
            Permission permission = db.Permissions.FirstOrDefault(x => x.Role_Id == role);
            if (permission.Is_Delete == true)
            {
                try
                {

                    var data = db.Questions.FirstOrDefault(x => x.QUESTION_ID == id);
                    var answerByQuestionId = db.Answers.Where(x => x.QUESTION_ID == id).ToList();
                    if (data == null)
                    {
                        return Json(new
                        {
                            Success = false,
                            Message = "Không tìm thấy đối tượng cần xóa.",

                        }, JsonRequestBehavior.AllowGet);
                    }
                    db.Questions.Remove(data);
                    for (int i = 0; i < answerByQuestionId.Count; i++)
                    {
                        db.Answers.Remove(answerByQuestionId[i]);
                    }
                    db.SaveChanges();
                    return Json(new
                    {
                        Success = true,
                        Message = "Xóa thành công",

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