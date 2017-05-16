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