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
        public int Parrent_id { get; set; }
        public string Head { get; set; }

        public Section Section_parrent { get; set; }
        public ICollection<Section> Sections { get; set; }

        public ICollection<Article> Articles { get; set; }
        public Section()
        {
            Id = 0;
            Parrent_id = 0;
            Head = "";
            Articles = new List<Article>();
            Sections = new List<Section>();
            Section_parrent = null;
        }
    }
}