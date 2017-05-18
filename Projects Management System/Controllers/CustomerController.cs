using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Projects_Management_System.Models;

namespace Projects_Management_System.Controllers
{
    public class CustomerController : Controller
    {
        private Managment db = new Managment();

        [Authorize(Roles ="Customer")]
        public ActionResult Index()
        {
            List<object> project = new List<object>();

            //  var projects = db.Projects.Include(p => p.Post).Include(p => p.User);
            var id  = (int)Session["id"];
            var v = from c in db.Projects where c.Post.User_ID == id select c;
            project.Add(v.ToList());
            var request = from req in db.Sending_Requests where req.Reciever_ID == id select req;
            var filter = from f in request
                         where !
        (
        from p in db.Projects
        select p.POST_ID


        ).Contains(f.Project_ID)
                    select f;
            project.Add(filter.ToList());

            var result = from y in db.Posts
                         where (
                                     from x in db.Responding_Posts

                                     select x.Post_ID
                                 ).Contains(y.ID)
                         select y;
            var userid = (int)Session["id"];
            var filterpost = from filterpos in result where filterpos.User_ID == userid && !(
                                     from x in db.Projects

                                     select x.POST_ID
                                 ).Contains(filterpos.ID)

                             select filterpos;

            var user = from u in db.Users where u.Job_Description == "PM" select u;
            project.Add(filterpost.ToList());
            project.Add( user.ToList());
            return View(project);
        }
 
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
           
            return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Exclude =  "ID,User_ID")] Post post)
        {
            if (ModelState.IsValid)
            {
                db.Entry(post).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index","Home");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Deleteposts(int postid)
        {
            var delete = db.Responding_Posts.FirstOrDefault(s => s.Post_ID == postid);
            db.Responding_Posts.Remove(delete);
           Post post = db.Posts.Find(postid);
         
            db.Posts.Remove(post);
         
            db.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult AcceptRequest(int postid , int ManagerID ,int requestid, Project project)
        {
            project.POST_ID = postid;
            project.Project_Manager_ID = ManagerID;
            project.stat = "On Progress";
            db.Projects.Add(project);
            Sending_Request req = db.Sending_Requests.Find(requestid);
            db.Sending_Requests.Remove(req);
            db.SaveChanges();



        return  RedirectToAction("Index","Customer");
        }
        [HttpPost]
        public ActionResult AssigntoPM (int postid , int managerid ,string stat ,Project project)
        {
            project.POST_ID = postid;
            project.Project_Manager_ID = managerid;
            project.stat = stat;
            db.Projects.Add(project);
            db.SaveChanges();

            return RedirectToAction("Index","Customer");
        }
        [HttpPost]
        public ActionResult DeleteRequest (int requestid , Sending_Request req)
        {

            req = db.Sending_Requests.Find(requestid);
            db.Sending_Requests.Remove(req);
            db.SaveChanges();

            return RedirectToAction("Index", "Customer");
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
