using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace server_info_web_desk.Models.Info
{
    public class ListData
    {
        public List<Article> Articles { get; set; }
        public List<Section> Sections { get; set; }


        public ListData()
        {
            Articles = new List<Article>();
            Sections = new List<Section>();
        }
    }
}