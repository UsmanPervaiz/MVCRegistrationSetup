using LogInLogOut.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LogInLogOut.Extensions;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Web.Security;

namespace LogInLogOut.Controllers
{
    public class UserController : Controller
    {

        [Authorize]
        [HttpPost]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "User");
        }


        [HttpGet]
        public ActionResult Login(string RegistrationMessage, bool RegistrationStatus=false)
        {
            ViewBag.RegistrationMessage = RegistrationMessage;
            ViewBag.RegistrationStatus = RegistrationStatus;
            return View();
        }

        [HttpPost]
        public ActionResult Login(Login login, string ReturnUrl="")
        {
            bool isPasswordMatching = false;
            string messageForLogin = "Email and Password Combination Does Not Match Our Records!";

            using (UsersDbContext db = new UsersDbContext())
            {
                User findUserWithEmail = db.Users.Where(usr => usr.Email.ToLower() == login.EmailId.ToLower()).FirstOrDefault();

                if (findUserWithEmail != null)
                {
                    if (findUserWithEmail.IsEmailVerified)
                    {
                        byte[] userDBPasswordAndSalt = Convert.FromBase64String(findUserWithEmail.Password);
                        byte[] salt = new byte[16];
                        Array.Copy(userDBPasswordAndSalt, salt, 16);

                        string logInFormPassword = login.HashPassword(login.Password, salt);//Extension Method

                        if (logInFormPassword == findUserWithEmail.Password)
                        {
                            isPasswordMatching = true;
                            messageForLogin = "You Have Succefully Logged In!";

                            FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(login.EmailId, login.RemeberMe, 720);                     
                            string encrypted = FormsAuthentication.Encrypt(authTicket);

                            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                            cookie.Expires = DateTime.Now.AddDays(1);
                            cookie.HttpOnly = true;
                            Response.Cookies.Add(cookie);

                            if(Url.IsLocalUrl(ReturnUrl))
                            {
                                return Redirect(ReturnUrl);
                            }
                            else
                            {
                                return RedirectToAction("Index", "Home");
                            }

                        }
                        else
                        {
                            ViewBag.IsPasswordMatching = isPasswordMatching;
                            ViewBag.MessageForLogin = messageForLogin;

                            return View(login);
                        }
                    }
                    else
                    {
                        ViewBag.IsPasswordMatching = isPasswordMatching;
                        ViewBag.MessageForLogin = "Before You Can Login Please Verify Your Email Address By Clicking On the Activation Link Sent To You At The Time Of Registration.";

                        return View(login);
                    }
                }
                else
                {
                    ViewBag.IsPasswordMatching = isPasswordMatching;
                    ViewBag.MessageForLogin = messageForLogin;
                    return View(login);
                }
            }
        }

        //Register User
        [HttpGet]
        public ActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Registration([Bind(Exclude = "Salt, IsEmailVerified, ActivationCode")] User user)
        {
            bool registrationStatus = false;
            string registrationMessage = "";

            if (ModelState.IsValid)
            {

                if (CheckIfEmailExists(user.Email))
                {
                    ModelState.AddModelError("Email", "Email Already Exists.");
                    return View(user);
                }

                byte[] salt = new byte[16];
                new RNGCryptoServiceProvider().GetBytes(salt);

                user.Password = user.HashPassword(user.Password, salt);
                user.ConfirmPassword = user.HashPassword(user.ConfirmPassword, salt);
                user.IsEmailVerified = true;

                Session["UserToSaveToDB"] = user;
                return RedirectToAction("SiteAdminEmailPassword");
            }
            else
            {
                registrationMessage = "Please Correct The Errors Below To Continue";
            }

            ViewBag.RegistrationMessage = registrationMessage;
            ViewBag.RegistrationStatus = registrationStatus;
            return View(user);
        }

        [HttpGet]
        public ActionResult SiteAdminEmailPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SiteAdminEmailPassword(string AdminPassword, string AdminEmailId)
        {
            string successfullRegistrationMessage = null;
            bool registrationWasSuccessfull = false;
            string adminEmailPasswordErrorMessage = "Error: Admin Please Confirm Your Email and Password";

            User user = Session["UserToSaveToDB"] as User;
            user.ActivationCode = Guid.NewGuid();

            using (UsersDbContext db = new UsersDbContext())
            {              
                db.Users.Add(user);
                try
                {
                    db.SaveChanges();
                    successfullRegistrationMessage = $"Registration successfully done. Account Activation link has been sent to {user.Email}";
                    registrationWasSuccessfull = true;
                }
                catch(System.Data.Entity.Validation.DbEntityValidationException dbException)
                {
                    ViewBag.DataBaseValidationExceptions = dbException;
                    ViewBag.RegistrationMessage = "Please Correct The Errors Below To Continue";
                    return View("Registration", Session["UserToSaveToDB"] as User);
                }
            
            }
            try
            {
                SendVerificationEmail(user.Email, AdminEmailId, AdminPassword, user.ActivationCode);
            }
            catch(System.Net.Mail.SmtpException)
            {
                ViewBag.AdminEmailPasswordErrorMessage = adminEmailPasswordErrorMessage;
                return View(new { });
            }
            
            return RedirectToAction("Login", "User", new { RegistrationMessage = successfullRegistrationMessage, RegistrationStatus = registrationWasSuccessfull });
        }

        [HttpGet]
        public ActionResult VerifyAccount(string Id)
        {
            bool isAccountVerified = false;
            using (UsersDbContext db = new UsersDbContext())
            {
                db.Configuration.ValidateOnSaveEnabled = false;

                User userWithActivationCode = db.Users.Where(user => user.ActivationCode == new Guid(Id)).FirstOrDefault();
                if (userWithActivationCode != null)
                {
                    userWithActivationCode.IsEmailVerified = true;
                    try
                    {
                        db.SaveChanges();
                        isAccountVerified = true;
                    }
                    catch (System.Data.Entity.Validation.DbEntityValidationException dbException)
                    {
                        ViewBag.DataBaseValidationExceptionsOnVerification = dbException;
                        ViewBag.AccountVerificationMessage = "We apologize, something went wrong and your account could not be verified.";
                        return View();
                    }
                }
                else
                {
                    ViewBag.AccountVerificationMessage = "Account could not be verified with the activation code provided.";
                }
            }
            ViewBag.IsAccountVerified = isAccountVerified;
            return View();
        }

        [NonAction]
        public bool CheckIfEmailExists(string email)
        {
            using (UsersDbContext db = new UsersDbContext())
            {
                bool exists = db.Users.Any(usr => usr.Email.ToLower() == email.ToLower());
                return exists;
            }
        }

        [NonAction]
        public void SendVerificationEmail(string userEmail, string AdminEmail, string AdminPassword, Guid activationCode)
        {
            var verifyUrl = "User/VerifyAccount/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, Url.Content("~")+verifyUrl);

            MailMessage mail = new MailMessage()
            {
                From = new MailAddress(AdminEmail, "GoldenGate"),
                Subject = "Your account is successfully created!",
                Body = $"<br/><br/>We are excited to tell you that your GoldenGate account is successfully created. Please click on the below link to verify your account: <br/><br/><a href='{link}'>{link}</a>",
                IsBodyHtml = true
            };
            mail.To.Add(new MailAddress(userEmail));

            SmtpClient smtp = new SmtpClient("smtp.gmail.com");
            smtp.Credentials = new System.Net.NetworkCredential(AdminEmail, AdminPassword);
            smtp.Port = 587;
            smtp.EnableSsl = true;

            smtp.Send(mail);
        }

    }
}