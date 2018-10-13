
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
    public class PostsController : Controller
    {
        private Context db = new Context();

        // GET: Posts
        public ActionResult Index()
        {
            var posts = db.Posts.Include(p => p.Client).Include(p => p.Genre);
            return View(posts.ToList());
        }

        // GET: Posts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // GET: Posts/DetailsByTitle?title=Hardwierd
        public ActionResult DetailsByTitle(string title)
        {
            if (title == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Where(x=> x.Title == title).FirstOrDefault();

            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // GET: Posts/Create
        public ActionResult Create()
        {
            if (AuthorizationMiddleware.Authorized(Session))
            {
                ViewBag.ClientID = new SelectList(db.Clients, "ID", "ClientName");
                ViewBag.GenreID = new SelectList(db.Genres, "ID", "Name");
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Posts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,clientId,GenreID,Title,Content")] Post post)
        {
            if (post.Content != null && post.Title != null && post.GenreID != 0)
            {
                if (AuthorizationMiddleware.Authorized(Session))
                {
                    if (ModelState.IsValid)
                    {
                        post.CreationDate = DateTime.Now;
                        db.Posts.Add(post);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }

                    ViewBag.ClientID = new SelectList(db.Clients, "ID", "ClientName", post.ClientID);
                    ViewBag.GenreID = new SelectList(db.Genres, "ID", "Name", post.GenreID);
                    return View(post);
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
            
            return RedirectToAction("Index", "Home"); 
        }

        // GET: Posts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (AuthorizationMiddleware.Authorized(Session))
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Post post = db.Posts.Find(id);
                if (post == null)
                {
                    return HttpNotFound();
                }
                ViewBag.ClientID = new SelectList(db.Clients, "ID", "ClientName", post.ClientID);
                ViewBag.GenreID = new SelectList(db.Genres, "ID", "Name", post.GenreID);
                return View(post);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,clientId,GenreID,Title,Content")] Post post)
        {
            if (post.Content != null && post.Title != null && post.Genre.Name != null)
            {
                if (AuthorizationMiddleware.Authorized(Session))
                {
                    if (ModelState.IsValid)
                    {
                        post.CreationDate = DateTime.Now;
                        db.Entry(post).State = EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    ViewBag.ClientID = new SelectList(db.Clients, "ID", "ClientName", post.ClientID);
                    ViewBag.GenreID = new SelectList(db.Genres, "ID", "Name", post.GenreID);
                    return View(post);
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index", "Home");
        }

        // GET: Posts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (AuthorizationMiddleware.Authorized(Session))
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Post post = db.Posts.Find(id);
                if (post == null)
                {
                    return HttpNotFound();
                }
                return View(post);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (AuthorizationMiddleware.Authorized(Session))
            {

                Post post = db.Posts.Find(id);

                // Getting all the comments of the post
                List<Comment> lstRemove = new List<Comment>();
                lstRemove = db.Comments.Where(x => x.Post.ID == id).ToList();

                // Removing all the comments of that post
                foreach (Comment cur in lstRemove)
                {
                    Comment comment = db.Comments.Find(cur.ID);
                    db.Comments.Remove(comment);
                }

                db.Posts.Remove(post);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult PostComment(int clientId, int postId, string content)
        {
            if (AuthorizationMiddleware.Authorized(Session))
            {
                Comment comment = new Comment
                {
                    Content = content,
                    ClientID = clientId,
                    PostID = postId,
                    CreationDate = DateTime.Now
                };

                db.Comments.Add(comment);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
            
        }

        // GET: Client/Stats
        public ActionResult Stats()
        {
           var query = db.Posts.Select(x => new PostCommentViewModel {Title = x.Title, NumberOfComment = x.Comments.Count}).ToList();
           return View(query);
        }

        public ActionResult StatsJson()
        {
            var query = db.Posts.Select(x => new PostCommentViewModel { Title = x.Title, NumberOfComment = x.Comments.Count }).ToList();
            var data = Json(query, JsonRequestBehavior.AllowGet);
            return data;
        }

        [HttpGet]
        public ActionResult Search(string content, string title, DateTime? date)
        {
            var queryPosts = new List<Post>();

            foreach (var post in db.Posts)
            {
                var contentNeeded = content != null && content.Length > 0;
                var titleNeeded = title != null && title.Length > 0;
                var dateNeeded = date != null;

                if ((contentNeeded ? post.Content != null && post.Content.Contains(content) : true) &&
                    (titleNeeded ? post.Title != null && post.Title.Contains(title) : true) &&
                    (dateNeeded ?  post.CreationDate.ToString("MM/dd/yyyy").Equals(date.Value.ToString("MM/dd/yyyy")) : true))
                {
                    queryPosts.Add(post);
                }
            }

            return View(queryPosts.OrderByDescending(x => x.CreationDate));
        }

        public ActionResult RecomendedPosts()
        {
            // join select for users and their posts
            var query = (from u in db.Clients
                         join post in db.Posts on u.ID equals post.ClientID
                         select new userPostsViewModel
                         {
                             UserName = u.ClientName,
                             FirstName = u.FirstName,
                             LastName = u.LastName,
                             Title = post.Title,
                             ID = u.ID,
                             GenreID = post.GenreID
                         });

            // TODO: Nadav 
            var BestGenrePost = query.ToList().GroupBy(x => x.GenreID).Select(n => new
            {
                PostGenre = n.Key,
                PostCount = n.Count()
            }
            )
            .OrderByDescending(n => n.PostCount).First();


            var posts = db.Posts.Include(p => p.Client).Include(p => p.Genre).Where(p => p.GenreID == BestGenrePost.PostGenre);

            return View(posts);
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
