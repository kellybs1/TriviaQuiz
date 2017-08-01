using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using kellybs1IN710Trivia.Models;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;

//TriviaController class
//Author: Brendan Kelly
//Date: 3 June 2017
//Description: Main controller for trivia quiz project

namespace kellybs1IN710Trivia.Controllers
{

    public class TriviaController : Controller
    {
        private TriviaDbDataContext triviaDb;
        private Random rand;

        public TriviaController()
        {
            triviaDb = new TriviaDbDataContext();
            rand = new Random();
        }

        // GET: Trivia
        public ActionResult Home()
        {
            return View();
        }
     
        
        //saves the current score for the current user in the db
        private bool SaveCurrentScore()
        {
            bool success = false;
            //get score and user information
            int score = (int)Session[Constants.KEY_NQUESTIONS_CORRECT];
            LoggedInUser user = (LoggedInUser)Session[Constants.KEY_CURRENTLY_LOGGED_USER];
            //make a new reult
            tblResult currentResult = new tblResult();
            currentResult.PlayerID = user.id;
            currentResult.ResultDateTime = DateTime.Now;
            currentResult.Score = score;
            //add changes to database
            triviaDb.tblResults.InsertOnSubmit(currentResult);
            try
            {
                triviaDb.SubmitChanges();
                success = true;
            }
            catch (Exception e)
            {
                //catch the error and go to error page
                Console.WriteLine(e);
                success = false;
            }

            return success;
        }
       

        //generate high scores page
        [HttpGet]
        public ActionResult HighScores()
        {
            try
            {
                //create viewmodel list for records of scores
                List<ViewModelHighScores> scores = new List<ViewModelHighScores>();
                //get the current month/year
                int currentMonth = DateTime.Now.Month;
                int currentYear = DateTime.Now.Year;
                //extract user
                var userIds = from p in triviaDb.tblPlayers
                              select new { p.PlayerID, p.PlayerName };

                //for each user
                foreach (var uId in userIds)
                {
                    //extract scores for single user from current month
                    var userScores = from r in triviaDb.tblResults
                                     where r.PlayerID.Equals(uId.PlayerID)
                                     where r.ResultDateTime.Month.Equals(currentMonth)
                                     where r.ResultDateTime.Year.Equals(currentYear)
                                     //must be a double before averaging or Linq to SQL computes it as an integer
                                     select (double)r.Score;
                    //average all scores
                    double avgScore = 0;
                    //if there are scores for this user, that is...
                    if (userScores.Count() > 0)
                    {
                        avgScore = userScores.Average();
                    }
                    //build viewmodel
                    ViewModelHighScores currentScore = new ViewModelHighScores();
                    currentScore.AvgScore = avgScore;
                    currentScore.PlayerName = uId.PlayerName;
                    scores.Add(currentScore);
                }

                //sort and return
                List<ViewModelHighScores> scoresSorted = scores.OrderByDescending(s => s.AvgScore).ToList();
                return View(scoresSorted);
            }
            catch (SqlException e)
            {
                Console.WriteLine(e);
                ViewModelResult dbFail = new ViewModelResult(Constants.ERROR_TYPE_DATABASE, Constants.ERROR_LOADING_DB);
                return View(Constants.VIEW_RESULT, dbFail);
            }     
        }


        //fetches list of questions from JSON object
        private List<QuestionModel> fetchQuestionsFromJSON()
        {
            List<QuestionModel> currentQuestions = new List<QuestionModel>();

            string triviaUrl = Constants.URL_TEN_MIXED;

            string rawJSON = "";
            //fetch raw json data
            try
            {
                HttpClient netClient = new HttpClient();
                rawJSON = netClient.GetStringAsync(triviaUrl).Result;
            }
            catch (AggregateException e)
            {
                jsonErrorHandler(e, Constants.ERROR_FETCHING_QUESTIONS);
            }

            //build list of questionModels
            try
            {
                //get the base JSON object
                JObject baseObject = JObject.Parse(rawJSON);
                List<JObject> JSONArray = new List<JObject>();
                //pull results
                JSONArray = baseObject["results"].Select(o => (JObject)o).ToList();
                foreach (JObject j in JSONArray)
                {
                    //build our question model
                    QuestionModel currentQ = new QuestionModel();
                    currentQ.Category = (String)j["category"];
                    currentQ.Type = (String)j["type"];
                    currentQ.Question = (String)j["question"];
                    currentQ.CorrectAnswer = (String)j["correct_answer"];

                    //now build list of incorrect answers
                    currentQ.IncorrectAnswers = new List<string>();
                    //get the incorrect answer array
                    List<JToken> wrongArray = j["incorrect_answers"].ToList();
                    foreach (JToken s in wrongArray)
                    {
                        currentQ.IncorrectAnswers.Add((String)s);
                    }

                    //now add it to the lsitof questions
                    currentQuestions.Add(currentQ);
                }
            }
            //handle exceptions
            catch (JsonReaderException e)
            {
                Console.WriteLine(e);
                jsonErrorHandler(e, Constants.ERROR_PARSING_JSON);
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine(e);
                jsonErrorHandler(e, Constants.ERROR_PARSING_JSON);
            }

            return currentQuestions;
        }


