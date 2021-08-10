using English_Quiz.DTO;
using English_Quiz.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
namespace English_Quiz.Controllers
{
    public class QuestionController : BaseController
    {
        English_QuizEntities db = new English_QuizEntities();
        [CheckPermission(PermissionName = "QuanLyCauHoi", Action = ConstantCommon.Action.View)]
        public ActionResult Index(int? ddlQuestionType , int? page)
        {
            var lstQuestionType = db.Question_Type.ToList();
            ViewBag.ddlQuestionType = new SelectList(lstQuestionType, "TYPE_ID", "TYPE_NAME");
            if (page == null) page = 1;
            int pageSize = 10;
            int pageNumber = (page ?? 1);
           
            if (ddlQuestionType==null)
            {
                var listQuestion = db.Questions.Where(x => x.LISTENING_ID == null && x.READING_ID == null).ToList().OrderBy(x => x.TYPE_ID);
                return View(listQuestion.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                int? questionType = ddlQuestionType;
                ViewBag.questionType = questionType;
                var question = db.Questions.Where(x => x.LISTENING_ID == null && x.READING_ID == null && x.TYPE_ID == questionType).ToList().OrderBy(x => x.TYPE_ID);
                return View(question.ToPagedList(pageNumber, pageSize));
            }
           
        }
        [CheckPermission(PermissionName = "QuanLyCauHoi", Action = ConstantCommon.Action.Add)]
        public ActionResult CreateQuestion()
        {
           
            ViewBag.ListType = new SelectList(db.Question_Type.ToList(), "TYPE_ID", "TYPE_NAME");
            ViewBag.ListListening = new SelectList(db.Listenings.ToList(), "LISTENING_ID", "LISTENING_NAME_VN");
            Question question = new Question();
            int totalQuestion = db.Questions.ToList().Count + 1;
            string questionId = "TOEIC" + totalQuestion;
            question.QUESTION_ID = questionId;
            List<Answer> ans = db.Answers.Where(x => x.QUESTION_ID == questionId).ToList();
            if(ans!=null && ans.Count > 0)
            {
                for (int i = 0; i < ans.Count; i++)
                {
                    db.Answers.Remove(ans[i]);
                }
                db.SaveChanges();
            }
            return View(question);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult CreateQuestion(Question question)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Questions.Add(question);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    ViewBag.errorMsg = "Thêm mới câu hỏi bị lỗi" ;
                    ViewBag.ListType = new SelectList(db.Question_Type.ToList(), "TYPE_ID", "TYPE_NAME");
                    ViewBag.ListListening = new SelectList(db.Listenings.ToList(), "LISTENING_ID", "LISTENING_NAME_VN");
                    return View();
                }
                
            }
            return null;
        }
        [CheckPermission(PermissionName = "QuanLyCauHoi", Action = ConstantCommon.Action.Edit)]
        public ActionResult EditQuestion(string id)
        {
           
            ViewBag.ListType = new SelectList(db.Question_Type.ToList(), "TYPE_ID", "TYPE_NAME");
            ViewBag.ListListening = new SelectList(db.Listenings.ToList(), "LISTENING_ID", "LISTENING_NAME_VN");
            return View(db.Questions.FirstOrDefault(x=>x.QUESTION_ID == id));
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult EditQuestion(string id , Question question)
        {
            Question oldQuestion = db.Questions.FirstOrDefault(x => x.QUESTION_ID == id);
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(oldQuestion).CurrentValues.SetValues(question);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    ViewBag.errorMsg = "Cập nhật câu hỏi bị lỗi";
                    ViewBag.ListType = new SelectList(db.Question_Type.ToList(), "TYPE_ID", "TYPE_NAME");
                    ViewBag.ListListening = new SelectList(db.Listenings.ToList(), "LISTENING_ID", "LISTENING_NAME_VN");
                    return View();
                }
                
            }
            return null;
        }
        public string GetAnswerByQuestionId()
        {
            string question_id = (Request["question_id"] == null) ? string.Empty : Request["question_id"].ToString();
            
            List<AnswerDTO> DTO = new List<AnswerDTO>();
            List<Answer> ans = db.Answers.Where(x => x.QUESTION_ID == question_id).ToList();
            for (int i = 0; i < ans.Count; i++)
            {
                AnswerDTO answerDTO = new AnswerDTO();
                answerDTO.ANSWER_ID = ans[i].ANSWER_ID;
                answerDTO.DESCRIPTION = ans[i].DESCRIPTION;
                answerDTO.IS_CORRECT = ans[i].IS_CORRECT;
                answerDTO.LIST_ORDER = ans[i].LIST_ORDER;
                answerDTO.QUESTION_ID = ans[i].QUESTION_ID;
                DTO.Add(answerDTO);
            }
            return JsonConvert.SerializeObject(DTO);
        }

        public string AddAnswer()
        {
            string description = (Request["description"] == null) ? string.Empty : Request["description"].ToString();
            string question_id = (Request["question_id"] == null) ? string.Empty : Request["question_id"].ToString();
            int list_order = (Request["list_order"] == null) ? 0 : int.Parse(Request["list_order"].ToString());
            bool is_correct = (Request["is_correct"] == null) ? false : bool.Parse(Request["is_correct"].ToString());
            Answer ans = new Answer();
            ans.DESCRIPTION = description;
            ans.IS_CORRECT = is_correct;
            ans.LIST_ORDER = list_order;
            ans.QUESTION_ID = question_id;
            db.Answers.Add(ans);
            db.SaveChanges();
            return JsonConvert.SerializeObject(ans);
        }
        public void UpdateAnswer()
        {
            int answer_id = (Request["answer_id"] == null) ? 0 : int.Parse( Request["answer_id"].ToString());
            string question_id = (Request["question_id"] == null) ? string.Empty : Request["question_id"].ToString();
            string description = (Request["description"] == null) ? string.Empty : Request["description"].ToString();
            int list_order = (Request["list_order"] == null) ? 0 : int.Parse(Request["list_order"].ToString());
            bool is_correct = (Request["is_correct"] == null) ? false : bool.Parse(Request["is_correct"].ToString());
            Answer ans = db.Answers.Where(x => x.ANSWER_ID == answer_id && x.QUESTION_ID == question_id).FirstOrDefault();
            if (ans != null)
            {
                Answer nAns = new Answer();
                nAns.DESCRIPTION = description;
                nAns.IS_CORRECT = is_correct;
                nAns.LIST_ORDER = list_order;
                nAns.QUESTION_ID = question_id;
                nAns.ANSWER_ID = answer_id;

                db.Entry(ans).CurrentValues.SetValues(nAns);
                db.SaveChanges();
            }
            
        }
        public void DeleteAnswer()
        {
            int answer_id = (Request["answer_id"] == null) ? 0 : int.Parse(Request["answer_id"].ToString());
            string question_id = (Request["question_id"] == null) ? string.Empty : Request["question_id"].ToString();
            Answer ans = db.Answers.Where(x => x.ANSWER_ID == answer_id && x.QUESTION_ID == question_id).FirstOrDefault();
            db.Answers.Remove(ans);
            db.SaveChanges();
        }
        public JsonResult DeleteQuestion(string id)
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
                            Message = "Không tìm thấy đối tượng cần xóa."
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