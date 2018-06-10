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
using static server_info_web_desk.Models.functions.FunctionsProject;

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

            if (res.IdUser != res.IdPage)
            {
                if (user.WallOpenWrite == true)
                    res.CanAddMeme = true;
                else
                    if (user.WallOpenWrite == null)
                {
                    if (!db.Entry(user).Collection(x1 => x1.Friends).IsLoaded)
                        db.Entry(user).Collection(x1 => x1.Friends).Load();
                    var ch_acc = user.Friends.FirstOrDefault(x1 => x1.Id == check_id);
                    if (ch_acc != null)
                        res.CanAddMeme = true;
                }
            }
            else
                res.CanAddMeme = true;



            db.Entry(user).Collection(x1 => x1.Albums).Load();

            res.Albums=Models.SocialNetwork.Album.GetAlbumShortForView(user.Albums,2);

            //var albums = user.Albums.Take(2);
            //foreach(var i in albums)
            //{
            //    db.Entry(i).Collection(x1 => x1.Images).Load();
            //    res.Albums.Add(new AlbumShort(i));
            //}
            res.MainImage = Models.SocialNetwork.Album.GetLastImageAlbum(user.Albums.First(),1).FirstOrDefault(); 

           // var albums_ph = user.Albums.ElementAt(1);

            //if (!db.Entry(albums_ph).Collection(x1 => x1.Images).IsLoaded)
            //    db.Entry(albums_ph).Collection(x1 => x1.Images).Load();
            

            res.Image.AddRange(Models.SocialNetwork.Album.GetLastImageAlbum(user.Albums.ElementAt(1), 4));

            db.Entry(user).Collection(x1 => x1.Group).Load();


            //TODO сейчас плохо будет работать
            res.GroupCount = user.Group.Count;
            res.Group.AddRange(user.Group.Take(7).Select(x1=>new GroupShort(x1)));
            res.WallMeme.AddRange(user.WallRecord.Skip(user.WallRecord .Count- 10));
           
            if(!db.Entry(user).Collection(x1=>x1.Friends).IsLoaded)
            {
                db.Entry(user).Collection(x1 => x1.Friends).Load();
            }
            if (check_id != null)
            {
                var check_friends = user.Friends.FirstOrDefault(x1 => x1.Id == check_id);
                if (check_friends != null)
                {
                    res.CanAddFriend = false;
                }
                else
                {
                    check_friends = user.Followers.FirstOrDefault(x1=>x1.Id==check_id);
                    if (check_friends == null)
                    {
                        res.CanAddFriend = true;
                    }
                    }
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

            if (!db.Entry(group).Collection(x1 => x1.Albums).IsLoaded)
                db.Entry(group).Collection(x1 => x1.Albums).Load();

             res.MainImage= Models.SocialNetwork.Album.GetLastImageAlbum(group.Albums.First(), 1).FirstOrDefault();

            res.Image = Models.SocialNetwork.Album.GetLastImageAlbum(group.Albums.First(), 4);


            res.WallMeme.AddRange(group.WallRecord.Skip(group.WallRecord.Count - 10));

            
            if (!group.AddMemesPrivate)
            {
                res.CanAddMeme=true;
            }
            else
            {
                if (!db.Entry(group).Collection(x1 => x1.Admins).IsLoaded)
                    db.Entry(group).Collection(x1 => x1.Admins).Load();
                var ch_adm=group.Admins.FirstOrDefault(x1 => x1.Id == check_id);
                if(ch_adm!=null)
                    res.CanAddMeme = true;
            }


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
        public ActionResult Dialog(int? id,string user_id=null)
        {
            return View();
        }

        [AllowAnonymous]
        //TODO
        public ActionResult Friends(int id)
        {
            //GroupUsers
            return View();
        }
        [AllowAnonymous]
        //TODO
        public ActionResult GroupUsers(int id)
        {
            //Friends
            return View();
        }
        public ActionResult GroupAdmins(int id)
        {
            //Friends
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




        //---------------------------------------------------ACTION---------------------------------
        [Authorize]
        public ActionResult AddFriend(string id)
        {
            string check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            

            return PartialView();
        }

        [Authorize]
        public ActionResult CreateGroup(Group a)
        {
            return View();
        }

        [Authorize]
        public ActionResult AddMemePerson(string id_user, HttpPostedFileBase[] uploadImage,string text)
        {
            string check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            //TODO проверять есть ли доступ к добавлению мемов на чужую стену
            bool access = false;
            var user = db.Users.FirstOrDefault(x1 => x1.Id == id_user);

            if (id_user == check_id)
                access = true;
            else
            {
                if (user.WallOpenWrite==true)
                    access = true;
                else
                    if(user.WallOpenWrite == null)
                {
                    if (!db.Entry(user).Collection(x1 => x1.Friends).IsLoaded)
                        db.Entry(user).Collection(x1 => x1.Friends).Load();
                    var ch_acc=user.Friends.FirstOrDefault(x1 => x1.Id == check_id);
                    if (ch_acc != null)
                        access = true;
                }
            }
            if (!access)
                return PartialView(null);
            var list_img_byte = Get_photo_post(uploadImage);
            Meme mem = new Meme() { Description=text, СreatorId=check_id };
            db.Memes.Add(mem);
            db.SaveChanges();
            var list_img = list_img_byte.Select(x1 => new Image() { MemeId=mem.Id, Data=x1, UserId=check_id });
            db.ImagesSocial.AddRange(list_img);
            db.SaveChanges();
            Record record = new Record() { MemeId=mem.Id, UserId= id_user };
            db.Record.Add(record);
            db.SaveChanges();
            if (!db.Entry(user).Collection(x1 => x1.Friends).IsLoaded)
                db.Entry(user).Collection(x1 => x1.Friends).Load();
            if (!db.Entry(user).Collection(x1 => x1.Followers).IsLoaded)
                db.Entry(user).Collection(x1 => x1.Followers).Load();
            user.Friends.Where(x1 =>
            {
                x1.WallRecord.Add(record);
                return true;
            });
            user.Followers.Where(x1 =>
            {
                x1.WallRecord.Add(record);
                return true;
            });
            db.SaveChanges();

            return PartialView(record);
        }
        [Authorize]
        public ActionResult AddMemeGroup(int id_group, HttpPostedFileBase[] uploadImage, string text)
        {
            string check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();


            return PartialView();
        }
    }
}