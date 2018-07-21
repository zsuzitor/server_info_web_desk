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
                return RedirectToAction("Login", "Account", new { });
                //http://localhost:64992/Account/Login
            }
            if (id==null)
                return RedirectToAction("PersonalRecord", "SocialNetwork", new { id = check_id });

            var user = ApplicationUser.GetUser(id) ;
            
            PersonalRecordView res = new PersonalRecordView(user);
            res.IdUser = check_id;

            res.CanAddMeme = user.CanAddRecordWall(res.IdUser);

            res.IdMainAlbum = user.GetAlbums(null,0,1,true,false).First().Id;
            res.IdNotMainAlbum = user.GetAlbums(null, 1, 1, true, false).First().Id;

            //res.Albums=Models.SocialNetwork.Album.GetAlbumShortListForView(user.Albums,2);
            res.Albums.AddRange((user.GetAlbums(null, 0, 2)).Select(x1 => new AlbumShort(x1)));

            res.MainImage = Models.SocialNetwork.Album.GetLastImageAlbum(user.Albums.First(),1).FirstOrDefault(); 

            res.Image.AddRange(Models.SocialNetwork.Album.GetLastImageAlbum(user.Albums.ElementAt(1), 4));

            res.Group.AddRange(user.UserGroupToShort(0,6));

            //TODO сейчас плохо будет работать
            res.GroupCount = user.Group.Count;

            res.CanAddFriend = user.CanFollow(check_id);

           

            res.Friends.AddRange((user.GetFriends(0,6))
                .Select(x1 => new Models.ApplicationUserShort(x1)));
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


            res.Users=group.GetUserShortList(0,6);
            res.Admins=group.GetAdminShortList( 6);

            res.IdMainAlbum = group.GetAlbums(null, 0, 1, true, false).First().Id;
            res.IdNotMainAlbum = group.GetAlbums(null, 1, 1, true, false).First().Id;


            //res.Albums = Models.SocialNetwork.Album.GetAlbumShortListForView((List<Album>)group.Albums, 2);
            res.Albums.AddRange((group.GetAlbums(null, 0, 2, true, false)).Select(x1 => new AlbumShort(x1)));

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
            ListAlbumsShortView res = new ListAlbumsShortView() {
                UserId = check_id,PageUserId=id, SelectAlbum=select_id };
            //var userPage = db.Users.FirstOrDefault(x1 => x1.Id == id);
            ApplicationUser userPage = ApplicationUser.GetUser(id);
            if (userPage == null)
                return new HttpStatusCodeResult(404);

           
            res.AlbumList.AddRange((userPage.GetAlbums(null,0,null, true, false)).Select(x1=>new AlbumShort(x1)));

            //провериить надо ли формы отображать
            int access = 10;
            
            if (check_id == id)
            {
                access = 0;
            }
            ViewBag.access = access;

            Session["NewMessageType"] = "2";
            return View("Albums",  res);//"SocialNetwork",
        }
        [AllowAnonymous]
        //TODOвместе с AlbumsPerson редиректить на 1 метод в котором все будет происходить????
        public ActionResult AlbumsGroup(int id, int? select_id)
        {
            string check_id = ApplicationUser.GetUserId();
            ListAlbumsShortView res = new ListAlbumsShortView() {
                UserId = check_id, PageGroupId = id, SelectAlbum = select_id };
            Group group = Group.GetGroup(id);
            if (group == null)
                return new HttpStatusCodeResult(404);

            //провериить надо ли формы отображать
            int access = 10;

            access = (group.HaveAccessAdminGroup(check_id)?0:10);
            if (access == 10)
            {
                bool can_add = group.CanAddMeme(check_id);
                if (can_add)
                    access = 3;
            }

            ViewBag.access = access;
            res.AlbumList.AddRange((group.GetAlbums(null, 0, null, true, false)).Select(x1 => new AlbumShort(x1)));
            
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
                    
                    chat = Chat.CreateNewChat(user, user2);
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
                ViewBag.CountNewMessage = user.GetCountNewMessages();
            return PartialView();
        }




        //---------------------------------------------------ACTION---------------------------------
        [Authorize]
        [HttpPost]
        public JsonResult LikeComment(int? id)
        {
            string check_id = ApplicationUser.GetUserId();

            var comm = Comment.GetComment(id) ;
            if (comm == null)
                return Json(null);

            bool? red_heart = comm.LikeAction(check_id);

            return Json(new { red_heart, id, count = comm.UsersLikes.Count });
        }

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
            Group res = Group.CreateNewGroup(a.Name);
            
            

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
            res=group.Follow(user);
            
            //res = !res;
            return Redirect(Url.Action("FollowGroupPartial", "SocialNetwork", new { IdGroup = id, CanFollow = res }));
        }

        [Authorize]
        public ActionResult DeleteFriend(string id)
        {
            //удалять прямо из списка
            if(string.IsNullOrWhiteSpace(id))
                return new HttpStatusCodeResult(404);
            ApplicationUser user_act = ApplicationUser.GetUser(ApplicationUser.GetUserId());
            ApplicationUser user = ApplicationUser.GetUser(id);
            if (user == null|| user_act==null)
                return new HttpStatusCodeResult(404);
           
            var res = user.CanFollow(user_act.Id);
            if (res == 2 || res == 3)
                res = user.ActionListUsers(res, user_act);

            return Redirect(Url.Action("FollowUserPartial", "SocialNetwork", new { Iduser = id, CanAddFriend = true }));
        }


        [Authorize]
        public ActionResult FollowPerson(string id)
        {
            //взаимодействовать на странице пользователя
            ApplicationUser user_act = ApplicationUser.GetUser(ApplicationUser.GetUserId());
            ApplicationUser user = ApplicationUser.GetUser(id);
            if (user_act == null|| user == null)
                return new HttpStatusCodeResult(404);
            int res = 0;
            
            
            res = user.CanFollow(user_act.Id);
           
            res = user.ActionListUsers(res, user_act);


            //res = !res;
            return Redirect(Url.Action("FollowUserPartial", "SocialNetwork", new {
                Iduser = id, CanAddFriend = res }));
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

            Record record = Record.AddRecordMem(check_id, id_user, null, list_img_byte, text);
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

            var record=Record.AddRecordMem(check_id,null, id_group, list_img_byte, text);
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
            user.AddImage(text, uploadImage, album_id);
           
            
            return RedirectToAction("PersonalRecord", "SocialNetwork", new { id = user.Id });
        }
        [Authorize]
        public ActionResult AddImageGroup(string text, HttpPostedFileBase[] uploadImage, int? album_id,int? id_group)
        {
            if (uploadImage.Count() < 1 || album_id == null|| id_group==null)
                return new HttpStatusCodeResult(404);
            
            Group group = Group.GetGroup(id_group);
            if (group == null)
                return new HttpStatusCodeResult(404);
            
            var fl=group.AddImage(text, uploadImage, album_id, id_group);
            if(fl==null)
                return new HttpStatusCodeResult(404);

            return RedirectToAction("GroupRecord", "SocialNetwork", new { id = id_group });
        }
        [Authorize]
        public ActionResult AddAlbumPerson(string name)
        {
            string check_id = ApplicationUser.GetUserId();

            if(check_id==null)
                return new HttpStatusCodeResult(404);
            var alb = Album.CreateNew(name, check_id,1);
            
            return RedirectToAction("AlbumsPerson", "SocialNetwork", new { id = check_id , select_id = alb .Id});
        }
        [Authorize]
        public ActionResult AddAlbumGroup(string name,int group_id)
        {
            string check_id = ApplicationUser.GetUserId();

            if (check_id == null)
                return new HttpStatusCodeResult(404);
            //TODO проверки

            Group group = Group.GetGroup(group_id);
            bool admin=group.HaveAccessAdminGroup(check_id);
            Album alb = null;
            if (admin)
                alb = Album.CreateNew(name, group_id.ToString(), 2);

            return RedirectToAction("AlbumsGroup", "SocialNetwork", new { id = group_id, select_id = alb?.Id });
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
            Album album = Album.GetAlbum(id);

            res.AddRange(album.GetAlbumImages(start, count));//.Select(x1=>new ImageShort() {Id=(int)x1.ImageId,Data=x1.Image.Data }

            return PartialView(res);
            
        }

        [AllowAnonymous]
        public ActionResult GroupsListPartial(string id,int start, int count)
        {
            ApplicationUser user = ApplicationUser.GetUser(id);
            if (user == null)
                return new HttpStatusCodeResult(404);
            List<GroupShort> res=user.UserGroupToShort(start, count);

            return PartialView(res);
        }




        [Authorize]
        public ActionResult DialogListPartial(int start, int count)
        {
            ApplicationUser user = ApplicationUser.GetUser(ApplicationUser.GetUserId());
            if (user == null)
                return new HttpStatusCodeResult(404);
            
            List<ChatShort> res = new List<ChatShort>();
            res.AddRange((user.GetChats(start, count).Select(x1 => x1.GetChatShort())));


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


        //[AllowAnonymous]
        //public ActionResult LoadCommentsRecord(int record)
        //{
        //    ViewBag.start = 0;
        //    ViewBag.count = null;
        //    ViewBag.id = record;

        //    return PartialView();
        //}
        [AllowAnonymous]
        public ActionResult LoadCommentsRecord(int id,int? start=0,int?count=null)
        {
            List<Comment> res = new List<Comment>();
            var rec = Record.GetRecord(id);
            res.AddRange(rec.GetComments());
            ViewBag.check_id = ApplicationUser.GetUserId();
            //грузить коммент для вью
            return PartialView(res);
        }


        [AllowAnonymous]
        public JsonResult SendComment(int id,string text)
        {
            var rec = Record.GetRecord(id);
            rec.AddComment(text,null);

            return Json(true);
        }
        

        [Authorize]
        public ActionResult LoadNewMessages(int dialog)
        {
            string check_id = ApplicationUser.GetUserId();
            string type_message_need = (string)Session["NewMessageType"];

            switch (type_message_need)
            {
                case "1":
                    return Redirect(Url.Action("ListMessagesUser", "SocialNetwork", new {
                        id = dialog, new_m = true }));//start = 0, int count = 10
                    break;
                case "2":
                    //отправить колличество
                    ApplicationUser user = ApplicationUser.GetUser(check_id);
                    
                        
                    return Redirect(Url.Action("ReturnStringPartial", "SocialNetwork",new {
                        str =user.GetCountNewMessages().ToString() }));
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
        public ActionResult FollowGroupPartial(int IdGroup,int? CanFollow)
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
            Image img = Image.GetImage((int)id);
            
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
            

           
            res.AddRange(group.GetUserShortList(0,6));
            //res.AddRange(group.Users.Skip(start).Take(count).Select(x1 => new Models.ApplicationUserShort(x1)));


            return PartialView("ListUsers", res);

        }


        [AllowAnonymous]
        public ActionResult LoadFriends(string id,int start, int count)
        {
            ApplicationUser user = ApplicationUser.GetUser(id);
            if (user == null)
                return new HttpStatusCodeResult(404);

            List<Models.ApplicationUserShort> res = new List<Models.ApplicationUserShort>();
            
                
            
            res.AddRange((user.GetFriends(start,count)).Select(x1 => new Models.ApplicationUserShort(x1)));
            //res.AddRange(user.Friends.Skip(start).Take(count).Select(x1 => new Models.ApplicationUserShort(x1)));

            
            return PartialView("ListUsers",res);

        }
        [AllowAnonymous]
        public ActionResult LoadFollowers(string id, int start, int count)
        {
            ApplicationUser user = ApplicationUser.GetUser(id);
            if (user == null)
                return new HttpStatusCodeResult(404);

            List<Models.ApplicationUserShort> res = new List<Models.ApplicationUserShort>();
            

            res.AddRange((user.GetFollowers(start,count)).Select(x1 => new Models.ApplicationUserShort(x1)));

            //res.AddRange(user.Followers.Skip(start).Take(count).Select(x1 => new Models.ApplicationUserShort(x1)));

            return PartialView("ListUsers", res);

        }
        [Authorize]
        public ActionResult DeleteRecordWall(int id)
        {
            Record rec = Record.GetRecord(id);
            string check_id = ApplicationUser.GetUserId();
            if(rec.CreatorId!=check_id)
                return new HttpStatusCodeResult(404);



            return View();
        }
        //удаляет как саму запись так и то что внитри - картинку мем и тд
        //[Authorize]
        //public ActionResult DeleteRecordInside(int id)
        //{


        //}
        [AllowAnonymous]
        public ActionResult LoadFollowUser(string id, int start, int count)
        {
            ApplicationUser user = ApplicationUser.GetUser(id);
            if (user == null)
                return new HttpStatusCodeResult(404);

            List<Models.ApplicationUserShort> res = new List<Models.ApplicationUserShort>();
            
            res.AddRange((user.GetFollow(start,count).Select(x1 => new Models.ApplicationUserShort(x1))));

           // res.AddRange(user.FollowUser.Skip(start).Take(count).Select(x1 => new Models.ApplicationUserShort(x1)));

            return PartialView("ListUsers", res);

        }

    }
}