using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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

        public string СreatorId { get; set; }
        public ApplicationUser Сreator { get; set; }//создатель

        public ICollection<Message> Messages { get; set; }
        public ICollection<ApplicationUser> Users { get; set; }


        public Chat()
        {
            Id = 0;
            Birthday = DateTime.Now;
            СreatorId = null;
            Сreator = null;
            Image = null;
            Name = null;
            Messages = new List<Message>();
            Users = new List<ApplicationUser>(); 



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