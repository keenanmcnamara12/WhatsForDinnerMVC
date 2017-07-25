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
				ViewBag.Message = "";
				this.Session["User"] = user;  // Save the user object so the search page can recover later.
				return RedirectToAction("index", "Search");
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
			var user = new User(collection["email"], collection["name"], collection["password"]);
			if (user.CreateNewUser())
			{
				ViewBag.Message = "Succesfully created account. You can login now!";
				return View("Index");
			}
			else
			{
				// On fail, set a fail view message and direct user to the register page.
				ViewBag.Message = "Username already taken - failed to create account.";
				return View("Register");
			}
		}
	}
}