using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RestBite.Models;

namespace RestBite.Controllers
{
    public class GenresController : Controller
    {
        private Context db = new Context();

        // GET: Genres
        public ActionResult Index()
        {
            if (AuthorizationMiddleware.AdminAuthorized(Session))
            {
                return View(db.Genres.ToList());
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Genres/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Genre genre = db.Genres.Find(id);

            if (genre == null)
            {
                return HttpNotFound();
            }

            if (AuthorizationMiddleware.AdminAuthorized(Session))
            {
                return View(genre);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Genres/Create
        public ActionResult Create()
        {
            if (((Client)Session["Client"]) != null && ((Client)Session["Client"]).isAdmin)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Genres/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name")] Genre genre)
        {
            if (AuthorizationMiddleware.AdminAuthorized(Session))
            {
                if (ModelState.IsValid)
                {
                    // Checking if the genre already exist
                    var isExist = db.Genres.Where(x => x.Name == genre.Name).FirstOrDefault();

                    if (isExist == null)
                    {
                        db.Genres.Add(genre);
                        db.SaveChanges();

                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return View(genre);
                    }
                }

                return View(genre);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }   
        }

        // GET: Genres/Edit/5
        public ActionResult Edit(int? id)
        {
            if (AuthorizationMiddleware.AdminAuthorized(Session))
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                Genre genre = db.Genres.Find(id);

                if (genre == null)
                {
                    return HttpNotFound();
                }

                return View(genre);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Genres/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name")] Genre genre)
        {
            if (AuthorizationMiddleware.AdminAuthorized(Session))
            {
                if (ModelState.IsValid)
                {
                    db.Entry(genre).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(genre);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Genres/Delete/5
        public ActionResult Delete(int? id)
        {
            if (AuthorizationMiddleware.AdminAuthorized(Session))
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Genre genre = db.Genres.Find(id);

                if (genre == null)
                {
                    return HttpNotFound();
                }

                return View(genre);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Genres/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (AuthorizationMiddleware.AdminAuthorized(Session))
            {
                Genre genre = db.Genres.Find(id);

                // Getting all the posts of the genre
                List<Post> lstPosts = new List<Post>();
                lstPosts= db.Posts.Where(x => x.Genre.ID == id).ToList();

                // Removing all the posts of that genre
                foreach (Post curPost in lstPosts)
                {
                    Post post = db.Posts.Find(curPost.ID);

                    List<Comment> lstComments = new List<Comment>();
                    lstComments = db.Comments.Where(x => x.PostID == curPost.ID).ToList();
                    
                    foreach (Comment curComm in lstComments)
                    {
                        db.Comments.Remove(curComm);
                    }

                    db.Posts.Remove(post);
                }

                db.Genres.Remove(genre);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
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
