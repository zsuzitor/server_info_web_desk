using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Newtonsoft.Json;
using System.Web.Mvc.Ajax;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;

using server_info_web_desk.Models.DataBase;
using static server_info_web_desk.Models.DataBase.DataBase;
using server_info_web_desk.Models.SocialNetwork;
using server_info_web_desk.Models.ViewModel;

namespace server_info_web_desk.Controllers
{
    public class SocialNetworkController : Controller
    {
        [AllowAnonymous]
        // GET: SocialNetwork
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        //TODO
        public ActionResult PersonalRecord(string id)
        {
            
            string check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            
            id = id ?? check_id;
            if (id == null)
            {
                return RedirectToAction("Index", "SocialNetworkController",new { });
            }

            var user = db.Users.First(x1=>x1.Id==id);
            
            PersonalRecordView res = new PersonalRecordView(user);
            res.IdUser = check_id;
            db.Entry(user).Collection(x1 => x1.Albums).Load();
            var albums = user.Albums.Take(2);
            foreach(var i in albums)
            {
                db.Entry(i).Collection(x1 => x1.Images).Load();
                res.Albums.Add(new AlbumShort(i));
            }
            res.MainImage = albums.First().Images.FirstOrDefault();

            var albums_ph = user.Albums.ElementAt(1);
            
            db.Entry(albums_ph).Collection(x1 => x1.Images).Load();
            res.Image.AddRange(albums_ph.Images.Skip(albums_ph.Images.Count-4));

            db.Entry(user).Collection(x1 => x1.Group).Load();


            //TODO сейчас плохо будет работать
            res.Group.AddRange(user.Group.Take(7).Select(x1=>new GroupShort(x1)));
            res.WallMeme.
           


            //    public List<Meme> WallMeme { get; set; }


            //public List<Group> Group { get; set; }
            

            return View(res);
        }
        [Authorize]
        //TODO
        public ActionResult EditPersonalRecord()
        {
            return View();
        }


        [AllowAnonymous]
        //TODO
        public ActionResult GroupRecord(string id)
        {
            return View();
        }
        [Authorize]
        //TODO
        public ActionResult EditGroupRecord(int id)
        {
            return View();
        }
        [Authorize]
        //TODO
        public ActionResult News(string id)
        {
            return View();
        }

        [Authorize]
        //TODO
        public ActionResult Messages()
        {
            return View();
        }


        [Authorize]
        //TODO
        public ActionResult Dialog(int id)
        {
            return View();
        }

        [AllowAnonymous]
        //TODO
        public ActionResult Friends(int id)
        {
            return View();
        }

        [AllowAnonymous]
        //TODO
        public ActionResult Groups(int id)
        {
            return View();
        }


        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult MainHeader()
        {
            return PartialView();
        }


        [Authorize]
        [ChildActionOnly]
        public ActionResult LeftMenuPersonal()
        {
            return PartialView();
        }
        }
}