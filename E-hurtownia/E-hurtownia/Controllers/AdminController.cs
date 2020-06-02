using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using E_hurtownia.Models;
using System.Runtime.Loader;
using System.Diagnostics.Eventing.Reader;

namespace E_hurtownia.Controllers {
    public class AdminController : Controller {
        private EhurtowniaContext databaseContext = new EhurtowniaContext();

        private IActionResult CheckAdminRights(Func<IActionResult> actionIfAdmin) { // Generic function which checks whether the user has Administrator rights (is in Admins group)
            if (Request.Cookies["COOKIE_LOGGED_USERNAME"] != null) {
                if (databaseContext.Users.Count() > 0) {
                    if (databaseContext.Users.Where(user => user.Login == Request.Cookies["COOKIE_LOGGED_USERNAME"]).First().FkGroup == 1) { // Checking whether user accessed this page is an administrator
                        return actionIfAdmin();
                    } else {
                        TempData["ErrorHeader"] = "Access denied";
                        TempData["ErrorMessage"] = "Page accessed as non-administrator user account";

                        return RedirectToAction("Error", "User");
                    }
                } else {
                    TempData["ErrorHeader"] = "No users";
                    TempData["ErrorMessage"] = "User accounts database is empty";

                    return RedirectToAction("Error", "User");
                }
            } else {
                TempData["ErrorHeader"] = "Access denied";
                TempData["ErrorMessage"] = "Sign in as administrator to access this page";

                return RedirectToAction("Error", "User");
            }
        }

        private IActionResult CheckStorehouseAddressForm(string returnAction, string returnController, int? id) {
            string addressStreet = "";
            int addressBuildingNum = -1;

            #nullable enable
            int? addressApartmentNum = null;
            #nullable disable

            string addressCity = "";
            string addressPostalCode = "";
            string addressCountry = "";

            if (Request.HasFormContentType == true) {
                addressStreet = Request.Form["address-street"];
                addressCity = Request.Form["address-city"];
                addressPostalCode = Request.Form["address-postal"];
                addressCountry = Request.Form["address-country"];

                if (Request.Form["address-bnum"].ToString() != String.Empty) {
                    addressBuildingNum = Int32.Parse(Request.Form["address-bnum"]);
                }

                if (Request.Form["address-anum"].ToString() != String.Empty) {
                    addressApartmentNum = Int32.Parse(Request.Form["address-anum"]);
                }
            } else {
                TempData["ErrorHeader"] = "Data transfer error";
                TempData["ErrorMessage"] = "Did not received any data, form is empty";

                return RedirectToAction("Error", "User");
            }

            bool invalidStreet = (addressStreet.Length < 5 || addressStreet.Length > 60);
            bool invalidBuildingNum = (addressBuildingNum < 0);
            bool invalidCity = (addressCity.Length < 5 || addressCity.Length > 30);
            bool invalidPostalCode = (addressPostalCode.Length < 5 || addressPostalCode.Length > 30);
            bool invalidCountry = (addressCountry.Length < 3 || addressCountry.Length > 30);

            if (invalidStreet == true || invalidBuildingNum == true || invalidCity == true || invalidPostalCode == true || invalidCountry == true) {
                if (invalidStreet) TempData["AddressResult_InvalidStreet"] = true;
                if (invalidBuildingNum) TempData["AddressResult_InvalidBuildingNum"] = true;
                if (invalidCity) TempData["AddressResult_InvalidCity"] = true;
                if (invalidPostalCode) TempData["AddressResult_InvalidPostalCode"] = true;
                if (invalidCountry) TempData["AddressResult_InvalidCountry"] = true;

                return RedirectToAction(returnAction, returnController, (id != null) ? (new { id = id }) : null); // Passes ID if retrieved is as argument, else - passes NULL
            } else {
                Addresses addressObject = null;

                if (id == null) { // New storehouse is created
                    addressObject = new Addresses();

                    if (databaseContext.Addresses.Count() > 0) {
                        addressObject.IdAddress = databaseContext.Addresses.OrderBy(address => address.IdAddress).Last().IdAddress + 1; // Retrieve ID of last existing address object in ADDRESSES table
                    } else {
                        addressObject.IdAddress = 1;
                    }
                } else { // Already existing storehouse (its address) is updated
                    addressObject = databaseContext.Addresses.Where(address => address.IdAddress == id).Single();
                }

                addressObject.Street = addressStreet;
                addressObject.BuildingNum = addressBuildingNum;
                addressObject.ApartmentNum = addressApartmentNum;
                addressObject.City = addressCity;
                addressObject.PostalCode = addressPostalCode;
                addressObject.Country = addressCountry;

                if (id == null) {
                    addressObject.Status = true;
                    databaseContext.Add(addressObject);

                    // New storehouse's address is created, now need to create object in STOREHOUSES table
                    Storehouses newStorehouse = new Storehouses();

                    if (databaseContext.Storehouses.Count() > 0) {
                        newStorehouse.IdStorehouse = databaseContext.Storehouses.OrderBy(storehouse => storehouse.IdStorehouse).Last().IdStorehouse + 1;
                    } else {
                        newStorehouse.IdStorehouse = 1;
                    }

                    newStorehouse.FkAddress = addressObject.IdAddress;
                    newStorehouse.Status = true;

                    databaseContext.Storehouses.Add(newStorehouse);
                } else {
                    databaseContext.Update(addressObject);
                }

                databaseContext.SaveChanges();
                return RedirectToAction("StorehousesList", "Admin");
            }
        }

