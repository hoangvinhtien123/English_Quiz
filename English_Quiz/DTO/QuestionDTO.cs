using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace English_Quiz.DTO
{
    public class QuestionDTO
    {
        public string QUESTION_ID { get; set; }
        public string QUESTION_TEXT { get; set; }
        public Nullable<int> OPTION_1 { get; set; }
        public Nullable<int> OPTION_2 { get; set; }
        public Nullable<int> OPTION_3 { get; set; }
        public Nullable<int> OPTION_4 { get; set; }
        public Nullable<double> POINT { get; set; }
        public Nullable<int> LEVEL_ID { get; set; }
        public Nullable<int> TYPE_ID { get; set; }
        public Nullable<int> ANSWER { get; set; }
        public string ANSWER_DESCRIPTION { get; set; }
    }
}