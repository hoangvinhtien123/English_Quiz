using English_Quiz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HashPassword;
using Newtonsoft.Json;
using English_Quiz.DTO;
using System.Data;
using PagedList;
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
        #region Quản lý bộ đề
        [CheckPermission(PermissionName = "QuanLyCauHoi", Action = ConstantCommon.Action.View)]
        public ActionResult Quizz(string ddlQuizType, int? page)
        {
            var lstQuizType = db.Quiz_Type.ToList();
            ViewBag.ddlQuizType = new SelectList(lstQuizType, "QUIZ_TYPE_ID", "QUIZ_TYPE_NAME");
            if (page == null) page = 1;
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            if ( ddlQuizType == null || ddlQuizType == string.Empty)
            {
                var listQuiz = db.Quizs.ToList().OrderBy(x => x.QUIZ_ID);
                return View(listQuiz.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                string quizType = ddlQuizType;
                ViewBag.quizType = quizType;
                var quiz = db.Quizs.Where(x => x.QUIZ_TYPE_ID == quizType).ToList().OrderBy(x => x.QUIZ_ID);
                return View(quiz.ToPagedList(pageNumber, pageSize));
            }
        }
        [CheckPermission(PermissionName = "QuanLyCauHoi", Action = ConstantCommon.Action.Add)]
        [HttpGet]
        public ActionResult CreateQuizz()
        {
            ViewBag.listType = new SelectList(db.Quiz_Type.ToList(), "QUIZ_TYPE_ID", "QUIZ_TYPE_NAME");
            Quiz quiz = new Quiz();
            int totalQuiz = db.Quizs.ToList().Count + 1;
            string quizId = "Quiz" + totalQuiz;
            quiz.QUIZ_ID = quizId;
            return View(quiz);
        }
        [HttpPost]
        public ActionResult CreateQuizz(Quiz quiz)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Quizs.Add(quiz);
                    db.SaveChanges();
                    return RedirectToAction("Quizz");
                }
                catch(Exception e)
                {
                    ViewBag.errorMsg = "Thêm mới bộ đề bị lỗi, lỗi là : " + e.Message;
                    ViewBag.listType = new SelectList(db.Quiz_Type.ToList(), "QUIZ_TYPE_ID", "QUIZ_TYPE_NAME");
                    return View();
                }
                
            }
            return null;
        }
        [CheckPermission(PermissionName = "QuanLyCauHoi", Action = ConstantCommon.Action.Edit)]
        [HttpGet]
        public ActionResult EditQuizz(string QuizId)
        {
            ViewBag.listType = new SelectList(db.Quiz_Type.ToList(), "QUIZ_TYPE_ID", "QUIZ_TYPE_NAME");
            Quiz quiz = db.Quizs.FirstOrDefault(x => x.QUIZ_ID == QuizId);
            return View(quiz);
        }
        public ActionResult EditQuizz(Quiz quiz , string QuizId)
        {
            Quiz oldQuiz = db.Quizs.FirstOrDefault(x => x.QUIZ_ID == QuizId);
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(oldQuiz).CurrentValues.SetValues(quiz);
                    db.SaveChanges();
                    return RedirectToAction("Quizz");
                }
                catch (Exception e)
                {
                    ViewBag.errorMsg = "Cập nhật bộ đề bị lỗi, lỗi là : " + e.Message;
                    ViewBag.listType = new SelectList(db.Quiz_Type.ToList(), "QUIZ_TYPE_ID", "QUIZ_TYPE_NAME");
                    return View();
                }
            }
            return null;
        }
        public JsonResult delete(string id)
        {
            Function function = db.Functions.FirstOrDefault(x => string.Compare(x.Form_Name, "QuanLyCauHoi", true) == 0);
            int role = int.Parse(Session["Role"].ToString());
            Permission permission = db.Permissions.FirstOrDefault(x => x.Role_Id == role && x.Function_Id == function.Id);
            if (permission.Is_Delete == true)
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
                        Message = "Không thể xóa đối tượng này. Vì sẽ ảnh hưởng đến dữ liệu khác."
                    }, JsonRequestBehavior.AllowGet) ;
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
        #region Quản lý nhóm câu hỏi
        public string getQuestionTypeByQuizId()
        {
            string quizId = (Request["id"] == null) ? string.Empty : Request["id"].ToString();
            List<Questions_Auto_Generate> auto_Generate = db.Questions_Auto_Generate.Where(x => x.QUIZ_ID == quizId).ToList();
            List<Models.Question_Type> questionsType = db.Question_Type.ToList();
            DataSet ds = new DataSet();
            DataTable questions = new DataTable();
            questions.Columns.Add("PR_KEY", typeof(Guid));
            questions.Columns.Add("QUIZ_ID", typeof(string));
            questions.Columns.Add("TYPE_ID", typeof(int));
            questions.Columns.Add("LIST_ORDER", typeof(int));
            questions.Columns.Add("TOTAL_QUESTION", typeof(int));
            questions.TableName = "Questions";
            for (int i = 0; i < auto_Generate.Count; i++)
            {
                questions.Rows.Add(auto_Generate[i].PR_KEY, auto_Generate[i].QUIZ_ID, auto_Generate[i].TYPE_ID, auto_Generate[i].LIST_ORDER, auto_Generate[i].TOTAL_QUESTION);
            }
            ds.Tables.Add(questions);

            DataTable types = new DataTable();
            types.Columns.Add("TYPE_ID", typeof(int));
            types.Columns.Add("TYPE_NAME", typeof(string));
            types.Columns.Add("DESCRIPTION", typeof(string));
            types.TableName = "QuestionTypes";
            for (int i = 0; i < questionsType.Count; i++)
            {
                types.Rows.Add(questionsType[i].TYPE_ID, questionsType[i].TYPE_NAME, questionsType[i].DESCRIPTION);
            }
            ds.Tables.Add(types);
            
            return JsonConvert.SerializeObject(ds);
        }

        public string AddTypeQuestion()
        {
            int type = (Request["type"] == null) ? 0 : int.Parse(Request["type"].ToString());
            string quiz_id = (Request["quiz_id"] == null) ? string.Empty : Request["quiz_id"].ToString();
            int list_order = (Request["list_order"] == null) ? 0 : int.Parse(Request["list_order"].ToString());
            int total_question = (Request["total_question"] == null) ? 0 : int.Parse(Request["total_question"].ToString());
            Questions_Auto_Generate auto_Generate = new Questions_Auto_Generate();
            auto_Generate.PR_KEY = Guid.NewGuid();
            auto_Generate.LIST_ORDER = list_order;
            auto_Generate.QUIZ_ID = quiz_id;
            auto_Generate.TOTAL_QUESTION = total_question;
            auto_Generate.TYPE_ID = type;
            db.Questions_Auto_Generate.Add(auto_Generate);
            db.SaveChanges();
            return JsonConvert.SerializeObject(auto_Generate);
        }
        public void UpdateTypeQuestion()
        {
            Guid pr_key = (Request["pr_key"] == null) ? Guid.NewGuid() : Guid.Parse(Request["pr_key"].ToString());
            int type = (Request["type"] == null) ? 0 : int.Parse(Request["type"].ToString());
            string quiz_id = (Request["quiz_id"] == null) ? string.Empty : Request["quiz_id"].ToString();
            int list_order = (Request["list_order"] == null) ? 0 : int.Parse(Request["list_order"].ToString());
            int total_question = (Request["total_question"] == null) ? 0 : int.Parse(Request["total_question"].ToString());
            Questions_Auto_Generate oldGenerate = db.Questions_Auto_Generate.FirstOrDefault(x => x.PR_KEY == pr_key);
            Questions_Auto_Generate newGenerate = new Questions_Auto_Generate();
            newGenerate.PR_KEY = oldGenerate.PR_KEY;
            newGenerate.LIST_ORDER = list_order;
            newGenerate.QUIZ_ID = quiz_id;
            newGenerate.TOTAL_QUESTION = total_question;
            newGenerate.TYPE_ID = type;
            db.Entry(oldGenerate).CurrentValues.SetValues(newGenerate);
            db.SaveChanges();
        }
        public void DeleteTypeQuestion()
        {
            int type = (Request["type"] == null) ? 0 : int.Parse(Request["type"].ToString());
            string quiz_id = (Request["quiz_id"] == null) ? string.Empty : Request["quiz_id"].ToString();
            Questions_Auto_Generate auto_Generate = db.Questions_Auto_Generate.FirstOrDefault(x => x.QUIZ_ID == quiz_id && x.TYPE_ID == type);
            db.Questions_Auto_Generate.Remove(auto_Generate);
            db.SaveChanges();
        }
        #endregion

        #region Quản lý bài nghe
        public string getListeningById()
        {
            DataSet ds = new DataSet();
            string quizId = (Request["id"] == null) ? string.Empty : Request["id"].ToString();
            List<Quiz_Listening> lstQuizListening = db.Quiz_Listening.Where(x => x.QUIZ_ID == quizId && x.ACTIVE==true).ToList();
            DataTable quizListeningTbl = new DataTable();
            quizListeningTbl.Columns.Add("PR_KEY", typeof(string));
            quizListeningTbl.Columns.Add("QUIZ_ID", typeof(string));
            quizListeningTbl.Columns.Add("LISTENING_ID", typeof(string));
            quizListeningTbl.Columns.Add("ACTIVE", typeof(bool));
            quizListeningTbl.TableName = "Quiz_Listening";
            foreach (var item in lstQuizListening)
            {
                quizListeningTbl.Rows.Add(item.PR_KEY,item.QUIZ_ID, item.LISTENING_ID, item.ACTIVE);
            }
            ds.Tables.Add(quizListeningTbl);
            List<Listening> lstListening = db.Listenings.Where(x => x.IS_TEST == true).ToList();
            DataTable listeningTbl = new DataTable();
            listeningTbl.Columns.Add("LISTENING_ID", typeof(string));
            listeningTbl.Columns.Add("LISTENING_NAME_VN", typeof(string));
            listeningTbl.TableName = "Listening";
            foreach (var item in lstListening)
            {
                listeningTbl.Rows.Add(item.LISTENING_ID, item.LISTENING_NAME_VN);
            }
            ds.Tables.Add(listeningTbl);
            lstListening = db.Listenings.Where(x => x.IS_TEST == false).ToList();
            DataTable practiceListeningTbl = new DataTable();
            practiceListeningTbl.Columns.Add("LISTENING_ID", typeof(string));
            practiceListeningTbl.Columns.Add("LISTENING_NAME_VN", typeof(string));
            practiceListeningTbl.TableName = "ListeningPractice";
            foreach (var item in lstListening)
            {
                practiceListeningTbl.Rows.Add(item.LISTENING_ID, item.LISTENING_NAME_VN);
            }
            ds.Tables.Add(practiceListeningTbl);
            return JsonConvert.SerializeObject(ds);
        }
        public string AddListening()
        {
            string quiz_id = (Request["quiz_id"] == null) ? string.Empty : Request["quiz_id"].ToString();
            string listening_id = (Request["listening_id"] == null) ? string.Empty : Request["listening_id"].ToString();
            bool active = (Request["active"] == null) ? true : bool.Parse(Request["active"].ToString());
            Quiz_Listening quiz_listening = new Quiz_Listening();
            quiz_listening.PR_KEY = Guid.NewGuid();
            quiz_listening.QUIZ_ID = quiz_id;
            quiz_listening.LISTENING_ID = listening_id;
            quiz_listening.ACTIVE = active;
            db.Quiz_Listening.Add(quiz_listening);
            db.SaveChanges();
            return JsonConvert.SerializeObject(quiz_listening);
        }
        public void UpdateListening()
        {
            Guid pr_key = (Request["pr_key"] == null) ? Guid.NewGuid() : Guid.Parse(Request["pr_key"].ToString());
            string quiz_id = (Request["quiz_id"] == null) ? string.Empty : Request["quiz_id"].ToString();
            string listening_id = (Request["listening_id"] == null) ? string.Empty : Request["listening_id"].ToString();
            bool active = (Request["active"] == null) ? true : bool.Parse(Request["active"].ToString());
            Quiz_Listening oldQuizListening = db.Quiz_Listening.FirstOrDefault(x => x.PR_KEY == pr_key);
            Quiz_Listening newQuizListening = new Quiz_Listening();
            newQuizListening.PR_KEY = pr_key;
            newQuizListening.QUIZ_ID = quiz_id;
            newQuizListening.LISTENING_ID = listening_id;
            newQuizListening.ACTIVE = active;
            db.Entry(oldQuizListening).CurrentValues.SetValues(newQuizListening);
            db.SaveChanges();
        }
        public void DeleteListening()
        {
            string quiz_id = (Request["quiz_id"] == null) ? string.Empty : Request["quiz_id"].ToString();
            string listening_id = (Request["listening_id"] == null) ? string.Empty : Request["listening_id"].ToString();
            Quiz_Listening quiz_Listening = db.Quiz_Listening.First(x => x.QUIZ_ID == quiz_id && x.LISTENING_ID == listening_id);
            db.Quiz_Listening.Remove(quiz_Listening);
            db.SaveChanges();
        }
        #endregion

        #region Quản lý bài đọc
        public string getReadingById()
        {
            DataSet ds = new DataSet();
            string quizId = (Request["id"] == null) ? string.Empty : Request["id"].ToString();
            List<Quiz_Reading> quizReading = db.Quiz_Reading.Where(x => x.QUIZ_ID == quizId).ToList();
            List<Reading_Type> readingType = db.Reading_Type.ToList();
            DataTable quizReadingTbl = new DataTable();
            quizReadingTbl.Columns.Add("PR_KEY", typeof(Guid));
            quizReadingTbl.Columns.Add("QUIZ_ID", typeof(string));
            quizReadingTbl.Columns.Add("READING_TYPE_ID", typeof(int));
            quizReadingTbl.Columns.Add("TOTAL_READING", typeof(int));
            quizReadingTbl.Columns.Add("ACTIVE", typeof(bool));
            quizReadingTbl.TableName = "QuizReading";
            for (int i = 0; i < quizReading.Count; i++)
            {
                quizReadingTbl.Rows.Add(quizReading[i].PR_KEY, quizReading[i].QUIZ_ID, quizReading[i].READING_TYPE_ID,quizReading[i].TOTAL_READING,quizReading[i].ACTIVE);
            }
            ds.Tables.Add(quizReadingTbl);

            DataTable types = new DataTable();
            types.Columns.Add("READING_TYPE_ID", typeof(int));
            types.Columns.Add("READING_TYPE_NAME", typeof(string));
            types.TableName = "ReadingType";
            for (int i = 0; i < readingType.Count; i++)
            {
                types.Rows.Add(readingType[i].READING_TYPE_ID, readingType[i].READING_TYPE_NAME);
            }
            ds.Tables.Add(types);
            return JsonConvert.SerializeObject(ds);
        }
        public string AddReading()
        {
            int readingType = (Request["reading_type_id"] == null) ? 0 : int.Parse(Request["reading_type_id"].ToString());
            string quiz_id = (Request["quiz_id"] == null) ? string.Empty : Request["quiz_id"].ToString();
            bool active = (Request["active"] == null) ?false : bool.Parse(Request["active"].ToString());
            int totalReading = (Request["total_reading"] == null) ? 0 : int.Parse(Request["total_reading"].ToString());
            Quiz_Reading quizReading = new Quiz_Reading();
            quizReading.ACTIVE = active;
            quizReading.READING_TYPE_ID = readingType;
            quizReading.QUIZ_ID = quiz_id;
            quizReading.TOTAL_READING = totalReading;
            quizReading.PR_KEY = Guid.NewGuid();
            db.Quiz_Reading.Add(quizReading);
            db.SaveChanges();
            return JsonConvert.SerializeObject(quizReading);
        }
        public void UpdateReading()
        {
            int readingType = (Request["reading_type_id"] == null) ? 0 : int.Parse(Request["reading_type_id"].ToString());
            string quiz_id = (Request["quiz_id"] == null) ? string.Empty : Request["quiz_id"].ToString();
            bool active = (Request["active"] == null) ? false : bool.Parse(Request["active"].ToString());
            int totalReading = (Request["total_reading"] == null) ? 0 : int.Parse(Request["total_reading"].ToString());
            Guid pr_key = (Request["pr_key"] == null) ? Guid.NewGuid() : Guid.Parse(Request["pr_key"].ToString());
            Quiz_Reading oldQuizReading = db.Quiz_Reading.FirstOrDefault(x => x.PR_KEY == pr_key);
            Quiz_Reading newQuizReading = new Quiz_Reading();
            newQuizReading.ACTIVE = active;
            newQuizReading.READING_TYPE_ID = readingType;
            newQuizReading.QUIZ_ID = quiz_id;
            newQuizReading.TOTAL_READING = totalReading;
            newQuizReading.PR_KEY = pr_key;
            db.Entry(oldQuizReading).CurrentValues.SetValues(newQuizReading);
            db.SaveChanges();
        }
        public void DeleteReading()
        {
            Guid pr_key = (Request["pr_key"] == null) ? Guid.NewGuid() : Guid.Parse(Request["pr_key"].ToString());
            int readingType = (Request["reading_type_id"] == null) ? 0 : int.Parse(Request["reading_type_id"].ToString());
            string quiz_id = (Request["quiz_id"] == null) ? string.Empty : Request["quiz_id"].ToString();
            Quiz_Reading quizReading = db.Quiz_Reading.FirstOrDefault(x => x.PR_KEY == pr_key && x.QUIZ_ID == quiz_id);
            db.Quiz_Reading.Remove(quizReading);
            db.SaveChanges();
        }
        #endregion
        #endregion


    }
}