        public IActionResult UsersList(int id) { // ACTION - SHOW USERS LIST
            return CheckAdminRights(() => {
                ViewBag.COOKIE_LOGGED_USERNAME = Request.Cookies["COOKIE_LOGGED_USERNAME"];
                ViewBag.Users = databaseContext.Users.ToList();
                ViewBag.Groups = databaseContext.Groups.ToList();
                ViewBag.Customers = databaseContext.Customers.ToList();

                return View();
            });
        }

        public IActionResult UsersListAction_DELETE(int id) { // Deleting account (from admin users list level)
            List<Users> usersToDelete = databaseContext.Users.Where(user => user.IdUser == id).ToList();

            if (usersToDelete.Count == 1) {
                Users selectedAccount = usersToDelete.First();
                List<Customers> customersToDelete = databaseContext.Customers.Where(customer => customer.FkUser == selectedAccount.IdUser).ToList();

                databaseContext.Customers.RemoveRange(customersToDelete);
                databaseContext.Users.Remove(selectedAccount);
                databaseContext.SaveChanges();
            } else {
                TempData["ErrorHeader"] = "Multiple accounts";
                TempData["ErrorMessage"] = "Encountered multiple user accounts with the same username, please fix this error manually in database";

                return RedirectToAction("Error", "User");
            }

            return RedirectToAction("UsersList", "Admin");
        }

        public IActionResult UsersListAction_MAKE_ADMIN(int id) { // Setting user group to ADMINS
            List<Users> usersToRegroup = databaseContext.Users.Where(user => user.IdUser == id).ToList();
            Users selectedAccount = usersToRegroup.First();
            
            selectedAccount.FkGroup = 1;
            databaseContext.Users.Update(selectedAccount);
            databaseContext.SaveChanges();

            return RedirectToAction("UsersList", "Admin");
        }

        public IActionResult UsersListAction_MAKE_CUSTOMER(int id) { // Setting user group to CUSTOMERS
            List<Users> usersToRegroup = databaseContext.Users.Where(user => user.IdUser == id).ToList();
            Users selectedAccount = usersToRegroup.First();

            selectedAccount.FkGroup = 2;
            databaseContext.Users.Update(selectedAccount);
            databaseContext.SaveChanges();

            return RedirectToAction("UsersList", "Admin");
        }

        public IActionResult UsersListAction_MAKE_STOREKEEPER(int id) { // Setting user group to STOREKEEPERS
            List<Users> usersToRegroup = databaseContext.Users.Where(user => user.IdUser == id).ToList();
            Users selectedAccount = usersToRegroup.First();

            selectedAccount.FkGroup = 3;
            databaseContext.Users.Update(selectedAccount);
            databaseContext.SaveChanges();

            return RedirectToAction("UsersList", "Admin");
        }

