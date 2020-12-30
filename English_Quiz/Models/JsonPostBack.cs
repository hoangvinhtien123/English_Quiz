using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace English_Quiz.Models
{
    public class JsonPostBack
    {
        public JsonPostBack(string message, bool issuccess)
        {
            this.Message = message;
            this.isSuccess = issuccess;
        }

        public string Message { get; set; }
        public bool isSuccess { get; set; }
    }
}