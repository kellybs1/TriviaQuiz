using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

//QuestionModel class
//Author: Brendan Kelly
//Date: 10 June 2017
//Description: Full Model class for holding information pulled from a JSON string

namespace kellybs1IN710Trivia.Models
{
    public class QuestionModel
    {

        public string Category { get; set; }
        public string Type { get; set; }
        public string  Question { get; set; }
        public string CorrectAnswer { get; set; }

        public List<string> IncorrectAnswers { get; set; }
        
        public QuestionModel() { }
    }
}