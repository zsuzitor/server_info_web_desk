using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static server_info_web_desk.Models.DataBase.DataBase;



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
        public bool AddMemesPrivate { get; set; }// отвечает за добавление записей и фотографий

        //public ICollection<SocialNetwork.Image> MainImages { get; set; }

        public string MainAdminId { get; set; }
        public ApplicationUser MainAdmin { get; set; }

        public ICollection<Record> RecordCreated { get; set; }

        public ICollection<ApplicationUser> Users { get; set; }
        public ICollection<ApplicationUser> Admins { get; set; }

        public ICollection<Album> Albums { get; set; }
        public ICollection<Record> WallRecord { get; set; }//тк есть еще записи которые можно репостить



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
            WallRecord = new List<Record>();
            RecordCreated = new List<Record>();

        }

        public static Group GetGroup(int? id)
        {
            //string check_id = ApplicationUser.GetUserId();
            Group res = null;

            using (ApplicationDbContext db = new ApplicationDbContext())
                res = Group.GetGroup(id,db);
            return res;
        }
        public static Group GetGroup(int? id, ApplicationDbContext db)
        {
            //string check_id = ApplicationUser.GetUserId();
            Group res = null;
            if (id == null || id < 1)
                return res;

                res = db.Groups.FirstOrDefault(x1 => x1.Id == id);
            return res;
        }



        public List<ApplicationUserShort> GetUserShortList( int count)
        {
            List<ApplicationUserShort> res = new List<ApplicationUserShort>();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Set<Group>().Attach(this);
                if (!db.Entry(this).Collection(x1 => x1.Users).IsLoaded)
                    db.Entry(this).Collection(x1 => x1.Users).Load();
            }
                
            res.AddRange(ApplicationUser.UserListToShort((List<ApplicationUser>)this.Users,null,count));
            return res;
        }
        public List<ApplicationUserShort> GetAdminShortList( int count)
        {
            List<ApplicationUserShort> res = new List<ApplicationUserShort>();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Set<Group>().Attach(this);
                if (!db.Entry(this).Collection(x1 => x1.Admins).IsLoaded)
                    db.Entry(this).Collection(x1 => x1.Admins).Load();
            }
                
            res.AddRange(ApplicationUser.UserListToShort((List<ApplicationUser>)this.Admins, null, count));
            return res;
        }

        public  void LoadDataForShort()
        {
            //картинка
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Set<Group>().Attach(this);
                if (!db.Entry(this).Collection(x1 => x1.Albums).IsLoaded)
                    db.Entry(this).Collection(x1 => x1.Albums).Load();
                if (!db.Entry(this.Albums.First()).Collection(x1 => x1.Images).IsLoaded)
                    db.Entry(this.Albums.First()).Collection(x1 => x1.Images).Load();
                var check = this.Albums.First().Images.LastOrDefault();
                if (check != null)
                {
                    if (!db.Entry(check).Reference(x1 => x1.Image).IsLoaded)
                        db.Entry(check).Reference(x1 => x1.Image).Load();
                }


                //подписчики
                if (!db.Entry(this).Collection(x1 => x1.Users).IsLoaded)
                    db.Entry(this).Collection(x1 => x1.Users).Load();
            }
            //GroupShort res = new GroupShort(this);

            return;
        }

        //добавляет на стену и раскидывает по новостям
        public void AddRecordMemeWall(Record record)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Set<Group>().Attach(this);
                db.Set<Record>().Attach(record);
                this.WallRecord.Add(record);

                if (!db.Entry(this).Collection(x1 => x1.Users).IsLoaded)
                    db.Entry(this).Collection(x1 => x1.Users).Load();
                ((List<Models.ApplicationUser>)record.UsersNews).AddRange(this.Users);
                
                db.SaveChanges();
            }
        }



        public List<Record> GetWallRecords(int start, int count)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Set<Group>().Attach(this);
                if (!db.Entry(this).Collection(x1 => x1.WallRecord).IsLoaded)
                    db.Entry(this).Collection(x1 => x1.WallRecord).Load();
            }
            
                
            
            List<Record> res = new List<Record>();//System.Collections.Generic.

            start = start > 0 ? start - 1 : 0;
            start = this.WallRecord.Count - start - count;

            res.AddRange(this.WallRecord.Skip(start).Take(count));
            foreach (var i in res)
            {
                i.RecordLoadForView();

            }

            return res;
        }
        public bool CanAddMeme(string user_id)
        {
            bool res = false;

            if (!this.AddMemesPrivate)
            {
                if (!this.OpenGroup)
                {
                    using (ApplicationDbContext db = new ApplicationDbContext())
                    {
                        db.Set<Group>().Attach(this);
                        if (!db.Entry(this).Collection(x1 => x1.Users).IsLoaded)
                            db.Entry(this).Collection(x1 => x1.Users).Load();
                    }
                    
                       
                    var u = this.Users.FirstOrDefault(x1 => x1.Id == user_id);
                    if (u == null)
                        res = false;
                }
                else
                    res = true;


            }
            else
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    db.Set<Group>().Attach(this);
                    if (!db.Entry(this).Collection(x1 => x1.Admins).IsLoaded)
                        db.Entry(this).Collection(x1 => x1.Admins).Load();
                }
                
                    
                var ch_adm = this.Admins.FirstOrDefault(x1 => x1.Id == user_id);
                if (ch_adm != null)
                    res = true;
            }
            
            return res;
        }
        //1 можно подписаться в списках нет, 2уже в одобренных подписках 3 user_id  подписан на this  но заявка не одобрена 0 ошибка или просто не отображать
        public int CanFollow(string user_id)
        {
            if (string.IsNullOrWhiteSpace(user_id))
                return 0;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Set<Group>().Attach(this);
                if (!db.Entry(this).Collection(x1 => x1.Users).IsLoaded)
                    db.Entry(this).Collection(x1 => x1.Users).Load();
                if (this.Users.Any(x1 => x1.Id == user_id))
                    return 2;
                //if (!db.Entry(this).Collection(x1 => x1.NotApproveUser).IsLoaded)
                //    db.Entry(this).Collection(x1 => x1.NotApproveUser).Load();
                //if (this.NotApproveUser.Any(x1 => x1.Id == user_id))
                //    return 3;
            }

            return 1;

           
        }


        //проверить может ли пользователь редактировать группу(является админом)
        public bool HaveAccessAdminGroup(string userId)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Set<Group>().Attach(this);
                if (!db.Entry(this).Collection(x1 => x1.Admins).IsLoaded)
                    db.Entry(this).Collection(x1 => x1.Admins).Load();
            }
            
                
            if (this.Admins.Any(x1 => x1.Id == userId))

                return true;
            return false;
        }

        //проверить может ли пользователь просматривать группу
        public bool HaveAccessGroup(string userId)
        {
           // bool res = false;
            if (!this.OpenGroup)
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    db.Set<Group>().Attach(this);
                    if (!db.Entry(this).Collection(x1 => x1.Users).IsLoaded)
                        db.Entry(this).Collection(x1 => x1.Users).Load();
                }
                    
                var us_c = this.Users.FirstOrDefault(x1 => x1.Id == userId);
                if (us_c == null)
                {
                    //TODO тут отправлять усеченную версию группы для тех кто не подписан и группа закрытая
                    return false;
                }
            }

            return true;
        }

        public List<Album> GetAlbums(int? id, int start = 0, int count = 1)
        {
            List<Album> res = new List<Album>();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Set<Group>().Attach(this);
                if (!db.Entry(this).Collection(x1 => x1.Albums).IsLoaded)
                    db.Entry(this).Collection(x1 => x1.Albums).Load();
            }
                
            if (id != null)
            {
                var al = this.Albums.FirstOrDefault(x1 => x1.Id == id);
                if (al != null)
                    res.Add(al);
            }
            else
            {
                start = start > 0 ? start - 1 : 0;
                //start = this.Albums.Count - start - count;
                res.AddRange(this.Albums.Skip(start).Take(count));
            }


            return res;
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
            Image = a.Albums.FirstOrDefault()?.Images.LastOrDefault()?.Image;
            CountFollowers = a.Users.Count;
        }
    }
}