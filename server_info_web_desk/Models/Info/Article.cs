using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace server_info_web_desk.Models.Info
{
    public class Article
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [UIHint("MultilineText")]
        [Display(Name = "Название")]
        [Required(ErrorMessage = "Поле должно быть установлено")]
        public string Head { get; set; }

        [UIHint("MultilineText")]
        [Display(Name = "Содержание")]
        public string Body { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int Section_parrentId { get; set; }
        public Section Section_parrent { get; set; }
        public Article()
        {
            Id = 0;
            Section_parrent = null;
            Head = null;
            Body = null;
            UserId = null;
            User = null;
            Section_parrentId = 0;
        }
        public Article(Article a)
        {
            Id = a.Id;
            Head = a.Head;
            Body = a.Body;
            UserId = a.UserId;
            User = a.User;
            Section_parrentId = a.Section_parrentId;
            Section_parrent = a.Section_parrent;
        }
        }
}