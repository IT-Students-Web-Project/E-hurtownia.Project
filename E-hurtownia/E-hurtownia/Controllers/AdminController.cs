using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using E_hurtownia.Models;
using System.Runtime.Loader;

namespace E_hurtownia.Controllers {
    public class AdminController : Controller {
        private EhurtowniaContext databaseContext = new EhurtowniaContext();

        public IActionResult UsersList() {
            if (Request.Cookies["COOKIE_LOGGED_USERNAME"] != null) {
                if (databaseContext.Users.Count() > 0) {
                    if (databaseContext.Users.Where(user => user.Login == Request.Cookies["COOKIE_LOGGED_USERNAME"]).First().FkGroup == 1) { // Checking whether user accessed this page is an administrator
                        ViewBag.COOKIE_LOGGED_USERNAME = Request.Cookies["COOKIE_LOGGED_USERNAME"];
                        ViewBag.Users = databaseContext.Users.ToList();
                        ViewBag.Groups = databaseContext.Groups.ToList();
                        ViewBag.Customers = databaseContext.Customers.ToList();

                        return View();
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

        public IActionResult StorehousesList() {
            if (Request.Cookies["COOKIE_LOGGED_USERNAME"] != null) {
                if (databaseContext.Users.Count() > 0) {
                    if (databaseContext.Users.Where(user => user.Login == Request.Cookies["COOKIE_LOGGED_USERNAME"]).First().FkGroup == 1) {
                        ViewBag.COOKIE_LOGGED_USERNAME = Request.Cookies["COOKIE_LOGGED_USERNAME"];
                        ViewBag.Addresses = databaseContext.Addresses.ToList();
                        ViewBag.Storehouses = databaseContext.Storehouses.ToList();

                        return View();
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

        public IActionResult StorehousesListAction_DELETE(int id) {
            Storehouses storehouseToDelete = databaseContext.Storehouses.Where(storehouse => storehouse.IdStorehouse == id).Single();
            Addresses addressToDelete = databaseContext.Addresses.Where(address => address.IdAddress == storehouseToDelete.FkAddress).Single();

            databaseContext.Addresses.Remove(addressToDelete);
            databaseContext.Storehouses.Remove(storehouseToDelete);
            databaseContext.SaveChanges();

            return RedirectToAction("StorehousesList", "Admin");
        }

        public IActionResult StorehousesListAction_UPDATE_ADDRESS(int id) {
            if (Request.Cookies["COOKIE_LOGGED_USERNAME"] != null) {
                if (databaseContext.Users.Count() > 0) {
                    if (databaseContext.Users.Where(user => user.Login == Request.Cookies["COOKIE_LOGGED_USERNAME"]).First().FkGroup == 1) {
                        ViewBag.COOKIE_LOGGED_USERNAME = Request.Cookies["COOKIE_LOGGED_USERNAME"];
                        ViewBag.Address = databaseContext.Addresses.Where(address => address.IdAddress == id).Single();

                        ViewBag.AddressResult_InvalidStreet = (TempData["AddressResult_InvalidStreet"] != null) ? true : ViewBag.AddressResult_InvalidStreet;
                        ViewBag.AddressResult_InvalidBuildingNum = (TempData["AddressResult_InvalidBuildingNum"] != null) ? true : ViewBag.AddressResult_InvalidBuildingNum;
                        ViewBag.AddressResult_InvalidCity = (TempData["AddressResult_InvalidCity"] != null) ? true : ViewBag.AddressResult_InvalidCity;
                        ViewBag.AddressResult_InvalidPostalCode = (TempData["AddressResult_InvalidPostalCode"] != null) ? true : ViewBag.AddressResult_InvalidPostalCode;
                        ViewBag.AddressResult_InvalidCountry = (TempData["AddressResult_InvalidCountry"] != null) ? true : ViewBag.AddressResult_InvalidCountry;

                        return View();
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

        public IActionResult StorehousesListAction_UPDATE_ADDRESS_CHECK(int id) {
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

            if (addressStreet.Length < 5 || addressStreet.Length > 60) {
                TempData["AddressResult_InvalidStreet"] = true;

                return RedirectToAction("StorehousesListAction_UPDATE_ADDRESS", "Admin", new { id = id });
            } else if (addressBuildingNum < 0) {
                TempData["AddressResult_InvalidBuildingNum"] = true;

                return RedirectToAction("StorehousesListAction_UPDATE_ADDRESS", "Admin", new { id = id });
            } else if (addressCity.Length < 5 || addressCity.Length > 30) {
                TempData["AddressResult_InvalidCity"] = true;

                return RedirectToAction("StorehousesListAction_UPDATE_ADDRESS", "Admin", new { id = id });
            } else if (addressPostalCode.Length < 5 || addressPostalCode.Length > 30) {
                TempData["AddressResult_InvalidPostalCode"] = true;

                return RedirectToAction("StorehousesListAction_UPDATE_ADDRESS", "Admin", new { id = id });
            } else if (addressCountry.Length < 3 || addressCountry.Length > 30) {
                TempData["AddressResult_InvalidCountry"] = true;

                return RedirectToAction("StorehousesListAction_UPDATE_ADDRESS", "Admin", new { id = id });
            } else {
                Addresses updatedAddress = databaseContext.Addresses.Where(address => address.IdAddress == id).Single();
                updatedAddress.Street = addressStreet;
                updatedAddress.BuildingNum = addressBuildingNum;
                updatedAddress.ApartmentNum = addressApartmentNum;
                updatedAddress.City = addressCity;
                updatedAddress.PostalCode = addressPostalCode;
                updatedAddress.Country = addressCountry;

                databaseContext.Update(updatedAddress);
                databaseContext.SaveChanges();

                return RedirectToAction("StorehousesList", "Admin");
            }
        }
    }
}