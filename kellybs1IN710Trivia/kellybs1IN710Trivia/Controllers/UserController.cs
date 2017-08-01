using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using kellybs1IN710Trivia.Models;
using System.Data.SqlClient;

//UserController class
//Author: Brendan Kelly
//Date: 14 June 2017
//Description: Controller for managing user accounts

namespace kellybs1IN710Trivia.Controllers
{
    public class UserController : Controller
    {
        private TriviaDbDataContext triviaDbUser;

        public UserController()
        {
            triviaDbUser = new TriviaDbDataContext();
        }


        //base login page
        [HttpGet]
        public ActionResult UserPage()
        {
            //if someone's already logged in take them straight to the OK login page
            if (Session[Constants.KEY_CURRENTLY_LOGGED_USER] != null)
                return View(Constants.VIEW_LOGGED_IN, Session[Constants.KEY_CURRENTLY_LOGGED_USER]);
            else
                return View();
        }


        //managers user log in 
        [HttpPost]
        public ActionResult UserPage(tblPlayer userFromPage)
        {
            //if fields are blank, error
            if (userFromPage.PlayerPassword == null || userFromPage.PlayerName == null)
            {
                ViewModelResult failPasswords = new ViewModelResult(Constants.ERROR_TYPE_REGISTER, Constants.ERROR_BLANK_FIELDS);
                return View(Constants.VIEW_RESULT, failPasswords);
            }

            try
            {
                //try all pull users from database
                var pulledUser = from u in triviaDbUser.tblPlayers
                                 where u.PlayerName.Equals(userFromPage.PlayerName)
                                 select new { u.PlayerID, u.PlayerName, u.PlayerPassword };

                //if no user with that name exists handle the error
                if (pulledUser.Count() == 0)
                {
                    ViewModelResult failUser = new ViewModelResult(Constants.ERROR_TYPE_LOGIN, Constants.ERROR_USER_NOT_EXIST);
                    return View(Constants.VIEW_RESULT, failUser);
                }

                else
                {
                    //else make a player out of the pulled user - names should be unique so it should only be one element long!
                    tblPlayer currentUser = new tblPlayer();
                    foreach (var user in pulledUser)
                    {
                        currentUser.PlayerID = user.PlayerID;
                        currentUser.PlayerName = user.PlayerName;
                        currentUser.PlayerPassword = user.PlayerPassword;
                    }

                    //is the password is correct
                    if (userFromPage.PlayerPassword == currentUser.PlayerPassword)
                    {
                        //then set the log in user for the session
                        LoggedInUser userLogged = new LoggedInUser(currentUser.PlayerID, currentUser.PlayerName);
                        Session[Constants.KEY_CURRENTLY_LOGGED_USER] = userLogged;
                        return View(Constants.VIEW_LOGGED_IN, userLogged);

                    }
                    else
                    {
                        //authentication error
                        ViewModelResult failUser = new ViewModelResult(Constants.ERROR_TYPE_LOGIN, Constants.ERROR_AUTH);
                        return View(Constants.VIEW_RESULT, failUser);
                    }
                }

            }
            //database exception error handling
            catch (SqlException e)
            {
                Console.WriteLine(e);
                ViewModelResult dbFail = new ViewModelResult(Constants.ERROR_TYPE_DATABASE, Constants.ERROR_LOADING_DB);
                return View(Constants.VIEW_RESULT, dbFail);
            }
        }

        //base registration page
        [HttpGet]
        public ActionResult RegisterUser()
        {
            return View();
        }


        //manages registration attempt
        [HttpPost]
        public ActionResult RegisterUser(RegisteringUser userFromPage)
        {
            //if fields are blank, error
            if (userFromPage.PlayerPassword1 == null ||
                userFromPage.PlayerPassword2 == null ||
                userFromPage.PlayerName == null)
            {
                ViewModelResult failPasswords = new ViewModelResult(Constants.ERROR_TYPE_REGISTER, Constants.ERROR_BLANK_FIELDS);
                return View(Constants.VIEW_RESULT, failPasswords);
            }

            //if the given passwords don't match, error
            if (!userFromPage.PlayerPassword1.Equals(userFromPage.PlayerPassword2))
            {
                ViewModelResult failPasswords = new ViewModelResult(Constants.ERROR_TYPE_REGISTER, Constants.ERROR_PASSWORD_MATCH);
                return View(Constants.VIEW_RESULT, failPasswords);
            }
            else
            {
                //passwords acceptable, check if player exists
                bool playerExists = false;
                try
                {
                    var allPlayers = from p in triviaDbUser.tblPlayers
                                     select new { p.PlayerName };
                    foreach (var player in allPlayers)
                    {
                        //if there's a matching player then it already exists - ignore casing
                        string lowerName = userFromPage.PlayerName.ToLower();
                        if (lowerName.Equals(player.PlayerName.ToLower()))
                        {
                            playerExists = true;
                            break;
                        }
                    }
                }
                catch (SqlException e)
                {
                    Console.WriteLine(e);
                    ViewModelResult dbFail = new ViewModelResult(Constants.ERROR_TYPE_DATABASE, Constants.ERROR_LOADING_DB);
                    return View(Constants.VIEW_RESULT, dbFail);
                }
                //if players exists can't create it again
                if (playerExists)
                {
                    ViewModelResult failReg = new ViewModelResult(Constants.ERROR_TYPE_REGISTER, Constants.ERROR_USER_EXISTS);
                    return View(Constants.VIEW_RESULT, failReg);
                }
                else
                //attempt to add the user to the db then provide feeedback
                {
                    if (commitNewUser(userFromPage))
                    {
                        ViewModelResult regOK = new ViewModelResult(Constants.TYPE_REGISTRATION_OK, Constants.MSG_REGISTRATION_OK);
                        return View(Constants.VIEW_RESULT, regOK);
                    }
                    else
                    {
                        ViewModelResult failReg = new ViewModelResult(Constants.ERROR_TYPE_REGISTER, Constants.ERROR_GENERIC_REGISTER);
                        return View(Constants.VIEW_RESULT, failReg);
                    }

                }
            }
        }


        //attempts to save user and provides boolean feedback on success
        private bool commitNewUser(RegisteringUser userFromPage)
        {
            bool success = false;
            //build new user
            tblPlayer newUser = new tblPlayer();
            newUser.PlayerName = userFromPage.PlayerName;
            newUser.PlayerPassword = userFromPage.PlayerPassword1;

            triviaDbUser.tblPlayers.InsertOnSubmit(newUser);
            // Submit the new user to the database
            try
            {
                triviaDbUser.SubmitChanges();
                success = true;
            }
            catch (Exception e)
            {
                success = false;
                Console.WriteLine(e);
            }

            return success;
        }


        //logs out by resetting session
        public ActionResult Logout()
        {
            //kill the session and reload login page
            Session.Abandon();
            return View(Constants.VIEW_USER_LOGIN_PAGE);
        }

    }
}