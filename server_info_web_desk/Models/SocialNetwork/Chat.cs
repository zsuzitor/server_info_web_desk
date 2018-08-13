using server_info_web_desk.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static server_info_web_desk.Models.DataBase.DataBase;
using server_info_web_desk.Models;


namespace server_info_web_desk.Models.SocialNetwork
{
    public class Chat: IDomain<int>, IDeleteDb<Chat>
    {
        [Key]
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }
        public DateTime Birthday { get; set; }
        public string Name { get; set; }

        // public int? ImageId { get; set; }
        // public Image Image { get; set; }
        public byte[] Image { get; set; }

        public string CreatorId { get; set; }
        public ApplicationUser Creator { get; set; }//создатель

        public ICollection<Message> Messages { get; set; }
        public ICollection<ApplicationUser> Users { get; set; }


        public Chat()
        {
            Id = 0;
            Birthday = DateTime.Now;
            CreatorId = null;
            Creator = null;
            //ImageId = null;
            Image = null;
            Name = null;
            Messages = new List<Message>();
            Users = new List<ApplicationUser>(); 



        }


        //TODO имхо СЛИШКОМ медленно будет , можно ускорить если искать с конца пока это сообщение не прочитано
        public int GetCountNewMessage(string user_id)
        {
            int res = 0;
            var user = ApplicationUser.GetUser(user_id);
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Set<Chat>().Attach(this);
                if (!db.Entry(this).Collection(x2 => x2.Messages).IsLoaded)
                    db.Entry(this).Collection(x2 => x2.Messages).Load();
                //this.Messages.
                foreach (var i in this.Messages)
                {
                    if (!db.Entry(i).Collection(x2 => x2.UserNeedRead).IsLoaded)
                        db.Entry(i).Collection(x2 => x2.UserNeedRead).Load();
                    if (i.UserNeedRead.Any(x1=>x1.Id== user.Id))
                        ++res;
                }
            }

            return res;
        }

        public static Chat GetChat(int? id)
        {

            Chat res = null;
            if (id == null)
                return null;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                res = db.Chats.FirstOrDefault(x1 => x1.Id == id);
            }

            return res;
        }


        public ChatShort GetChatShort()
        {
           
                var res = new ChatShort() { Id = this.Id, Image = new Image() {Data= this.Image }, Name = this.Name };
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Set<Chat>().Attach(this);
                if (!db.Entry(this).Collection(x2 => x2.Messages).IsLoaded)
                    db.Entry(this).Collection(x2 => x2.Messages).Load();
            }
            var last_message = this.Messages.LastOrDefault();
            if (last_message == null)
            {
                res.User = null;
                res.Text = null;
            }
            else
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    db.Set<Message>().Attach(last_message);
                    if (!db.Entry(last_message).Reference(x2 => x2.Creator).IsLoaded)
                        db.Entry(last_message).Reference(x2 => x2.Creator).Load();
                }
                if (last_message?.Creator != null)

                    res.User = new Models.ApplicationUserShort(last_message?.Creator);
                res.Text = last_message.Text;
                res.CountNewMessage = this.GetCountNewMessage(ApplicationUser.GetUserId());
            }

           
            return res;
           
        }

        public bool CanDelete()
        {
            //bool res = false;
            var check_id = ApplicationUser.GetUserId();
            if (this.CreatorId == check_id)
                return true;
            //var user = ApplicationUser.GetUser(check_id);
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Set<Chat>().Attach(this);
                if (!db.Entry(this).Collection(x2 => x2.Users).IsLoaded)
                    db.Entry(this).Collection(x2 => x2.Users).Load();
                if (this.Users.Count <= 1 && this.Users.FirstOrDefault(x1 => x1.Id == check_id) != null)
                    return true;
            }

                return false;
        }

        public bool LeaveUser(string user_id)
        {
            bool res = false;
            //var user = ApplicationUser.GetUser(ApplicationUser.GetUserId());
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Set<Chat>().Attach(this);
                if (!db.Entry(this).Collection(x2 => x2.Users).IsLoaded)
                    db.Entry(this).Collection(x2 => x2.Users).Load();
                var user = this.Users.FirstOrDefault(x1=>x1.Id== ApplicationUser.GetUserId());
                if (user == null)
                    return false;
                if (this.Users.Count <= 1)
                {
                    bool suc = false;
                    this.DeleteFull(out suc,db);
                    return suc;
                }
                this.Users.Remove(user);
                db.SaveChanges();

                var message_leave = new Message() { Text=user.Name+" "+ user.Surname + " покинул беседу"
                    ,ChatId=this.Id };
                db.Messages.Add(message_leave);
                db.SaveChanges();
                foreach (var i in this.Users)
                    message_leave.UserNeedRead.Add(i);
                db.SaveChanges();

            }


            return res;
        }

        public Chat TryDeleteFull(out bool success, ApplicationDbContext db)
        {
            success = false;
            if (this.CanDelete())
            {
                this.DeleteFull(out success);
            }
            return this;

        }
        public Chat TryDeleteFull(out bool success)
        {
            success = false;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                this.TryDeleteFull(out success, db);
            }
            return this;
        }


        public Chat DeleteFull(out bool success, ApplicationDbContext db)
        {
            success = false;
            db.Set<Chat>().Attach(this);
            if (!db.Entry(this).Collection(x1 => x1.Messages).IsLoaded)
                db.Entry(this).Collection(x1 => x1.Messages).Load();
            foreach (var i in this.Messages.ToList())
            {
                bool suc;
                i.DeleteFull(out suc, db);
            }
            db.SaveChanges();
            if (!db.Entry(this).Collection(x1 => x1.Users).IsLoaded)
                db.Entry(this).Collection(x1 => x1.Users).Load();


            db.Chats.Remove(this);
            db.SaveChanges();
            success = true;
            return this;
        }
        public Chat DeleteFull(out bool success)
        {
            success = false;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                this.DeleteFull(out success, db);

            }
            //success = true;
            return this;
        }



        public static Chat CreateNewChat(ApplicationUser creator, ApplicationUser user)
        {

           var chat = new Chat() { CreatorId = creator.Id };
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Set<ApplicationUser>().Attach(creator);
                try
                {
                    db.Set<ApplicationUser>().Attach(user);
                }
                catch
                {
                    //оставить именно так а не методом
                    user = ApplicationUser.GetUser(user.Id, db);
                }
                //db.Set<ApplicationUser>().Attach(user);

                db.Chats.Add(chat);
                db.SaveChanges();
                chat.Users.Add(user);
                chat.Users.Add(creator);
                db.SaveChanges();
            }
            return chat;
        }
        }


    public class ChatShort
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }
        public DateTime LastMessageDate { get; set; }
        public Image Image { get; set; }

        //public Message Message { get; set; }
        public ApplicationUserShort User { get; set; }
        public string Text { get; set; }

        public string Name { get; set; }
        public int CountNewMessage { get; set; }

        public ChatShort()
        {
            Id = 0;
            LastMessageDate =DateTime.Now;
            Image = null;
            User = null;
            Text = null;
            Name = null;
            CountNewMessage = 0;
        }
    }
    }