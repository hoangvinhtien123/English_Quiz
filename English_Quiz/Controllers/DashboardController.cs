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
    public class DashboardController : BaseController
    {
        English_QuizEntities db = new English_QuizEntities();
        // GET: Dashboard
        public ActionResult Index()
        {
            

            return View();
        }
        public string getChart()
        {
            DataSet ds = new DataSet();
            List<Quiz> lstQuiz = db.Quizs.ToList();
            DataTable quizTbl = new DataTable();
            quizTbl.TableName = "Quiz";
            quizTbl.Columns.Add("QUIZ_ID", typeof(string));
            quizTbl.Columns.Add("QUIZ_NAME", typeof(string));
            quizTbl.Columns.Add("TOTAL_USER_TAKE_QUIZ", typeof(string));
            foreach (var item in lstQuiz)
            {
                quizTbl.Rows.Add(item.QUIZ_ID, item.QUIZ_NAME + "-" + item.Quiz_Type.QUIZ_TYPE_NAME, 0);
            }
            List<History_Quiz> lstHistory = db.History_Quiz.ToList();
            for (int i = 0; i < quizTbl.Rows.Count; i++)
            {
                int countUser = 0;
                foreach (var item in lstHistory)
                {
                    if (item.Quiz_ID == quizTbl.Rows[i]["QUIZ_ID"].ToString())
                    {
                        countUser++;
                    }
                }
                quizTbl.Rows[i]["TOTAL_USER_TAKE_QUIZ"] = countUser;
            }
            ds.Tables.Add(quizTbl);
            return JsonConvert.SerializeObject(ds);
        }
    }
}