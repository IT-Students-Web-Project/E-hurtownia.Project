using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using E_hurtownia.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;

namespace E_hurtownia.Controllers {
    public class UserController : Controller {
        private EhurtowniaContext databaseContext = new EhurtowniaContext();

        public IActionResult Index() { // ACTION - MAIN PAGE
            ViewBag.COOKIE_LOGGED_USERNAME = Request.Cookies["COOKIE_LOGGED_USERNAME"];

            if (ViewBag.COOKIE_LOGGED_USERNAME != null) {
                Users currentUser = databaseContext.Users.Where(user => user.Login == Request.Cookies["COOKIE_LOGGED_USERNAME"]).First();
                ViewBag.UserGroup = currentUser.FkGroup;
            }

            if (TempData["PasswordChanged"] != null) {
                ViewBag.PasswordChanged = true;
            }

            return View();
        }

        public IActionResult Error() { // ACTION - GLOBAL ERROR PAGE
            ViewBag.COOKIE_LOGGED_USERNAME = Request.Cookies["COOKIE_LOGGED_USERNAME"];

            // Set all values below before running this action
            // TempData["ErrorHeader"] = "Error header";
            // TempData["ErrorMessage"] = "Error message";

            string errorHeader = (string) TempData["ErrorHeader"];
            string errorMessage = (string) TempData["ErrorMessage"];

            ViewBag.ErrorHeader = errorHeader;
            ViewBag.ErrorMessage = errorMessage;

            TempData["ErrorHeader"] = errorHeader;
            TempData["ErrorMessage"] = errorMessage;

            return View();
        }

        public IActionResult Login() { // ACTION - LOGIN/REGISTER PAGE
            string lastLoginResult = (string) TempData["login-result"];
            string lastRegisterResult = (string) TempData["register-result"];

            if (lastLoginResult == "user-not-found") {
                ViewBag.UserNotFound = true;
            } else if (lastLoginResult == "password-incorrect") {
                ViewBag.PasswordIncorrect = true;
                ViewBag.LastUserName = (string) TempData["login-name"];
            }

            if (lastRegisterResult == "invalid-username") {
                ViewBag.InvalidUsername = true;
            } else if (lastRegisterResult == "invalid-password") {
                ViewBag.InvalidPassword = true;
                ViewBag.LastUserName = (string) TempData["register-name"];
            } else if (lastRegisterResult == "user-exists") {
                ViewBag.UserExists = true;
            }

            return View();
        }

        public IActionResult AccountDelete() { // ACTION - ACCOUNT DELETION PROCEDURE
            if (Request.Cookies["COOKIE_LOGGED_USERNAME"] != null) {
                string usernameToDelete = Request.Cookies["COOKIE_LOGGED_USERNAME"];
                List<Users> accountsToDelete = databaseContext.Users.Where(user => user.Login == usernameToDelete).ToList();

                if (accountsToDelete.Count() == 1) {
                    Users selectedAccount = accountsToDelete.First();

                    databaseContext.Users.Remove(selectedAccount);
                    databaseContext.SaveChanges();

                    Response.Cookies.Delete("COOKIE_LOGGED_USERNAME");
                    return RedirectToAction("Index", "User");
                } else {
                    TempData["ErrorHeader"] = "Multiple accounts";
                    TempData["ErrorMessage"] = "Encountered multiple user accounts with the same username, please fix this error manually in database";

                    return RedirectToAction("Error", "User");
                }
            } else {
                TempData["ErrorHeader"] = "Session expired";
                TempData["ErrorMessage"] = "User session has expired, please try signing in again";

                return RedirectToAction("Error", "User");
            }
        }

        public IActionResult Logout() { // ACTION - LOGOUT PROCEDURE
            Response.Cookies.Delete("COOKIE_LOGGED_USERNAME");

            return RedirectToAction("Index", "User");
        }

        public IActionResult CheckLogin() { // ACTION - LOGIN CHECK PROCEDURE
            string userName = "";
            string userPass = "";

            if (Request.HasFormContentType == true) {
                userName = Request.Form["login-username"];
                userPass = Request.Form["login-password"];
            } else {
                TempData["ErrorHeader"] = "Data transfer error";
                TempData["ErrorMessage"] = "Did not received any data, form is empty";

                return RedirectToAction("Error", "User");
            }

            bool userFound = false;
            bool passCorrect = false;

            if (databaseContext.Users.Count() > 0) {
                List<Users> foundUsers = databaseContext.Users.Where(user => user.Login == userName).ToList();

                if (foundUsers.Count() > 0) {
                    userFound = true;

                    if (userPass == foundUsers.First().Password) {
                        passCorrect = true;
                    }
                }
            }

            if (userFound == false) {
                TempData["login-result"] = "user-not-found";

                return RedirectToAction("Login", "User");
            } else if (passCorrect == false) {
                TempData["login-result"] = "password-incorrect";
                TempData["login-name"] = userName;

                return RedirectToAction("Login", "User");
            } else {
                Response.Cookies.Append("COOKIE_LOGGED_USERNAME", userName); // User logged in correctly, now its 'session' is enabled

                return RedirectToAction("Index", "User");
            }
        }

