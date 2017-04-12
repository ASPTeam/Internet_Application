using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Projects_Management_System.Areas.Customers.Controllers
{
    public class HomeController : Controller
    {
        // GET: Customers/Home
        public ActionResult Index()
        {
            return View();
        }
    }
}