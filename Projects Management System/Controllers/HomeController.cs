using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Projects_Management_System.Models;
using System.Net.Mail;
using System.Net;
using System.Security.Authentication;
using Projects_Management_System.MyRoleProvider;

namespace Projects_Management_System.Controllers
{
    public class HomeController : Controller
    {
        [Authorize(Roles ="Customer,Admin,PM,TL,JE")]
        public ActionResult Index()
        {
            List<object> mylsit = new List<object>();

            Managment db = new Managment();
            

                var result = from y in db.Posts
                             where (
                                         from x in db.Responding_Posts
                                        
                                         select x.Post_ID
                                     ).Contains(y.ID)
                             select y;

            var z = from f in result  where !
                    (
                    from p in db.Projects 
                    select p.POST_ID


                    ).Contains(f.ID)
                    select f;

                mylsit.Add(z.ToList());
            mylsit.Add(db.Sending_Requests.ToList());
            
            

                return View(mylsit);
        }

      

        [HttpGet]
        [AllowAnonymous]
        public ActionResult register()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult register([Bind(Exclude = "IsEmailVerified,ActivationCode")]  User user)
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
                    message = "Registeraton successfully done !we have sent an activation link to your Email " + user.Email;
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

        [HttpGet]
        public ActionResult verifyAccount(string id)
        {
            bool status = false;
            using (Managment db = new Managment())
            {
                db.Configuration.ValidateOnSaveEnabled = false;
                var v = db.Users.Where(a => a.ActivationCode == new Guid(id)).FirstOrDefault();
                if (v!=null)
                {
                    v.IsEmailVerified = true;
                    db.SaveChanges();
                    status = true;
                }
                else
                {
                    ViewBag.Message = "Invalid Request";
                }
            }
            ViewBag.status = status;
                return View();
        }

        [HttpGet]
        public ActionResult login()
        {
            if (Session["First"] == null)
            {
                return View();
            }
            else
                return RedirectToAction("Index","Home");
           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult login(login user, string Retunurl)
        {

            using (Managment db = new Managment())
            {
                var v = db.Users.Where(a => a.Email == user.Email).FirstOrDefault();

                if (v != null)
                {
                    if (string.Compare(crypto.Hash(user.password), v.password) == 0)
                    {
                        int timeout = user.RememberMe ? 525600 : 10;
                        var tickt = new FormsAuthenticationTicket(user.Email, user.RememberMe, timeout);
                        string encrypting = FormsAuthentication.Encrypt(tickt);
                        var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypting);
                        cookie.Expires = DateTime.Now.AddMinutes(timeout);
                        Response.Cookies.Add(cookie);
                        Session["First"] = v.First_Name;
                        Session["Last"] = v.Last_Name;
                        Session["Mobile"] = v.Mobile;
                        Session["jop"] = v.Job_Description;
                        Session["photo"] = v.Photo;
                        Session["id"] = v.ID;
                        Session["Role"] = v.Type;
                        Session["Email"] = v.Email;
                       

                        if (Url.IsLocalUrl(Retunurl))
                        {
                            Redirect(Retunurl);

                        }
                        else
                        {


                            if (v.Type == "Admin")
                                return RedirectToAction("Index", "Admin");
                            else if (v.Type == "Customer")
                                return RedirectToAction("Index", "Customer");
                            else if (v.Type == "PM")
                                return RedirectToAction("Index", "PM");
                            else if (v.Type == "TL")
                                return RedirectToAction("Index", "TL");
                            else if (v.Type == "JE")
                                return RedirectToAction("Index", "JE");



                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Email address or password is wrong");

                    }

                }
                else
                {
                    ModelState.AddModelError("", "Email address or password is wrong");
                }
            }


            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("login", "Home");

        }

        [HttpGet]
        public ActionResult PostNewProject()
        {

            return View();
        }

        [HttpPost]
       // [ValidateAntiForgeryToken]
        public ActionResult PostNewProject(Post post )
        {
            var role = Session["Role"];
            var id = Session["id"];
            post.User_ID = (int)id;
            if(ModelState.IsValid)
            {
                if((string)role !="Customer")
                {
                    ModelState.AddModelError("Not Allowd", "You are not allowed to post a project");
                    return View();
                }
                else
                    using (Managment db = new Managment())
                    {
                        db.Posts.Add(post);
                        db.SaveChanges();
                    }
            
                
            }
            return RedirectToAction("Index", "Home");


        }
        [NonAction]
        public bool  ISemailExisit(string EmailID )
        {
            using (Managment db = new Managment())
            {
                var v = db.Users.Where(a => a.Email == EmailID).FirstOrDefault();
                return v !=null;
            }
        }
        [NonAction]
        public void sendverficationlink(string emailID , string activationcode)
        {
            var verify = "/Home/verifyAccount/" + activationcode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verify);
            var fromemail = new MailAddress("ramimohsen20@gmail.com", "Rami Mohsen");
            var toemail = new MailAddress(emailID);
            var password = "ramimohsen20";
            string subject = "your account has been successflly created ";
            string body = "<br/><br/> <strong> we are so happy to have you with us now you are able to post projects "+
                "and contribute with your project manager we wish we can help you make your work more easy "+
                " <br/>Press the following link to activate your account and get started  ! </strong> "+
                " <br/><br/> <a href='"+link+"' >"+link+"</a> ";
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