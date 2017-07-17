using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WhatsForDinnerMVC.Models;

namespace WhatsForDinnerMVC.Controllers
{
    public class LoginController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SubmitAction(FormCollection collection)
        {
            var user = new User(collection["email"], collection["password"]);
            // If valid user, direct to the menu page!
            if (user.IsValid)
            {
                return View("Index"); // TODO - go to the page after login.
            }
            
            // Not valid login - reload the login page and display viewmessage. 
            ViewBag.Message = "Unable to login.";
            return View("Index");
        }

        public ActionResult Register()
        {
            return View("Register");
        }

        [HttpPost]
        public ActionResult SubmitRegister(FormCollection collection)
        {
            // Attempt to create account.  On Success, set a success view message and direct user to the register.
            if (false)
            {
                ViewBag.Message = "Succesfully created account. You can login now!";
                return View("Index");
            }

            // On fail, set a fail view message and direct user to the register page.
            ViewBag.Message = "Username already taken - failed to create account.";
            return View("Register");
        }
    }
}