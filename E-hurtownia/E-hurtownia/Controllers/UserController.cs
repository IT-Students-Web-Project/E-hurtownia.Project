using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using E_hurtownia.Models;

namespace E_hurtownia.Controllers {
    public class UserController : Controller {
        private readonly EhurtowniaContext databaseContext = new EhurtowniaContext();

        public IActionResult Index() { // ACTION - MAIN PAGE
            ViewBag.COOKIE_LOGGED_USERNAME = Request.Cookies["COOKIE_LOGGED_USERNAME"];
            ViewBag.Products = databaseContext.Products.ToList();
            ViewBag.Stocks = databaseContext.Stocks.ToList();
            ViewBag.Units = databaseContext.Units.ToList();

            if (ViewBag.COOKIE_LOGGED_USERNAME != null) {
                Users currentUser = databaseContext.Users.Where(user => user.Login == Request.Cookies["COOKIE_LOGGED_USERNAME"]).First();
                ViewBag.UserGroup = currentUser.FkGroup;
                
                if (databaseContext.Customers.Where(customer => customer.FkUser == currentUser.IdUser).Count() > 0) {
                    ViewBag.MeCustomer = databaseContext.Customers.Where(customer => customer.FkUser == currentUser.IdUser).Single();
                }
            }

            if (TempData["PasswordChanged"] != null) {
                ViewBag.PasswordChanged = true;
            }

            if (TempData["AddressChanged"] != null) {
                ViewBag.AddressChanged = true;
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
                Users deletedUser = databaseContext.Users.Where(user => user.Login == usernameToDelete).Single();
                List<Storekeepers> deletedStorekeepers = databaseContext.Storekeepers.Where(storekeeper => storekeeper.FkUser == deletedUser.IdUser).ToList();

                if (databaseContext.Customers.Where(customer => customer.FkUser == deletedUser.IdUser).Count() > 0) {
                    Customers deletedCustomer = databaseContext.Customers.Where(customer => customer.FkUser == deletedUser.IdUser).Single();
                    Persons deletedPerson = databaseContext.Persons.Where(person => person.IdPerson == deletedCustomer.FkPerson).Single();
                    Addresses deletedAddress = databaseContext.Addresses.Where(address => address.IdAddress == deletedPerson.FkAddress).Single();

                    databaseContext.Customers.Remove(deletedCustomer);
                    databaseContext.Persons.Remove(deletedPerson);
                    databaseContext.Addresses.Remove(deletedAddress);
                }

                if (deletedStorekeepers.Count() > 0) {
                    databaseContext.Storekeepers.RemoveRange(deletedStorekeepers);
                }

                databaseContext.Users.Remove(deletedUser);
                databaseContext.SaveChanges();

                Response.Cookies.Delete("COOKIE_LOGGED_USERNAME");
                return RedirectToAction("Index", "User");
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
                TempData["register-name"] = newUsername;
                TempData["register-password"] = newPassword;

                return RedirectToAction("RegisterPerson", "User");
            }
        }

        public IActionResult RegisterPerson() { // ACTION - REGISTER PERSONAL FORM
            if (TempData["AddressResult_InvalidStreet"] != null) ViewBag.AddressResult_InvalidStreet = true;
            if (TempData["AddressResult_InvalidBuildingNum"] != null) ViewBag.AddressResult_InvalidBuildingNum = true;
            if (TempData["AddressResult_InvalidCity"] != null) ViewBag.AddressResult_InvalidCity = true;
            if (TempData["AddressResult_InvalidPostalCode"] != null) ViewBag.AddressResult_InvalidPostalCode = true;
            if (TempData["AddressResult_InvalidCountry"] != null) ViewBag.AddressResult_InvalidCountry = true;

            if (TempData["PersonResult_InvalidFirstname"] != null) ViewBag.PersonResult_InvalidFirstname = true;
            if (TempData["PersonResult_InvalidLastname"] != null) ViewBag.PersonResult_InvalidLastname = true;

            return View();
        }

