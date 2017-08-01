using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

//ViewModelResult class
//Author: Brendan Kelly
//Date: 5 June 2017
//Description: View Model for results page - to be called after performing action to provide feedback to user

namespace kellybs1IN710Trivia.Models
{
    public class ViewModelResult
    {
        public string ResultType { get; set; }
        public string ResultDescription { get; set; }

        //overload for setting the message at construction
        public ViewModelResult(String type, String description)
        {
            ResultType = type;
            ResultDescription = description;
        }

        //empty constructor
        public ViewModelResult() { }

    }
}