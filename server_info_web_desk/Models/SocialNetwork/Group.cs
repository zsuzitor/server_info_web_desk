using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using server_info_web_desk.Models.Interfaces;
using static server_info_web_desk.Models.DataBase.DataBase;
using static server_info_web_desk.Models.functions.FunctionsProject;



namespace server_info_web_desk.Models.SocialNetwork
{
    public class Group : IPageR, IHaveAlbum, IDomain<int>, IDeleteDb<Group>
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

        //public List<Record> RecordCreated { get; set; }

        public List<ApplicationUser> Users { get; set; }
        public List<ApplicationUser> Admins { get; set; }

        public List<Album> Albums { get; set; }
        public List<Record> WallRecord { get; set; }//тк есть еще записи которые можно репостить

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
            //RecordCreated = new List<Record>();

        }



        public static Group CreateNewGroup(string name)
        {
            ApplicationUser user = ApplicationUser.GetUser(ApplicationUser.GetUserId());
            Group res = new Group() { Name = name, MainAdminId = user.Id };
            
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Groups.Add(res);
                db.SaveChanges();

                db.Albums.Add(new Album()
                {
                    Name = "Main",
                    Description = "Сюда добавляются главные фотографии с вашей страницы",
                    //User = admin
                    GroupId = res.Id
                });
                db.Albums.Add(new Album()
                {
                    Name = "NotMain",
                    Description = "Сюда добавляются фотографии с вашей страницы",
                    GroupId = res.Id
                });

                db.Set<ApplicationUser>().Attach(user);

                res.Admins.Add(user);
                res.Users.Add(user);
                db.SaveChanges();
            }
            return res;
        }

        public Group DeleteFull(out bool success)
        {
            success = false;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                if (!db.Entry(this).Collection(x1 => x1.WallRecord).IsLoaded)
                    db.Entry(this).Collection(x1 => x1.WallRecord).Load();
                foreach(var i in this.WallRecord)
                {
                    bool suc;
                    i.DeleteFull(out suc);
                }
                db.SaveChanges();

                if (!db.Entry(this).Collection(x1 => x1.Albums).IsLoaded)
                    db.Entry(this).Collection(x1 => x1.Albums).Load();
                foreach (var i in this.Albums)
                {
                    bool suc;
                    i.DeleteFull(out suc);
                }
                db.SaveChanges();


                if (!db.Entry(this).Collection(x1 => x1.Users).IsLoaded)
                    db.Entry(this).Collection(x1 => x1.Users).Load();
                if (!db.Entry(this).Collection(x1 => x1.Admins).IsLoaded)
                    db.Entry(this).Collection(x1 => x1.Admins).Load();


                db.Groups.Remove(this);
                db.SaveChanges();

            }
            success = true;
            return this;
        }


        public int Follow(ApplicationUser user)
        {
            int res = 0;
            if (this == null)
                return 0;
            res = this.CanFollow(user.Id);
            var group = this;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                
                db.Set<ApplicationUser>().Attach(user);
                try
                {
                    db.Set<Group>().Attach(group);
                }
                catch
                {
                    //оставить именно так а не методом
                    //group = db.Groups.First(x1 => x1.Id == group.Id);
                    group = Group.GetGroup(group.Id, db);
                }
                if (!db.Entry(group).Collection(x1 => x1.Users).IsLoaded)
                    db.Entry(group).Collection(x1 => x1.Users).Load();
                switch (res)
                {
                    case 1:
                        group.Users.Add(user);
                        res = 2;
                        break;
                    case 2:
                        group.Users.Remove(user);
                        res = 1;
                        break;
                    case 3:
                        break;
                }
              
                db.SaveChanges();
            }
            return res;
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


        public List<ApplicationUserShort> GetFollowersShortList(int count)
        {
            //TODO пока что нет фоловеров
            List<ApplicationUserShort> res = new List<ApplicationUserShort>();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                //db.Set<Group>().Attach(this);
                //if (!db.Entry(this).Collection(x1 => x1.).IsLoaded)
                //    db.Entry(this).Collection(x1 => x1.Users).Load();
            }

            //res.AddRange(ApplicationUser.UserListToShort((List<ApplicationUser>)this.Users, this.Users.Count, count));
            res.AddRange(((List<ApplicationUser>)GetPartialList<ApplicationUser>(this.Users, 0, count)).Select(x1 =>
                      new Models.ApplicationUserShort(x1)));

            return res;
        }
        public List<ApplicationUserShort> GetUserShortList(int start, int count)
        {
            List<ApplicationUserShort> res = new List<ApplicationUserShort>();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Set<Group>().Attach(this);
                if (!db.Entry(this).Collection(x1 => x1.Users).IsLoaded)
                    db.Entry(this).Collection(x1 => x1.Users).Load();
            }
                
            //res.AddRange(ApplicationUser.UserListToShort((List<ApplicationUser>)this.Users, this.Users.Count, count));
            res.AddRange(((List<ApplicationUser>)GetPartialList<ApplicationUser>(this.Users, start, count)).Select(x1 =>
                      new Models.ApplicationUserShort(x1)));

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
                
            //res.AddRange(ApplicationUser.UserListToShort((List<ApplicationUser>)this.Admins, this.Admins.Count, count));
            res.AddRange(((List<ApplicationUser>)GetPartialList<ApplicationUser>(this.Admins, 0, count)).Select(x1 =>
                      new Models.ApplicationUserShort(x1)));
            return res;
        }

        public  bool LoadDataForShort()
        {
            Album alb = null;
            //картинка
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Set<Group>().Attach(this);
                //подписчики
                if (!db.Entry(this).Collection(x1 => x1.Users).IsLoaded)
                    db.Entry(this).Collection(x1 => x1.Users).Load();
                if (!db.Entry(this).Collection(x1 => x1.Albums).IsLoaded)
                    db.Entry(this).Collection(x1 => x1.Albums).Load();

                alb = this.Albums.First();

            }
            alb.LoadDataForShort();
            //GroupShort res = new GroupShort(this);

            return true;
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

            res.AddRange((List<Record>)GetPartialList<Record>(this.WallRecord, start, count));

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


        public bool? AddImage(string text, HttpPostedFileBase[] uploadImage, int? album_id, int? id_group)
        {
            ApplicationUser user = null;
            string check_id = ApplicationUser.GetUserId();
            bool can_add = this.CanAddMeme(check_id);
            if (!can_add)
                return null;

            var album = this.GetAlbums(album_id).FirstOrDefault();
            //TODO проверка
            var ch_al = this.GetAlbums(null, 0, 1,true,false).First();

            if (can_add && ch_al.Id == album.Id)
            {
                can_add = this.HaveAccessAdminGroup(check_id);
            }
            if (!can_add)
                return null;

            if (album == null)
                return null;
            var list_img_byte = Get_photo_post(uploadImage);
            if (user == null)
                user = ApplicationUser.GetUser(check_id);
            //user =db.Users.FirstOrDefault(x1 => x1.Id == check_id);

            var record = Record.AddRecordImage(album, user, this, list_img_byte, text);

            this.AddRecordMemeWall(record);


            return true;
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

        public List<Album> GetAlbums(int? id, int start = 0, int? count = null, bool get_start = false, bool end = true)
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
                
                res.AddRange((List<Album>)GetPartialList<Album>(this.Albums, start, count, get_start, end));
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