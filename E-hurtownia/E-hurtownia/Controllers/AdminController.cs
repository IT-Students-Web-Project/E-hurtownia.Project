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
    }
}