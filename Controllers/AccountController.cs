using System;
using System.Linq;
using System.Web.Mvc;
using Library.Models;
using System.Security.Cryptography;
using System.Text;
using System.Web.Security;

namespace Library.Controllers
{
    public class AccountController : Controller
    {
        private Learning_1Entities db = new Learning_1Entities();

        // GET: Account/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserLoginViewModel model,string returnUrl)
        {
            if (ModelState.IsValid)
            {
                string hashedPassword = HashPassword(model.Password);
                var user = db.Users.FirstOrDefault(u => u.Email == model.Email && u.PasswordHash == hashedPassword);

                if (user != null)
                {
                    // Successful login
                    FormsAuthentication.SetAuthCookie(model.Email, false);  //fasle will prevent the persistent cookies
                    Session["UserId"] = user.UserId;
                    // Check if returnUrl is null or empty
                    if (string.IsNullOrEmpty(returnUrl))
                    {
                        returnUrl = Url.Action("Index", "Home"); // Default redirection path
                    }

                    return Redirect(returnUrl);
                }
                else
                {
                    ViewBag.Error = "Invalid credentials or user does not exist.";
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        //POST: Account/Logout
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session["UserId"] = null; // Corrected the assignment operator
            return RedirectToAction("Index", "Home"); // Using RedirectToAction for better practice
        }


        // GET: Account/Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(UserRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    PasswordHash = HashPassword(model.Password),
                    CreatedDate = DateTime.Now
                };

                db.Users.Add(user);
                db.SaveChanges();

                ViewBag.Message = "User registered successfully!";
                return RedirectToAction("Login");
            }
            return View(model);
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}