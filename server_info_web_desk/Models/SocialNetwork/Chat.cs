using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static server_info_web_desk.Models.DataBase.DataBase;


namespace server_info_web_desk.Models.SocialNetwork
{
    public class Chat
    {
        [Key]
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }
        public DateTime Birthday { get; set; }
        public string Name { get; set; }
        public Image Image { get; set; }

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
            Image = null;
            Name = null;
            Messages = new List<Message>();
            Users = new List<ApplicationUser>(); 



        }
        public ChatShort GetChatShort()
        {
            var res = new ChatShort() { Id = this.Id, Image = this.Image, Name = this.Name };
            if (!db.Entry(this).Collection(x2 => x2.Messages).IsLoaded)
                db.Entry(this).Collection(x2 => x2.Messages).Load();
            var last_message = this.Messages.LastOrDefault();
            if (last_message == null)
            {
                res.User = null;
                res.Text = null;
            }
            else
            {
                if (!db.Entry(last_message).Reference(x2 => x2.Creator).IsLoaded)
                    db.Entry(last_message).Reference(x2 => x2.Creator).Load();
                res.User = new Models.ApplicationUserShort(last_message?.Creator);
                res.Text = last_message.Text;
            }


            return res;
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