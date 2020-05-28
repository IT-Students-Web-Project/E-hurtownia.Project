using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace E_hurtownia.Controllers {
    public class UserController : Controller {
        public IActionResult Index() {
            return View();
        }
    }
}