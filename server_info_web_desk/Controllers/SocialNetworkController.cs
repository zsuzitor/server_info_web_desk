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
//using server_info_web_desk.Models;

namespace server_info_web_desk.Controllers
{
    public class SocialNetworkController : Controller
    {
        [AllowAnonymous]
        // GET: SocialNetwork
        public ActionResult Index()
        {
           // Url.Action("Index", "Book", new { author = "Толстой", id = 10 }, null);
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

            res.Albums=Models.SocialNetwork.Album.GetAlbumShortListForView(user.Albums,2);

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

            res.Group.AddRange(user.UserGroupToShort(0,6));

            //db.Entry(user).Collection(x1 => x1.Group).Load();


            //TODO сейчас плохо будет работать
            res.GroupCount = user.Group.Count;
            //res.Group.AddRange(user.Group.Take(7).Select(x1=>new GroupShort(x1)));




            res.WallMeme.AddRange(user.GetWallRecords(0,10));
            //if(!db.Entry(user).Collection(x1 => x1.WallRecord).IsLoaded)
            //    db.Entry(user).Collection(x1 => x1.WallRecord).Load();
            //res.WallMeme.AddRange(user.WallRecord.Skip(user.WallRecord.Count-10>0?user.WallRecord.Count-10:0));
           //foreach(var i in res.WallMeme)
           // {
           //     if (i.ImageId != null && !db.Entry(i).Reference(x1 => x1.Image).IsLoaded)
           //         db.Entry(i).Reference(x1 => x1.Image).Load();
           //     //можно загружать усеченную версию
           //     if (!db.Entry(i).Collection(x1 => x1.UsersLikes).IsLoaded)
           //         db.Entry(i).Collection(x1 => x1.UsersLikes).Load();

           //     //колличество нужно +5 записей последних
           //     if (!db.Entry(i).Collection(x1 => x1.Comments).IsLoaded)
           //         db.Entry(i).Collection(x1 => x1.Comments).Load();
           //     //колличество нужно +5 записей последних
           //     if (!db.Entry(i).Collection(x1 => x1.RecordRiposters).IsLoaded)
           //         db.Entry(i).Collection(x1 => x1.RecordRiposters).Load();
           //     if (!db.Entry(i).Reference(x1 => x1.Meme).IsLoaded)
           //         db.Entry(i).Reference(x1 => x1.Meme).Load();

           //     if (!db.Entry(i.Meme).Collection(x1 => x1.Images).IsLoaded)
           //         db.Entry(i.Meme).Collection(x1 => x1.Images).Load();
           //     //i.Meme_NM.Record_NM=


           // }






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
            res.IdUser = check_id;
            if (!db.Entry(group).Collection(x1 => x1.Users).IsLoaded)
                db.Entry(group).Collection(x1 => x1.Users).Load();
            if (!db.Entry(group).Collection(x1 => x1.Admins).IsLoaded)
                db.Entry(group).Collection(x1 => x1.Admins).Load();

            
                res.CanFollow = group.CanFollow(check_id);
           

            Group.GetUserShortList(group,res.Users,-6);
            Group.GetAdminShortList(group, res.Admins, 6);
            

            if (!db.Entry(group).Collection(x1 => x1.Albums).IsLoaded)
                db.Entry(group).Collection(x1 => x1.Albums).Load();

             res.MainImage= Models.SocialNetwork.Album.GetLastImageAlbum(group.Albums.First(), 1).FirstOrDefault();

            res.Image = Models.SocialNetwork.Album.GetLastImageAlbum(group.Albums.First(), 4);


            //res.WallMeme.AddRange(group.WallRecord.Skip(group.WallRecord.Count - 10));
            res.WallMeme.AddRange(group.GetWallRecords(0, 10));

            //if (!group.AddMemesPrivate)
            //{
            //    res.CanAddMeme=true;
            //}
            //else
            //{
            //    if (!db.Entry(group).Collection(x1 => x1.Admins).IsLoaded)
            //        db.Entry(group).Collection(x1 => x1.Admins).Load();
            //    var ch_adm=group.Admins.FirstOrDefault(x1 => x1.Id == check_id);
            //    if(ch_adm!=null)
            //        res.CanAddMeme = true;
            //}
            res.CanAddMeme = group.CanAddMeme(check_id);

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
        public ActionResult Groups(string id)
        {
            if(id==null)
                return new HttpStatusCodeResult(404);
            string check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var user = db.Users.FirstOrDefault(x1=>x1.Id==id);
            if(user==null)
                return new HttpStatusCodeResult(404);
            GroupsListView res = new GroupsListView();


            res.Groups.AddRange(user.UserGroupToShort(0,10));

            
            return View(res);
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
        [HttpPost]
        public JsonResult LikeRecord(int? id)
        {
            string check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            if(id==null)
                return Json(null);
            var record = db.Record.FirstOrDefault(x1=>x1.Id==id);
            if (record == null)
                return Json(null);
            //TODO тут обработать ошибки и уровень доступа, нельзя лайкнуть если нельзя просмотреть
            if (!db.Entry(record).Collection(x1 => x1.UsersLikes).IsLoaded)
                db.Entry(record).Collection(x1 => x1.UsersLikes).Load();
            var like = record.UsersLikes.FirstOrDefault(x1 => x1.Id == check_id);
            bool red_heart = false;
            if (like == null)
            {
                record.UsersLikes.Add(db.Users.First(x1 => x1.Id == check_id));
                red_heart = true;
            }

            else
            {
                record.UsersLikes.Remove(db.Users.First(x1 => x1.Id == check_id));
                red_heart = false;
            }
              
            db.SaveChanges();



            return Json(new { red_heart, id,count= record.UsersLikes.Count });
        }
        [Authorize]
        public ActionResult GroupCreate([Bind(Include="Name")]GroupShort a)
        {
            string check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            if(string.IsNullOrWhiteSpace(a.Name))
                return new HttpStatusCodeResult(404);
            Group res = new Group() { Name=a.Name, MainAdminId=check_id };
            db.Groups.Add(res);
            db.SaveChanges();

            db.Albums.Add(new Album()
            {
                Name = "Main",
                Description = "Сюда добавляются главные фотографии с вашей страницы",
                //User = admin
                GroupId = res.Id
            });
            db.Albums.Add(new Album()
            {
                Name = "NotMain",
                Description = "Сюда добавляются фотографии с вашей страницы",
                GroupId = res.Id
            });
            var user = db.Users.FirstOrDefault(x1 => x1.Id == check_id);
            res.Admins.Add(user);
            res.Users.Add(user);
            db.SaveChanges();
            
            return RedirectToAction("GroupRecord", "SocialNetwork",new {id=res.Id });
        }

        [Authorize]
        public ActionResult FollowGroup(int? id)
        {
            string check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            bool? res = true;
            if(id==null)
                return new HttpStatusCodeResult(404);
            var group = db.Groups.FirstOrDefault(x1=>x1.Id==id);
            if(group==null)
                return new HttpStatusCodeResult(404);
            res = group.CanFollow(check_id);
            var user = db.Users.First(x1 => x1.Id == check_id);
            if (res == true)
            {
                group.Users.Add(user);
            }
            if (res == false)
            {
                group.Users.Remove(user);
            }
            db.SaveChanges();


            res = !res;
            return Redirect(Url.Action("FollowGroupPartial", "SocialNetwork", new { IdGroup = id, CanFollow = res }));
        }

        [Authorize]
        public ActionResult FollowPerson(string id)
        {
            string check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            bool? res = true;
            if (id == null)
                return new HttpStatusCodeResult(404);
            var user = db.Users.FirstOrDefault(x1 => x1.Id == id);
            if (user == null)
                return new HttpStatusCodeResult(404);
            res = user.CanFollow(check_id);
            var user_act = db.Users.First(x1 => x1.Id == check_id);
            if (res == true)
            {
                user.Friends.Add(user_act);
            }
            if (res == false)
            {
                user.Friends.Remove(user_act);
            }
            if (res == null)
            {
                user.Followers.Remove(user_act);
            }
            db.SaveChanges();


            res = !res;
            return Redirect(Url.Action("FollowUserPartial", "SocialNetwork", new { Iduser = id, CanAddFriend = res }));
        }




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
            //int gg = Request.Files.Count;

            bool access = false;
            id_user = id_user ?? check_id;
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
                return RedirectToAction("PersonalRecord", "SocialNetwork", new { id = id_user });
                //return PartialView(null);

            var list_img_byte = Get_photo_post(uploadImage);

            Record record = new Record() { UserId = id_user };
            db.Record.Add(record);
            db.SaveChanges();

            Meme mem = new Meme() {Id=record.Id, Description=text, СreatorId=check_id };
            db.Memes.Add(mem);
            db.SaveChanges();
            var list_img = list_img_byte.Select(x1 => new Image() { MemeId=mem.Id, Data=x1, UserId=check_id });
            db.ImagesSocial.AddRange(list_img);
            db.SaveChanges();
            
            if (!db.Entry(user).Collection(x1 => x1.Friends).IsLoaded)
                db.Entry(user).Collection(x1 => x1.Friends).Load();
            if (!db.Entry(user).Collection(x1 => x1.Followers).IsLoaded)
                db.Entry(user).Collection(x1 => x1.Followers).Load();
            //user.WallRecord.Add(record);
            user.Friends.Where(x1 =>
            {
                x1.News.Add(record);
                return true;
            });
            user.Followers.Where(x1 =>
            {
                x1.News.Add(record);
                return true;
            });
            db.SaveChanges();

            return RedirectToAction("PersonalRecord", "SocialNetwork",new {id= id_user });
           // return PartialView(record);
        }
        [Authorize]
        public ActionResult AddMemeGroup(int id_group, HttpPostedFileBase[] uploadImage, string text)
        {
            string check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var group=db.Groups.FirstOrDefault(x1 => x1.Id == id_group);
            if(group==null)
                return new HttpStatusCodeResult(404);


            bool CanAddMeme = group.CanAddMeme(check_id);

            if (!CanAddMeme)
                return new HttpStatusCodeResult(404);


            var list_img_byte = Get_photo_post(uploadImage);

            Record record = new Record() { GroupId = id_group };
            db.Record.Add(record);
            db.SaveChanges();

            Meme mem = new Meme() { Id = record.Id, Description = text, СreatorId = check_id };
            db.Memes.Add(mem);
            db.SaveChanges();
            var list_img = list_img_byte.Select(x1 => new Image() { MemeId = mem.Id, Data = x1, UserId = check_id });
            db.ImagesSocial.AddRange(list_img);
            db.SaveChanges();

            group.WallRecord.Add(record);
            if (!db.Entry(group).Collection(x1 => x1.Users).IsLoaded)
                db.Entry(group).Collection(x1 => x1.Users).Load();
            ((List<Models.ApplicationUser>)record.UsersNews).AddRange(group.Users);
            //foreach(var i in group.Users)
            //{
            //    i.News.Add(record);
            //}
            db.SaveChanges();

            return RedirectToAction("GroupRecord", "SocialNetwork",new {id= id_group });
        }

        //------------------------------------------------------------
        [ChildActionOnly]
        [AllowAnonymous]
            public ActionResult MemeRecordPartial(int id_record, Record a)
        {
            string check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            if (a == null)
            {
                //загрузить
            }
            //загрузить все ужные данные для отображения
            ViewBag.check_id = check_id;
            return PartialView(a);
        }



        //[ChildActionOnly]
        [Authorize]
        public ActionResult FollowGroupPartial(int IdGroup,bool? CanFollow)
        {
            string check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            ViewBag.IdGroup = IdGroup;
            ViewBag.check_id = check_id;

            ViewBag.CanFollow = CanFollow;
            return PartialView();
        }
        [Authorize]
        public ActionResult FollowUserPartial(string Iduser, bool? CanAddFriend)
        {
            string check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            ViewBag.IdPage = Iduser;
            ViewBag.check_id = check_id;

            ViewBag.CanAddFriend = CanAddFriend;
            return PartialView();
        }
    }
}