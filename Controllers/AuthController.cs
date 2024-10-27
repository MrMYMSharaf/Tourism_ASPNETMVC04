using MSTourism.Models;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using System.Web.Security;

namespace MSTourism.Controllers
{
    public class AuthController : Controller
    {
        TourismDBEntities db = new TourismDBEntities();

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(user_tbl obj)
        {
            // Hash the input password to compare with the stored hashed password
            var hashedPassword = HashPassword(obj.Password);

            // Find user with matching name and hashed password
            var user = db.user_tbl.FirstOrDefault(u => u.Name == obj.Name && u.Password == hashedPassword);

            if (user != null)
            {
                // Successful login
                Session["UserID"] = user.Id;
                Session["UserRole"] = user.user_type;
                if(user.user_type == "Admin") 
                {
                    return RedirectToAction("Index", "Admin", new { Area = "Admin" }); // or any other view
                }
                else 
                {
                    return RedirectToAction("Index", "User", new { Area = "User" });
                }
                
            } else
            {
                ViewBag.Error = "Invalid username or password";
                return RedirectToAction("Index", "Auth");
            }
        }

        public ActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Signup(user_tbl obj)
        {
            if (ModelState.IsValid)
            {
                obj.create_time = DateTime.Now;
                obj.last_update = DateTime.Now;

                // Hash the password before storing it
                obj.Password = HashPassword(obj.Password);

                if (string.IsNullOrEmpty(obj.user_type))
                {
                    obj.user_type = "user";
                }

                db.user_tbl.Add(obj);
                db.SaveChanges();

                return RedirectToAction("Index", "Auth");
            }
            return View(obj);
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Auth");
        }


        // Hashing function using SHA-256
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
