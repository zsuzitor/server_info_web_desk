using server_info_web_desk.Models.Interfaces;
using server_info_web_desk.Models.SocialNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace server_info_web_desk.Models.ViewModel
{
    public class PersonalRecordView:Iuser
    {
        public string IdUser { get; set; }
        public string IdPage { get; set; }

        public string Name { get; set; }
        public string Status { get; set; }
        public DateTime? Birthday { get; set; }
        public bool Sex { get; set; }
        public int? Age { get; set; }
        public string Surname { get; set; }
        public string Country { get; set; }
        public string Town { get; set; }
        public string Street { get; set; }
        public bool Online { get; set; }
        public string Description { get; set; }
        public Image MainImage { get; set; }

        public int AlbumCount { get; set; }
        public int ImageCount { get; set; }
        public int WallMemeCount { get; set; }
        public int GroupCount { get; set; }

        public List<Meme> WallMeme { get; set; }
       
        public List<AlbumShort> Albums { get; set; }
        public List<GroupShort> Group { get; set; }
        public List<Image> Image { get; set; }

        public PersonalRecordView()
        {
            Age = null;

            Online = false;
            Surname = null;
            Country = null;
            Town = null;
            Street = null;
            Status = null;
            Birthday = null;
            Sex = true;
            MainImage = null;
            //о себе 
            Description = null;

            AlbumCount = 0;
            ImageCount = 0;
            WallMemeCount = 0;
            GroupCount = 0;

            WallMeme = new List<Meme>();
            Albums = new List<AlbumShort>();
            Group = new List<GroupShort>();
        }

        public PersonalRecordView(ApplicationUser a)
        {
            Age = a.Age;
            IdPage = a.Id;
            Online = a.Online;
            Surname = a.Surname;
            Country = a.Country;
            Town = a.Town;
            Street = a.Street;
            Status = a.Status;
            Birthday = a.Birthday;
            Sex = a.Sex;


            MainImage = null;
            //о себе 
            Description = null;

            WallMeme = null;
            Albums = null;
            Group = null;
        }



    }
}