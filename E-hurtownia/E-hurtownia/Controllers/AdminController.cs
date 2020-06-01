using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using E_hurtownia.Models;

namespace E_hurtownia.Controllers {
    public class AdminController : Controller {
        private EhurtowniaContext databaseContext = new EhurtowniaContext();

        public IActionResult UsersList() {
            if (Request.Cookies["COOKIE_LOGGED_USERNAME"] != null) {
                if (databaseContext.Users.Count() > 0) {
                    if (databaseContext.Users.Where(user => user.Login == Request.Cookies["COOKIE_LOGGED_USERNAME"]).First().FkGroup == 1) { // Checking whether user accessed this page is an administrator
                        ViewBag.Users = databaseContext.Users.ToList();
                        ViewBag.Groups = databaseContext.Groups.ToList();
                        ViewBag.Customers = databaseContext.Customers.ToList();
                        ViewBag.COOKIE_LOGGED_USERNAME = Request.Cookies["COOKIE_LOGGED_USERNAME"];

                        return View();
                    } else {
                        TempData["ErrorHeader"] = "Access denied";
                        TempData["ErrorMessage"] = "Page accessed as non-administrator user account";

                        return RedirectToAction("Error", "User");
                    }
                } else {
                    TempData["ErrorHeader"] = "No users";
                    TempData["ErrorMessage"] = "User accounts database is empty";

                    return RedirectToAction("Error", "User");
                }
            } else {
                TempData["ErrorHeader"] = "Access denied";
                TempData["ErrorMessage"] = "Sign in as administrator to access this page";

                return RedirectToAction("Error", "User");
            }
        }

        public IActionResult UsersListAction_DELETE(int id) { // Deleting account from admin users list
            List<Users> usersToDelete = databaseContext.Users.Where(user => user.IdUser == id).ToList();

            if (usersToDelete.Count == 1) {
                Users selectedAccount = usersToDelete.First();
                List<Customers> customersToDelete = databaseContext.Customers.Where(customer => customer.FkUser == selectedAccount.IdUser).ToList();

                databaseContext.Customers.RemoveRange(customersToDelete);
                databaseContext.Users.Remove(selectedAccount);
                databaseContext.SaveChanges();
            } else {
                TempData["ErrorHeader"] = "Multiple accounts";
                TempData["ErrorMessage"] = "Encountered multiple user accounts with the same username, please fix this error manually in database";

                return RedirectToAction("Error", "User");
            }

            return RedirectToAction("UsersList", "Admin");
        }

        public IActionResult UsersListAction_MAKE_ADMIN(int id) { // Setting user group to ADMINS
            List<Users> usersToRegroup = databaseContext.Users.Where(user => user.IdUser == id).ToList();
            Users selectedAccount = usersToRegroup.First();
            
            selectedAccount.FkGroup = 1;
            databaseContext.Users.Update(selectedAccount);
            databaseContext.SaveChanges();

            return RedirectToAction("UsersList", "Admin");
        }

        public IActionResult UsersListAction_MAKE_CUSTOMER(int id) { // Setting user group to CUSTOMERS
            List<Users> usersToRegroup = databaseContext.Users.Where(user => user.IdUser == id).ToList();
            Users selectedAccount = usersToRegroup.First();

            selectedAccount.FkGroup = 2;
            databaseContext.Users.Update(selectedAccount);
            databaseContext.SaveChanges();

            return RedirectToAction("UsersList", "Admin");
        }

        public IActionResult UsersListAction_MAKE_STOREKEEPER(int id) { // Setting user group to STOREKEEPERS
            List<Users> usersToRegroup = databaseContext.Users.Where(user => user.IdUser == id).ToList();
            Users selectedAccount = usersToRegroup.First();

            selectedAccount.FkGroup = 3;
            databaseContext.Users.Update(selectedAccount);
            databaseContext.SaveChanges();

            return RedirectToAction("UsersList", "Admin");
        }

        public IActionResult UsersListAction_MAKE_OFFERENT(int id) { // Setting user group to OFFERENTS
            List<Users> usersToRegroup = databaseContext.Users.Where(user => user.IdUser == id).ToList();
            Users selectedAccount = usersToRegroup.First();

            selectedAccount.FkGroup = 4;
            databaseContext.Users.Update(selectedAccount);
            databaseContext.SaveChanges();

            return RedirectToAction("UsersList", "Admin");
        }
    }
}