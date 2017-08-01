using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

//LoggedInUser class
//Author: Brendan Kelly
//Date: 5 June 2017
//Description: Model class for a user that is the currently logged in user - only relevant information

namespace kellybs1IN710Trivia.Models
{
    public class LoggedInUser
    {
        public int id { get; set; }
        public String name { get; set; }
        public LoggedInUser(int inId, String inName) 
        {
            id = inId;
            name = inName;
        }
 
    }
}