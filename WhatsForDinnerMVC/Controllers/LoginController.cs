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
        private string userName = "";
        private string password = "";

        // GET: Login
        public ActionResult Index()
        {
            Login login = new Login() { userName = userName, password = password };
            return View(login);
        }

        //public ActionResult SumbitAction() {
        //    var login = new Login() { userName = userName, password = password };
        //    return View("Index", login);
        //}

        [HttpPost]
        public ActionResult SubmitAction(FormCollection collection)
        {
            //https://stackoverflow.com/questions/5088450/how-to-retrieve-form-values-from-httppost-dictionary-or
            //https://stackoverflow.com/questions/6995285/how-to-access-my-formcollection-in-action-method-asp-net-mvc
            userName = collection["email"];
            password = collection["password"];

            var login = new Login() { userName = userName, password = password};

            return View("Index", login);
        }

    }
}