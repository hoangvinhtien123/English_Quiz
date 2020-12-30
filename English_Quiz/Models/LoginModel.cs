using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace English_Quiz.Models
{
    public class LoginModel
    {
        public string username { get; set; }
        public string password { get; set; }

        public bool rememberme { get; set; }
    }
}