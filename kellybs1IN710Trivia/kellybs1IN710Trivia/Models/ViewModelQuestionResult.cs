using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

//ViewModelQuestionResult class
//Author: Brendan Kelly
//Date: 10 June 2017
//Description: View Model for outputting result of answering a single question

namespace kellybs1IN710Trivia.Models
{
    public class ViewModelQuestionResult
    {

        public string ResultType { get; set; }
        public string ResultDescription { get; set; }

        public int nQuestionsCorrect { get; set; }
        public int nQuestionsTotal { get; set; }

        //empty constructor
        public ViewModelQuestionResult() { }
    }
}