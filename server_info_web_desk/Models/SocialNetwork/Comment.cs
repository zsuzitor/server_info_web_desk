﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace server_info_web_desk.Models.SocialNetwork
{
    public class Comment
    {
        [Key]
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }
        public DateTime Birthday { get; set; }

        [UIHint("MultilineText")]
        [Display(Name = "Текст")]
        public string Text { get; set; }


        public string СreatorId { get; set; }
        public ApplicationUser Сreator { get; set; }//создатель

        public int? AnswerCommentId { get; set; }
        public Comment AnswerComment { get; set; }//комментарийна который отвечают этим
        public ICollection<Comment> AnswerComments { get; set; }//комментарии которые ответили на этот коммент


        public int? MemeId { get; set; }
        public Meme Meme { get; set; }//запись к которой написали коммент

        public int? ImageId { get; set; }
        public Image Image { get; set; }//картинка к которой написали коммент

        public ICollection<Image> Images { get; set; }//список картинок в комменте
        public ICollection<ApplicationUser> UsersLikes { get; set; }//список лайкнувших


        public Comment()
        {
            Id = 0;
            Birthday = DateTime.Now;
            СreatorId = null;
            Сreator= null;
            AnswerCommentId = null;
            AnswerComment = null;
            MemeId = null;
            Meme = null;
            ImageId = null;
            Image = null;
            Images = new List<Image>();
            UsersLikes = new List<ApplicationUser>();


        }
    }
}