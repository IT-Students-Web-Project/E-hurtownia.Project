using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using E_hurtownia.Models;
using Microsoft.AspNetCore.Mvc;

namespace E_hurtownia.Controllers {
    public class CartController : Controller {
        private EhurtowniaContext databaseContext = new EhurtowniaContext();

        public IActionResult Index() {
            ViewBag.COOKIE_LOGGED_USERNAME = Request.Cookies["COOKIE_LOGGED_USERNAME"];
            ViewBag.Products = databaseContext.Products.ToList();
            ViewBag.Units = databaseContext.Units.ToList();

            List<CartElement> cartList = new List<CartElement>();
            string cartJSON = Request.Cookies["COOKIE_CART_CONTENT"];

            if (cartJSON != "") {
                cartList = JsonSerializer.Deserialize<List<CartElement>>(cartJSON);
            }

            return View(cartList);
        }
    }
}