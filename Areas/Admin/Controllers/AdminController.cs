using MSTourism.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MSTourism.Areas.Admin.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin/Admin
        TourismDBEntities db = new TourismDBEntities();
        // GET: Admin
        public ActionResult Index()
        {
            List<user_tbl> users = db.user_tbl.ToList();
            return View(users);
        }

        public ActionResult Delete(int id)
        {
            var data = db.user_tbl.Where(x => x.Id == id).FirstOrDefault();
            db.user_tbl.Remove(data);
            db.SaveChanges();
            ViewBag.Messsage = "Record Delete Successfully";
            return RedirectToAction("index");
        }
        public ActionResult AddUser()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddUser(user_tbl obj)
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

                return RedirectToAction("Index", "Admin");
            }
            return View(obj);
        }
        [HttpGet]
        public ActionResult Post()
        {
            var listPost = db.post_tbl.ToList();
            return View(listPost);
        }

        public ActionResult CreatePost()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreatePost(HttpPostedFileBase file, post_tbl obj)
        {
            if (file != null && file.ContentLength > 0)
            {
                string filename = Path.GetFileName(file.FileName);
                string _filename = DateTime.Now.ToString("yymmssfff") + filename;
                string extension = Path.GetExtension(file.FileName);
                string path = Path.Combine(Server.MapPath("~/images/Uploads/"), _filename);

                // Update the image path in the object for the database
                obj.image = "~/images/Uploads/" + _filename;

                // Validate file extension and size
                if (extension.ToLower() == ".jpg" || extension.ToLower() == ".jpeg" || extension.ToLower() == ".png")
                {
                    if (file.ContentLength <= 1000000) // 1 MB limit
                    {
                        try
                        {
                            // Ensure the directory exists
                            string directoryPath = Server.MapPath("~/images/Uploads/");
                            if (!Directory.Exists(directoryPath))
                            {
                                Directory.CreateDirectory(directoryPath);
                            }

                            // Save the file
                            file.SaveAs(path);

                            // Save the record in the database
                            db.post_tbl.Add(obj);
                            if (db.SaveChanges() > 0)
                            {
                                ViewBag.msg = "Record Added Successfully";
                                ModelState.Clear();
                            }
                        }
                        catch (Exception ex)
                        {
                            ViewBag.msg = "An error occurred: " + ex.Message;

                            // Check for entity validation errors
                            if (ex is DbEntityValidationException dbEx)
                            {
                                foreach (var validationError in dbEx.EntityValidationErrors)
                                {
                                    foreach (var error in validationError.ValidationErrors)
                                    {
                                        Console.WriteLine($"Property: {error.PropertyName} Error: {error.ErrorMessage}");
                                        ViewBag.msg += $"<br/>Property: {error.PropertyName} Error: {error.ErrorMessage}";
                                    }
                                }
                            }
                        }

                    }
                    else
                    {
                        ViewBag.msg = "Size is not valid. Maximum size is 1 MB.";
                    }
                }
                else
                {
                    ViewBag.msg = "File format not supported. Please upload .jpg, .jpeg, or .png files.";
                }
            }
            else
            {
                ViewBag.msg = "Please upload an image.";
            }

            return View();
        }

        public ActionResult EditPost(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var post_tbl = db.post_tbl.Find(id);
            if (post_tbl == null)
            {
                return HttpNotFound(); // Post not found
            }

            Session["imgPath"] = post_tbl.image; // Store the current image path in session

            return View(post_tbl); // Pass the model to the view
        }

        [HttpPost]
        public ActionResult EditPost(HttpPostedFileBase file, post_tbl emp)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    string filename = Path.GetFileName(file.FileName);
                    string _filename = DateTime.Now.ToString("yymmssfff") + filename;
                    string extension = Path.GetExtension(file.FileName);
                    string path = Path.Combine(Server.MapPath("~/images/Uploads/"), _filename);
                    emp.image = "~/images/Uploads/" + _filename; // Update the image path

                    // Validate file extension and size
                    if ((extension.ToLower() == ".jpg" || extension.ToLower() == ".jpeg" || extension.ToLower() == ".png"))
                    {
                        if (file.ContentLength <= 1000000)
                        {
                            // Mark entity as modified
                            db.Entry(emp).State = EntityState.Modified;
                            string oldImagePath = Request.MapPath(Session["imgPath"].ToString());
                            if (db.SaveChanges() > 0)
                            {
                                file.SaveAs(path); // Save the new image

                                if (System.IO.File.Exists(oldImagePath))
                                {
                                    System.IO.File.Delete(oldImagePath); // Delete the old image
                                }

                                TempData["msg"] = "Record updated successfully.";
                            }
                        }
                        else
                        {
                            ViewBag.msg = "Invalid file size or format.";
                        }
                    }
                }
                else
                {
                    // If no file uploaded, retain the existing image path
                    emp.image = Session["imgPath"].ToString();
                    db.Entry(emp).State = EntityState.Modified;

                    if (db.SaveChanges() > 0)
                    {
                        TempData["msg"] = "Data updated successfully.";
                        return RedirectToAction("Post");
                    }
                }
            }

            return View(emp); // Return the model to the view if ModelState is invalid
        }

        public ActionResult DeletePost(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var post_tbl = db.post_tbl.Find(id);
            if (post_tbl == null)
            {
                return HttpNotFound(); // Post not found
            }

            // Check if the image path exists
            string currentimg = post_tbl.image != null ? Request.MapPath(post_tbl.image) : null;
            db.Entry(post_tbl).State = EntityState.Deleted;

            if (db.SaveChanges() > 0)
            {
                // Delete the file if it exists
                if (!string.IsNullOrEmpty(currentimg) && System.IO.File.Exists(currentimg))
                {
                    System.IO.File.Delete(currentimg);
                }

                TempData["msg"] = "Data deleted!";
                return RedirectToAction("Post"); // Ensure "Post" matches your actual view/action name
            }

            TempData["msg"] = "An error occurred while deleting the data.";
            return RedirectToAction("Post"); // Redirect to the post list even if deletion fails
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