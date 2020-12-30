using English_Quiz.DTO;
using English_Quiz.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace English_Quiz.Controllers
{
    public class Take_QuizController : Controller
    {
        English_QuizEntities db = new English_QuizEntities();
        // GET: Take_Quiz
        public ActionResult test()
        {
            return View();
        }
        #region Take quizz
        [HttpGet]
        public ActionResult UserLogin()
        {
            return View();
        }
        [HttpPost]
        public ActionResult UserLogin(User login_user)
        {
            try
            {
                var name = login_user.FULL_NAME;
                var pass = toMD5.MD5Hash(login_user.PASSWORD);
                User user = db.Users.Where(x => x.FULL_NAME.Equals(name) && x.PASSWORD == pass).First();
                if (user != null)
                {
                    Session[ConstantData.USER_QUIZZ_SESSION] = user;
                    return RedirectToAction("SelectQuizz");
                }
            }
            catch (Exception e)
            {
                ViewBag.msg = "Mật khẩu hoặc tài khoản bạn nhập sai";
                return View();
            }
            return View();
        }
        [HttpGet]
        public ActionResult SelectQuizz()
        {
            if (Session[ConstantData.USER_QUIZZ_SESSION] == null)
            {
                return RedirectToAction("UserLogin");
            }
            User user = (User)Session[ConstantData.USER_QUIZZ_SESSION];
            List<History_Quiz> lstHistory = db.History_Quiz.Where(x => x.User_Name == user.FULL_NAME).ToList();
            string quiz_id_history = "(";
            int count = 0;
            if (lstHistory.Count > 0)
            {
                foreach(History_Quiz item in lstHistory)
                {
                    count++;
                    if(count == lstHistory.Count)
                    {
                        quiz_id_history += "'" + item.Quiz_ID + "')";
                    }
                    else
                    {
                        quiz_id_history += "'" + item.Quiz_ID + "',";
                    }
                }
                List<Quiz> lstQuiz = db.Quizs.SqlQuery("SELECT * FROM QUIZ WHERE QUIZ_ID not in" + quiz_id_history).ToList<Quiz>();
                ViewBag.ListQuizz = new SelectList(lstQuiz, "QUIZ_ID", "QUIZ_NAME");
            }
            else
            {
                ViewBag.ListQuizz = new SelectList(db.Quizs.ToList(), "QUIZ_ID", "QUIZ_NAME");
            }
            return View();
        }
        [HttpPost]
        public ActionResult SelectQuizz(Quiz quizz)
        {
            List<Quiz> data = db.Quizs.Where(p => p.QUIZ_ID == quizz.QUIZ_ID).ToList();
            if (data != null && data.Count>0)
            {
                Session["QuizzSelected"] = data;
                return RedirectToAction("QuizTest");
            }
            else
            {
                ViewBag.ListQuizz = new SelectList(db.Quizs.ToList(), "QUIZ_ID", "QUIZ_NAME");
                ViewBag.error = "Bạn cần chọn đề trước khi vào thi";
                return View();
            }
           
        }
        [HttpGet]
        public ActionResult QuizTest()
        {
            if (Session[ConstantData.USER_QUIZZ_SESSION] == null)
            {
                return RedirectToAction("UserLogin");
            }
            List<Quiz> quizzSelected = (List<Quiz>)Session["QuizzSelected"];

            List<Question> question = db.Questions.SqlQuery(@"select * from Questions").ToList();
            if (quizzSelected != null && quizzSelected.Count>0)
            {
                
                var list = db.Quiz_Questions.Where(x => x.QUIZ_ID == quizzSelected[0].QUIZ_ID);
                question.Where(p => {
                    foreach (var f in list) if (p.QUESTION_ID == f.QUESTION_ID) return (true); return (false);
                    }
                );
            }
            
            return View(question);
        }
        public string GetAllAnswer()
        {
            List<Quiz> quizzSelected = (List<Quiz>)Session["QuizzSelected"];

            List<Question> question = db.Questions.SqlQuery(@"select * from Questions").ToList();
            if (quizzSelected != null && quizzSelected.Count > 0)
            {

                var list = db.Quiz_Questions.Where(x => x.QUIZ_ID == quizzSelected[0].QUIZ_ID);
                question.Where(p => {
                    foreach (var f in list) if (p.QUESTION_ID == f.QUESTION_ID) return (true); return (false);
                }
                );
            }
            List<QuestionDTO> list_question = new List<QuestionDTO>();
            foreach (var item in question)
            {
                QuestionDTO dto = new QuestionDTO();
                dto.QUESTION_ID = item.QUESTION_ID;
                dto.ANSWER_DESCRIPTION = item.Answer11.DESCRIPTION;
                dto.ANSWER = item.ANSWER;
                dto.POINT = item.POINT;
                list_question.Add(dto);
            }
            return JsonConvert.SerializeObject(list_question);
        }
        public void Quiz_History()
        {
            User user = (User)Session[ConstantData.USER_QUIZZ_SESSION];
            List<Quiz> quizzSelected = (List<Quiz>)Session["QuizzSelected"];
            int quiz_id = quizzSelected[0].QUIZ_ID;
            float point = (Request["point"] == null) ? 0 : float.Parse(Request["point"].ToString());
            History_Quiz quiz_history = db.History_Quiz.Where(x => x.Quiz_ID == quiz_id && x.User_Name == user.FULL_NAME).FirstOrDefault();
            if(quiz_history == null)
            {
                quiz_history = new History_Quiz();
                quiz_history.Quiz_ID = quizzSelected[0].QUIZ_ID;
                quiz_history.Quiz_Name = quizzSelected[0].QUIZ_NAME;
                quiz_history.User_Name = user.FULL_NAME;
                quiz_history.Point = point;
                quiz_history.Date_Take_Quiz = DateTime.Now;
                db.History_Quiz.Add(quiz_history);
                db.SaveChanges();
            }
        }

        public ActionResult History()
        {
            User user = (User)Session[ConstantData.USER_QUIZZ_SESSION];
            if (user != null)
            {
                List<History_Quiz> quiz_history = db.History_Quiz.Where(x => x.User_Name == user.FULL_NAME).ToList();
                return View(quiz_history);
            }
            return View();
        }
        public ActionResult LogOut()
        {
            Session[ConstantData.USER_QUIZZ_SESSION] = null;
            return RedirectToAction("UserLogin");
        }
        #endregion
    }
}