        public IActionResult UsersListAction_MAKE_OFFERENT(int id) { // Setting user group to OFFERENTS
            List<Users> usersToRegroup = databaseContext.Users.Where(user => user.IdUser == id).ToList();
            Users selectedAccount = usersToRegroup.First();

            selectedAccount.FkGroup = 4;
            databaseContext.Users.Update(selectedAccount);
            databaseContext.SaveChanges();

            return RedirectToAction("UsersList", "Admin");
        }

        public IActionResult StorehousesList() { // ACTION - SHOW STOREHOUSES LIST
            return CheckAdminRights(() => {
                ViewBag.COOKIE_LOGGED_USERNAME = Request.Cookies["COOKIE_LOGGED_USERNAME"];
                ViewBag.Addresses = databaseContext.Addresses.ToList();
                ViewBag.Storehouses = databaseContext.Storehouses.ToList();

                return View();
            });
        }

        public IActionResult StorehousesListAction_DELETE(int id) {
            Storehouses storehouseToDelete = databaseContext.Storehouses.Where(storehouse => storehouse.IdStorehouse == id).Single();
            Addresses addressToDelete = databaseContext.Addresses.Where(address => address.IdAddress == storehouseToDelete.FkAddress).Single();

            databaseContext.Addresses.Remove(addressToDelete);
            databaseContext.Storehouses.Remove(storehouseToDelete);
            databaseContext.SaveChanges();

            return RedirectToAction("StorehousesList", "Admin");
        }

        public IActionResult StorehousesListAction_UPDATE_ADDRESS(int id) { // ACTION - SHOW STOREHOUSES ADDRESS UPDATING FORM
            return CheckAdminRights(() => {
                ViewBag.COOKIE_LOGGED_USERNAME = Request.Cookies["COOKIE_LOGGED_USERNAME"];
                ViewBag.Address = databaseContext.Addresses.Where(address => address.IdAddress == id).Single();

                if (TempData["AddressResult_InvalidStreet"] != null) ViewBag.AddressResult_InvalidStreet = true;
                if (TempData["AddressResult_InvalidBuildingNum"] != null) ViewBag.AddressResult_InvalidBuildingNum = true;
                if (TempData["AddressResult_InvalidCity"] != null) ViewBag.AddressResult_InvalidCity = true;
                if (TempData["AddressResult_InvalidPostalCode"] != null) ViewBag.AddressResult_InvalidPostalCode = true;
                if (TempData["AddressResult_InvalidCountry"] != null) ViewBag.AddressResult_InvalidCountry = true;

                return View();
            });
        }

        public IActionResult StorehousesListAction_UPDATE_ADDRESS_CHECK(int id) { // Checks whether all entered address data is correct
            return CheckStorehouseAddressForm("StorehousesListAction_UPDATE_ADDRESS", "Admin", id);
        }

        public IActionResult StorehousesListAction_CREATE() { // ACTION - SHOW STOREHOUSE CREATING FORM
            return CheckAdminRights(() => {
                ViewBag.COOKIE_LOGGED_USERNAME = Request.Cookies["COOKIE_LOGGED_USERNAME"];

                if (TempData["AddressResult_InvalidStreet"] != null) ViewBag.AddressResult_InvalidStreet = true;
                if (TempData["AddressResult_InvalidBuildingNum"] != null) ViewBag.AddressResult_InvalidBuildingNum = true;
                if (TempData["AddressResult_InvalidCity"] != null) ViewBag.AddressResult_InvalidCity = true;
                if (TempData["AddressResult_InvalidPostalCode"] != null) ViewBag.AddressResult_InvalidPostalCode = true;
                if (TempData["AddressResult_InvalidCountry"] != null) ViewBag.AddressResult_InvalidCountry = true;

                return View();
            });
        }

        public IActionResult StorehousesListAction_CREATE_CHECK() {
            return CheckStorehouseAddressForm("StorehousesListAction_CREATE", "Admin", null);
        }
    }
}