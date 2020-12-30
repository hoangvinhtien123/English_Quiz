using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace English_Quiz.Models
{
    public class ConstantCommon
    {
        public enum Action
        {
            Add = 1,
            Edit = 2,
            Update = 3,
            Delete = 4,
            View = 5,
            Report = 6,
            Login = 7,
            Logout = 8,
            ChangePassword = 9
        }
    }
}