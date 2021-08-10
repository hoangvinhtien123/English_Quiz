using English_Quiz.DTO;
using English_Quiz.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
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
            ViewData["UserEvaluate"] = db.User_Evaluate_Website.Where(x=>x.ACTIVE == true).ToList();
            return View();
        }
        public ActionResult AboutUs()
        {
            return View();
        }

        public ActionResult LessonType()
        {
            return View(db.Quiz_Type.Where(x => x.IS_TEST == false).ToList());
        }
        public ActionResult SelectLesson(string QuizTypeId)
        {
            if (QuizTypeId != null && QuizTypeId != string.Empty)
            {
                return View(db.Quizs.Where(x => x.QUIZ_TYPE_ID == QuizTypeId && x.IS_TEST == false).OrderBy(x => x.ORDER_NUMBER).ToList());
            }
            else
            {
                return View(db.Quizs.Where(x => x.IS_TEST == false).OrderBy(x => x.ORDER_NUMBER).ToList());
            }
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
                var name = login_user.USER_NAME;
                var pass = toMD5.MD5Hash(login_user.PASSWORD);
                User user = db.Users.Where(x => x.USER_NAME.Equals(name) && x.PASSWORD == pass).First();
                if (user != null)
                {
                    ViewBag.type = "success";
                    ViewBag.msg = "Đăng nhập thành công";
                    Session[ConstantData.USER_QUIZZ_SESSION] = user;
                    return View();
                }
            }
            catch (Exception e)
            {
                ViewBag.type = "false";
                ViewBag.msg = "Mật khẩu hoặc tài khoản bạn nhập sai";
                return View();
            }
            return View();
        }
        public ActionResult UserRegister()
        {
            return View();
        }
        [HttpPost]
        public ActionResult UserRegister(User user)
        {
            string user_name = user.USER_NAME.Trim();
            if (user.FULL_NAME == null || user_name == null || user.PASSWORD == null)
            {
                ViewBag.msg = "Cần nhập đầy đủ thông tin trước khi đăng ký";
                ViewBag.type = "false";
                return View();
            }
            User existUser = db.Users.FirstOrDefault(x => x.USER_NAME == user_name);
            if (existUser != null)
            {
                ViewBag.msg = "Tên đăng nhập đã tồn tại";
                ViewBag.type = "false";
                return View();
            }
            user.PASSWORD = toMD5.MD5Hash(user.PASSWORD);
            user.IS_MANAGE = false;
            user.USER_NAME = user_name;
            user.PROFILE_IMAGE = "default_user_image.png";
            db.Users.Add(user);
            db.SaveChanges();
            ViewBag.msg = "Đăng ký tài khoản thành công";
            ViewBag.type = "success";
            return View();
        }
        public ActionResult LogOut()
        {
            Session[ConstantData.USER_QUIZZ_SESSION] = null;
            return RedirectToAction("UserLogin");
        }

        public ActionResult QuizType()
        {
            return View(db.Quiz_Type.Where(x => x.IS_TEST == true).ToList());
        }
        [HttpGet]
        public ActionResult SelectQuiz(string QuizTypeId)
        {
            if (QuizTypeId != null && QuizTypeId != string.Empty)
            {
                return View(db.Quizs.Where(x => x.QUIZ_TYPE_ID == QuizTypeId && x.IS_TEST == true).OrderBy(x=>x.ORDER_NUMBER).ToList());
            }
            else
            {
                return View(db.Quizs.Where(x => x.IS_TEST == true).OrderBy(x => x.ORDER_NUMBER).ToList());
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
            string listFinalQuestions = "(";
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
                    if (data.IS_TEST == true)
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
                            listFinalQuestions += "'" + lstQuestion[j].QUESTION_ID + "',";
                        }
                    }
                    else
                    {
                        int random = r.Next(0, lstQuestion.Count);
                        int count = random;
                        for (int j = 0; j < number_question; j++)
                        {
                            finalLstQuestion.Add(lstQuestion[random]);
                            listFinalQuestions += "'" + lstQuestion[random].QUESTION_ID + "',";
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
            lstQuestion = db.Questions.Where(x => x.LISTENING_ID != null).OrderBy(x => x.LIST_ORDER).ToList();
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
                                    listFinalQuestions += "'" + lstQuestion[j].QUESTION_ID + "',";
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            #region Reading
            List<Quiz_Reading> quizReading = db.Quiz_Reading.Where(x => x.QUIZ_ID == quizId).ToList();
            List<Reading> readings = new List<Reading>();
            if (quizReading.Count > 0)
            {
                total = 0;
                List<Reading> readingByType = new List<Reading>();
                ViewData["ReadingType"] = db.Reading_Type.ToList();
                for (int i = 0; i < quizReading.Count; i++)
                {

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
                                if (readings[j].READING_TEXT != null && readings[j].READING_TEXT != string.Empty)
                                {
                                    readings[j].READING_TEXT = HttpUtility.HtmlDecode(readings[j].READING_TEXT);
                                }
                                readingByType.Add(readings[j]);
                                int readingId = readings[j].READING_ID;
                                lstQuestion = db.Questions.Where(x => x.READING_ID == readingId).OrderBy(x => x.LIST_ORDER).ToList();
                                for (int m = 0; m < lstQuestion.Count; m++)
                                {
                                    finalLstQuestion.Add(lstQuestion[m]);
                                    listFinalQuestions += "'" + lstQuestion[m].QUESTION_ID + "',";
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
                                if (readingByType[j].READING_TEXT != null && readingByType[j].READING_TEXT != string.Empty)
                                {
                                    readingByType[j].READING_TEXT = HttpUtility.HtmlEncode(readingByType[j].READING_TEXT);
                                }
                                int readingId = readingByType[j].READING_ID;
                                lstQuestion = db.Questions.Where(x => x.READING_ID == readingId).OrderBy(x => x.LIST_ORDER).ToList();
                                for (int m = 0; m < lstQuestion.Count; m++)
                                {
                                    finalLstQuestion.Add(lstQuestion[m]);
                                    listFinalQuestions += "'" + lstQuestion[m].QUESTION_ID + "',";
                                }
                            }
                        }
                    }
                }
                ViewData["Reading"] = readingByType;
            }
            listFinalQuestions += "'')";
            #endregion
            ViewData["Answer"] = db.Answers.SqlQuery($"select * from Answer where QUESTION_ID in {listFinalQuestions}  ORDER BY LIST_ORDER ASC").ToList();

            return View(finalLstQuestion);
        }
        public string GetAllAnswer()
        {
            Quiz quizzSelected = (Quiz)Session["QuizzSelected"];

            List<Question> question = db.Questions.SqlQuery(@"select * from Questions").ToList();
            if (quizzSelected != null)
            {

                var list = db.Quiz_Questions.Where(x => x.QUIZ_ID == quizzSelected.QUIZ_ID);
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
                list_question.Add(dto);
            }
            return JsonConvert.SerializeObject(list_question);
        }
        public string Quiz_History()
        {
            User user = (User)Session[ConstantData.USER_QUIZZ_SESSION];
            Quiz quizzSelected = (Quiz)Session["QuizzSelected"];
            string quiz_id = quizzSelected.QUIZ_ID;
            float point = (Request["point"] == null) ? 0 : float.Parse(Request["point"].ToString());
            History_Quiz quiz_history = null;
            Evaluate evaluate = db.Evaluates.SqlQuery($"select * from Evaluate where FROM_POINT<={point} and TO_POINT >={point}").FirstOrDefault();
            quiz_history = new History_Quiz();
            if (user != null)
            {
                
                quiz_history.Quiz_ID = quizzSelected.QUIZ_ID;
                quiz_history.Quiz_Name = quizzSelected.QUIZ_NAME;
                quiz_history.User_Name = user.USER_NAME;
                quiz_history.Point = point;
                quiz_history.Date_Take_Quiz = DateTime.Today;
                quiz_history.PR_KEY = Guid.NewGuid();
                if (evaluate != null)
                {
                    quiz_history.EVALUATE_ID = evaluate.EVALUATE_ID;
                }
                db.History_Quiz.Add(quiz_history);
                db.SaveChanges();

            }
            else
            {
                quiz_history.Quiz_ID = quizzSelected.QUIZ_ID;
                quiz_history.Quiz_Name = quizzSelected.QUIZ_NAME;
                quiz_history.User_Name = "unknown";
                quiz_history.Point = point;
                quiz_history.Date_Take_Quiz = DateTime.Today;
                quiz_history.PR_KEY = Guid.NewGuid();
                if (evaluate != null)
                {
                    quiz_history.EVALUATE_ID = evaluate.EVALUATE_ID;
                }
                db.History_Quiz.Add(quiz_history);
                db.SaveChanges();
            }
            if (evaluate != null)
            {
                return JsonConvert.SerializeObject(evaluate.EVALUATE_DESCRIPTION);
            }
            else
            {
                return JsonConvert.SerializeObject("Chưa có đánh giá cho thang điểm này");
            }
        }

        public string loadHistoryQuiz()
        {
            string quizId = (Request["quizId"] == null) ? string.Empty : Request["quizId"].ToString();
            User user = (User)Session[ConstantData.USER_QUIZZ_SESSION];
            DataSet ds = new DataSet();
            if (user != null)
            {
                List<History_Quiz> quiz_history = db.History_Quiz.Where(x => x.User_Name == user.USER_NAME && x.Quiz_ID == quizId).ToList();
                if (quiz_history.Count > 0)
                {
                    DataTable quizHistoryTbl = new DataTable();
                    quizHistoryTbl.TableName = "QuizHistory";
                    quizHistoryTbl.Columns.Add("Quiz_ID", typeof(string));
                    quizHistoryTbl.Columns.Add("User_Name", typeof(string));
                    quizHistoryTbl.Columns.Add("Point", typeof(int));
                    quizHistoryTbl.Columns.Add("Quiz_Name", typeof(string));
                    quizHistoryTbl.Columns.Add("Evaluate", typeof(string));
                    quizHistoryTbl.Columns.Add("Date_Take_Quiz", typeof(DateTime));
                    foreach (var item in quiz_history)
                    {

                        quizHistoryTbl.Rows.Add(item.Quiz_ID, item.User_Name, item.Point, item.Quiz_Name, item.Evaluate.EVALUATE_DESCRIPTION, item.Date_Take_Quiz.Value);
                    }
                    ds.Tables.Add(quizHistoryTbl);
                }

            }
            return JsonConvert.SerializeObject(ds);
        }
        public ActionResult PlayFile(string filePath)
        {
            return new FilePathResult(filePath, "audio/wav");
        }

        #endregion

        public ActionResult TipType()
        {
            return View(db.Tip_Type.Where(x=>x.ACTIVE == true).ToList());
        }
        public ActionResult ListTip(int tipTypeId)
        {
            Tip_Type tipType = db.Tip_Type.FirstOrDefault(x => x.TIP_TYPE_ID == tipTypeId && x.ACTIVE == true);
            ViewBag.TipTypeName = tipType.TIP_TYPE_NAME;
            return View(db.Tips.Where(x=>x.TIP_TYPE_ID == tipTypeId && x.ACTIVE == true).ToList());
        }
        public JsonResult SubmitReview()
        {
            try
            {
                string Name = (Request["Name"] == null) ? string.Empty : Request["Name"].ToString();
                string Job = (Request["Job"] == null) ? string.Empty : Request["Job"].ToString();
                string Content = (Request["Content"] == null) ? string.Empty : Request["Content"].ToString();
                User_Evaluate_Website evaluate = new User_Evaluate_Website();
                evaluate.PR_KEY = Guid.NewGuid();
                evaluate.USER_CONTENT_EVALUATE = Content;
                evaluate.USER_JOB = Job;
                evaluate.USER_NAME_EVALUATE = Name;
                evaluate.ACTIVE = false;
                db.User_Evaluate_Website.Add(evaluate);
                db.SaveChanges();
                return Json(new
                {
                    Success = true,
                    Message = "Gửi đánh giá thành công"
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new
                {
                    Success = false,
                    Message = "Gửi đánh giá lỗi, vui lòng thử lại"
                }, JsonRequestBehavior.AllowGet);
            }
            
        }
    }
}