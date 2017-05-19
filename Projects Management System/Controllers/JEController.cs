using Projects_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Projects_Management_System.Controllers
{
    public class JEController : Controller
    {
        private Managment db = new Managment();

        [Authorize(Roles ="JE")]
        public ActionResult Index()
        {
            List<object> TL = new List<object>();
            var id = (int)Session["id"];

            //  var req = from r in db.Sending_Requests where r.Reciever_ID == id select r;
            var result = from y in db.Sending_Requests
                         where !(
                                     from x in db.Responding_Requests

                                     select x.Request_ID
                                 ).Contains(y.ID)
                         select y;

            TL.Add(result.ToList());
            var project = from c in db.Projects
                          where (

from y in db.Sending_Requests
where (
      from x in db.Responding_Requests

      select x.Request_ID
  ).Contains(y.ID)
select y.Project_ID).Contains(c.ID)
                          select c;

            TL.Add(project.ToList());

            return View(TL);
        }

        [HttpPost]
        public ActionResult AcceptORreject(int requestid, int userid, bool stat, Responding_Request respond)
        {
            respond.Request_ID = requestid;
            respond.User_ID = userid;
            respond.Respond = stat;
            db.Responding_Requests.Add(respond);
            db.SaveChanges();


            return RedirectToAction("Index", "TL");
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