using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

//Constants class
//Author: Brendan Kelly
//Date: 5 June 2017
//Description: Static class to hold constant values used through the project

namespace kellybs1IN710Trivia
{
    public static class Constants
    {
        //urls
        public static string URL_TEN_MULTIPLE_CHOICE = "https://opentdb.com/api.php?amount=10&difficulty=medium&type=multiple";
        public static string URL_TEN_MIXED = "https://opentdb.com/api.php?amount=10&difficulty=medium";
        public static string URL_TEN_TF = "https://opentdb.com/api.php?amount=10&difficulty=medium&type=boolean";
   


        //errors
        public static string ERROR_USER_NOT_EXIST = "User does not exist";
        public static string ERROR_AUTH = "Unable to authenticate user";
        public static string ERROR_USER_EXISTS = "User already exists";
        public static string ERROR_PASSWORD_MATCH = "Password fields do not match";
        public static string ERROR_GENERIC_REGISTER = "Failed to register user";
        public static string ERROR_BLANK_FIELDS = "Fields must not be blank";
        public static string ERROR_FETCHING_QUESTIONS = "Failed to fetch trivia questions";
        public static string ERROR_PARSING_JSON = "Error parsing JSON";
        public static string ERROR_SAVING_SCORE = "Error saving score";
        public static string ERROR_LOADING_DB = "Failed to load data from database";

        //OKs
        public static string MSG_REGISTRATION_OK = "Registered Sucessfully";

        //OK types
        public static string TYPE_REGISTRATION_OK = "Registration Complete";

        //error types
        public static string ERROR_TYPE_LOGIN = "Login error";
        public static string ERROR_TYPE_REGISTER = "Registration error";
        public static string ERROR_TYPE_JSON = "JSON error";
        public static string ERROR_TYPE_DATABASE = "Database error";

        //checking answers
        public static string ANSWER_ACCURACY = "Your score is";
        public static string CORRECT = "Correct!";
        public static string WRONG = "Wrong :-(";

        //view names
        public static string VIEW_RESULT = "Result";
        public static string VIEW_LOGGED_IN = "LoggedIn";
        public static string VIEW_USER_LOGIN_PAGE = "UserPage";
        public static string VIEW_USER_LOGIN_EXTERNAL = "~/Views/User/UserPage.cshtml";
        public static string VIEW_HOME = "Home";
        public static string VIEW_REGISTER = "RegisterUser";
        public static string VIEW_HIGH_SCORES = "HighScores";
        public static string VIEW_QUIZ = "Quiz";
        public static string VIEW_QUESTION_RESULT = "QuestionResult";
        public static string VIEW_FINAL_RESULTS = "FinalResults";

        //key/value pairs
        public static string KEY_CURRENTLY_LOGGED_USER = "LoggedInPlayer";
        public static string KEY_CORRECT_ANSWER_INDEX = "CorrectAnswerIndex";
        public static string KEY_LIST_OF_QUESTIONS = "PulledQuestions";
        public static string KEY_CURRENT_QUESTION_INDEX = "CurrentQuestionIndex";
        public static string KEY_NQUESTIONS_CORRECT = "nAnsweredCorrectly";
        public static string KEY_NTOTAL_QUESTIONS = "nTotalQuestions";
        public static string KEY_PREVIOUSLY_COMPLETED_QUIZ = "CompletedQuiz";
        public static string KEY_USER_ANSWER = "UserA";

        //controllers
        public static string CONTROLLER_QUIZ = "Quiz";
        public static string CONTROLLER_USER = "User";


        //ints
        public static int NON_RESULT = -1;
    }
}