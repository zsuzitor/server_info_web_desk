using server_info_web_desk.Models.SocialNetwork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace server_info_web_desk.Models.ViewModel
{
    public class GroupRecordView
    {
        
        [HiddenInput(DisplayValue = false)]
        public int IdGroup { get; set; }
        public string IdUser { get; set; }

        public bool NoAccess { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public DateTime Birthday { get; set; }
        //public byte[] Image { get; set; }
        public bool OpenGroup { get; set; }
        public bool AddMemesPrivate { get; set; }

        public bool? CanFollow { get; set; }//true-можно подать заявку false-можно убрать из групп null-можно отписаться
        public bool CanAddMeme { get; set; }

        public Image MainImage { get; set; }

        public List<Image> Image { get; set; }

        public List<ApplicationUserShort> Users { get; set; }
        public List<ApplicationUserShort> Admins { get; set; }

        public List<Album> Albums { get; set; }
        public List<Record> WallMeme { get; set; }

        public GroupRecordView()
        {
            IdGroup = 0;
            NoAccess = true;
            IdUser = null;
            Name = null;
            Status = null;
            MainImage = null;
            Birthday = DateTime.Now;
            OpenGroup = false;
            AddMemesPrivate = true;
            CanAddMeme = false;
            Users = new List<ApplicationUserShort>();
            Admins = new List<ApplicationUserShort>(); 
            Albums = new List<Album>();
            WallMeme = new List<Record>();
            Image = new List<SocialNetwork.Image>();

        }
        public GroupRecordView(Group a)
        {
            IdGroup = a.Id;
            IdUser = null;
            Name = a.Name;
            Status = a.Status;
            Birthday = a.Birthday;
            OpenGroup = a.OpenGroup;
            AddMemesPrivate = a.AddMemesPrivate;
            MainImage = null;
            CanAddMeme = false;

            Users = new List<ApplicationUserShort>();
            Admins = new List<ApplicationUserShort>();
            Albums = new List<Album>();
            WallMeme = new List<Record>();
            Image = new List<SocialNetwork.Image>();

        }

    }
}