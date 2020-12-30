using English_Quiz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HashPassword;
using Newtonsoft.Json;

namespace English_Quiz.Controllers
{
    public class QuizzController : BaseController
    {
        private DbConnect.DataProvider dbConnect = null;
        public QuizzController()
        {
            dbConnect = new DbConnect.DataProvider();
        }
        English_QuizEntities db = new English_QuizEntities();
        // GET: Quizz
        #region Manage quizz
        [CheckPermission(PermissionName = "QuanLyCauHoi", Action = ConstantCommon.Action.View)]
        public ActionResult Quizz()
        {
            return View(db.Quizs.ToList());
        }
        [CheckPermission(PermissionName = "QuanLyCauHoi", Action = ConstantCommon.Action.Add)]
        [HttpGet]
        public ActionResult CreateQuizz()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateQuizz(Quiz quiz)
        {
            if (ModelState.IsValid)
            {
                db.Quizs.Add(quiz);
                db.SaveChanges();
                return RedirectToAction("Quizz");
            }
            return null;
        }
        [CheckPermission(PermissionName = "QuanLyCauHoi", Action = ConstantCommon.Action.Edit)]
        [HttpGet]
        public ActionResult EditQuizz(int QuizId)
        {
            Quiz quiz = db.Quizs.FirstOrDefault(x => x.QUIZ_ID == QuizId);
            return View(quiz);
        }
        public ActionResult EditQuizz(Quiz quiz , int QuizId)
        {
            Quiz oldQuiz = db.Quizs.FirstOrDefault(x => x.QUIZ_ID == QuizId);
            if (ModelState.IsValid)
            {
                db.Entry(oldQuiz).CurrentValues.SetValues(quiz);
                db.SaveChanges();
                return RedirectToAction("Quizz");
            }
            return null;
        }
        [CheckPermission(PermissionName = "QuanLyCauHoi", Action = ConstantCommon.Action.Delete)]
        public JsonResult delete(int id)
        {
            try
            {
                var data = db.Quizs.FirstOrDefault(x => x.QUIZ_ID == id);
                if (data == null)
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "Không tìm thấy đối tượng cần xóa."
                    }, JsonRequestBehavior.AllowGet);
                }
                db.Quizs.Remove(data);
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
        public JsonResult CreateQuestion(Question question)
        {
            try
            {
                string question_name = (Request["question"] == null) ? string.Empty : Request["question"].ToString();
                string option_a = (Request["option_a"] == null) ? string.Empty : Request["option_a"].ToString();
                string option_b = (Request["option_b"] == null) ? string.Empty : Request["option_b"].ToString();
                string option_c = (Request["option_c"] == null) ? string.Empty : Request["option_c"].ToString();
                string option_d = (Request["option_d"] == null) ? string.Empty : Request["option_d"].ToString();
                string answer = (Request["answer"] == null) ? string.Empty : Request["answer"].ToString();
                string quiz_id = (Request["quiz_id"] == null) ? string.Empty : Request["quiz_id"].ToString();
                string point = (Request["point"] == null) ? string.Empty : Request["point"].ToString();
                string level = (Request["level"] == null) ? "1" : Request["level"].ToString();
                string type = (Request["type"] == null) ? "1" : Request["type"].ToString();
                string insert = @"INSERT INTO [dbo].[Questions]
           ([QUESTION_TEXT]
           ,[OPTION_1]
           ,[OPTION_2]
           ,[OPTION_3]
           ,[OPTION_4]
           ,[POINT]
           ,[LEVEL_ID]
           ,[TYPE_ID]
           ,[ANSWER])
     VALUES
           ('" + question_name + "','" + option_a + "','" + option_b + "','" + option_c + "','" + option_d + "','" + point + "','" + level + "','"+type+"','"+answer+"')";
                dbConnect.ExecuteNonQuery(insert);
                return Json(new
                {
                    Success = true,
                    Message = "Thêm mới thành công"
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new
                {
                    Success = false,
                    Message = e.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }
        public string ListQuestion()
        {
            List<Question> question = db.Questions.SqlQuery("select * from Questions").ToList();
            int quiz_id = (Request["quiz_id"] == null) ? 0 : int.Parse(Request["quiz_id"].ToString());
            var list = db.Quiz_Questions.Where(x => x.QUIZ_ID == quiz_id);
            question.Where(p => {
                foreach (var f in list) if (p.QUESTION_ID == f.QUESTION_ID) return (true); return (false);
            }
            );
            ViewBag.listQuestion = question;
            return JsonConvert.SerializeObject(question);
        }
        public string GetQuestionById(Question question)
        {
            int quiz_id = (Request["quiz_id"] == null) ? 0 : int.Parse(Request["quiz_id"].ToString());

            question = db.Questions.FirstOrDefault(x => int.Parse(x.User_Quiz_Question.ToString()) == quiz_id);
            return JsonConvert.SerializeObject(question);
        }
        public string UpdateQuestion(Question question)
        {
           string question_name = (Request["question"] == null) ? string.Empty : Request["question"].ToString();
            string option_a = (Request["option_a"] == null) ? string.Empty : Request["option_a"].ToString();
            string option_b = (Request["option_b"] == null) ? string.Empty : Request["option_b"].ToString();
            string option_c = (Request["option_c"] == null) ? string.Empty : Request["option_c"].ToString();
            string option_d = (Request["option_d"] == null) ? string.Empty : Request["option_d"].ToString();
            string answer = (Request["answer"] == null) ? string.Empty : Request["answer"].ToString();
            int quiz_id = (Request["quiz_id"] == null) ? 0: int.Parse(Request["quiz_id"].ToString());
            float point = (Request["point"] == null) ? 0 : float.Parse(Request["point"].ToString());
            string level = (Request["level"] == null) ? "1" : Request["level"].ToString();
            string type = (Request["type"] == null) ? "1" : Request["type"].ToString();
            string update = @"UPDATE QUESTIONS SET QUESTION_NAME='" + question_name + "' , OPTION_1='" + option_a + "', OPTION_2='" + option_b + "', OPTION_3='"+option_c+"', OPTION_4='"+option_d+"', POINT='"+point+"', LEVEL_ID='"+level+"', TYPE_ID='"+type+"', ANSWER='"+answer+"'";
            dbConnect.ExecuteNonQuery(update);
            return ListQuestion();
        }
        public JsonResult deleteQuestion(int id)
        {
            try
            {
                var data = "";
                if (data == null)
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "Không tìm thấy đối tượng cần xóa."
                    }, JsonRequestBehavior.AllowGet);
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
        #endregion


    }
}
