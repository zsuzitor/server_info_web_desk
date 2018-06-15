using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static server_info_web_desk.Models.DataBase.DataBase;
using  server_info_web_desk.Models.SocialNetwork;

namespace server_info_web_desk.Models.ViewModel
{
    public class ListAlbumsShortView
    {
        public string UserId { get; set; }
        public string PageUserId { get; set; }
        public int? PageGroupId { get; set; }
        public int? SelectAlbum { get; set; }
        public List<AlbumShort> AlbumList { get; set; }

        public ListAlbumsShortView()
        {
            UserId = null;
            PageUserId = null;
            PageGroupId = null;
            SelectAlbum = null;
            AlbumList = new List<AlbumShort>();
        }
    }
}