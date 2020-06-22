using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using E_hurtownia.Models;
using Microsoft.AspNetCore.Mvc;

namespace E_hurtownia.Controllers {
    public class PartialViewsController : Controller {
        private EhurtowniaContext databaseContext = new EhurtowniaContext();

        public IActionResult FormAddress() {
            return PartialView();
        }

        public IActionResult FormPerson() {
            return PartialView();
        }

        public IActionResult NavBarUser() {
            return PartialView();
        }

        public IActionResult ProductList() {
            return PartialView();
        }

        public IActionResult OrderList() {
            return PartialView();
        }
    }
}