        //quick handler method for dealing with errors getting weather from JSON
        private ActionResult jsonErrorHandler(Exception e, String errorMessage)
        {
            //output the error somewhere
            Console.WriteLine(e);
            //go to error page
            ViewModelResult JSONFail = new ViewModelResult();
            JSONFail.ResultType = Constants.ERROR_TYPE_JSON;
            JSONFail.ResultDescription = errorMessage;
            return View(Constants.VIEW_RESULT, JSONFail);
        }


        //runs a quiz
        [HttpGet]
        public ActionResult Quiz()
        {
            //if no one's logged in go to user page
            if (Session[Constants.KEY_CURRENTLY_LOGGED_USER] == null)
                return View(Constants.VIEW_USER_LOGIN_EXTERNAL); //user page should be in "Shared" but could not get it to work

            //if we previously completed a quiz start a new set of quiz session variables
            if (Session[Constants.KEY_PREVIOUSLY_COMPLETED_QUIZ] != null)
            {
                if ((bool)Session[Constants.KEY_PREVIOUSLY_COMPLETED_QUIZ])
                    wipeQuizSessionVariables();
            }
                
            initQuizSessionVariables();

            //get new question index and reference to list of question
            int currentQIndex = (int)Session[Constants.KEY_CURRENT_QUESTION_INDEX];
            List<QuestionModel> currentQuestions = (List<QuestionModel>)Session[Constants.KEY_LIST_OF_QUESTIONS];

            //if total number of questions isn't set, set it now that we have questions
            if (Session[Constants.KEY_NTOTAL_QUESTIONS] == null)
            {
                int nQs = currentQuestions.Count();
                Session[Constants.KEY_NTOTAL_QUESTIONS] = nQs;
            }

            //if the last question has been answered
            if (currentQIndex >= (int)Session[Constants.KEY_NTOTAL_QUESTIONS])
            {
                Session[Constants.KEY_PREVIOUSLY_COMPLETED_QUIZ] = true;
                //submit results to database 
                if (!SaveCurrentScore())
                {
                    //handle submission failure
                    ViewModelResult failReg = new ViewModelResult(Constants.ERROR_TYPE_DATABASE, Constants.ERROR_SAVING_SCORE);
                    return View(Constants.VIEW_RESULT, failReg);
                }
                //go to final results page
                ViewModelQuestionResult finalResult = new ViewModelQuestionResult();
                finalResult.nQuestionsCorrect = (int)Session[Constants.KEY_NQUESTIONS_CORRECT];
                finalResult.nQuestionsTotal = (int)Session[Constants.KEY_NTOTAL_QUESTIONS];
                return View(Constants.VIEW_FINAL_RESULTS, finalResult);
            }

            //pull question and generate viewmodel
            QuestionModel currentQuestion = currentQuestions.ElementAt(currentQIndex);
            ViewModelQuestion currentQuestionViewModel = buildQuestionChoiceViewModel(currentQuestion);
            currentQuestionViewModel.currentQuestion = currentQIndex + 1;
            currentQuestionViewModel.nQuestionsTotal = (int)Session[Constants.KEY_NTOTAL_QUESTIONS];
        
            return View(currentQuestionViewModel);
        }


        //initialises quiz session variables
        private void initQuizSessionVariables()
        {
            //if quiz hasn't been started or finished 
            if (Session[Constants.KEY_PREVIOUSLY_COMPLETED_QUIZ] == null)
                Session[Constants.KEY_PREVIOUSLY_COMPLETED_QUIZ] = false;

            //if there are no questions yet, pull some
            if (Session[Constants.KEY_LIST_OF_QUESTIONS] == null)
                Session[Constants.KEY_LIST_OF_QUESTIONS] = fetchQuestionsFromJSON();

            //if starting index isn't, then make it
            if (Session[Constants.KEY_CURRENT_QUESTION_INDEX] == null)
                Session[Constants.KEY_CURRENT_QUESTION_INDEX] = 0;

            //if no questions have been answered set correct count
            if (Session[Constants.KEY_NQUESTIONS_CORRECT] == null)
                Session[Constants.KEY_NQUESTIONS_CORRECT] = 0;
        }
        

