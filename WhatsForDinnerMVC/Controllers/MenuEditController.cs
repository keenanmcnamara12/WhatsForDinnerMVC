using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WhatsForDinnerMVC.Models;

namespace WhatsForDinnerMVC.Controllers
{
    public class MenuEditController : Controller
    {
        // GET: MenuEdit
        public ActionResult Index()
        {
            // Get user object from the session, make sure valid.
            User user = (User)this.Session["user"];
            if (user == null || !user.IsValid)
            {
                return RedirectToAction("index", "Login");
            }
            return View(user.SelectedMenu);
        }

        [HttpPost]
        public ActionResult PerformSearch(FormCollection collection)
        {
            string searchString = collection["searchString"];
            User user = (User)this.Session["user"];
            Menu menu = user.SelectedMenu;
            menu.PerformSearch(searchString);
            Session["user"] = user;
            return View("index", user.SelectedMenu);
        }

        /// <summary>
        /// When the user wants to get out of the menu edit screen.
        /// </summary>
        /// <returns></returns>
        public ActionResult BackToMenus()
        {
            return RedirectToAction("index", "MenuList");
        }

        [HttpPost]
        public JsonResult RecipeDeleteSelectionChanged(int id)
        {
            // Updating the current selection gets us ready for a deletion
            User user = (User)this.Session["user"];
            Menu selectedMenu = user.SelectedMenu;
            selectedMenu.UpdateDeleteSelectedRecipe(id);
            Session["user"] = user;
            return null;
        }

        [HttpPost]
        public JsonResult RecipeAddSelectionChanged(int id)
        {
            // Updating the current selection gets us ready for a deletion
            User user = (User)this.Session["user"];
            Menu selectedMenu = user.SelectedMenu;
            selectedMenu.UpdateAddSelectedRecipe(id);
            Session["user"] = user;
            return null;
        }
    }
}