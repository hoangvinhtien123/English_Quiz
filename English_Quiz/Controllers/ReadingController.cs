using English_Quiz.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace English_Quiz.Controllers
{
    public class ReadingController : Controller
    {
        English_QuizEntities db = new English_QuizEntities();
        [CheckPermission(PermissionName = "QuanLyCauHoi", Action = ConstantCommon.Action.View)]
        public ActionResult Index(int? ddlReadingType , int? page)
        {
            var lstReadingType = db.Reading_Type.ToList();
            ViewBag.ddlReadingType = new SelectList(lstReadingType, "READING_TYPE_ID", "READING_TYPE_NAME");
            if (page == null) page = 1;
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            if (ddlReadingType == null)
            {
                var listReading = db.Readings.ToList().OrderBy(x => x.READING_ID);
                return View(listReading.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                int? readingType = ddlReadingType;
                ViewBag.readingType = readingType;
                var reading = db.Readings.Where(x=>x.READING_TYPE_ID == readingType).ToList().OrderBy(x => x.READING_ID);
                return View(reading.ToPagedList(pageNumber, pageSize));
            }
        }
        [CheckPermission(PermissionName = "QuanLyCauHoi", Action = ConstantCommon.Action.Add)]
        public ActionResult CreateReading()
        {
            ViewBag.ListType = new SelectList(db.Reading_Type.ToList(), "READING_TYPE_ID", "READING_TYPE_NAME");
            return View();
        }
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult CreateReading(Reading reading , HttpPostedFileBase fUpload)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (fUpload != null)
                    {
                        fUpload.SaveAs(Server.MapPath($"~/Content/img/Reading/{fUpload.FileName}"));
                        reading.READING_IMAGE = fUpload.FileName;
                    }

                    db.Readings.Add(reading);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    ViewBag.errorMsg = "Thêm mới bài đọc bị lỗi, lỗi là : " + e.Message;
                    ViewBag.ListType = new SelectList(db.Reading_Type.ToList(), "READING_TYPE_ID", "READING_TYPE_NAME");
                    return View();
                }
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
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult EditReading(int id, Reading reading , HttpPostedFileBase fUpload)
        {
            Reading oldReading = db.Readings.FirstOrDefault(x => x.READING_ID == id);
            if (ModelState.IsValid)
            {
                try
                {
                    if (fUpload != null)
                    {
                        fUpload.SaveAs(Server.MapPath($"~/Content/img/Reading/{fUpload.FileName}"));
                        reading.READING_IMAGE = fUpload.FileName;
                    }
                    else
                    {
                        reading.READING_IMAGE = oldReading.READING_IMAGE;
                    }
                    db.Entry(oldReading).CurrentValues.SetValues(reading);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    ViewBag.errorMsg = "Cập nhật bài đọc bị lỗi, lỗi là : "+ e.Message;
                    ViewBag.ListType = new SelectList(db.Reading_Type.ToList(), "READING_TYPE_ID", "READING_TYPE_NAME");
                    return View();
                }
                
            }
            return null;
        }
        public JsonResult DeleteReading(int id)
        {
            Function function = db.Functions.FirstOrDefault(x => string.Compare(x.Form_Name, "QuanLyCauHoi", true) == 0);
            int role = int.Parse(Session["Role"].ToString());
            Permission permission = db.Permissions.FirstOrDefault(x => x.Role_Id == role && x.Function_Id == function.Id);
            if (permission.Is_Delete == true)
            {
                try
                {
                    var data = db.Readings.FirstOrDefault(x => x.READING_ID == id);
                    List<Question> questionByReadingId = null;
                    List<Answer> answerByQuestionId = null;
                    if (data == null)
                    {
                        return Json(new
                        {
                            Success = false,
                            Message = "Không tìm thấy đối tượng cần xóa."
                        }, JsonRequestBehavior.AllowGet);
                    }
                    
                    questionByReadingId = db.Questions.Where(x => x.READING_ID == id).ToList();
                    if (questionByReadingId !=null)
                    {
                        var questionId = string.Empty;
                        for (int i = 0; i < questionByReadingId.Count; i++)
                        {
                            questionId = questionByReadingId[i].QUESTION_ID;
                            answerByQuestionId = db.Answers.Where(x => x.QUESTION_ID == questionId).ToList();
                            if (answerByQuestionId != null)
                            {
                                for (int j = 0; j < answerByQuestionId.Count; j++)
                                {
                                    db.Answers.Remove(answerByQuestionId[j]);
                                }
                            }
                            db.Questions.Remove(questionByReadingId[i]);
                        }
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

        public string getQuestionByReadingId()
        {
            DataSet ds = new DataSet();
            Function function = db.Functions.FirstOrDefault(x => string.Compare(x.Form_Name, "QuanLyCauHoi", true) == 0);
            int role = int.Parse(Session["Role"].ToString());
            Permission permission = db.Permissions.FirstOrDefault(x => x.Role_Id == role && x.Function_Id == function.Id);
            if (permission.Is_Add == true || permission.Is_Edit == true || permission.Is_Delete == true)
            {
                int readingId = (Request["readingId"] == null) ? 0 : int.Parse(Request["readingId"].ToString());
                List<Question> lstQuestion = db.Questions.Where(x => x.READING_ID == readingId && x.LISTENING_ID == null).ToList();
                DataTable questionTbl = new DataTable();
                questionTbl.Columns.Add("QUESTION_ID", typeof(string));
                questionTbl.Columns.Add("QUESTION_TEXT", typeof(string));
                questionTbl.Columns.Add("POINT", typeof(int));
                questionTbl.Columns.Add("READING_ID", typeof(int));
                questionTbl.Columns.Add("LIST_ORDER", typeof(int));
                questionTbl.TableName = "Question";
                foreach (var item in lstQuestion)
                {
                    questionTbl.Rows.Add(item.QUESTION_ID, item.QUESTION_TEXT, item.POINT, item.READING_ID, item.LIST_ORDER);
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
                return JsonConvert.SerializeObject(new { 
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

        public string saveReadingQuestion()
        {
            int readingId = (Request["readingId"] == null) ? 0 : int.Parse(Request["readingId"].ToString());
            int point = (Request["point"] == null) ? 0 : int.Parse(Request["point"].ToString());
            string questionId = (Request["questionId"] == null) ? string.Empty : Request["questionId"].ToString();
            string questionText = (Request["questionText"] == null) ? string.Empty : Request["questionText"].ToString();
            bool isNew = (Request["isNew"] == null) ? false : bool.Parse(Request["isNew"].ToString());
            int listOrder = (Request["listOrder"] == null) ? 0 : int.Parse(Request["listOrder"].ToString());
            if (isNew)
            {
                Question question = new Question();
                question.QUESTION_ID = questionId;
                question.READING_ID = readingId;
                question.QUESTION_TEXT = questionText;
                question.POINT = point;
                question.LIST_ORDER = listOrder;
                db.Questions.Add(question);
                db.SaveChanges();
            }
            else
            {
                Question oldQuestion = db.Questions.FirstOrDefault(x => x.QUESTION_ID == questionId && x.READING_ID == readingId);
                Question newQuestion = new Question();
                newQuestion.QUESTION_ID = questionId;
                newQuestion.READING_ID = readingId;
                newQuestion.QUESTION_TEXT = questionText;
                newQuestion.POINT = point;
                newQuestion.LIST_ORDER = listOrder;
                db.Entry(oldQuestion).CurrentValues.SetValues(newQuestion);
                db.SaveChanges();
            }
            return getQuestionByReadingId();
        }

        public JsonResult deleteReadingQuestion(string id)
        {
            Function function = db.Functions.FirstOrDefault(x => string.Compare(x.Form_Name, "QuanLyCauHoi", true) == 0);
            int role = int.Parse(Session["Role"].ToString());
            Permission permission = db.Permissions.FirstOrDefault(x => x.Role_Id == role && x.Function_Id == function.Id);
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
                            
                        }, JsonRequestBehavior.AllowGet) ;
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