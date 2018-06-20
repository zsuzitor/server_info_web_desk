using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static server_info_web_desk.Models.DataBase.DataBase;

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

        public static List<Image> GetLastImageAlbum(Album a, int count)
        {
            List<Image> res = new List<Image>();
            if (!db.Entry(a).Collection(x1 => x1.Images).IsLoaded)
                db.Entry(a).Collection(x1 => x1.Images).Load();
            res.AddRange(a.Images.Skip(a.Images.Count-count>0? a.Images.Count - count:0).Select(x1=> x1.Image));

                return res;
        }
        public static List<AlbumShort> GetAlbumShortListForView(List<Album> a,int count)
        {
            List<AlbumShort> res = new List<AlbumShort>();
            for (var i=0;i<count;++i)
            {
                res.Add(GetAlbumShortForView(a[i]));
            }

            return res;
        }

        public static AlbumShort GetAlbumShortForView(Album a)
        {
            if (!db.Entry(a).Collection(x1 => x1.Images).IsLoaded)
                db.Entry(a).Collection(x1 => x1.Images).Load();
            var check = a.Images.LastOrDefault();
            if (check != null)
                if (!db.Entry(check).Reference(x1 => x1.Image).IsLoaded)
                    db.Entry(check).Reference(x1 => x1.Image).Load();
            return new AlbumShort(a);

        }

        public  void GetAlbumImages( int start, int count)
        {
            if (!db.Entry(this).Collection(x1 => x1.Images).IsLoaded)
                db.Entry(this).Collection(x1 => x1.Images).Load();
            foreach(var i in this.Images)
            {
                if (!db.Entry(i).Reference(x1 => x1.Image).IsLoaded)
                    db.Entry(i).Reference(x1 => x1.Image).Load();
            }


            return;

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