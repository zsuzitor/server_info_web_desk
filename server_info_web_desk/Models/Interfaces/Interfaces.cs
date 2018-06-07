using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace server_info_web_desk.Models.Interfaces
{
    public abstract class AImage
    {
        //int Id { get; set; }
        [HiddenInput(DisplayValue = false)]
         public int Id { get; set; }

        public byte[] Data { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public AImage()
        {
            Id = 0;
            Data = null;
            
            UserId = null;
            User = null;
        }
    }


    public class Interfaces
    {
    }
}