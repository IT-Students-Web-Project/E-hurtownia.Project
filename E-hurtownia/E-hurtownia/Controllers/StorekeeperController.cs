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
            ViewBag.MeGroup = databaseContext.Users.Where(user => user.Login == Request.Cookies["COOKIE_LOGGED_USERNAME"]).Single().FkGroup;
            ViewBag.Orders = databaseContext.Orders.ToList();
            ViewBag.OrderItems = databaseContext.OrderItems.ToList();
            ViewBag.OrderStatuses = databaseContext.OrderStatuses.ToList();
            ViewBag.Products = databaseContext.Products.ToList();

            return View();
        }
    }
}