        public IActionResult RegisterPersonCheck() { // ACTION - REGISTER PERSONAL DATA CHECKING PROCEDURE
            string newUsername = (string) TempData["register-name"];
            string newPassword = (string) TempData["register-password"];

            string addressStreet = "";
            int addressBuildingNum = -1;

            #nullable enable
            int? addressApartmentNum = null;
            #nullable disable

            string addressCity = "";
            string addressPostalCode = "";
            string addressCountry = "";
            string personalFirstname = "";
            string personalLastname = "";
            string personalSex = "";

            if (Request.HasFormContentType == true) {
                addressStreet = Request.Form["address-street"];
                addressCity = Request.Form["address-city"];
                addressPostalCode = Request.Form["address-postal"];
                addressCountry = Request.Form["address-country"];

                if (Request.Form["address-bnum"].ToString() != String.Empty) {
                    addressBuildingNum = Int32.Parse(Request.Form["address-bnum"]);
                }

                if (Request.Form["address-anum"].ToString() != String.Empty) {
                    addressApartmentNum = Int32.Parse(Request.Form["address-anum"]);
                }

                personalFirstname = Request.Form["person-firstname"];
                personalLastname = Request.Form["person-lastname"];
                personalSex = Request.Form["person-sex"];
            } else {
                TempData["ErrorHeader"] = "Data transfer error";
                TempData["ErrorMessage"] = "Did not received any data, form is empty";

                return RedirectToAction("Error", "User");
            }

            bool invalidStreet = (addressStreet.Length < 5 || addressStreet.Length > 60);
            bool invalidBuildingNum = (addressBuildingNum < 0);
            bool invalidCity = (addressCity.Length < 5 || addressCity.Length > 30);
            bool invalidPostalCode = (addressPostalCode.Length < 5 || addressPostalCode.Length > 30);
            bool invalidCountry = (addressCountry.Length < 3 || addressCountry.Length > 30);
            bool invalidFirstname = (personalFirstname.Length < 3 || personalFirstname.Length > 60);
            bool invalidLastname = (personalLastname.Length < 5 || personalLastname.Length > 60);

            if (invalidStreet == true || invalidBuildingNum == true || invalidCity == true || invalidPostalCode == true || invalidCountry == true || invalidFirstname == true || invalidLastname == true) {
                if (invalidStreet) TempData["AddressResult_InvalidStreet"] = true;
                if (invalidBuildingNum) TempData["AddressResult_InvalidBuildingNum"] = true;
                if (invalidCity) TempData["AddressResult_InvalidCity"] = true;
                if (invalidPostalCode) TempData["AddressResult_InvalidPostalCode"] = true;
                if (invalidCountry) TempData["AddressResult_InvalidCountry"] = true;

                if (invalidFirstname) TempData["PersonResult_InvalidFirstname"] = true;
                if (invalidLastname) TempData["PersonResult_InvalidLastname"] = true;

                return RedirectToAction("RegisterPerson", "User");
            } else {
                // STAGE 1 - CREATING ADDRESS
                Addresses customerAddress = new Addresses {
                    IdAddress = (databaseContext.Addresses.Count() > 0) ? databaseContext.Addresses.OrderBy(address => address.IdAddress).Last().IdAddress + 1 : 1,
                    Street = addressStreet,
                    BuildingNum = addressBuildingNum,
                    ApartmentNum = addressApartmentNum,
                    City = addressCity,
                    PostalCode = addressPostalCode,
                    Country = addressCountry,
                    Status = true
                };

                // STAGE 2 - CREATING PERSON
                Persons customerPerson = new Persons {
                    IdPerson = (databaseContext.Persons.Count() > 0) ? databaseContext.Persons.OrderBy(person => person.IdPerson).Last().IdPerson + 1 : 1,
                    Firstname = personalFirstname,
                    Lastname = personalLastname,
                    Sex = personalSex,
                    FkAddress = customerAddress.IdAddress
                };

                // STAGE 3 - CREATING USER
                Users customerUser = new Users {
                    IdUser = (databaseContext.Users.Count() > 0) ? databaseContext.Users.OrderBy(user => user.IdUser).Last().IdUser + 1 : 1,
                    FkGroup = 2, // By default, the target group is CUSTOMERS
                    Login = newUsername,
                    Password = newPassword,
                    Status = true
                };

                // STAGE 4 - CREATING CUSTOMER
                Customers customerObject = new Customers {
                    IdCustomer = (databaseContext.Customers.Count() > 0) ? databaseContext.Customers.OrderBy(customer => customer.IdCustomer).Last().IdCustomer + 1 : 1,
                    FkPerson = customerPerson.IdPerson,
                    FkCompany = null,
                    FkUser = customerUser.IdUser,
                    Status = true
                };

                // Adding to database
                databaseContext.Addresses.Add(customerAddress);
                databaseContext.Persons.Add(customerPerson);
                databaseContext.Users.Add(customerUser);
                databaseContext.Customers.Add(customerObject);
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

            Users currentUser = databaseContext.Users.Where(user => user.Login == Request.Cookies["COOKIE_LOGGED_USERNAME"]).Single();

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
                currentUser.Password = newPassword;
                databaseContext.Users.Update(currentUser);
                databaseContext.SaveChanges();

                TempData["PasswordChanged"] = true;
                return RedirectToAction("Index", "User");
            }
        }

