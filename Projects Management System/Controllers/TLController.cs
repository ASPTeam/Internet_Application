using Projects_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Projects_Management_System.Controllers
{
    public class TLController : Controller
    {
        private Managment db = new Managment();

        [Authorize(Roles ="TL")]
        public ActionResult Index()
        {
            List<object> TL = new List<object>();
            var id = (int)Session["id"];

            var req = from r in db.Sending_Requests where r.Reciever_ID == id select r;
            TL.Add(req.ToList());

            return View(TL);
        }

        [HttpPost]
        public ActionResult AcceptORreject(int requestid , int userid, bool stat ,Responding_Request respond)
        {
            respond.Request_ID = requestid;
            respond.User_ID = userid;
            respond.Respond = stat;
            db.Responding_Requests.Add(respond);
            db.SaveChanges();


          return  RedirectToAction("Index", "TL");
        }




        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}