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
            ViewBag.Products = databaseContext.Products.ToList();
            ViewBag.Units = databaseContext.Units.ToList();

            List<CartElement> cartList = new List<CartElement>();
            string cartJSON = Request.Cookies["COOKIE_CART_CONTENT"];

            if (cartJSON != "") {
                cartList = JsonSerializer.Deserialize<List<CartElement>>(cartJSON);
            }

            return View(cartList);
        }

        public IActionResult DeleteItem(int id) {
            string cartJSON = Request.Cookies["COOKIE_CART_CONTENT"];
            List<CartElement> cartList = JsonSerializer.Deserialize<List<CartElement>>(cartJSON);

            if (cartList.ElementAt(id) != null) {
                cartList.RemoveAt(id);
            }

            Response.Cookies.Delete("COOKIE_CART_CONTENT");
            Response.Cookies.Append("COOKIE_CART_CONTENT", JsonSerializer.Serialize<List<CartElement>>(cartList));

            return RedirectToAction("Index", "Cart");
        }
    }
}