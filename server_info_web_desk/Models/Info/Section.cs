using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;



namespace server_info_web_desk.Models.Info
{
    public class Section
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }
        //public int Parrent_id { get; set; }

        [UIHint("MultilineText")]
        [Display(Name = "Название")]
        [Required(ErrorMessage = "Поле должно быть установлено")]
        public string Head { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int? Section_parrentId { get; set; }//для главной секции будет у всех пользователей null
        public Section Section_parrent { get; set; }

        public ICollection<Section> Sections { get; set; }

        public ICollection<Article> Articles { get; set; }
        public Section()
        {
            Id = 0;
            //Parrent_id = 0;
            Head = "";
            Section_parrentId = null;
            Articles = new List<Article>();
            Sections = new List<Section>();
            Section_parrent = null;
            UserId = null;
            User = null;
        }
        public Section(Section a,bool with_out_reference=false)
        {
            this.Id = a.Id;
            this.Head = a.Head;
            this.UserId = a.UserId;
            this.Section_parrentId = a.Section_parrentId;
            if (!with_out_reference) { 
            this.Section_parrent = a.Section_parrent;
            this.User = a.User;
            this.Sections = a.Sections;
            this.Articles = a.Articles;
            }
            else
            {
                this.Section_parrent = null;
                this.User = null;
                this.Sections = null;
                this.Articles = null;
            }
        }
        }
}