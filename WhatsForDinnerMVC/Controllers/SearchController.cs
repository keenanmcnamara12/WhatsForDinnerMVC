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

            string name = user.Name;
            ViewBag.Message = "Hello " + name;

            SearchTester searchTester = new SearchTester();
            searchTester.PopulateAllUsers();

            return View(searchTester);
        }

        [HttpPost]
        public ActionResult SearchSubmitAction(FormCollection collection)
        {
            string searchString = collection["searchString"];

            SearchTester searchTester = new SearchTester();
            searchTester.PerformSearch(searchString);
            return View("Index", searchTester);
        }

    }
}