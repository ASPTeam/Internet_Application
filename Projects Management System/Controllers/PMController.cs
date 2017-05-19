using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Projects_Management_System.Models;
using System.Data.Entity;
using System.Net;

namespace Projects_Management_System.Controllers
{
   
    public class PMController : Controller
    {
        private Managment db = new Managment();

        [Authorize(Roles = "PM")]
        public ActionResult Index()
        {
            List<object> listprojects = new List<object>();
            Managment db = new Managment();
            listprojects.Add(db.Projects.ToList());
            var user = from u in db.Users where u.Job_Description == "TL" || u.Job_Description == "JE" select u;
            listprojects.Add(user.ToList());
                return View(listprojects);
        }

        [HttpPost]
        public ActionResult Leave(int postid)
        {
            using (Managment db = new Managment())
            {
                Project project = db.Projects.Find(postid);
                db.Projects.Remove(project);
                db.SaveChanges();

            }
               


            return RedirectToAction("Index", "PM");
        }
 
        public ActionResult SetStatus(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            
            ViewBag.POST_ID = new SelectList(db.Posts, "ID", "post_Description", project.POST_ID);
            ViewBag.Project_Manager_ID = new SelectList(db.Users, "ID", "User_Name", project.Project_Manager_ID);
            return View(project);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetStatus(Project project)
        {
            if (ModelState.IsValid)
            {
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.POST_ID = new SelectList(db.Posts, "ID", "post_Description", project.POST_ID);
            ViewBag.Project_Manager_ID = new SelectList(db.Users, "ID", "User_Name", project.Project_Manager_ID);
            return View(project);
        }
        [HttpGet]
        public ActionResult Comment()
        {
            return View();

        }
        [HttpPost]
        public ActionResult Comment(int postid , Comment comment  )
        {
            comment.Project_Manager_ID = (int)Session["id"];
            comment.Post_ID = postid;
            if (ModelState.IsValid)
            {
                db.Comments.Add(comment);
                db.SaveChanges();
            }

            return RedirectToAction("Index", "PM");

        }
        [HttpGet]
        public ActionResult Report()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Report(int userid, Make_Report r)
        {
            r.Customer_ID = userid;
            r.Project_Manager_ID = (int)Session["id"];

            if (ModelState.IsValid)
            { 
                db.Make_Reports.Add(r);
                db.SaveChanges();
               

            }
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public ActionResult SendingRequestToCustomer (int userid , int projectid , Sending_Request request)
        {
            request.Sender_ID =(int)Session["id"];
            request.Reciever_ID = userid;
            request.Project_ID = projectid;
            db.Sending_Requests.Add(request);
            db.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult SendRequest(int senderid , int postid, Sending_Request send)
        {
            
           var v = Request["mail"];
            var mail = Session["Email"];
            if (v != (string)mail)
            {
                var f = db.Users.Where(e => e.Email == v).FirstOrDefault();
                send.Sender_ID = senderid;
                send.Project_ID = postid;
                send.Reciever_ID = f.ID;
                db.Sending_Requests.Add(send);
                db.SaveChanges();
            }

            
            return RedirectToAction("Index", "PM");
          
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