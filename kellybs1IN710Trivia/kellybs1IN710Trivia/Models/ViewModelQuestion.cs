using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

//ViewModelQuestion class
//Author: Brendan Kelly
//Date: 10 June 2017
//Description: Model class for displaying a trivia question

namespace kellybs1IN710Trivia.Models
{
    public class ViewModelQuestion
    {      
        public List<string> Choices { get; set; }
        public string Question { get; set; }

        public int currentQuestion { get; set; }
        public int nQuestionsTotal { get; set; }

        public ViewModelQuestion()
        {
            Choices = new List<string>();
        }
    }

}