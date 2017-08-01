using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

//ViewModelHighScores class
//Author: Brendan Kelly
//Date: 5 June 2017
//Description: View Model for listing user scores

namespace kellybs1IN710Trivia.Models
{
    public class ViewModelHighScores : IComparable
    {

        public double AvgScore { get; set; }
        public string PlayerName { get; set; }

        public ViewModelHighScores() { }

        //override default sort to sort by highest score
        //https://msdn.microsoft.com/en-us/library/system.icomparable.compareto(v=vs.110).aspx
        public int CompareTo(object obj)
        {
            if (obj == null)
                return 1;

            if (obj is ViewModelHighScores)
            {
                ViewModelHighScores comparedTo = obj as ViewModelHighScores;
                if (this.AvgScore > comparedTo.AvgScore)
                    return -1;
                else if (this.AvgScore < comparedTo.AvgScore)
                    return 1;
                else
                    return 0;
            }
            else
                throw new ArgumentException("Object not a ViewModelHighScores");
        }
    }
}