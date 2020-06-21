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
            ViewBag.Product = databaseContext.Products.Where(product => product.IdProduct == id).Single();
            ViewBag.Stocks = databaseContext.Stocks.ToList();
            ViewBag.Units = databaseContext.Units.ToList();

            if (Request.Cookies["COOKIE_LOGGED_USERNAME"] != null) {
                string username = Request.Cookies["COOKIE_LOGGED_USERNAME"];
                int userID = databaseContext.Users.Where(user => user.Login == username).Single().IdUser;
                int? userGroup = databaseContext.Users.Where(user => user.IdUser == userID).Single().FkGroup;

                Customers meCustomer = databaseContext.Customers.Where(customer => customer.FkUser == userID).SingleOrDefault();

                if ((meCustomer != default(Customers)) && (userGroup == 2)) { // Current user is a Customer and is associated to Customers group
                    ViewBag.IsCustomer = true;
                }
            }

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

        public IActionResult CreateOrder() {
            List<CartElement> orderCart = JsonSerializer.Deserialize<List<CartElement>>(Request.Cookies["COOKIE_CART_CONTENT"]);
            int meUserID = databaseContext.Users.Where(user => user.Login == Request.Cookies["COOKIE_LOGGED_USERNAME"]).Single().IdUser;
            int meCustomerID = databaseContext.Customers.Where(customer => customer.FkUser == meUserID).Single().IdCustomer;
            int orderItemIndex = (databaseContext.OrderItems.Count() > 0) ? databaseContext.OrderItems.Max(item => item.IdOrderItem) : 0;

            Orders newOrder = new Orders {
                IdOrder = (databaseContext.Orders.Count() > 0) ? databaseContext.Orders.Max(order => order.IdOrder) + 1 : 1,
                FkCustomer = meCustomerID,
                DateOrdered = DateTime.Now,
                FkOrderStatus = 1,
                Status = true
            };

            databaseContext.Orders.Add(newOrder);
            databaseContext.SaveChanges();

            foreach (CartElement cartElement in orderCart) {
                OrderItems orderItem = new OrderItems {
                    IdOrderItem = ++orderItemIndex,
                    FkOrder = newOrder.IdOrder,
                    FkProduct = cartElement.ProductID,
                    Amount = cartElement.ProductQuantity
                };

                databaseContext.OrderItems.Add(orderItem);
            }

            databaseContext.SaveChanges();
            Response.Cookies.Delete("COOKIE_CART_CONTENT");
            Response.Cookies.Append("COOKIE_CART_CONTENT", "");

            return RedirectToAction("Index", "User");
        }
    }
}