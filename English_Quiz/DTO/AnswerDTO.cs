using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace English_Quiz.DTO
{
    public class AnswerDTO
    {
        public int ANSWER_ID { get; set; }
        public string DESCRIPTION { get; set; }
        public string QUESTION_ID { get; set; }
        public Nullable<bool> IS_CORRECT { get; set; }
        public Nullable<int> LIST_ORDER { get; set; }
    }
}