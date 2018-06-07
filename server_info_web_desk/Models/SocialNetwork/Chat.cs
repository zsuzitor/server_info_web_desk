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
            Messages = new List<Message>();
            Users = new List<ApplicationUser>(); 



        }
    }
}