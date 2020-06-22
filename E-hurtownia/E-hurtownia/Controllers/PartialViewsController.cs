using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace E_hurtownia.Controllers {
    public class PartialViewsController : Controller {
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