        public IActionResult ChangeAddress() {
            ViewBag.COOKIE_LOGGED_USERNAME = Request.Cookies["COOKIE_LOGGED_USERNAME"];

            if (databaseContext.Users.Count() > 0) {
                Users meUser = databaseContext.Users.Where(user => user.Login == Request.Cookies["COOKIE_LOGGED_USERNAME"]).Single();
                Customers meCustomer = databaseContext.Customers.Where(customer => customer.FkUser == meUser.IdUser).Single();
                Persons mePerson = databaseContext.Persons.Where(person => person.IdPerson == meCustomer.FkPerson).Single();
                Addresses myAddress = databaseContext.Addresses.Where(address => address.IdAddress == mePerson.FkAddress).Single();

                TempData["UpdatedAddressID"] = myAddress.IdAddress;
                ViewBag.Address = myAddress;
            }

            if (TempData["AddressResult_InvalidStreet"] != null) ViewBag.AddressResult_InvalidStreet = true;
            if (TempData["AddressResult_InvalidBuildingNum"] != null) ViewBag.AddressResult_InvalidBuildingNum = true;
            if (TempData["AddressResult_InvalidCity"] != null) ViewBag.AddressResult_InvalidCity = true;
            if (TempData["AddressResult_InvalidPostalCode"] != null) ViewBag.AddressResult_InvalidPostalCode = true;
            if (TempData["AddressResult_InvalidCountry"] != null) ViewBag.AddressResult_InvalidCountry = true;

            return View();
        }

        public IActionResult ChangeAddressCheck() {
            string addressStreet = "";
            int addressBuildingNum = -1;

            #nullable enable
            int? addressApartmentNum = null;
            #nullable disable

            string addressCity = "";
            string addressPostalCode = "";
            string addressCountry = "";

            if (Request.HasFormContentType == true) {
                addressStreet = Request.Form["address-street"];
                addressCity = Request.Form["address-city"];
                addressPostalCode = Request.Form["address-postal"];
                addressCountry = Request.Form["address-country"];

                if (Request.Form["address-bnum"].ToString() != String.Empty) {
                    addressBuildingNum = Int32.Parse(Request.Form["address-bnum"]);
                }

                if (Request.Form["address-anum"].ToString() != String.Empty) {
                    addressApartmentNum = Int32.Parse(Request.Form["address-anum"]);
                }
            } else {
                TempData["ErrorHeader"] = "Data transfer error";
                TempData["ErrorMessage"] = "Did not received any data, form is empty";

                return RedirectToAction("Error", "User");
            }

            bool invalidStreet = (addressStreet.Length < 5 || addressStreet.Length > 60);
            bool invalidBuildingNum = (addressBuildingNum < 0);
            bool invalidCity = (addressCity.Length < 5 || addressCity.Length > 30);
            bool invalidPostalCode = (addressPostalCode.Length < 5 || addressPostalCode.Length > 30);
            bool invalidCountry = (addressCountry.Length < 3 || addressCountry.Length > 30);

            if (invalidStreet == true || invalidBuildingNum == true || invalidCity == true || invalidPostalCode == true || invalidCountry == true) {
                if (invalidStreet) TempData["AddressResult_InvalidStreet"] = true;
                if (invalidBuildingNum) TempData["AddressResult_InvalidBuildingNum"] = true;
                if (invalidCity) TempData["AddressResult_InvalidCity"] = true;
                if (invalidPostalCode) TempData["AddressResult_InvalidPostalCode"] = true;
                if (invalidCountry) TempData["AddressResult_InvalidCountry"] = true;

                return RedirectToAction("ChangeAddress", "User");
            } else {
                int updatedAddressID = (int) TempData["UpdatedAddressID"];
                Addresses updatedAddress = databaseContext.Addresses.Where(address => address.IdAddress == updatedAddressID).Single();

                updatedAddress.Street = addressStreet;
                updatedAddress.BuildingNum = addressBuildingNum;
                updatedAddress.ApartmentNum = addressApartmentNum;
                updatedAddress.City = addressCity;
                updatedAddress.PostalCode = addressPostalCode;
                updatedAddress.Country = addressCountry;

                databaseContext.Addresses.Update(updatedAddress);
                databaseContext.SaveChanges();

                TempData["AddressChanged"] = true;
                return RedirectToAction("Index", "User");
            }
        }
    }
}