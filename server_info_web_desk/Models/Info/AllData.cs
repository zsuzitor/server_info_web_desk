using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace server_info_web_desk.Models.Info
{
    //класс для сохранения и загрузки данных
    public class AllData
    {
        public List<Section> Sections { get; set; }
        public List<Article> Articles { get; set; }
        public List<ImageInfo> Images { get; set; }

        public AllData()
        {
            Sections = new List<Section>();
            Articles = new List<Article>();
            Images = new List<ImageInfo>();
        }
    }
}