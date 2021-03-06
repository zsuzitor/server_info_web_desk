﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace server_info_web_desk.Models.Info
{
    public class ImageInfo : server_info_web_desk.Models.Interfaces.AImage
    {
        //[HiddenInput(DisplayValue = false)]
        //public int Id { get; set; }
        //public byte[] Data { get ; set;}


        public int? Article_parrentId { get; set; }//для главной секции будет у всех пользователей null
        public Article Article_parrent { get; set; }

        //public string UserId { get; set; }
        //public ApplicationUser User { get; set; }

        public ImageInfo() : base()
        {
            //Id = 0;
            //Data = null;
            Article_parrentId = null;
            Article_parrent = null;
            //UserId = null;
            //User = null;

            //Base_url_img = "/Content/images/camera_200.png";
        }
        public ImageInfo(ImageInfo a, bool with_out_reference = false)
        {
            Id = a.Id;
            Data = a.Data;
            Article_parrentId = a.Article_parrentId;
            UserId = a.UserId;

            if (!with_out_reference)
            {
                Article_parrent = a.Article_parrent;
                User = a.User;
            }
            else
            {
                Article_parrent = null;
                User = null;
            }
        }
    }
}