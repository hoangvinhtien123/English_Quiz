using English_Quiz.DTO;
using English_Quiz.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace English_Quiz.Controllers
{
    public class QuestionController : BaseController
    {
        English_QuizEntities db = new English_QuizEntities();
        public ActionResult Index()
        {
            return View(db.Questions.ToList());
        }
        public ActionResult CreateQuestion()
        {
            ViewBag.ListLevel = new SelectList(db.Levels.ToList(), "LEVEL_ID", "LEVEL_NAME");
            ViewBag.ListType = new SelectList(db.Types.ToList(), "TYPE_ID", "TYPE_NAME");
            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult CreateQuestion(Question question)
        {
            if (ModelState.IsValid)
            {
                db.Questions.Add(question);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return null;
        }
        public ActionResult EditQuestion(string id)
        {
            ViewBag.ListLevel = new SelectList(db.Levels.ToList(), "LEVEL_ID", "LEVEL_NAME");
            ViewBag.ListType = new SelectList(db.Types.ToList(), "TYPE_ID", "TYPE_NAME");
            return View(db.Questions.FirstOrDefault(x=>x.QUESTION_ID == id));
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

        public void AddAnswer()
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
        }
        public void UpdateAnswer()
        {
            int answer_id = (Request["answer_id"] == null) ? 0 : int.Parse( Request["answer_id"].ToString());
            string question_id = (Request["question_id"] == null) ? string.Empty : Request["question_id"].ToString();
            string description = (Request["description"] == null) ? string.Empty : Request["description"].ToString();
            int list_order = (Request["list_order"] == null) ? 0 : int.Parse(Request["list_order"].ToString());
            bool is_correct = (Request["is_correct"] == null) ? false : bool.Parse(Request["is_correct"].ToString());
            Answer ans = db.Answers.Where(x => x.ANSWER_ID == answer_id && x.QUESTION_ID == question_id).FirstOrDefault();
            Answer nAns = new Answer();
            nAns.DESCRIPTION = description;
            nAns.IS_CORRECT = is_correct;
            nAns.LIST_ORDER = list_order;
            nAns.QUESTION_ID = question_id;
            nAns.ANSWER_ID = answer_id;
            db.Entry(ans).CurrentValues.SetValues(nAns);
            db.SaveChanges();
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
                    Message = "Không thể xóa đối tượng này. Vì sẽ ảnh hưởng đến dữ liệu khác." + e.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}