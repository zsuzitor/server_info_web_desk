using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using server_info_web_desk.Models;
using static server_info_web_desk.Models.DataBase.DataBase;
using Microsoft.AspNet.Identity;




namespace server_info_web_desk.Hubs
{
    public class UserForHub
    {
        public string UserId { get; set; }
        public string ConnectionId { get; set; }

        public UserForHub()
        {
            UserId = null;
            ConnectionId = null;
        }
    }

    public class ChatHub : Hub
    {
        //https://metanit.com/sharp/mvc5/16.2.php
        static List<UserForHub> Users = new List<UserForHub>();

        // тут надо проверить есть ли в диалоге сообщение не прочитанноа автором и отправить его ВСЕМ если оно есть
        public void Send(int id_dialog)
        {
            //Clients.All.NeedDownloadChangesMessages(id_dialog);
            //string check_id = ApplicationUser.GetUserId();
            ApplicationUser user = ApplicationUser.GetUser(ApplicationUser.GetUserId());
            //var user = db.Users.First(x1 => x1.Id == check_id);
            if(user==null)
                return;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Set<ApplicationUser>().Attach(user);
                if (!db.Entry(user).Collection(x1 => x1.Chats).IsLoaded)
                    db.Entry(user).Collection(x1 => x1.Chats).Load();
                var chat = user.Chats.FirstOrDefault(x1 => x1.Id == id_dialog);
                if (chat == null)
                    return;
                if (!db.Entry(chat).Collection(x1 => x1.Messages).IsLoaded)
                    db.Entry(chat).Collection(x1 => x1.Messages).Load();
                var need_read_mess_im = chat.Messages.FirstOrDefault(x1 =>
                {
                    if (x1.CreatorId == user.Id)
                    {
                        if (!db.Entry(x1).Collection(x2 => x2.UserNeedRead).IsLoaded)
                            db.Entry(x1).Collection(x2 => x2.UserNeedRead).Load();
                        var us = x1.UserNeedRead.FirstOrDefault(x2 => x2.Id == user.Id);
                        if (us != null)
                            return true;
                    }
                    return false;
                });
            

            if (need_read_mess_im != null)
            {
                Clients.Group(id_dialog.ToString()).NeedDownloadChangesMessages(id_dialog);
            }
            }
        }
        public void JoinToHub()
        {
            //string check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();

            ApplicationUser user = ApplicationUser.GetUser(ApplicationUser.GetUserId());
           
                
                var id = Context.ConnectionId;

                //db.Set<ApplicationUser>().Attach(user);
                if (!Users.Any(x => x.ConnectionId == id))
                {
                    Users.Add(new UserForHub { ConnectionId = id, UserId = user?.Id });


                    //подключание к чатам\диалогам
                    if (user != null)
                    {
                    user.SetOnline(true);
                    using (ApplicationDbContext db = new ApplicationDbContext())
                    {
                        db.Set<ApplicationUser>().Attach(user);
                        if (!db.Entry(user).Collection(x1 => x1.Chats).IsLoaded)
                            db.Entry(user).Collection(x1 => x1.Chats).Load();
                        foreach (var i in user.Chats)
                        {
                            Groups.Add(Context.ConnectionId, i.Id.ToString());
                        }
                    }

                }
            }

        }


        //// Подключение нового пользователя
        //private void Connect(ApplicationUser user,int id_dialog)
        //{
        //    var id = Context.ConnectionId;


        //    
        //}

        // Отключение пользователя
        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            var item = Users.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (item != null)
            {
                var user = ApplicationUser.GetUser(ApplicationUser.GetUserId());
                user.SetOnline(false);
                Users.Remove(item);
                var id = Context.ConnectionId;
                //Clients.All.onUserDisconnected(id);
            }

            return base.OnDisconnected(stopCalled);
        }
    }
}