        public IActionResult CheckRegister() { // ACTION - REGISTER CHECK INITIAL PROCEDURE
            string newUsername = "";
            string newPassword = "";

            if (Request.HasFormContentType == true) {
                newUsername = Request.Form["login-username"];
                newPassword = Request.Form["login-password"];
            } else {
                TempData["ErrorHeader"] = "Data transfer error";
                TempData["ErrorMessage"] = "Did not received any data, form is empty";

                return RedirectToAction("Error", "User");
            }

            bool correctUsername = false;
            bool correctPassword = false;
            bool userExists = false;

            if (databaseContext.Users.Count() > 0) {
                List<Users> foundUsers = databaseContext.Users.Where(user => user.Login == newUsername).ToList();

                if (foundUsers.Count() > 0) {
                    userExists = true;
                }
            }

            if (newUsername.Length <= 30 && newUsername.Length >= 5) {
                correctUsername = true;
            }

            if (newPassword.Length <= 30 && newPassword.Length >= 5) {
                correctPassword = true;
            }

            if (correctUsername == false) {
                TempData["register-result"] = "invalid-username";

                return RedirectToAction("Login", "User");
            } else if (userExists == true) {
                TempData["register-result"] = "user-exists";

                return RedirectToAction("Login", "User");
            } else if (correctPassword == false) {
                TempData["register-result"] = "invalid-password";
                TempData["register-name"] = newUsername;

                return RedirectToAction("Login", "User");
            } else {
                int userId = 1;
                int userGroup = 2; // Default group is 2 = CUSTOMER
                string userLogin = newUsername;
                string userPassword = newPassword;
                bool userStatus = true;

                if (databaseContext.Users.Count() > 0) {
                    List<Users> allUsers = databaseContext.Users.OrderBy(user => user.IdUser).ToList();
                    userId = allUsers.Last().IdUser + 1;
                }

                databaseContext.Users.Add(new Users() {
                    IdUser = userId,
                    FkGroup = userGroup,
                    Login = userLogin,
                    Password = userPassword,
                    Status = userStatus
                });

                int customerId = 1;
                bool customerActive = false; // Customer status is 'false' by default - cannot buy products - until complete personal data, then the status will be 'true'

                if (databaseContext.Customers.Count() > 0) {
                    List<Customers> allCustomers = databaseContext.Customers.OrderBy(customer => customer.IdCustomer).ToList();
                    customerId = allCustomers.Last().IdCustomer + 1;
                }

                databaseContext.Customers.Add(new Customers() {
                    IdCustomer = customerId,
                    FkPerson = null,
                    FkCompany = null,
                    FkUser = userId, // Referencing new customer account to this (newly created) user account
                    Status = customerActive
                });

                databaseContext.SaveChanges();
                Response.Cookies.Append("COOKIE_LOGGED_USERNAME", newUsername); // User registered properly, automatically logged in

                return RedirectToAction("Index", "User");
            }
        }

        public IActionResult ChangePassword() { // ACTION - CHANGE PASSWORD
            ViewBag.COOKIE_LOGGED_USERNAME = Request.Cookies["COOKIE_LOGGED_USERNAME"];

            if (TempData["CurrentPasswordIncorrect"] != null) {
                ViewBag.CurrentPasswordIncorrect = true;
            } else if (TempData["NewPasswordInvalid"] != null) {
                ViewBag.NewPasswordInvalid = true;
            } else if (TempData["RepeatPasswordDoNotMatch"] != null) {
                ViewBag.RepeatPasswordDoNotMatch = true;
            }

            return View();
        }

        public IActionResult ChangePasswordConfirm() { // ACTION - CHANGE PASSWORD CHECK & CONFIRM PROCEDURE
            string oldPassword = "";
            string newPassword = "";
            string repeatPassword = "";

            if (Request.HasFormContentType == true) {
                oldPassword = Request.Form["password-old"];
                newPassword = Request.Form["password-new"];
                repeatPassword = Request.Form["password-new-repeat"];
            } else {
                TempData["ErrorHeader"] = "Data transfer error";
                TempData["ErrorMessage"] = "Did not received any data, form is empty";

                return RedirectToAction("Error", "User");
            }

            Users currentUser = databaseContext.Users.Where(user => user.Login == Request.Cookies["COOKIE_LOGGED_USERNAME"]).First();

            if (currentUser.Password != oldPassword) {
                TempData["CurrentPasswordIncorrect"] = true;

                return RedirectToAction("ChangePassword", "User");
            } else if (newPassword.Length < 5 || newPassword.Length > 30) {
                TempData["NewPasswordInvalid"] = true;

                return RedirectToAction("ChangePassword", "User");
            } else if (newPassword != repeatPassword) {
                TempData["RepeatPasswordDoNotMatch"] = true;

                return RedirectToAction("ChangePassword", "User");
            } else {
                databaseContext.Users.Remove(currentUser);
                databaseContext.SaveChanges();

                currentUser.Password = newPassword;
                databaseContext.Users.Add(currentUser);
                databaseContext.SaveChanges();

                TempData["PasswordChanged"] = true;
                return RedirectToAction("Index", "User");
            }
        }
    }
}