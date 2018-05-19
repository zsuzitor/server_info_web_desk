using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using server_info_web_desk.Models;
using server_info_web_desk.Models.Info;

namespace server_info_web_desk.Models.ViewModel
{
    public class IndexInfoView
    {
        public List<Article> Articles { get; set; }
        public List<Section> Sections { get; set; }

        public IndexInfoView(List<Article> articles, List<Section> sections)
        {
            Articles = articles;
            Sections = sections;
        }
        public IndexInfoView()
        {
            Articles = new List<Article>();
            Sections = new List<Section>();
        }

    }
}