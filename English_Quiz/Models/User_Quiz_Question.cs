//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace English_Quiz.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class User_Quiz_Question
    {
        public int USER_ID { get; set; }
        public string QUIZ_ID { get; set; }
        public string QUESTION_ID { get; set; }
        public string USER_ANSWER { get; set; }
    
        public virtual Question Question { get; set; }
        public virtual Quiz Quiz { get; set; }
        public virtual User User { get; set; }
    }
}
