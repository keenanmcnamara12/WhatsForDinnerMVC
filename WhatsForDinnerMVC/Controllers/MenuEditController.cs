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
            // Return the view from the session if it's there.
            // Menu selectedMenu = (Menu)Session["selectedMenu"];
            if (user.SelectedMenu != null)
            {
                return View(user.SelectedMenu);
            }
            // If there wasn't a view in the session, return the selected view.
            return View(user.SelectedMenu);
        }

        // Whenever I accidentally compile and load during modal editting... (and a user would be able to do the same).
        public ActionResult Modal()
        {
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult PerformSearch(FormCollection collection)
        {
            string searchString = collection["searchString"];
            User user = (User)this.Session["user"];
            if (user == null || !user.IsValid)
            {
                return RedirectToAction("index", "Login");
            }
            if (string.IsNullOrWhiteSpace(searchString))
            {
                return View("Index", user.SelectedMenu);
            }
            Menu menu = user.SelectedMenu;
            menu.PerformSearch(searchString);
            Session["user"] = user;
            //Session["selectedMenu"] = user.SelectedMenu;
            return View("Index", user.SelectedMenu);
        }

        /// <summary>
        /// When the user wants to get out of the menu edit screen.
        /// </summary>
        /// <returns></returns>
        public ActionResult BackToMenus()
        {
            return RedirectToAction("Index", "MenuList");
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

        [HttpPost]
        public ActionResult AddSelectedRecipeToMenu()
        {
            User user = (User)Session["user"];
            if (user == null || !user.IsValid)
            {
                return RedirectToAction("index", "Login");
            }
            user.SelectedMenu.AddSelectedRecipeToMenu();
            Session["user"] = user;
            return View("Index", user.SelectedMenu);
        }

        [HttpPost]
        public ActionResult DeleteSelectedRecipeFromMenu()
        {
            User user = (User)Session["user"];
            if (user == null || !user.IsValid)
            {
                return RedirectToAction("index", "Login");
            }
            user.SelectedMenu.DeleteSelectedRecipeFromMenu();
            Session["user"] = user;
            return View("Index", user.SelectedMenu);
        }

        [HttpPost]
        public ActionResult GetModalAddRecipe(int id)
        {
            User user = (User)Session["user"];
            if (user == null || !user.IsValid)
            {
                return RedirectToAction("index", "Login");
            }
            // To prevent the user from needing to select the row then the button, we update the selected menu based on the
            // row that the clicked button was in.
            Menu selectedMenu = user.SelectedMenu;
            selectedMenu.UpdateAddSelectedRecipe(id);
            return PartialView("Modal", user.SelectedMenu.SelectedAddRecipe);
        }

        [HttpPost]
        public ActionResult GetModalDeleteRecipe(int id)
        {
            User user = (User)Session["user"];
            if (user == null || !user.IsValid)
            {
                return RedirectToAction("index", "Login");
            }
            // To prevent the user from needing to select the row then the button, we update the selected menu based on the
            // row that the clicked button was in.
            Menu selectedMenu = user.SelectedMenu;
            selectedMenu.UpdateDeleteSelectedRecipe(id);
            return PartialView("Modal", user.SelectedMenu.SelectedDeleteRecipe);
        }

        //[HttpPost]
        //public ActionResult Launch_RecipeModal()
        //{
        //    User user = (User)Session["user"];
        //    if (user == null || !user.IsValid)
        //    {
        //        return RedirectToAction("index", "Login");
        //    }
        //    user.SelectedMenu.ShowModel = true;
        //    Session["user"] = user;
        //    return View("Index", user.SelectedMenu);
        //}

        //[HttpPost]
        //public ActionResult Close_RecipeModal()
        //{
        //    User user = (User)Session["user"];
        //    if (user == null || !user.IsValid)
        //    {
        //        return RedirectToAction("index", "Login");
        //    }
        //    user.SelectedMenu.ShowModel = false;
        //    Session["user"] = user;
        //    return View("Index", user.SelectedMenu);
        //}

    }
}