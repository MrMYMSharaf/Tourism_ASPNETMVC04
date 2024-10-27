using MSTourism.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;

using System.Text;

namespace MSTourism.Controllers
{
    public class AuthController : Controller
    {
        // GET: Auth
        TourismDBEntities db = new TourismDBEntities();
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(user_tbl obj)
        {
            if (ModelState.IsValid)
            {
                
            }
            return View(obj);
        }
        public ActionResult signup()
        {
            return View();
        }

        [HttpPost]
        public ActionResult signup(user_tbl obj)
        {
            if (ModelState.IsValid)
            {
                obj.create_time = DateTime.Now;
                obj.last_update = DateTime.Now;
               
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
    }

}