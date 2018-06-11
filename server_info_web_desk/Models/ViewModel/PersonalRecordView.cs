using server_info_web_desk.Models.Interfaces;
using server_info_web_desk.Models.SocialNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace server_info_web_desk.Models.ViewModel
{
    public class PersonalRecordView:Iuser
    {
        [HiddenInput(DisplayValue = false)]
        public string IdUser { get; set; }
        [HiddenInput(DisplayValue = false)]
        public string IdPage { get; set; }

        public string Name { get; set; }
        public string Status { get; set; }
        public DateTime? Birthday { get; set; }
        public bool Sex { get; set; }
        public bool? WallOpenWrite { get; set; }//кто может писать на стену false только владелец страницы true-все null-друзья
        public bool PrivatePage { get; set; }//кто может просмотреть всю страницу false только владелец страницы true-все null-друзья

        public int? Age { get; set; }
        public string Surname { get; set; }
        public string Country { get; set; }
        public string Town { get; set; }
        public string Street { get; set; }
        public bool Online { get; set; }
        public string Description { get; set; }
        public Image MainImage { get; set; }

        public bool? CanAddFriend { get; set; }//true-можно добавить false-удалить null-отписаться
        public bool CanAddMeme { get; set; }

        public int AlbumCount { get; set; }
        public int ImageCount { get; set; }
        public int WallMemeCount { get; set; }
        public int GroupCount { get; set; }

        public List<ApplicationUserShort> Friends { get; set; }

        public List<Record> WallMeme { get; set; }
       
        public List<AlbumShort> Albums { get; set; }
        public List<GroupShort> Group { get; set; }
        public List<Image> Image { get; set; }

        public PersonalRecordView()
        {
            Age = null;

            Online = false;
            Surname = null;
            Name = null;
            Country = null;
            Town = null;
            Street = null;
            Status = null;
            Birthday = null;
            Sex = true;
            MainImage = null;
            PrivatePage = false;

            //о себе 
            Description = null;
            WallOpenWrite = false;
            CanAddFriend = null;
            CanAddMeme = false;
            AlbumCount = 0;
            ImageCount = 0;
            WallMemeCount = 0;
            GroupCount = 0;

            Friends = new List<ApplicationUserShort>();
            WallMeme = new List<Record>();
            Albums = new List<AlbumShort>();
            Group = new List<GroupShort>();
            Image = new List<Image>();
        }

        public PersonalRecordView(ApplicationUser a)
        {
            Age = a.Age;
            IdPage = a.Id;
            Online = a.Online;
            Surname = a.Surname;
            Name = a.Name;
            Country = a.Country;
            Town = a.Town;
            Street = a.Street;
            Status = a.Status;
            Birthday = a.Birthday;
            Sex = a.Sex;
            WallOpenWrite = a.WallOpenWrite;
            CanAddFriend = null;
            MainImage = null;
            PrivatePage = a.PrivatePage;
            //о себе 
            Description = null;
            CanAddMeme = false;
            Friends = new List<ApplicationUserShort>();
            WallMeme = new List<Record>();
            Albums = new List<AlbumShort>();
            Group = new List<GroupShort>();
            Image = new List<Image>();
        }



    }
}