using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WhatsForDinnerMVC.Models;


namespace WhatsForDinnerMVC.Controllers
{
    public class SearchController : Controller
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
            return View(user);
        }

        [HttpPost]
        public ActionResult SearchSubmitAction(FormCollection collection)
        {
            string searchString = collection["searchString"];

            SearchTester searchTester = new SearchTester();
            searchTester.PerformSearch(searchString);
            Session["searchTester"] = searchTester;
            return View("Index", searchTester);
        }

        [HttpPost]
        public ActionResult NewMenuName(FormCollection collection)
        {
            string newMenuName = collection["newMenuName"];
            //TODO need to create a new menu record.
            return RedirectToAction("index", "MenuEdit");
        }

        [HttpPost]
        public ActionResult EditRecipe()
        {
            return RedirectToAction("index","MenuEdit");
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