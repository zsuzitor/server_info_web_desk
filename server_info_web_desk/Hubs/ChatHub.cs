using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using server_info_web_desk.Models;
using static server_info_web_desk.Models.DataBase.DataBase;


namespace server_info_web_desk.Hubs
{
    public class UserForHub
    {
        public ApplicationUser User { get; set; }
        public string ConnectionId { get; set; }

        public UserForHub()
        {
            User = null;
            ConnectionId = null;
        }
    }

    public class ChatHub : Hub
    {
        //https://metanit.com/sharp/mvc5/16.2.php
        static List<UserForHub> Users = new List<UserForHub>();

        // Отправка сообщений
        public void Send(int id_dialog)
        {
            //Clients.All.NeedDownloadChangesMessages(id_dialog);
            Clients.Group(id_dialog.ToString()).NeedDownloadChangesMessages(id_dialog);
        }

        // Подключение нового пользователя
        public void Connect(ApplicationUser user,int id_dialog)
        {
            var id = Context.ConnectionId;


            if (!Users.Any(x => x.ConnectionId == id))
            {
                Users.Add(new UserForHub { ConnectionId = id, User = user });

                if (!db.Entry(user).Collection(x1 => x1.Chats).IsLoaded)
                    db.Entry(user).Collection(x1 => x1.Chats).Load();
                foreach(var i in user.Chats)
                {
                    Groups.Add(Context.ConnectionId, id_dialog.ToString());
                }
                
            }
        }

        // Отключение пользователя
        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            var item = Users.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (item != null)
            {
                Users.Remove(item);
                var id = Context.ConnectionId;
                Clients.All.onUserDisconnected(id);
            }

            return base.OnDisconnected(stopCalled);
        }
    }
}