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
            Url.Action("Index", "Book", new { author = "Толстой", id = 10 }, null);
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
            res.Image.AddRange(albums_ph.Images.Skip(albums_ph.Images.Count-4).Select(x1=>x1.Image));

            db.Entry(user).Collection(x1 => x1.Group).Load();


            //TODO сейчас плохо будет работать
            res.GroupCount = user.Group.Count;
            res.Group.AddRange(user.Group.Take(7).Select(x1=>new GroupShort(x1)));
            res.WallMeme.AddRange(user.WallRecord.Take(10));
           
            if(!db.Entry(user).Reference(x1=>x1.Friends).IsLoaded)
            {
                db.Entry(user).Reference(x1 => x1.Friends).Load();
            }
            res.Friends.AddRange(user.Friends.Skip(user.Friends.Count-6>0? (user.Friends.Count-6):0).Select(x1=>new Models.ApplicationUserShort(x1)));

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
        public ActionResult GroupRecord(int id)
        {
            string check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();

            var group = db.Groups.FirstOrDefault(x1 => x1.Id == id);
            if (group == null)
            {
                return new HttpStatusCodeResult(404);
            }
            if (!group.OpenGroup)
            {
                db.Entry(group).Collection(x1 => x1.Users).Load();
                var us_c=group.Users.FirstOrDefault(x1 => x1.Id == check_id);
                if (us_c == null)
                {
                    //TODO тут отправлять усеченную версию группы для тех кто не подписан и группа закрытая
                    return new HttpStatusCodeResult(404);
                }
            }

            GroupRecordView res = new GroupRecordView(group);

            if(!db.Entry(group).Collection(x1 => x1.Users).IsLoaded)
                db.Entry(group).Collection(x1 => x1.Users).Load();

            res.Users.AddRange(group.Users.Skip(group.Users.Count-6).Select(x1=> 
                 new Models.ApplicationUserShort(x1)));
            res.Admins.AddRange(group.Admins.Skip((group.Admins.Count - 6)>0?(group.Admins.Count - 6):0)
                .Select(x1 =>new Models.ApplicationUserShort(x1)));

            
            //Albums = new List<Album>();
            //WallRecord = new List<Record>();

            return View(res);
        }
        [Authorize]
        //TODO
        public ActionResult EditGroupRecord(int id)
        {
            return View();
        }
        [Authorize]
        //TODO
        public ActionResult News()
        {
            return View();
        }

        [Authorize]
        //TODO
        public ActionResult Messages()
        {
            return View();
        }
        [AllowAnonymous]
        //TODO вместе с AlbumsGroup редиректить на 1 метод в котором все будет происходить????
        public ActionResult AlbumsPerson(string id)
        {
            return View();
        }
        [AllowAnonymous]
        //TODOвместе с AlbumsPerson редиректить на 1 метод в котором все будет происходить????
        public ActionResult AlbumsGroup(int id)
        {
            return View();
        }

        [AllowAnonymous]
        //TODO 
        public ActionResult Album(int id)
        {
            return View();
        }

        [AllowAnonymous]
        //TODO 
        public ActionResult Image(int id)
        {
            //если прикреплено к записи то открывать запись
            return PartialView();
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


        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult LeftMenuPersonal()
        {
            string check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            ViewBag.id = check_id;

            return PartialView();
        }
        }
}