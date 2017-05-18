using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Projects_Management_System.Controllers
{
    public class JEController : Controller
    {
        [Authorize(Roles ="JE")]
        public ActionResult Index()
        {
            return View();
        }
    }
}