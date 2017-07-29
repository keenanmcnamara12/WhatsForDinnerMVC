﻿using System;
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
            Menu selectedMenu = (Menu)Session["selectedMenu"];
            if (selectedMenu != null)
            {
                return View(selectedMenu);
            }
            // If there wasn't a view in the session, return the selected view.
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
            Session["selectedMenu"] = user.SelectedMenu;
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
            Menu selectedMenu = user.SelectedMenu;
            selectedMenu.Recipes.Add(selectedMenu.SelectedAddRecipe);
            Session["user"] = user;
            return View("Index", user.SelectedMenu);
        }
        
    }
}