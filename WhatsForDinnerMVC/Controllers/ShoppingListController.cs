using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WhatsForDinnerMVC.Models;

namespace WhatsForDinnerMVC.Controllers
{
	public class ShoppingListController : Controller
	{
		// GET: ShoppingList
		public ActionResult Index()
		{
			// Get user object from the session, make sure valid.
			User user = (User)this.Session["user"];
			if (user == null || !user.IsValid)
			{
				return RedirectToAction("index", "Login");
			}

			Menu selectedMenu = (Menu)Session["selectedMenu"];

			ShoppingList newShoppingList;
			if (selectedMenu != null)
			{
				newShoppingList = new ShoppingList(selectedMenu.MenuID);
				return View(newShoppingList);
			}
			// If there wasn't a view in the session, return the selected view.
			newShoppingList = new ShoppingList(user.SelectedMenu.MenuID);
			return View(newShoppingList);//return View(user.SelectedMenu);

		}
	}
}