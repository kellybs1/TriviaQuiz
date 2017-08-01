using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

//RegisteringUser class
//Author: Brendan Kelly
//Date: 5 June 2017
//Description: Model class for a user in the process of registering - required as has additional password field

namespace kellybs1IN710Trivia.Models
{
    public class RegisteringUser
    {
        public string PlayerName { get; set; }
        public string PlayerPassword1 { get; set; }
        public string PlayerPassword2 { get; set; }
        public RegisteringUser() { }
    }
}