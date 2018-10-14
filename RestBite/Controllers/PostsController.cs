
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RestBite.Models;
using Accord.MachineLearning.Bayes;


namespace RestBite.Controllers
{
    public class PostsController : Controller
    {
        private Context db = new Context();

        // GET: Posts
        public ActionResult Index()
        {
            var posts = db.Posts.Include(p => p.Client).Include(p => p.Genre);
            var x = posts.ToList();
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

                if (content != string.Empty)
                {
                    db.Comments.Add(comment);
                    db.SaveChanges();
                }

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

        class DatasetRecommndedPosts
        {
            public int clientID;
            public int generID;
        }

        public ActionResult RecomendedPosts()
        {
            // Query for DatasetRecommndedPosts
            var query = (from u in db.Clients
                         join post in db.Posts on u.ID equals post.ClientID
                         select new DatasetRecommndedPosts
                         { clientID = u.ID, generID = post.GenreID});

            // Create dataset for learning
            DatasetRecommndedPosts[] datasetRecommndedPosts = query.ToArray();

            if (datasetRecommndedPosts.Length == 0)
            {
                return (View(new List<Post>()));
            }

            int numOfGener = db.Genres.ToList().Count;
            int numOfClients = db.Clients.ToList().Count;

            int[][] input = new int[datasetRecommndedPosts.Length][]; /* ClientID */
            List<int> output = new List<int>();                       /* GenderID */

            // Fill dataset
            for (int i = 0; i < datasetRecommndedPosts.Length; i++)
            {
                input[i] = new int[] { datasetRecommndedPosts[i].clientID};
                output.Add(datasetRecommndedPosts[i].generID);
            }

            var bayes = new NaiveBayes(numOfGener, new[] { numOfClients });
            var learning = new NaiveBayesLearning()
            {
                Model = bayes
            };

            // Map for Consecutive numbers
            Dictionary<int, int> inputMapper = new Dictionary<int, int>();
            Dictionary<int, int> outputMapper = new Dictionary<int, int>();

            int key = 0;
            for (int index = 0; index < input.Length; index++)
            {
                if (!inputMapper.ContainsKey(input[index][0]))
                {
                    inputMapper.Add(input[index][0], key);
                    input[index][0] = key;
                    key++;
                }
                else
                {
                    input[index][0] = inputMapper[input[index][0]];
                }
            }

            key = 0;
            for (int index = 0; index < output.Count; index++)
            {
                if (!outputMapper.ContainsKey(output[index]))
                {
                    outputMapper.Add(output[index], key);
                    output[index] = key;
                    key++;
                }
                else
                {
                    output[index] = outputMapper[output[index]];
                }
            }


            learning.Learn(input, output.ToArray());

            int currentClientID = ((Client)Session["Client"]).ID;
            if (!inputMapper.ContainsKey(currentClientID))
            {
                return View(new List<Post>());
            }
            int answerGenderID = bayes.Decide(new int[] { inputMapper[currentClientID] });
            int mapAnswerGenderID = 0;
            foreach (var n in outputMapper)
            {
                if (n.Value == answerGenderID)
                {
                    mapAnswerGenderID = n.Key;
                }
            }
            var posts = db.Posts.Include(p => p.Client).Include(p => p.Genre).Where(p => p.GenreID == (mapAnswerGenderID));
            return (View(posts));

            /*
            int[][] inputs =
            {
                //      input     output
                new [] { 0, 1 }, //  0 
                new [] { 0, 2 }, //  0
                new [] { 0, 1 }, //  0
                new [] { 1, 2 }, //  1
                new [] { 0, 2 }, //  1
                new [] { 0, 2 }, //  1
                new [] { 1, 1 }, //  2
                new [] { 0, 1 }, //  2
                new [] { 1, 1 }, //  2
            };

            int


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
            var groupPosts = query.GroupBy(x => x.GenreID).ToList();

            if (groupPosts.Count > 0)
            {
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

            return (View(new List<Post>()));
            */
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
