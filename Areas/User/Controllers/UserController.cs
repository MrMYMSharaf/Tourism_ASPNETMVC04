using MSTourism.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MSTourism.Areas.User.Controllers
{
    public class UserController : Controller
    {
        TourismDBEntities db = new TourismDBEntities();
        public ActionResult Index()
        {
            List<post_tbl> post = db.post_tbl.ToList();
            return View(post);
        }
        public ActionResult Details(int id)
        {
            var data = db.post_tbl.Where(x => x.Id == id).FirstOrDefault();
            return View(data);
        }
        public ActionResult PayNow()
        {
            List<post_tbl> post = db.post_tbl.ToList();
            return View(post);
        }
    }
}