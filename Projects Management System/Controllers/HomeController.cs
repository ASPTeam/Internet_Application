using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Projects_Management_System.Models;
using System.Net.Mail;
using System.Net;

namespace Projects_Management_System.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult register()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult register([Bind(Exclude = "IsEmailVerified,ActivationCode")]  User user)
        {
            bool status = false;
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
                user.confirmpassword = crypto.Hash(user.password);
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
                }
                #endregion
                sendverficationlink(user.Email, user.ActivationCode.ToString());
                message = "Registeraton successfully done ! we have sent an activation link sent to your Email" + user.Email;
                status = true;
            }

            else
            {
                message = "Invalid Request";
            }
            ViewBag.Message = message;
            ViewBag.status = status;
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
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult login(login user,string Retunurl)
        {
            string message = "";
            using (Managment db = new Managment())
            {
                var v = db.Users.Where(a => a.Email == user.Email).FirstOrDefault();
                if (v!=null)
                {
                    if(string.Compare(crypto.Hash(user.password),v.password)==0)
                    {
                        int timeout = user.RememberMe ? 525600 : 10;
                        var tickt = new FormsAuthenticationTicket(user.Email, user.RememberMe, timeout);
                        string encrypting = FormsAuthentication.Encrypt(tickt);
                        var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypting);
                        cookie.Expires = DateTime.Now.AddMinutes(timeout);
                        Response.Cookies.Add(cookie);

                        if (Url.IsLocalUrl(Retunurl))
                        {
                            Redirect(Retunurl);

                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        message = "Email address or password is wrong";

                    }

                }
                else
                {
                    message = "Email address or password is wrong";
                }
            }
                ViewBag.Message = message;

            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("login", "Home");

        }
       
        


























        [NonAction]
        public Boolean ISemailExisit(string EmailID )
        {
            using (Managment db = new Managment())
            {
                return (db.Users.Any(e => e.Email == EmailID));
            }
        }
        [NonAction]
        public void sendverficationlink(string emailID , string activationcode)
        {
            var verify = "/user/verifyAccount" + activationcode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verify);
            var fromemail = new MailAddress("ramimohsen20@gmail.com", "Rami Mohsen");
            var toemail = new MailAddress(emailID);
            var password = "ramimohsen20";
            string subject = "your account has been successflly created ";
            string body = "<br/><br/> we are so happy to have you with us now you are able to post projects"+
                "and contribute with your project manager we wish we can help you make your work more easy"+
                "Press the following link to activate ypur account and get started !"+
                " < br />< br /> <a href='"+link+"' >"+link+"</a> ";
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