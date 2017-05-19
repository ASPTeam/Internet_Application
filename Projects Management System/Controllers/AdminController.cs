using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Projects_Management_System.Models;
using System.Net.Mail;

namespace Projects_Management_System.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AdminController : Controller
    {
        private Managment db = new Managment();

        public ActionResult Index()
        {
            List<object> mymodel = new List<object>();
            mymodel.Add(db.Users.ToList());
         var result=   from y in db.Posts
            where !(
                        from x in db.Responding_Posts
                       
                        select x.Post_ID
                    ).Contains(y.ID)
            select y;
            mymodel.Add(result.ToList());

          
            return View(mymodel);
        }
       
     
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "IsEmailVerified,ActivationCode")] User user)
        {

            
            bool Status = false;
            string message = "";

            if (ModelState.IsValid)
            {

                #region // Email validation 
                var isexsist = ISemailExisit(user.Email);
                if (isexsist)
                {
                    ModelState.AddModelError("Exist", "Email Already Exsit");
                    return View(user);
                }

                #endregion


                #region // Activation code generation 
                user.ActivationCode = Guid.NewGuid();
                #endregion

                #region // password Hashing
                user.password = crypto.Hash(user.password);
                user.confirmpassword = crypto.Hash(user.confirmpassword);
                #endregion

                #region // Convert a photo to binary 
                byte[] data = new byte[user.File.ContentLength];
                user.File.InputStream.Read(data, 0, user.File.ContentLength);
                user.Photo = data;
                #endregion

                user.IsEmailVerified = false;

                #region // save to the database 
                using (Managment db = new Managment())
                {

                    db.Users.Add(user);
                    db.SaveChanges();
                    sendverficationlink(user.Email, user.ActivationCode.ToString());
                    message = "Creatation  successfully done ! we have sent an activation link  to That Email " + user.Email;
                    Status = true;
                }
                #endregion

            }

            else
            {
                message = "Invalid Request";
            }
            ViewBag.Message = message;
            ViewBag.Status = Status;
            return View(user);
        }

        // GET: Admin/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);            
                db.Users.Remove(user);

            db.SaveChanges();
             

                return RedirectToAction("Index");
        }


        [HttpPost]
       public ActionResult Approveposts(int postid ,Responding_Post respond )
        {
            var adminid = Session["id"];
            respond.Admin_ID = (int)adminid;
            respond.Post_ID = postid;
            respond.post_stat = true;
            if(ModelState.IsValid)
            {
                db.Responding_Posts.Add(respond);
                db.SaveChanges();
            }

            return RedirectToAction("Index", "Admin");

        }


        [HttpPost]
       public ActionResult Deleteposts(int postid)
        {
            Post post = db.Posts.Find(postid);
            db.Posts.Remove(post);
            db.SaveChanges();

            return RedirectToAction("Index", "Admin");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        [NonAction]
        public bool ISemailExisit(string EmailID)
        {
            using (Managment db = new Managment())
            {
                var v = db.Users.Where(a => a.Email == EmailID).FirstOrDefault();
                return v != null;
            }
        }
        [NonAction]
        public void sendverficationlink(string emailID, string activationcode)
        {
            var verify = "/Home/verifyAccount/" + activationcode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verify);
            var fromemail = new MailAddress("ramimohsen20@gmail.com", "Rami Mohsen");
            var toemail = new MailAddress(emailID);
            var password = "ramimohsen20";
            string subject = "your account has been successflly created ";
            string body = "<br/><br/> <strong> we are so happy to have you with us now you are able to post projects " +
                "and contribute with your project manager we wish we can help you make your work more easy " +
                " <br/>Press the following link to activate your account and get started  ! </strong> " +
                " <br/><br/> <a href='" + link + "' >" + link + "</a> ";
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromemail.Address, password)

            };
            using (var message = new MailMessage(fromemail, toemail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true

            })
                smtp.Send(message);
        }
    }
}
