using server_info_web_desk.Models.SocialNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace server_info_web_desk.Models.Interfaces
{
    public interface IDomain<T>
    {
        T Id { get; set; }
    }
    public interface IPageR
    {
        bool LoadDataForShort();
    }
    public interface IHaveAlbum
    {
        List<Album> Albums { get; set; }
        List<Album> GetAlbums(int? id, int start = 0, int? count = null);

    }
    public interface IHaveUsersList
    {

    }


    public abstract class AImage
    {
        //int Id { get; set; }
        [HiddenInput(DisplayValue = false)]
         public int Id { get; set; }
        public DateTime Birthday { get; set; }
        public byte[] Data { get; set; }

        public string UserId { get; set; }//тот кто загрузил??
        public ApplicationUser User { get; set; }

        public AImage()
        {
            Id = 0;
            Data = null;
            Birthday = DateTime.Now;
            UserId = null;
            User = null;
        }
    }

    public interface Iuser
    {

    }


    public class Interfaces
    {
    }
}