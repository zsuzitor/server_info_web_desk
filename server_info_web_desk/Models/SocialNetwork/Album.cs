﻿using System;
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
        [UIHint("Html")]
        [Display(Name = "Название")]
        public string Name{ get; set; }
        [UIHint("MultilineText")]
        [Display(Name = "Описание")]
        public string Description { get; set; }
        public bool PublicAlbum { get; set; }

        public bool MainAlbum { get; set; }

        public int? GroupId { get; set; }
        public Group Group { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }




        public ICollection<Image> Images { get; set; }


        public Album()
        {
            Id = 0;
            Name = null;
                Description= null;
            PublicAlbum = true;
            GroupId = null;
            Group = null;
            UserId = null;
            User = null;
            Images = new List<Image>();


        }
    }
}