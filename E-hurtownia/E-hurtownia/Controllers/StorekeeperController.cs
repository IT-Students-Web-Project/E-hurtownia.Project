using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using E_hurtownia.Models;
using Microsoft.AspNetCore.Mvc;

namespace E_hurtownia.Controllers {
    public class StorekeeperController : Controller {
        private EhurtowniaContext databaseContext = new EhurtowniaContext();

        public IActionResult OrderList() {
            int meUserID = databaseContext.Users.Where(user => user.Login == Request.Cookies["COOKIE_LOGGED_USERNAME"]).Single().IdUser;
            int? meGroup = databaseContext.Users.Where(user => user.Login == Request.Cookies["COOKIE_LOGGED_USERNAME"]).Single().FkGroup;

            ViewBag.MeGroup = meGroup;
            ViewBag.Customers = databaseContext.Customers.ToList();
            ViewBag.Orders = databaseContext.Orders.ToList();
            ViewBag.OrderItems = databaseContext.OrderItems.ToList();
            ViewBag.OrderStatuses = databaseContext.OrderStatuses.ToList();
            ViewBag.Persons = databaseContext.Persons.ToList();
            ViewBag.Products = databaseContext.Products.ToList();

            return View();
        }
    }
}