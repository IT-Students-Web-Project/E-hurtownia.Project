using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using E_hurtownia.Models;
using Microsoft.AspNetCore.Mvc;

namespace E_hurtownia.Controllers {
    public class CustomerController : Controller {
        private EhurtowniaContext databaseContext = new EhurtowniaContext();

        public IActionResult Product(int id) {
            ViewBag.COOKIE_LOGGED_USERNAME = Request.Cookies["COOKIE_LOGGED_USERNAME"];
            ViewBag.Product = databaseContext.Products.Where(product => product.IdProduct == id).Single();
            ViewBag.Stocks = databaseContext.Stocks.ToList();
            ViewBag.Units = databaseContext.Units.ToList();

            return View();
        }
    }
}