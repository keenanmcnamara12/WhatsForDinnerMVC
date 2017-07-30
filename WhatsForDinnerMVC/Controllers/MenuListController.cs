using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WhatsForDinnerMVC.Models;


namespace WhatsForDinnerMVC.Controllers
{
	public class MenuListController : Controller
	{
		// GET: Search
		public ActionResult Index()
		{
			// Get user object from the session, make sure valid.
			User user = (User)this.Session["user"];
			if (user == null || !user.IsValid)
			{
				return RedirectToAction("index", "Login");
			}
			return View("index", user);
		}

		[HttpPost]
		public ActionResult NewMenuName(FormCollection collection)
		{
			string newMenuName = collection["newMenuName"];
			// Make sure the user create a manu name!
			if (String.IsNullOrWhiteSpace(newMenuName))
			{
				return RedirectToAction("index", "Search");
			}

			// create a new menu record and make it the selected menu
			User user = (User)Session["user"];
			int newMenuId = user.AddNewMenu(newMenuName);
			user.UpdateSelectedMenu(newMenuId);
			Session["user"] = user;

			return RedirectToAction("index", "MenuEdit");
		}

		[HttpPost]
		public ActionResult EditRecipe()
		{
			return RedirectToAction("index", "MenuEdit");
		}


		[HttpPost]
		public ActionResult ViewShoppingList()
		{
			return RedirectToAction("index", "ShoppingList");
		}

		[HttpPost]
		public JsonResult SelectionChanged(int id)
		{
			User user = (User)this.Session["user"];
			user.UpdateSelectedMenu(id);
			Session["user"] = user;
			return null;
		}

	}
}