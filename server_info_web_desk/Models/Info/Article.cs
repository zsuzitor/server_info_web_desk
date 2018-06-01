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

        public int Order { get; set; }

        [UIHint("MultilineText")]
        [Display(Name = "Название")]
        [Required(ErrorMessage = "Поле должно быть установлено")]
        public string Head { get; set; }

        [UIHint("MultilineText")]
        [Display(Name = "Содержание")]
        public string Body { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public ICollection<Image> Images { get; set; }

        public int Section_parrentId { get; set; }
        public Section Section_parrent { get; set; }
        public Article()
        {
            this.Id = 0;
            this.Order = 0;
            this.Section_parrent = null;
            this.Head = null;
            this.Body = null;
            this.UserId = null;
            this.User = null;
            this.Section_parrentId = 0;
            this.Images = new List<Image>();
        }
        public Article(Article a, bool with_out_reference = false)
        {
            this.Id = a.Id;
            this.Order = a.Order;
            this.Head = a.Head;
            this.Body = a.Body;
            this.UserId = a.UserId;
            this.Section_parrentId = a.Section_parrentId;
            if (!with_out_reference)
            {
                this.User = a.User;
                this.Section_parrent = a.Section_parrent;
                this.Images = a.Images;
            }
            else
            {
                this.User = null;
                this.Section_parrent = null;
                this.Images = null;
            }
        }
        }
}