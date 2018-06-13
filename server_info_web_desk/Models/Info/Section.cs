using Newtonsoft.Json;
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
        [Key]
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }
        //public int Parrent_id { get; set; }

        public int Order { get; set; }
        
    [UIHint("MultilineText")]
        [Display(Name = "Название")]
        [Required(ErrorMessage = "Поле должно быть установлено")]
        public string Head { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        //[JsonProperty("Section_parrentId")]
        public int? SectionParrentId { get; set; }//для главной секции будет у всех пользователей null
        public Section SectionParrent { get; set; }

        public List<Section> Sections { get; set; }

        public List<Article> Articles { get; set; }
        public Section()
        {
            this.Id = 0;
            this.Order = 0;
            //Parrent_id = 0;
            this.Head = "";
            this.SectionParrentId = null;
            this.Articles = new List<Article>();
            this.Sections = new List<Section>();
            this.SectionParrent = null;
            this.UserId = null;
            this.User = null;
        }
        public Section(Section a,bool with_out_reference=false)
        {
            this.Id = a.Id;
            this.Order = a.Order;
            this.Head = a.Head;
            this.UserId = a.UserId;
            this.SectionParrentId = a.SectionParrentId;

            if (!with_out_reference) { 
            this.SectionParrent = a.SectionParrent;
            this.User = a.User;
            this.Sections = a.Sections;
            this.Articles = a.Articles;
            }
            else
            {
                this.SectionParrent = null;
                this.User = null;
                this.Sections = null;
                this.Articles = null;
            }
        }
        }
}