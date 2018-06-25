using server_info_web_desk.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static server_info_web_desk.Models.DataBase.DataBase;
using static server_info_web_desk.Models.functions.FunctionsProject;

namespace server_info_web_desk.Models.SocialNetwork
{
    public class Album:IDomain<int>
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
        //1 альбом для пользователя 2-для группы
        public static Album CreateNew(string name,string id,int type)
        {
            Album res = new Album() { Name=name };
            if (type == 1)
            {
                res.UserId = id;
            }
            if (type == 2)
            {
                res.GroupId = Convert.ToInt32(id);
            }
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Albums.Add(res);
                db.SaveChanges();
            }

                return res;
        }

        public static List<Image> GetLastImageAlbum(Album a, int count)
        {
            List<Image> res = new List<Image>();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Set<Album>().Attach(a);
                if (!db.Entry(a).Collection(x1 => x1.Images).IsLoaded)
                    db.Entry(a).Collection(x1 => x1.Images).Load();
            }
                
            res.AddRange(a.Images.Skip(a.Images.Count-count>0? a.Images.Count - count:0).Select(x1=> x1.Image));

                return res;
        }
        //public static List<AlbumShort> GetAlbumShortListForView(List<Album> a,int count)
        //{
        //    List<AlbumShort> res = new List<AlbumShort>();
        //    for (var i=0;i<count;++i)
        //    {
        //        res.Add(new AlbumShort(a[i]));
        //    }

        //    return res;
        //}

        //public static AlbumShort GetAlbumShortForView(Album a)
        //{
        //    a.LoadDataForShort();
        //    return new AlbumShort(a);

        //}
        public bool LoadDataForShort()
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Set<Album>().Attach(this);
                if (!db.Entry(this).Collection(x1 => x1.Images).IsLoaded)
                    db.Entry(this).Collection(x1 => x1.Images).Load();
                var check = this.Images.LastOrDefault();
                if (check != null)
                    if (!db.Entry(check).Reference(x1 => x1.Image).IsLoaded)
                        db.Entry(check).Reference(x1 => x1.Image).Load();

            }

            return true;
        }
        public  List<Image> GetAlbumImages( int start, int count)
        {
            List<Record> res = new List<Record>();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Set<Album>().Attach(this);
                if (!db.Entry(this).Collection(x1 => x1.Images).IsLoaded)
                    db.Entry(this).Collection(x1 => x1.Images).Load();

                res.AddRange((List<Record>)GetPartialList<Record>(this.Images, start, count));
                foreach (var i in res)
                {
                    if (!db.Entry(i).Reference(x1 => x1.Image).IsLoaded)
                        db.Entry(i).Reference(x1 => x1.Image).Load();
                }
            }

            return res.Select(x1 => x1.Image).ToList();

        }


        public static Album GetAlbum(int id)
        {
            Album album = null;
            using (ApplicationDbContext db = new ApplicationDbContext())
                album = db.Albums.FirstOrDefault(x1 => x1.Id == id);
            return album;
        }

            //----------------------------------------------------------------------------обобщенные
            //public static List<AlbumShort> GetAlbums<T>(T a, int start, int count)
            //{
            //    using (ApplicationDbContext db = new ApplicationDbContext())
            //    {
            //        db.Set<T>().Attach(a);
            //        if (!db.Entry(a).Collection(x1 => x1.Albums).IsLoaded)
            //            db.Entry(userPage).Collection(x1 => x1.Albums).Load();
            //    }

            //}
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

            a.LoadDataForShort();

            CountImage = a.Images.Count;
            Image = a.Images.LastOrDefault()?.Image;
        }
    }
}