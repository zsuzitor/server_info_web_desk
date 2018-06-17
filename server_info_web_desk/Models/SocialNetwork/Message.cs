﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace server_info_web_desk.Models.SocialNetwork
{
    public class Message
    {
        [Key]
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }
        public DateTime Birthday { get; set; }

        [UIHint("MultilineText")]
        [Display(Name = "Текст")]
        public string Text { get; set; }


        public string CreatorId { get; set; }
        public ApplicationUser Creator { get; set; }//создатель

        public int? MemeId { get; set; }
        public Meme Meme { get; set; }

        public int? ChatId { get; set; }
        public Chat Chat { get; set; }

        public ICollection<Image> Images { get; set; }
        //public ICollection<ApplicationUser> Users { get; set; }

        public ICollection<ApplicationUser> UserNeedRead { get; set; }

        public Message()
        {
            Id = 0;
            Birthday = DateTime.Now;
            Text = null;
            CreatorId = null;
            Creator = null;
            MemeId = null;
            Meme = null;
            Chat = null;
            ChatId = null;
            Images = new List<Image>();
            UserNeedRead = new List<ApplicationUser>();

        }
    }
}