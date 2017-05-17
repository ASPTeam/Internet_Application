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
            return View(project);
        }

        // GET: Customer/Details/5
        public ActionResult Details(int? id)
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
            return View(project);
        }

        // GET: Customer/Create
        public ActionResult Create()
        {
            ViewBag.POST_ID = new SelectList(db.Posts, "ID", "post_Description");
            ViewBag.Project_Manager_ID = new SelectList(db.Users, "ID", "User_Name");
            return View();
        }

        // POST: Customer/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,POST_ID,Project_Manager_ID,stat")] Project project)
        {
            if (ModelState.IsValid)
            {
                db.Projects.Add(project);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.POST_ID = new SelectList(db.Posts, "ID", "post_Description", project.POST_ID);
            ViewBag.Project_Manager_ID = new SelectList(db.Users, "ID", "User_Name", project.Project_Manager_ID);
            return View(project);
        }

        // GET: Customer/Edit/5
        public ActionResult Edit(int? id)
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

        // POST: Customer/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,POST_ID,Project_Manager_ID,stat")] Project project)
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

        // GET: Customer/Delete/5
        public ActionResult Delete(int? id)
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
            return View(project);
        }

        // POST: Customer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = db.Projects.Find(id);
            db.Projects.Remove(project);
            db.SaveChanges();
            return RedirectToAction("Index");
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