        //forces a new set of quiz variables
        private void wipeQuizSessionVariables()
        {
            //null current quiz variables
            Session[Constants.KEY_PREVIOUSLY_COMPLETED_QUIZ] = null;
            Session[Constants.KEY_NTOTAL_QUESTIONS] = null;
            Session[Constants.KEY_LIST_OF_QUESTIONS] = null;
            Session[Constants.KEY_CURRENT_QUESTION_INDEX] = null;
            Session[Constants.KEY_NQUESTIONS_CORRECT] = null;
            Session[Constants.KEY_USER_ANSWER] = null;
        }


        //build viewmodel ready for displaying new question
        private ViewModelQuestion buildQuestionChoiceViewModel (QuestionModel currentQuestion)
        {
            ViewModelQuestion currentQuestionViewModel = new ViewModelQuestion();

            string correctAnswer = currentQuestion.CorrectAnswer;
            currentQuestionViewModel.Question = currentQuestion.Question;
            //add the incorrect answers
            int nInCorrects = currentQuestion.IncorrectAnswers.Count();        
            //randomize the order all answers are added - otherwise refreshing quiz page gives away answer!    
            for (int i = 0; i < nInCorrects; i++)
            {
                //randomly pick the first one
                int currentPickIndex = rand.Next(nInCorrects);
                string currentPick = currentQuestion.IncorrectAnswers.ElementAt(currentPickIndex);             
                //randomly pick the rest as lnog as they're not repeating
                while (currentQuestionViewModel.Choices.Contains(currentPick))
                {
                    currentPickIndex = rand.Next(nInCorrects);
                    currentPick = currentQuestion.IncorrectAnswers.ElementAt(currentPickIndex);
                }
                currentQuestionViewModel.Choices.Add(currentPick);
            }

            //randomly insert correct answer
            int nChoices = currentQuestionViewModel.Choices.Count();
            int insertLocation = rand.Next(nChoices + 1);
            currentQuestionViewModel.Choices.Insert(insertLocation, correctAnswer);
            //record the index of the correct answer
            Session[Constants.KEY_CORRECT_ANSWER_INDEX] = currentQuestionViewModel.Choices.IndexOf(correctAnswer);

            return currentQuestionViewModel;
        }


        [HttpPost]
        public ActionResult Quiz(int userAnswer)
        {
            Session[Constants.KEY_USER_ANSWER] = userAnswer;
            //increment index for next question
            int currentQIndex = (int)Session[Constants.KEY_CURRENT_QUESTION_INDEX];
            currentQIndex++;
            Session[Constants.KEY_CURRENT_QUESTION_INDEX] = currentQIndex;
            //redirect to another method - stops user re-POSTing answer index
            RedirectResult safeRedir = new RedirectResult(Constants.VIEW_QUESTION_RESULT);
            return safeRedir;
        }


        //redirect method to avoid re-sending post data to Quiz
        public ActionResult QuestionResult()
        {
            //pull answer
            int userAnswer = (int)Session[Constants.KEY_USER_ANSWER];
            //build viewmodel
            ViewModelQuestionResult qResult = buildQuestionResultViewModel(userAnswer);          
            //set the userAnswer to a non-value so isn't checked if page refreshed
            userAnswer = Constants.NON_RESULT;
            Session[Constants.KEY_USER_ANSWER] = userAnswer;
            //send to result page
            return View(Constants.VIEW_QUESTION_RESULT, qResult);
        }


        //builds the viewmodel for result on a single question
        private ViewModelQuestionResult buildQuestionResultViewModel(int userAnswer)
        {
            //build viewmodel
            ViewModelQuestionResult qResult = new ViewModelQuestionResult();
            qResult.ResultType = Constants.ANSWER_ACCURACY;
            qResult.nQuestionsTotal = (int)Session[Constants.KEY_NTOTAL_QUESTIONS];
            int nQuestionsCorrect = (int)Session[Constants.KEY_NQUESTIONS_CORRECT];
            //check that userAnswer has been updated, so isn't just a page refresh       
            if (userAnswer != Constants.NON_RESULT)
            {
                //check if answer from page is correct
                if (userAnswer == (int)Session[Constants.KEY_CORRECT_ANSWER_INDEX])
                {
                    //set correct values
                    qResult.ResultDescription = Constants.CORRECT;
                    nQuestionsCorrect++;
                    Session[Constants.KEY_NQUESTIONS_CORRECT] = nQuestionsCorrect;
                }
                else
                    qResult.ResultDescription = Constants.WRONG;
            }

            //send user to question results page with right messages
            qResult.nQuestionsCorrect = nQuestionsCorrect;
            //set the userAnswer to a non-value so isn't checked if page refreshed

            return qResult;
        }
    }
}