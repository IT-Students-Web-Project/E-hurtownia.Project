using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
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

        public IActionResult Buy() {
            if (Request.HasFormContentType == true) {
                int productID = Int32.Parse(Request.Form["product-id"]);
                int productQuantity = Int32.Parse(Request.Form["product-qty"]);
                string currentCart = Request.Cookies["COOKIE_CART_CONTENT"];
                List<CartElement> currentCartList = new List<CartElement>();

                if (currentCart != "") {
                    currentCartList = JsonSerializer.Deserialize<List<CartElement>>(currentCart);
                }

                CartElement cartElement = new CartElement {
                    ProductID = productID,
                    ProductQuantity = productQuantity
                };

                currentCartList.Add(cartElement);
                currentCart = JsonSerializer.Serialize<List<CartElement>>(currentCartList);

                Response.Cookies.Delete("COOKIE_CART_CONTENT");
                Response.Cookies.Append("COOKIE_CART_CONTENT", currentCart);

                return RedirectToAction("Index", "User");
            } else {
                TempData["ErrorHeader"] = "Data transfer error";
                TempData["ErrorMessage"] = "Did not received any data, form is empty";

                return RedirectToAction("Error", "User");
            }
        }
    }
}