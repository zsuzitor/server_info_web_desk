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
using server_info_web_desk.Models;
using System.Data.Entity;
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
            string check_id = ApplicationUser.GetUserId();
            
            //id = id ?? check_id;
            if (id == null&&check_id==null)
            {
                return RedirectToAction("Index", "SocialNetwork",new { });
            }
            if(id==null)
                return RedirectToAction("PersonalRecord", "SocialNetwork", new { id = check_id });

            var user = ApplicationUser.GetUser(id) ;
            
            PersonalRecordView res = new PersonalRecordView(user);
            res.IdUser = check_id;

            res.CanAddMeme = user.CanAddRecordWall(res.IdUser);

            res.IdMainAlbum = user.GetAlbums(null,0,1).First().Id;
            res.IdNotMainAlbum = user.GetAlbums(null, 1, 1).First().Id;

            res.Albums=Models.SocialNetwork.Album.GetAlbumShortListForView(user.Albums,2);

            
            res.MainImage = Models.SocialNetwork.Album.GetLastImageAlbum(user.Albums.First(),1).FirstOrDefault(); 

            res.Image.AddRange(Models.SocialNetwork.Album.GetLastImageAlbum(user.Albums.ElementAt(1), 4));

            res.Group.AddRange(user.UserGroupToShort(0,6));

            //TODO сейчас плохо будет работать
            res.GroupCount = user.Group.Count;

            res.CanAddFriend = user.CanFollow(check_id);

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Set<ApplicationUser>().Attach(user);
                if (!db.Entry(user).Collection(x1 => x1.Friends).IsLoaded)
                    db.Entry(user).Collection(x1 => x1.Friends).Load();
            }
            res.Friends.AddRange(user.Friends.Skip(user.Friends.Count-6>0? (user.Friends.Count-6):0).Select(x1=>new Models.ApplicationUserShort(x1)));
            Session["NewMessageType"] = "2";
            return View(res);
        }
        [Authorize]
        //TODO
        public ActionResult EditPersonalRecord()
        {

            ApplicationUser user = ApplicationUser.GetUser(ApplicationUser.GetUserId());
            if (user == null)
                return new HttpStatusCodeResult(404);

            Session["NewMessageType"] = "2";
            return View(user);
        }


        [AllowAnonymous]
        //TODO
        public ActionResult GroupRecord(int id)
        {
            string check_id = ApplicationUser.GetUserId();
           // var ffff = db.Record.ToList();
            Group group = Group.GetGroup(id) ;
            if (group == null)
                return new HttpStatusCodeResult(404);

            if (!group.HaveAccessGroup(check_id))
            {
                //TODO тут отправлять усеченную версию группы для тех кто не подписан и группа закрытая
                return new HttpStatusCodeResult(404);
            }
            

            GroupRecordView res = new GroupRecordView(group);
            res.IdUser = check_id;

            if (group.HaveAccessAdminGroup(check_id))
                res.Admin = true;
            
                res.CanFollow = group.CanFollow(check_id);


            res.Users=group.GetUserShortList(-6);
            res.Admins=group.GetAdminShortList( 6);

            res.IdMainAlbum = group.GetAlbums(null, 0, 1).First().Id;
            res.IdNotMainAlbum = group.GetAlbums(null, 1, 1).First().Id;

            res.Albums = Models.SocialNetwork.Album.GetAlbumShortListForView((List<Album>)group.Albums, 2);
            res.MainImage= Models.SocialNetwork.Album.GetLastImageAlbum(group.Albums.First(), 1).FirstOrDefault();

            res.Image = Models.SocialNetwork.Album.GetLastImageAlbum(group.Albums.ElementAt(1), 4);
            
            res.WallMeme.AddRange(group.GetWallRecords(0, 10));

           
            res.CanAddMeme = group.CanAddMeme(check_id);

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
            string check_id = ApplicationUser.GetUserId();
            ViewBag.check_id = check_id;
            Session["NewMessageType"] = "2";
            return View();
        }

        [Authorize]
        //TODO
        public ActionResult Messages()
        {
            ApplicationUser user = ApplicationUser.GetUser(ApplicationUser.GetUserId());
            if (user == null)
                return new HttpStatusCodeResult(404);
           

            Session["NewMessageType"] = "3";
            return View();
        }
        [AllowAnonymous]
        //TODO вместе с AlbumsGroup редиректить на 1 метод в котором все будет происходить????
        public ActionResult AlbumsPerson(string id,int? select_id)
        {
            //TODO проверять есть ли доступ
            string check_id = ApplicationUser.GetUserId();
            ListAlbumsShortView res = new ListAlbumsShortView() {UserId= check_id,PageUserId=id, SelectAlbum=select_id };
            //var userPage = db.Users.FirstOrDefault(x1 => x1.Id == id);
            ApplicationUser userPage = ApplicationUser.GetUser(id);
            if (userPage == null)
                return new HttpStatusCodeResult(404);
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Set<ApplicationUser>().Attach(userPage);
                if (!db.Entry(userPage).Collection(x1 => x1.Albums).IsLoaded)
                    db.Entry(userPage).Collection(x1 => x1.Albums).Load();
            }
            
                
            
             res.AlbumList.AddRange(Models.SocialNetwork.Album.GetAlbumShortListForView(userPage.Albums, userPage.Albums.Count));

            Session["NewMessageType"] = "2";
            return View("Albums",  res);//"SocialNetwork",
        }
        [AllowAnonymous]
        //TODOвместе с AlbumsPerson редиректить на 1 метод в котором все будет происходить????
        public ActionResult AlbumsGroup(int id, int? select_id)
        {
            string check_id = ApplicationUser.GetUserId();
            ListAlbumsShortView res = new ListAlbumsShortView() { UserId = check_id, PageGroupId = id, SelectAlbum = select_id };
            Group group = Group.GetGroup(id);
            if (group == null)
                return new HttpStatusCodeResult(404);
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Set<Group>().Attach(group);
                if (!db.Entry(group).Collection(x1 => x1.Albums).IsLoaded)
                    db.Entry(group).Collection(x1 => x1.Albums).Load();
            }
                

            
            res.AlbumList.AddRange(Models.SocialNetwork.Album.GetAlbumShortListForView((List<Album>)group.Albums, group.Albums.Count));
            Session["NewMessageType"] = "2";
            return View("Albums",  res);//"SocialNetwork",
        }



        [Authorize]
        //TODO
        public ActionResult Dialog(int? id = null, string user_id = null)//
        {

            if(id==null&& user_id==null)
                return new HttpStatusCodeResult(404);

            //TODO УРОВЕНЬ ДОСТУПА

            ApplicationUser user = ApplicationUser.GetUser(ApplicationUser.GetUserId());
            if (user == null)
                return new HttpStatusCodeResult(404);
            Chat res = new Chat();

            //if (!db.Entry(user).Collection(x1 => x1.Chats).IsLoaded)
            //    db.Entry(user).Collection(x1 => x1.Chats).Load();
            if (id != null)
            {
                var dialog = user.GetChat(id);
                if (dialog == null)
                    return new HttpStatusCodeResult(404);
                res = dialog;

            }
            else
            if (user_id != null)
            {
                Chat chat = user.GetChat(user_id);

                //СОЗДАТЬ НОВЫЙ ЧАТ
                if (chat == null)
                {
                    ApplicationUser user2 = ApplicationUser.GetUser(user_id);
                    
                    //var user2 = db.Users.FirstOrDefault(x1=>x1.Id== user_id);
                    if (user2 == null)
                        return new HttpStatusCodeResult(404);
                    chat = new Chat() { CreatorId = user.Id };
                    using (ApplicationDbContext db = new ApplicationDbContext())
                    {
                        db.Set<ApplicationUser>().Attach(user2);
                        try
                        {
                            db.Set<ApplicationUser>().Attach(user);
                        }
                        catch
                        {
                            //оставить именно так а не методом
                            user = ApplicationUser.GetUser(user.Id,db);
                        }
                        //db.Set<ApplicationUser>().Attach(user);
                        
                        db.Chats.Add(chat);
                        db.SaveChanges();
                        chat.Users.Add(user);
                        chat.Users.Add(user2);
                        db.SaveChanges();
                    }
                }
                return RedirectToAction("Dialog", "SocialNetwork",new { id= chat.Id });
            }

            Session["NewMessageType"] = "1";

            return View(res);
        }

        [AllowAnonymous]
        //TODO
        public ActionResult Friends(string id)
        {

            string check_id = ApplicationUser.GetUserId();

            ViewBag.id = id;
            ViewBag.check_id = check_id;
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
            ApplicationUser user = ApplicationUser.GetUser(id);
            if (user == null)
                return new HttpStatusCodeResult(404);
            ViewBag.id = id;

            GroupsListView res = new GroupsListView();

            

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
            ApplicationUser user = ApplicationUser.GetUser(ApplicationUser.GetUserId());
            ViewBag.id = user?.Id;
            if (user != null)
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    db.Set<ApplicationUser>().Attach(user);
                    if (!db.Entry(user).Collection(x1 => x1.MessageNeedRead).IsLoaded)
                        db.Entry(user).Collection(x1 => x1.MessageNeedRead).Load();
                }
                    
                ViewBag.CountNewMessage = user.MessageNeedRead.Count;
            }
           
            return PartialView();
        }




        //---------------------------------------------------ACTION---------------------------------
        [Authorize]
        [HttpPost]
        public JsonResult LikeRecord(int? id)
        {
            string check_id = ApplicationUser.GetUserId();
            
            var record = Record.GetRecord(id) ;
            if (record == null)
                return Json(null);

            bool? red_heart=record.LikeAction(check_id);
            
            return Json(new { red_heart, id,count= record.UsersLikes.Count });
        }
        [Authorize]
        public ActionResult GroupCreate([Bind(Include="Name")]GroupShort a)
        {
            string check_id = ApplicationUser.GetUserId();
            if (string.IsNullOrWhiteSpace(a.Name))
                return new HttpStatusCodeResult(404);
            Group res = new Group() { Name=a.Name, MainAdminId=check_id };
            ApplicationUser user = ApplicationUser.GetUser(check_id);
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
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
                
                db.Set<ApplicationUser>().Attach(user);
                
                res.Admins.Add(user);
                res.Users.Add(user);
                db.SaveChanges();
            }
            return RedirectToAction("GroupRecord", "SocialNetwork",new {id=res.Id });
        }

        [Authorize]
        public ActionResult FollowGroup(int? id)
        {
            ApplicationUser user = ApplicationUser.GetUser(ApplicationUser.GetUserId());
            if (user == null)
                return new HttpStatusCodeResult(404);

            int res = 0;
            
            Group group = Group.GetGroup(id);
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
               
                if (group == null)
                    return new HttpStatusCodeResult(404);
                res = group.CanFollow(user.Id);
                db.Set<ApplicationUser>().Attach(user);
                try
                {
                    db.Set<Group>().Attach(group);
                }
                catch
                {
                    //оставить именно так а не методом
                    //group = db.Groups.First(x1 => x1.Id == group.Id);
                    group = Group.GetGroup(group.Id, db);
                }
               
                if (res == 1)
                {
                    group.Users.Add(user);
                    res = -1;
                }
                else
                if (res == 2)
                {
                    group.Users.Remove(user);
                    res = -1;
                }
                


                db.SaveChanges();
            }
               

            
            //res = !res;
            return Redirect(Url.Action("FollowGroupPartial", "SocialNetwork", new { IdGroup = id, CanFollow = res }));
        }

        [Authorize]
        public ActionResult DeleteFriend(string id)
        {
            //удалять прямо из списка
            if(string.IsNullOrWhiteSpace(id))
                return new HttpStatusCodeResult(404);
            ApplicationUser user = ApplicationUser.GetUser(ApplicationUser.GetUserId());
            if (user == null)
                return new HttpStatusCodeResult(404);
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Set<ApplicationUser>().Attach(user);
                if (!db.Entry(user).Collection(x1 => x1.Friends).IsLoaded)
                    db.Entry(user).Collection(x1 => x1.Friends).Load();
                var us = user.Friends.FirstOrDefault(x1 => x1.Id == id);
                if (us != null)
                {
                    user.Friends.Remove(us);
                    if (!db.Entry(user).Collection(x1 => x1.Followers).IsLoaded)
                        db.Entry(user).Collection(x1 => x1.Followers).Load();
                    user.Followers.Add(us);
                }
                else
                {
                    if (!db.Entry(user).Collection(x1 => x1.FollowUser).IsLoaded)
                        db.Entry(user).Collection(x1 => x1.FollowUser).Load();
                    us = user.FollowUser.FirstOrDefault(x1 => x1.Id == id);
                    if (us != null)
                    {
                        user.FollowUser.Remove(us);
                    }
                }
                
                db.SaveChanges();
            }
            return Redirect(Url.Action("FollowUserPartial", "SocialNetwork", new { Iduser = id, CanAddFriend = true }));
        }


        [Authorize]
        public ActionResult FollowPerson(string id)
        {
            //взаимодействовать на странице пользователя
            ApplicationUser user_act = ApplicationUser.GetUser(ApplicationUser.GetUserId());
            if (user_act == null)
                return new HttpStatusCodeResult(404);
            int res = 0;
            ApplicationUser user = ApplicationUser.GetUser(id);
            if (user == null)
                return new HttpStatusCodeResult(404);
            
            res = user.CanFollow(user_act.Id);
            
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                
                db.Set<ApplicationUser>().Attach(user);
                try
                {
                    db.Set<ApplicationUser>().Attach(user_act);
                }
                catch
                {
                    //оставить именно так а не методом
                    user_act = ApplicationUser.GetUser(ApplicationUser.GetUserId(), db);
                   // user_act = db.Users.First(x1 => x1.Id == id);
                }


                switch (res)
                {
                    case 1:
                        if (!db.Entry(user).Collection(x1 => x1.Followers).IsLoaded)
                            db.Entry(user).Collection(x1 => x1.Followers).Load();
                        user.Followers.Add(user_act);
                        res = 3;
                        break;
                    case 2:
                        if (!db.Entry(user).Collection(x1 => x1.Friends).IsLoaded)
                            db.Entry(user).Collection(x1 => x1.Friends).Load();
                        if (!db.Entry(user).Collection(x1 => x1.FriendUser).IsLoaded)
                            db.Entry(user).Collection(x1 => x1.FriendUser).Load();
                        user.Friends.Remove(user_act);
                        user.FriendUser.Remove(user_act);
                        user.FollowUser.Add(user_act);
                        res = 3;
                        break;
                    case 3:
                        if (!db.Entry(user).Collection(x1 => x1.Followers).IsLoaded)
                            db.Entry(user).Collection(x1 => x1.Followers).Load();
                        user.Followers.Remove(user_act);
                        res = 1;
                        break;
                    case 4:
                        if (!db.Entry(user).Collection(x1 => x1.FollowUser).IsLoaded)
                            db.Entry(user).Collection(x1 => x1.FollowUser).Load();
                        if (!db.Entry(user).Collection(x1 => x1.Friends).IsLoaded)
                            db.Entry(user).Collection(x1 => x1.Friends).Load();
                        if (!db.Entry(user).Collection(x1 => x1.FriendUser).IsLoaded)
                            db.Entry(user).Collection(x1 => x1.FriendUser).Load();
                        user.FollowUser.Remove(user_act);
                        user.Friends.Add(user_act);
                        user.FriendUser.Add(user_act);
                        res = 2;
                        break;
                }
                
                
                db.SaveChanges();
            }

            //res = !res;
            return Redirect(Url.Action("FollowUserPartial", "SocialNetwork", new { Iduser = id, CanAddFriend = res }));
        }



        [Authorize]
        public ActionResult AddMemePerson(string id_user, HttpPostedFileBase[] uploadImage,string text)
        {
            string check_id = ApplicationUser.GetUserId();
            //TODO проверять есть ли доступ к добавлению мемов на чужую стену
            //int gg = Request.Files.Count;

            bool access = false;
            id_user = id_user ?? check_id;
            //var user = db.Users.FirstOrDefault(x1 => x1.Id == id_user);
            ApplicationUser user = ApplicationUser.GetUser(id_user);
            if (user == null)
                return new HttpStatusCodeResult(404);

            access = user.CanAddRecordWall(check_id);
            
            if (!access)
                return RedirectToAction("PersonalRecord", "SocialNetwork", new { id = id_user });
                //return PartialView(null);

            var list_img_byte = Get_photo_post(uploadImage);

            Record record = Record.AddRecordMem(check_id, null, list_img_byte, text);
            user.AddRecordWall(record);
            
            return RedirectToAction("PersonalRecord", "SocialNetwork",new {id= id_user });
           // return PartialView(record);
        }
        [Authorize]
        public ActionResult AddMemeGroup(int id_group, HttpPostedFileBase[] uploadImage, string text)
        {
            string check_id = ApplicationUser.GetUserId();
            Group group = Group.GetGroup(id_group);
            if (group == null)
                return new HttpStatusCodeResult(404);

            bool CanAddMeme = group.CanAddMeme(check_id);

            if (!CanAddMeme)
                return new HttpStatusCodeResult(404);
            
            var list_img_byte = Get_photo_post(uploadImage);

            var record=Record.AddRecordMem(check_id, id_group, list_img_byte, text);
            group.AddRecordMemeWall(record);
            return RedirectToAction("GroupRecord", "SocialNetwork",new {id= id_group });
        }

        [Authorize]
        public ActionResult AddImagePerson(string text, HttpPostedFileBase[] uploadImage, int? album_id)
        {
            if(uploadImage.Count()<1|| album_id==null)
                return new HttpStatusCodeResult(404);
            ApplicationUser user = ApplicationUser.GetUser(ApplicationUser.GetUserId());
            if (user == null)
                return new HttpStatusCodeResult(404);

            var album = user.GetAlbums(album_id).FirstOrDefault();
            if(album==null)
                return new HttpStatusCodeResult(404);
            var list_img_byte = Get_photo_post(uploadImage);

            var record=Record.AddRecordImage(album, user,null, list_img_byte, text);
            if(record == null)
                return new HttpStatusCodeResult(404);
            user.AddRecordWall(record);

            return RedirectToAction("PersonalRecord", "SocialNetwork", new { id = user.Id });
        }
        [Authorize]
        public ActionResult AddImageGroup(string text, HttpPostedFileBase[] uploadImage, int? album_id,int? id_group)
        {
            if (uploadImage.Count() < 1 || album_id == null|| id_group==null)
                return new HttpStatusCodeResult(404);


            string check_id = ApplicationUser.GetUserId();

            Group group = Group.GetGroup(id_group);
            if (group == null)
                return new HttpStatusCodeResult(404);

            ApplicationUser user = null;

            bool can_add = group.CanAddMeme(check_id);
            if (!can_add)
                return new HttpStatusCodeResult(404);


            var album = group.GetAlbums(album_id).FirstOrDefault();
            //TODO проверка
            var ch_al = group.GetAlbums(null,0,1).First();

            if (can_add&&ch_al.Id==album.Id)
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    db.Set<Group>().Attach(group);
                    if (!db.Entry(group).Collection(x1 => x1.Admins).IsLoaded)
                        db.Entry(group).Collection(x1 => x1.Admins).Load();
                }
                    user = group.Admins.FirstOrDefault(x1 => x1.Id == check_id);
                if (user == null)
                    can_add = false;
            }
            if(!can_add)
                return new HttpStatusCodeResult(404);

            if (album == null)
                return new HttpStatusCodeResult(404);
            var list_img_byte = Get_photo_post(uploadImage);
            if(user==null)
                 user = ApplicationUser.GetUser(check_id);
            //user =db.Users.FirstOrDefault(x1 => x1.Id == check_id);

            var record = Record.AddRecordImage(album, user, group, list_img_byte, text);
            
            group.AddRecordMemeWall(record);
            return RedirectToAction("GroupRecord", "SocialNetwork", new { id = id_group });
        }


        [Authorize]
        //TODO
        public ActionResult SaveChangesPersonalRecord(Models.ApplicationUser a)
        {
            //TODO валидация
            ApplicationUser user = ApplicationUser.GetUser(ApplicationUser.GetUserId());
            if (user == null)
                return new HttpStatusCodeResult(404);
            
            
                user.ChageUserData(a);

            return RedirectToAction("EditPersonalRecord", "SocialNetwork",new { });
        }

        //------------------------------------------------------------

        [AllowAnonymous]
        public ActionResult LoadImagesAlbum(int id, int start, int count)
        {
            List<Image> res = new List<Models.SocialNetwork.Image>();
            Album album = null;
            using (ApplicationDbContext db = new ApplicationDbContext())
                album = db.Albums.FirstOrDefault(x1 => x1.Id == id);
            album.GetAlbumImages(start, count);
            res.AddRange(album.Images.Select(x1=>x1.Image));//.Select(x1=>new ImageShort() {Id=(int)x1.ImageId,Data=x1.Image.Data }

            return PartialView(res);
            
        }

        [AllowAnonymous]
        public ActionResult GroupsListPartial(string id,int start, int count)
        {
            ApplicationUser user = ApplicationUser.GetUser(id);
            if (user == null)
                return new HttpStatusCodeResult(404);
            List<GroupShort> res=user.UserGroupToShort(start, count);

            return View(res);
        }




        [Authorize]
        public ActionResult DialogListPartial(int start, int count)
        {
            ApplicationUser user = ApplicationUser.GetUser(ApplicationUser.GetUserId());
            if (user == null)
                return new HttpStatusCodeResult(404);
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Set<ApplicationUser>().Attach(user);
                if (!db.Entry(user).Collection(x1 => x1.Chats).IsLoaded)
                    db.Entry(user).Collection(x1 => x1.Chats).Load();
            }

            
            List<ChatShort> res = new List<ChatShort>();
            res.AddRange(user.Chats.Select(x1 => x1.GetChatShort()));


            return PartialView(res);
        }


        [AllowAnonymous]
        public ActionResult MemeRecordListPartial(string id,int type,int start,int count)
        {
            //type: 1-PersonalRecord 2-GroupRecord 3-NewsPresonal
            List<Record> res = new List<Record>();

            //@Healpers.MemeRecord(Model, (string)ViewBag.check_id)


            ApplicationUser user = ApplicationUser.GetUser(ApplicationUser.GetUserId());
            ViewBag.check_id = user?.Id;
            switch (type)
            {
                case 1:
                    if (id == null)
                        return new HttpStatusCodeResult(404);
                    var user_page = ApplicationUser.GetUser(id);
                    res.AddRange(user_page.GetWallRecords(start, count));
                    break;

                case 2:
                    if (id == null)
                        return new HttpStatusCodeResult(404);
                    int int_id = Convert.ToInt32(id);
                    Group group = Group.GetGroup(int_id);
                    if (group == null)
                        return new HttpStatusCodeResult(404);
                    res.AddRange(group.GetWallRecords(start, count));
                    break;

                case 3:
                    
                    
                    if (user?.Id == null)
                        return new HttpStatusCodeResult(404);
                    
                    //var user_page2 = db.Users.FirstOrDefault(x1 => x1.Id == user.Id);
                    res.AddRange(user.GetNewsRecords(start, count));

                    
                    break;
            }

            return PartialView(res);
        }


        [Authorize]
        public ActionResult LoadNewMessages(int dialog)
        {
            string check_id = ApplicationUser.GetUserId();
            string type_message_need = (string)Session["NewMessageType"];

            switch (type_message_need)
            {
                case "1":
                    return Redirect(Url.Action("ListMessagesUser", "SocialNetwork", new { id = dialog, new_m = true }));//start = 0, int count = 10
                    break;
                case "2":
                    //отправить колличество
                    ApplicationUser user = ApplicationUser.GetUser(check_id);
                    using (ApplicationDbContext db = new ApplicationDbContext())
                    {
                        db.Set<ApplicationUser>().Attach(user);
                        if (!db.Entry(user).Collection(x1 => x1.MessageNeedRead).IsLoaded)
                            db.Entry(user).Collection(x1 => x1.MessageNeedRead).Load();
                    }
                        
                    return Redirect(Url.Action("ReturnStringPartial", "SocialNetwork",new { str=user.MessageNeedRead.Count.ToString() }));
                    break;

                case "3":
                    //shortchat
                    break;

            }

            return View();

        }
        [Authorize]
        public JsonResult SendNewMessageForm(int dialog, HttpPostedFileBase[] uploadImage, string text)
        {
            ApplicationUser user = ApplicationUser.GetUser(ApplicationUser.GetUserId());
            if (user == null)
                return null;

            var list_img_byte = Get_photo_post(uploadImage);
            var res=user.SendNewMessage(dialog, list_img_byte,text);

            return Json(new { dialog= dialog });
        }

       


        //[ChildActionOnly]
        [Authorize]
        public ActionResult FollowGroupPartial(int IdGroup,bool? CanFollow)
        {
            string check_id = ApplicationUser.GetUserId();
            ViewBag.IdGroup = IdGroup;
            ViewBag.check_id = check_id;

            ViewBag.CanFollow = CanFollow;
            return PartialView();
        }
        [Authorize]
        public ActionResult FollowUserPartial(string Iduser, int CanAddFriend)
        {
            string check_id = ApplicationUser.GetUserId();
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
            ApplicationUser user = ApplicationUser.GetUser(ApplicationUser.GetUserId());
            if (user == null)
                return new HttpStatusCodeResult(404);
            var res=user.GetListMessages(id, new_m, start, count);
            return PartialView(res);
        }

        [AllowAnonymous]
        public ActionResult LoadShowImageRecord(int? id)
        {
            //TODO проверить доступ загрузить соседние id и тд
            string check_id = ApplicationUser.GetUserId();

            if (id==null)
                return new HttpStatusCodeResult(404);

            ViewBag.check_id = check_id;
            Image img = null;
            using (ApplicationDbContext db = new ApplicationDbContext())
                img = db.ImagesSocial.FirstOrDefault(x1 => x1.Id == id);
            img.GetRecordForShow();

           
            return PartialView(img.Record_NM);
        }


        [AllowAnonymous]
        public ActionResult ReturnStringPartial(string str)
        {
            ViewBag.str = str;
            return PartialView();

        }
        [AllowAnonymous]
        public ActionResult LoadFollowersGroup(string id, int start, int count)
        {
            if (string.IsNullOrWhiteSpace(id))
                return new HttpStatusCodeResult(404);
            int int_id = Convert.ToInt32(id);
            Group group = Group.GetGroup(int_id);
            if (group == null)
                return new HttpStatusCodeResult(404);

            List<Models.ApplicationUserShort> res = new List<Models.ApplicationUserShort>();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Set<Group>().Attach(group);
                if (!db.Entry(group).Collection(x1 => x1.Users).IsLoaded)
                    db.Entry(group).Collection(x1 => x1.Users).Load();
            }
                
            start = start > 0 ? start - 1 : 0;
            start = group.Users.Count - start - count;
            res.AddRange(group.Users.Skip(start).Take(count).Select(x1 => new Models.ApplicationUserShort(x1)));


            return PartialView("ListUsers", res);

        }


        [AllowAnonymous]
        public ActionResult LoadFriends(string id,int start, int count)
        {
            ApplicationUser user = ApplicationUser.GetUser(id);
            if (user == null)
                return new HttpStatusCodeResult(404);

            List<Models.ApplicationUserShort> res = new List<Models.ApplicationUserShort>();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Set<ApplicationUser>().Attach(user);
                if (!db.Entry(user).Collection(x1 => x1.Friends).IsLoaded)
                    db.Entry(user).Collection(x1 => x1.Friends).Load();
            }
                
            start = start > 0 ? start - 1 : 0;
            start = user.Friends.Count - start - count;
            res.AddRange(user.Friends.Skip(start).Take(count).Select(x1 => new Models.ApplicationUserShort(x1)));

            
            return PartialView("ListUsers",res);

        }
        [AllowAnonymous]
        public ActionResult LoadFollowers(string id, int start, int count)
        {
            ApplicationUser user = ApplicationUser.GetUser(id);
            if (user == null)
                return new HttpStatusCodeResult(404);

            List<Models.ApplicationUserShort> res = new List<Models.ApplicationUserShort>();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Set<ApplicationUser>().Attach(user);
                if (!db.Entry(user).Collection(x1 => x1.Followers).IsLoaded)
                    db.Entry(user).Collection(x1 => x1.Followers).Load();
            }
                
            start = start > 0 ? start - 1 : 0;
            start = user.Followers.Count - start - count;
            res.AddRange(user.Followers.Skip(start).Take(count).Select(x1 => new Models.ApplicationUserShort(x1)));

            return PartialView("ListUsers", res);

        }
        [AllowAnonymous]
        public ActionResult LoadFollowUser(string id, int start, int count)
        {
            ApplicationUser user = ApplicationUser.GetUser(id);
            if (user == null)
                return new HttpStatusCodeResult(404);

            List<Models.ApplicationUserShort> res = new List<Models.ApplicationUserShort>();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Set<ApplicationUser>().Attach(user);
                
                if (!db.Entry(user).Collection(x1 => x1.FollowUser).IsLoaded)
                    db.Entry(user).Collection(x1 => x1.FollowUser).Load();
            }
               
            start = start > 0 ? start - 1 : 0;
            start = user.FollowUser.Count - start - count;
            res.AddRange(user.FollowUser.Skip(start).Take(count).Select(x1 => new Models.ApplicationUserShort(x1)));

            return PartialView("ListUsers", res);

        }

    }
}