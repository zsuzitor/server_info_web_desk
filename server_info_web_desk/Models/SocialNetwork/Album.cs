using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace server_info_web_desk.Models.SocialNetwork
{
    public class Album
    {
        [Key]
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }
        //[UIHint("Html")]
        [Display(Name = "Название")]
        public string Name { get; set; }
        //[UIHint("MultilineText")]
        [Display(Name = "Описание")]
        public string Description { get; set; }
        public bool PublicAlbum { get; set; }

        //public bool MainAlbum { get; set; }

        public int? GroupId { get; set; }
        public Group Group { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }




        public ICollection<Record> Images { get; set; }


        public Album()
        {
            Id = 0;
            Name = null;
            Description = null;
            PublicAlbum = true;
            GroupId = null;
            Group = null;
            UserId = null;
            User = null;
            Images = new List<Record>();


        }
    }

    public class AlbumShort
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }
        [UIHint("Html")]
        [Display(Name = "Название")]
        public string Name { get; set; }

        public int CountImage { get; set; }
        public Image Image { get; set; }

        public AlbumShort()
        {
            Id = 0;
            Name = null;
            CountImage = 0;
            Image = null;
        }
        public AlbumShort(Album a)
        {
            Id = a.Id;
            Name = a.Name;
            CountImage = a.Images.Count;
            Image = a.Images.LastOrDefault()?.Image;
        }
    }
}