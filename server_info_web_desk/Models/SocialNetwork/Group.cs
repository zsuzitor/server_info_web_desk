using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace server_info_web_desk.Models.SocialNetwork
{
    public class Group
    {
        [Key]
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        
        [Required(ErrorMessage = "Имя группы должно быть установлено")]
        public string Name { get; set; }
        public string Status { get; set; }
        public DateTime Birthday { get; set; }
        //public byte[] Image { get; set; }
        public bool OpenGroup { get; set; }
        public bool AddMemesPrivate { get; set; }

        //public ICollection<SocialNetwork.Image> MainImages { get; set; }

        public string MainAdminId { get; set; }
        public ApplicationUser MainAdmin { get; set; }

        public ICollection<ApplicationUser> Users { get; set; }
        public ICollection<ApplicationUser> Admins { get; set; }

        public ICollection<Album> Albums { get; set; }
        public ICollection<Meme> WallMeme { get; set; }



        public Group()
        {
            Id = 0;
            Name = null;
            Status = null;
            Birthday = DateTime.Now;
            OpenGroup = true;
            AddMemesPrivate = false;
            MainAdminId = null;
            MainAdmin = null;
            Users = new List<ApplicationUser>();
            Admins = new List<ApplicationUser>();
            Albums = new List<Album>(); 
            WallMeme = new List<Meme>();

        }


    }


    public class GroupShort
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public Image Image { get; set; }
        public int CountFollowers { get; set; }

        public GroupShort()
        {
            Id = 0;
            Name = null;
            Status = null;
            Image = null;
            CountFollowers = 0;
        }
        public GroupShort(Group a)
        {
            Id = a.Id;
            Name = a.Name;
            Status = a.Status;
            Image = a.Albums.FirstOrDefault()?.Images.Last();
            CountFollowers = a.Users.Count;
        }
    }
}