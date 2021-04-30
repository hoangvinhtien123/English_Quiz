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
    public class Take_QuizController : BaseUserController
    {
        English_QuizEntities db = new English_QuizEntities();
        // GET: Take_Quiz
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult AboutUs()
        {
            return View();
        }
        public ActionResult QuizType()
        {
            return View(db.Quiz_Type.ToList());
        }
        public ActionResult LessonType()
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
                    string action = Session["ActionName"].ToString();
                    return RedirectToAction(action);
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
        public ActionResult SelectQuiz(string QuizTypeId)
        {
            if (QuizTypeId != null && QuizTypeId != string.Empty)
            {
                return View(db.Quizs.Where(x => x.QUIZ_TYPE_ID == QuizTypeId).ToList());
            }
            else
            {
                return View(db.Quizs.ToList());
            }
            //if (Session[ConstantData.USER_QUIZZ_SESSION] == null)
            //{
            //    return RedirectToAction("UserLogin");
            //}
            //User user = (User)Session[ConstantData.USER_QUIZZ_SESSION];
            //List<History_Quiz> lstHistory = db.History_Quiz.Where(x => x.User_Name == user.FULL_NAME).ToList();
            //string quiz_id_history = "(";
            //int count = 0;
            //if (lstHistory.Count > 0)
            //{
            //    foreach(History_Quiz item in lstHistory)
            //    {
            //        count++;
            //        if(count == lstHistory.Count)
            //        {
            //            quiz_id_history += "'" + item.Quiz_ID + "')";
            //        }
            //        else
            //        {
            //            quiz_id_history += "'" + item.Quiz_ID + "',";
            //        }
            //    }
            //    List<Quiz> lstQuiz = db.Quizs.SqlQuery("SELECT * FROM QUIZ WHERE QUIZ_ID not in" + quiz_id_history).ToList<Quiz>();
            //    ViewBag.ListQuizz = new SelectList(lstQuiz, "QUIZ_ID", "QUIZ_NAME");
            //}
            //else
            //{
            //    ViewBag.ListQuizz = new SelectList(db.Quizs.ToList(), "QUIZ_ID", "QUIZ_NAME");
            //}

        }
        [HttpPost]
        public ActionResult SelectQuiz(Quiz quiz)
        {
            List<Quiz> data = db.Quizs.Where(p => p.QUIZ_ID == quiz.QUIZ_ID).ToList();
            if (data != null && data.Count > 0)
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
        public ActionResult QuizTest(string quizId)
        {
            Quiz data = db.Quizs.Where(p => p.QUIZ_ID == quizId).FirstOrDefault();
            List<Question> lstQuestion = new List<Question>();
            List<Question> finalLstQuestion = new List<Question>();
            Random r = new Random();
            int total = 0;
            if (data != null)
            {
                Session["QuizzSelected"] = data;
                ViewBag.time = data.TIME;
                ViewBag.description = data.SOURCE_DESCRIPTION;
                ViewBag.link = data.SOURCE_LINK;
            }
            #region question
            List<Questions_Auto_Generate> lstAutoGenerate = db.Questions_Auto_Generate.SqlQuery($"select * from Questions_Auto_Generate where QUIZ_ID='{quizId}'").ToList();
            for (int i = 0; i < lstAutoGenerate.Count; i++)
            {
                int? type_id = lstAutoGenerate[i].TYPE_ID;
                int? total_question_per_type = lstAutoGenerate[i].TOTAL_QUESTION;

                if (type_id != null && total_question_per_type != null)
                {
                    List<int> lstNumberRandom = new List<int>();
                    int number_question = Convert.ToInt32(total_question_per_type);
                    total += number_question;
                    if(data.IS_TEST == true)
                    {
                        lstQuestion = db.Questions.Where(x => x.TYPE_ID == type_id && x.IS_TEST == true).ToList();
                    }
                    else
                    {
                        lstQuestion = db.Questions.Where(x => x.TYPE_ID == type_id && x.IS_TEST == false).ToList();
                    }
                    if (number_question >= lstQuestion.Count)
                    {
                        for (int j = 0; j < lstQuestion.Count; j++)
                        {
                            finalLstQuestion.Add(lstQuestion[j]);
                        }
                    }
                    else
                    {
                        int random = r.Next(0, lstQuestion.Count);
                        int count = random;
                        for (int j = 0; j < number_question; j++)
                        {
                            finalLstQuestion.Add(lstQuestion[random]);
                            lstNumberRandom.Add(random);
                            while (lstNumberRandom.Contains(random) && finalLstQuestion.Count < total)
                            {
                                count++;
                                if (count < lstQuestion.Count)
                                {
                                    random = count;
                                }
                                else
                                {
                                    count = 0;
                                }
                            }
                        }
                    }
                }
            }
            #endregion
            #region Listening
            lstQuestion = db.Questions.Where(x => x.IS_LISTENING == true).ToList();
            List<Quiz_Listening> lstQuestionListening = db.Quiz_Listening.Where(x => x.QUIZ_ID == quizId).ToList();
            if (lstQuestionListening.Count > 0)
            {
                ViewData["ListeningType"] = db.Listening_Type.SqlQuery("select * from Listening_Type order by ORDER_BY asc").ToList();
                ViewData["Listening"] = db.Listenings.ToList();
                for (int i = 0; i < lstQuestionListening.Count; i++)
                {

                    Quiz_Listening quiz_Listening = lstQuestionListening[i];
                    if (quiz_Listening.ACTIVE == true)
                    {
                        if (quiz_Listening.QUIZ_ID == quizId)
                        {
                            for (int j = 0; j < lstQuestion.Count; j++)
                            {
                                if (lstQuestion[j].LISTENING_ID == quiz_Listening.LISTENING_ID)
                                {
                                    finalLstQuestion.Add(lstQuestion[j]);
                                }
                            }
                        }
                    }
                }
            }
            #endregion
           
            List<Quiz_Reading> quizReading = db.Quiz_Reading.Where(x => x.QUIZ_ID == quizId).ToList();
            List<Reading> readings = new List<Reading>();
            if (quizReading.Count > 0)
            {
                List<Reading> readingByType = new List<Reading>();
                ViewData["ReadingType"] = db.Reading_Type.ToList();
                for (int i = 0; i < quizReading.Count; i++)
                {
                    
                    total = 0;
                    int? readingType = quizReading[i].READING_TYPE_ID;
                    int? totalReading = quizReading[i].TOTAL_READING;
                    if (readingType != null && totalReading != null)
                    {
                        List<int> lstNumberRandom = new List<int>();
                        int number_reading = Convert.ToInt32(totalReading);
                        total += number_reading;
                        if (data.IS_TEST == true)
                        {
                            readings = db.Readings.Where(x => x.IS_TEST == true && x.READING_TYPE_ID == readingType).ToList();
                        }
                        else
                        {
                            readings = db.Readings.Where(x => x.IS_TEST == false && x.READING_TYPE_ID == readingType).ToList();
                        }
                        if (number_reading >= readings.Count)
                        {
                            for (int j = 0; j < readings.Count; j++)
                            {
                                readingByType.Add(readings[j]);
                                lstQuestion = db.Questions.Where(x => x.READING_ID == readings[j].READING_ID).ToList();
                                for (int m = 0; m < lstQuestion.Count; m++)
                                {
                                    finalLstQuestion.Add(lstQuestion[m]);
                                }
                            }
                        }
                        else
                        {
                            int random = r.Next(0, readings.Count);
                            int count = random;
                            for (int j = 0; j < number_reading; j++)
                            {
                                readingByType.Add(readings[random]);
                                lstNumberRandom.Add(random);
                                while (lstNumberRandom.Contains(random) && readingByType.Count < total)
                                {
                                    count++;
                                    if (count < readings.Count)
                                    {
                                        random = count;
                                    }
                                    else
                                    {
                                        count = 0;
                                    }
                                }
                            }
                            for (int j = 0; j < readingByType.Count; j++)
                            {
                                int readingId = readingByType[j].READING_ID;
                                lstQuestion = db.Questions.Where(x => x.READING_ID == readingId).ToList();
                                for (int m = 0; m < lstQuestion.Count; m++)
                                {
                                    finalLstQuestion.Add(lstQuestion[m]);
                                }
                            }
                        }
                    }
                }
                ViewData["Reading"] = readingByType;
            }
            
            
            ViewData["Answer"] = db.Answers.SqlQuery(@"select * from Answer ORDER BY LIST_ORDER ASC").ToList();

            return View(finalLstQuestion);
        }
        public string GetAllAnswer()
        {
            List<Quiz> quizzSelected = (List<Quiz>)Session["QuizzSelected"];

            List<Question> question = db.Questions.SqlQuery(@"select * from Questions").ToList();
            if (quizzSelected != null && quizzSelected.Count > 0)
            {

                var list = db.Quiz_Questions.Where(x => x.QUIZ_ID == quizzSelected[0].QUIZ_ID);
                question.Where(p =>
                {
                    foreach (var f in list) if (p.QUESTION_ID == f.QUESTION_ID) return (true); return (false);
                }
                );
            }
            List<QuestionDTO> list_question = new List<QuestionDTO>();
            foreach (var item in question)
            {
                QuestionDTO dto = new QuestionDTO();
                dto.QUESTION_ID = item.QUESTION_ID;
                Answer ans = db.Answers.SqlQuery($"select * from Answer where QUESTION_ID='{item.QUESTION_ID}' and IS_CORRECT=1 ").FirstOrDefault();
                dto.ANSWER_ID = ans.ANSWER_ID;
                dto.POINT = item.POINT;
                dto.ANSWER_DESCRIPTION = ans.DESCRIPTION;
                //dto.ANSWER_DESCRIPTION = item.DESCRIPTION;
                //dto.ANSWER = item.;
                //dto.POINT = item.POINT;
                list_question.Add(dto);
            }
            return JsonConvert.SerializeObject(list_question);
        }
        //public void Quiz_History()
        //{
        //    User user = (User)Session[ConstantData.USER_QUIZZ_SESSION];
        //    if (user != null)
        //    {
        //        List<Quiz> quizzSelected = (List<Quiz>)Session["QuizzSelected"];
        //        string quiz_id = quizzSelected[0].QUIZ_ID;
        //        float point = (Request["point"] == null) ? 0 : float.Parse(Request["point"].ToString());
        //        History_Quiz quiz_history = db.History_Quiz.Where(x => x.Quiz_ID == quiz_id && x.User_Name == user.FULL_NAME).FirstOrDefault();
        //        if (quiz_history == null)
        //        {
        //            quiz_history = new History_Quiz();
        //            quiz_history.Quiz_ID = quizzSelected[0].QUIZ_ID;
        //            quiz_history.Quiz_Name = quizzSelected[0].QUIZ_NAME;
        //            quiz_history.User_Name = user.FULL_NAME;
        //            quiz_history.Point = point;
        //            quiz_history.Date_Take_Quiz = DateTime.Now;
        //            db.History_Quiz.Add(quiz_history);
        //            db.SaveChanges();
        //        }
        //    }

        //}

        //public ActionResult History()
        //{
        //    User user = (User)Session[ConstantData.USER_QUIZZ_SESSION];
        //    if (user != null)
        //    {
        //        List<History_Quiz> quiz_history = db.History_Quiz.Where(x => x.User_Name == user.FULL_NAME).ToList();
        //        return View(quiz_history);
        //    }
        //    return View();
        //}
        public ActionResult PlayFile(string filePath)
        {
            return new FilePathResult(filePath, "audio/wav");
        }
        public ActionResult LogOut()
        {
            Session[ConstantData.USER_QUIZZ_SESSION] = null;
            return RedirectToAction("UserLogin");
        }
        #endregion
    }
}