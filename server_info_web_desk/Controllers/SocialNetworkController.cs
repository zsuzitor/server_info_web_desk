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
            Session["NewMessageType"] = "2";
            return View();
        }

        [AllowAnonymous]
        //TODO
        public ActionResult PersonalRecord(string id)
        {
            //Session["NameAction"] = "WallMeme";
            //Session["StartLoad"] = 0;
            //Session["CountLoad"] = 10;
            string check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            
            //id = id ?? check_id;
            if (id == null&&check_id==null)
            {
                return RedirectToAction("Index", "SocialNetwork",new { });
            }
            if(id==null)
                return RedirectToAction("PersonalRecord", "SocialNetwork", new { id = check_id });

             var user =  db.Users.First(x1=>x1.Id==id);
            
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


            if (!db.Entry(user).Collection(x1 => x1.Albums).IsLoaded)
                db.Entry(user).Collection(x1 => x1.Albums).Load();
            res.IdMainAlbum = user.Albums.First().Id;
            res.IdNotMainAlbum = user.Albums.ElementAt(1).Id;

            res.Albums=Models.SocialNetwork.Album.GetAlbumShortListForView(user.Albums,2);

            
            res.MainImage = Models.SocialNetwork.Album.GetLastImageAlbum(user.Albums.First(),1).FirstOrDefault(); 

            res.Image.AddRange(Models.SocialNetwork.Album.GetLastImageAlbum(user.Albums.ElementAt(1), 4));

            res.Group.AddRange(user.UserGroupToShort(0,6));

            //TODO сейчас плохо будет работать
            res.GroupCount = user.Group.Count;
            
            //res.WallMeme.AddRange(user.GetWallRecords(0,10));
           

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
            Session["NewMessageType"] = "2";
            return View(res);
        }
        [Authorize]
        //TODO
        public ActionResult EditPersonalRecord()
        {
            string check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            if (check_id == null)
                return new HttpStatusCodeResult(404);
            var user = db.Users.FirstOrDefault(x1 => x1.Id == check_id);
            if (user == null)
                return new HttpStatusCodeResult(404);
            Session["NewMessageType"] = "2";
            return View(user);
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
            if (group.Admins.FirstOrDefault(x1 => x1.Id == check_id) != null)
                res.Admin = true;
            
                res.CanFollow = group.CanFollow(check_id);


            group.GetUserShortList(res.Users,-6);
            group.GetAdminShortList( res.Admins, 6);
            

            if (!db.Entry(group).Collection(x1 => x1.Albums).IsLoaded)
                db.Entry(group).Collection(x1 => x1.Albums).Load();
            
            res.IdMainAlbum = group.Albums.First().Id;
            res.IdNotMainAlbum = group.Albums.ElementAt(1).Id;


            res.Albums = Models.SocialNetwork.Album.GetAlbumShortListForView((List<Album>)group.Albums, 2);
            res.MainImage= Models.SocialNetwork.Album.GetLastImageAlbum(group.Albums.First(), 1).FirstOrDefault();

            res.Image = Models.SocialNetwork.Album.GetLastImageAlbum(group.Albums.ElementAt(1), 4);


           
            //res.WallMeme.AddRange(group.GetWallRecords(0, 10));

           
            res.CanAddMeme = group.CanAddMeme(check_id);

            //Albums = new List<Album>();
            //WallRecord = new List<Record>();
            Session["NewMessageType"] = "2";
            return View(res);
        }
        [Authorize]
        //TODO
        public ActionResult EditGroupRecord(int id)
        {
            Session["NewMessageType"] = "2";
            return View();
        }
        [Authorize]
        //TODO
        public ActionResult News()
        {
            string check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            ViewBag.check_id = check_id;
            Session["NewMessageType"] = "2";
            return View();
        }

        [Authorize]
        //TODO
        public ActionResult Messages()
        {
            string check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var user = db.Users.First(x1 => x1.Id == check_id);
            if (!db.Entry(user).Collection(x1 => x1.Chats).IsLoaded)
                db.Entry(user).Collection(x1 => x1.Chats).Load();


            List<ChatShort> res = new List<ChatShort>();
            res.AddRange(user.Chats.Select(x1 => {
                var us_s = new ChatShort() { Id=x1.Id,Image=x1.Image, Name=x1.Name };

                if (!db.Entry(x1).Collection(x2 => x2.Messages).IsLoaded)
                    db.Entry(x1).Collection(x2 => x2.Messages).Load();
                var last_message = x1.Messages.LastOrDefault();
                if (last_message == null)
                {
                    us_s.User = null;
                    us_s.Text = null;
                }
                else
                {
                    if (!db.Entry(last_message).Reference(x2 => x2.Creator).IsLoaded)
                        db.Entry(last_message).Reference(x2 => x2.Creator).Load();
                    us_s.User = new Models.ApplicationUserShort(last_message?.Creator);
                    us_s.Text = last_message.Text;
                }
               
                //us_s.CountNewMessage=;

                return us_s;
                }));


            Session["NewMessageType"] = "3";
            return View(res);
        }
        [AllowAnonymous]
        //TODO вместе с AlbumsGroup редиректить на 1 метод в котором все будет происходить????
        public ActionResult AlbumsPerson(string id,int? select_id)
        {
            //TODO проверять есть ли доступ
            string check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            ListAlbumsShortView res = new ListAlbumsShortView() {UserId= check_id,PageUserId=id, SelectAlbum=select_id };
            var userPage = db.Users.FirstOrDefault(x1 => x1.Id == id);
            if (!db.Entry(userPage).Collection(x1 => x1.Albums).IsLoaded)
                db.Entry(userPage).Collection(x1 => x1.Albums).Load();
            
          

             res.AlbumList.AddRange(Models.SocialNetwork.Album.GetAlbumShortListForView(userPage.Albums, userPage.Albums.Count));

            Session["NewMessageType"] = "2";
            return View("Albums",  res);//"SocialNetwork",
        }
        [AllowAnonymous]
        //TODOвместе с AlbumsPerson редиректить на 1 метод в котором все будет происходить????
        public ActionResult AlbumsGroup(int id, int? select_id)
        {
            string check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            ListAlbumsShortView res = new ListAlbumsShortView() { UserId = check_id, PageGroupId = id, SelectAlbum = select_id };
            var GroupPage = db.Groups.FirstOrDefault(x1 => x1.Id == id);
            if (!db.Entry(GroupPage).Collection(x1 => x1.Albums).IsLoaded)
                db.Entry(GroupPage).Collection(x1 => x1.Albums).Load();



            res.AlbumList.AddRange(Models.SocialNetwork.Album.GetAlbumShortListForView((List<Album>)GroupPage.Albums, GroupPage.Albums.Count));
            Session["NewMessageType"] = "2";
            return View("Albums",  res);//"SocialNetwork",
        }

        //[AllowAnonymous]
        ////TODO 
        //public ActionResult Album(int id)
        //{
        //    return View();
        //}

        [AllowAnonymous]
        //TODO 
        public ActionResult Image(int id)
        {
            //если прикреплено к записи то открывать запись
            Session["NewMessageType"] = "2";
            return PartialView();
        }


        [Authorize]
        //TODO
        public ActionResult Dialog(int? id = null, string user_id = null)//
        {

            if(id==null&& user_id==null)
                return new HttpStatusCodeResult(404);

            //TODO УРОВЕНЬ ДОСТУПА

            string check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var user = db.Users.FirstOrDefault(x1 => x1.Id == check_id);
            Chat res = new Chat();

            if (!db.Entry(user).Collection(x1 => x1.Chats).IsLoaded)
                db.Entry(user).Collection(x1 => x1.Chats).Load();
            if (id != null)
            {
                var dialog = user.Chats.FirstOrDefault(x1 => x1.Id == id);
                if (dialog == null)
                    return new HttpStatusCodeResult(404);
                res = dialog;

            }
            else
            if (user_id != null)
            {
                Chat chat = user.Chats.FirstOrDefault(x1 =>
                {

                    if (!db.Entry(x1).Collection(x2 => x2.Users).IsLoaded)
                        db.Entry(x1).Collection(x2 => x2.Users).Load();
                    if (x1.Users.FirstOrDefault(x2 => x2.Id == user_id) != null && x1.Users.Count == 2)
                        return true;

                    return false;
                });
                //Chat chat = null;
                //foreach (var i in user.Chats)
                //{
                //    if (!db.Entry(i).Collection(x2 => i.Users).IsLoaded)
                //        db.Entry(i).Collection(x2 => i.Users).Load();
                //    if (i.Users.FirstOrDefault(x2 => x2.Id == user_id) != null && i.Users.Count == 2)
                //        chat = i;
                //    break;
                //}



                //СОЗДАТЬ НОВЫЙ ЧАТ
                if (chat == null)
                {
                    var user2 = db.Users.FirstOrDefault(x1=>x1.Id== user_id);
                    if(user2==null)
                        return new HttpStatusCodeResult(404);
                    chat = new Chat() { CreatorId=check_id };
                    db.Chats.Add(chat);
                    db.SaveChanges();
                    chat.Users.Add(user);
                    chat.Users.Add(user2);
                    db.SaveChanges();
                }
                return RedirectToAction("Dialog", "SocialNetwork",new { id= chat.Id });
            }

            Session["NewMessageType"] = "1";

            return View(res);
        }

        [AllowAnonymous]
        //TODO
        public ActionResult Friends(int id)
        {
            //GroupUsers
            Session["NewMessageType"] = "2";
            return View();
        }
        [AllowAnonymous]
        //TODO
        public ActionResult GroupUsers(int id)
        {
            //Friends
            Session["NewMessageType"] = "2";
            return View();
        }
        public ActionResult GroupAdmins(int id)
        {
            //Friends
            Session["NewMessageType"] = "2";
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

            Session["NewMessageType"] = "2";
            return View(res);
        }









        //-----------------------------------------------------------------------------------------------------------------------------------------------

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
            if (check_id != null)
            {
                var user = db.Users.FirstOrDefault(x1 => x1.Id == check_id);
                if (!db.Entry(user).Collection(x1 => x1.MessageNeedRead).IsLoaded)
                    db.Entry(user).Collection(x1 => x1.MessageNeedRead).Load();
                ViewBag.CountNewMessage = user.MessageNeedRead.Count;
            }
           
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
                res = false;
            }
            else
            if (res == false)
            {
                group.Users.Remove(user);
                res = true;
            }
            else
                 if (res == null)
            {
                group.Users.Remove(user);
                res = true;
            }
            db.SaveChanges();

            
            //res = !res;
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
                res = false;
            }
            else
            if (res == false)
            {
                user.Friends.Remove(user_act);
                res = true;
            }
            else
            if (res == null)
            {
                user.Followers.Remove(user_act);
                res = true;
            }
            db.SaveChanges();


            //res = !res;
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

            Meme mem = new Meme() {Id=record.Id, Description=text, CreatorId = check_id };
            db.Memes.Add(mem);
            db.SaveChanges();
            var list_img = list_img_byte.Select(x1 => new Image() { MemeId=mem.Id, Data=x1, UserId=check_id });
            db.ImagesSocial.AddRange(list_img);
            db.SaveChanges();
            user.AddRecordMemeWall(record);



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

            Meme mem = new Meme() { Id = record.Id, Description = text, CreatorId = check_id };
            db.Memes.Add(mem);
            db.SaveChanges();
            var list_img = list_img_byte.Select(x1 => new Image() { MemeId = mem.Id, Data = x1, UserId = check_id });
            db.ImagesSocial.AddRange(list_img);
            db.SaveChanges();

            

            return RedirectToAction("GroupRecord", "SocialNetwork",new {id= id_group });
        }

        [Authorize]
        public ActionResult AddImagePerson(string text, HttpPostedFileBase[] uploadImage, int? album_id)
        {
            if(uploadImage.Count()<1|| album_id==null)
                return new HttpStatusCodeResult(404);
            string check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var user = db.Users.FirstOrDefault(x1=>x1.Id==check_id);
            if (!db.Entry(user).Collection(x1 => x1.Albums).IsLoaded)
                db.Entry(user).Collection(x1 => x1.Albums).Load();
            var album=user.Albums.FirstOrDefault(x1 => x1.Id == album_id);
            if(album==null)
                return new HttpStatusCodeResult(404);
            var list_img_byte = Get_photo_post(uploadImage);
            var img= new Image() {  Data = list_img_byte?.ElementAt(0), UserId = check_id };
            db.ImagesSocial.Add(img);
            db.SaveChanges();
            
            var record = new Record() {  AlbumId= album.Id, UserId=check_id, Description = text };
            db.Record.Add(record);
            db.SaveChanges();
            img.RecordId= record.Id;
            record.ImageId=img.Id;
            
            db.SaveChanges();
            user.AddRecordMemeWall(record);

            return RedirectToAction("PersonalRecord", "SocialNetwork", new { id = check_id});
        }
        [Authorize]
        public ActionResult AddImageGroup(string text, HttpPostedFileBase[] uploadImage, int? album_id,int? id_group)
        {
            if (uploadImage.Count() < 1 || album_id == null|| id_group==null)
                return new HttpStatusCodeResult(404);


            string check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();

            var group = db.Groups.FirstOrDefault(x1 => x1.Id == id_group);
            if (group == null)
                return new HttpStatusCodeResult(404);


            //if (group.AddMemesPrivate)
            //{
            //    if (!db.Entry(group).Collection(x1 => x1.Admins).IsLoaded)
            //        db.Entry(group).Collection(x1 => x1.Admins).Load();
            //    var a=group.Admins.FirstOrDefault(x1 => x1.Id == check_id);
            //    if(a==null)
            //        return new HttpStatusCodeResult(404);
            //}
            //if (!group.OpenGroup)
            //{
            //    if (!db.Entry(group).Collection(x1 => x1.Users).IsLoaded)
            //        db.Entry(group).Collection(x1 => x1.Users).Load();
            //    var u = group.Users.FirstOrDefault(x1 => x1.Id == check_id);
            //    if (u == null)
            //        return new HttpStatusCodeResult(404);
            //}
            bool can_add = group.CanAddMeme(check_id);
            if (!can_add)
                return new HttpStatusCodeResult(404);
            if (!db.Entry(group).Collection(x1 => x1.Albums).IsLoaded)
                db.Entry(group).Collection(x1 => x1.Albums).Load();

            
            var album = group.Albums.FirstOrDefault(x1 => x1.Id == album_id);
            //TODO проверка
            var ch_al = group.Albums.First();
            
            if (can_add&&ch_al.Id==album.Id)
            {
                var a=group.Admins.FirstOrDefault(x1 => x1.Id == check_id);
                if (a == null)
                    can_add = false;
            }
            if(!can_add)
                return new HttpStatusCodeResult(404);

            if (album == null)
                return new HttpStatusCodeResult(404);
            var list_img_byte = Get_photo_post(uploadImage);
            var img = new Image() { Data = list_img_byte?.ElementAt(0), UserId = check_id };
            db.ImagesSocial.Add(img);
            db.SaveChanges();

            var record = new Record() { AlbumId = album.Id, GroupId = group.Id, Description = text };

            db.SaveChanges();
            img.RecordId = record.Id;
            record.ImageId = img.Id;
            db.SaveChanges();
            group.AddRecordMemeWall(record);
            return RedirectToAction("GroupRecord", "SocialNetwork", new { id = id_group });
        }


        [Authorize]
        //TODO
        public ActionResult SaveChangesPersonalRecord(Models.ApplicationUser a)
        {
            //TODO валидация
            string check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            if(check_id==null)
                return new HttpStatusCodeResult(404);
            var user = db.Users.FirstOrDefault(x1 => x1.Id == check_id);
            if (user == null)
                return new HttpStatusCodeResult(404);

            if (!string.IsNullOrWhiteSpace(a.Name))
                user.Name = a.Name;
            if (!string.IsNullOrWhiteSpace(a.Surname))
                user.Surname = a.Surname;

            user.Status = a.Status;
            user.Birthday = a.Birthday;
            user.Sex = a.Sex;
            user.Country = a.Country;
            user.Town = a.Town;
            user.Street = a.Street;
            user.Description = a.Description;
            user.WallOpenWrite = a.WallOpenWrite;
            user.PrivatePage = a.PrivatePage;
            user.Open_data_info = a.Open_data_info;
            db.SaveChanges();
            return RedirectToAction("EditPersonalRecord", "SocialNetwork",new { });
        }

        //------------------------------------------------------------


        //не помню зачем нужно мб что бы при создании мема его возвращать
        //[ChildActionOnly]
        //[AllowAnonymous]
        //public ActionResult MemeRecordPartial(int id_record, Record a)
        //{
        //    //@Healpers.MemeRecord(Model, (string)ViewBag.check_id)
        //    string check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();
        //    if (a == null)
        //    {
        //        //загрузить
        //    }
        //    //загрузить все ужные данные для отображения
        //    ViewBag.check_id = check_id;
        //    return PartialView(a);
        //}


        [AllowAnonymous]
        public ActionResult LoadImagesAlbum(int id, int start, int count)
        {
            List<Image> res = new List<Models.SocialNetwork.Image>();
            var album = db.Albums.FirstOrDefault(x1=>x1.Id== id);
            album.GetAlbumImages(start, count);
            res.AddRange(album.Images.Select(x1=>x1.Image));//.Select(x1=>new ImageShort() {Id=(int)x1.ImageId,Data=x1.Image.Data }

            return PartialView(res);
            
        }

        
        [AllowAnonymous]
        public ActionResult MemeRecordListPartial(string id,int type,int start,int count)
        {
            //type: 1-PersonalRecord 2-GroupRecord 3-NewsPresonal
            List<Record> res = new List<Record>();

            //@Healpers.MemeRecord(Model, (string)ViewBag.check_id)
            string check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            ViewBag.check_id = check_id;
            Models.ApplicationUser user = null;
            if (check_id != null)
                user = db.Users.FirstOrDefault(x1 => x1.Id == check_id);
            switch (type)
            {
                case 1:
                    if (id == null)
                        return new HttpStatusCodeResult(404);
                    var user_page = db.Users.FirstOrDefault(x1 => x1.Id == id);
                    res.AddRange(user_page.GetWallRecords(start, count));
                    break;

                case 2:
                    if (id == null)
                        return new HttpStatusCodeResult(404);
                    int int_id = Convert.ToInt32(id);
                    var group_page = db.Groups.FirstOrDefault(x1 => x1.Id == int_id);
                    res.AddRange(group_page.GetWallRecords(start, count));
                    break;

                case 3:
                    
                    
                    if (check_id==null)
                        return new HttpStatusCodeResult(404);
                    
                    var user_page2 = db.Users.FirstOrDefault(x1 => x1.Id == check_id);
                    res.AddRange(user_page2.GetNewsRecords(start, count));

                    
                    break;
            }

            return PartialView(res);
        }


        [Authorize]
        public ActionResult LoadNewMessages(int dialog)
        {
            string check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            string type_message_need = (string)Session["NewMessageType"];

            switch (type_message_need)
            {
                case "1":
                    return Redirect(Url.Action("ListMessagesUser", "SocialNetwork", new { id = dialog, new_m = true }));//start = 0, int count = 10
                    break;
                case "2":
                    //отправить колличество
                    var user = db.Users.FirstOrDefault(x1 => x1.Id == check_id);
                    if (!db.Entry(user).Collection(x1 => x1.MessageNeedRead).IsLoaded)
                        db.Entry(user).Collection(x1 => x1.MessageNeedRead).Load();
                    return Redirect(Url.Action("ReturnStringPartial", "SocialNetwork",new { str=user.MessageNeedRead.Count.ToString() }));
                    break;

                case "3":
                    //shortchat
                    break;

            }

            return View();


            //List<Message> res = new List<Message>();
            //var user = db.Users.First(x1 => x1.Id == check_id);
            ////if (!db.Entry(user).Collection(x1 => x1.Chats).IsLoaded)
            ////    db.Entry(user).Collection(x1 => x1.Chats).Load();
            ////var chat = user.Chats.FirstOrDefault(x1 => x1.Id == dialog);
            ////if(chat==null)
            ////    return new HttpStatusCodeResult(404);

            //if (!db.Entry(user).Collection(x1 => x1.MessageNeedRead).IsLoaded)
            //    db.Entry(user).Collection(x1 => x1.MessageNeedRead).Load();
            //res.AddRange(user.MessageNeedRead.Where(x1 =>
            //{
            //    if (!db.Entry(x1).Reference(x2 => x2.Chat).IsLoaded)
            //        db.Entry(x1).Reference(x2 => x2.Chat).Load();
            //    if (x1.Chat.Id == dialog)
            //        return true;
            //    return false;
            //}));

            ////вынести + написать представление которое должно стандартно отобразиться
            //return PartialView("ListMessagesUser", new Chat() { Messages = res });//"SocialNetwork",
        }
        [Authorize]
        public JsonResult SendNewMessageForm(int dialog, HttpPostedFileBase[] uploadImage, string text)
        {
            string check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            //TODO проверить доступ к диалогу
            var user = db.Users.FirstOrDefault(x1=>x1.Id==check_id);
            if (!db.Entry(user).Collection(x1 => x1.Chats).IsLoaded)
                db.Entry(user).Collection(x1 => x1.Chats).Load();
            var chat = user.Chats.FirstOrDefault(x1=>x1.Id==dialog);
            if(chat==null)
                return null;
            var list_img_byte = Get_photo_post(uploadImage);
            
            Message res = new Message() { Text=text, CreatorId = check_id, ChatId=dialog };

            db.Messages.Add(res);
            db.SaveChanges();

            List<Image> image_list = new List<Models.SocialNetwork.Image>();
            foreach (var i in list_img_byte)
            {
                //кратинки еще и в бд и тдтд

                var img = new Image() { Data = i, UserId = check_id, MessageId = res.Id };
                db.ImagesSocial.Add(img);
                db.SaveChanges();
                image_list.Add(img);

            }
            
            if (!db.Entry(chat).Collection(x1 => x1.Users).IsLoaded)
                db.Entry(chat).Collection(x1 => x1.Users).Load();
            foreach (var i in chat.Users)
                res.UserNeedRead.Add(i);
            db.SaveChanges();

            user.SendNewMessage(res);

            return Json(new { dialog= dialog });
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


        [Authorize]
        public ActionResult ListMessagesUser(int? id,bool? new_m, int start=0, int count=10)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(404);
            }
                string check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var user = db.Users.FirstOrDefault(x1 => x1.Id == check_id);
            Chat res = new Chat();
            List<Message> not_res = new List<Message>();
            if (!db.Entry(user).Collection(x1 => x1.Chats).IsLoaded)
                db.Entry(user).Collection(x1 => x1.Chats).Load();
            
                var dialog = user.Chats.FirstOrDefault(x1 => x1.Id == id);
                if (dialog == null)
                    return new HttpStatusCodeResult(404);
                res = new Chat() {Id=dialog.Id };
            if (!db.Entry(dialog).Collection(x1 => x1.Messages).IsLoaded)
                db.Entry(dialog).Collection(x1 => x1.Messages).Load();

            //dialog.Messages= dialog.Messages.Reverse().ToList();
            start = start > 0 ? start - 1 : 0;
            start = dialog.Messages.Count - start - count;
            not_res.AddRange(dialog.Messages.Skip(start).Take(count));
            //if (new_m == true)
            //{
            //    ((List<Message>)res.Messages).RemoveRange(res.Messages.Where());
            //}
            foreach(var i in not_res)
            {
                if (!db.Entry(i).Collection(x1 => x1.UserNeedRead).IsLoaded)
                    db.Entry(i).Collection(x1 => x1.UserNeedRead).Load();
                var us = i.UserNeedRead.FirstOrDefault(x1=>x1.Id==check_id);
                if (us != null)
                {
                    i.UserNeedRead.Remove(us);
                }
                if (new_m == true)
                {
                    if (us != null)
                    {
                        res.Messages.Add(i);
                    }
                }
                else
                    res.Messages.Add(i);
            }
            db.SaveChanges();
            return PartialView(res);
        }

        [AllowAnonymous]
        public ActionResult LoadShowImageRecord(int? id)
        {
            //TODO проверить доступ загрузить соседние id и тд
            string check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            
            if(id==null)
                return new HttpStatusCodeResult(404);

            ViewBag.check_id = check_id;

            var img = db.ImagesSocial.FirstOrDefault(x1=>x1.Id==id);
            if (img.RecordId != null)
            {
                img.Record_NM = db.Record.FirstOrDefault(x1 => x1.Id == img.RecordId);
                img.Record_NM.Image = img;
                if (!db.Entry(img.Record_NM).Collection(x1 => x1.UsersLikes).IsLoaded)
                    db.Entry(img.Record_NM).Collection(x1 => x1.UsersLikes).Load();
            }
            else
            {
                img.Record_NM = new Record();
                img.Record_NM.Image = img;
                if (!db.Entry(img.Record_NM).Collection(x1 => x1.UsersLikes).IsLoaded)
                    db.Entry(img.Record_NM).Collection(x1 => x1.UsersLikes).Load();
            }
            
            //Image res = new Models.SocialNetwork.Image();


            return PartialView(img.Record_NM);
        }


        [AllowAnonymous]
        public ActionResult ReturnStringPartial(string str)
        {
            ViewBag.str = str;
            return View();

        }
        }
}