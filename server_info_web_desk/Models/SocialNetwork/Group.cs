using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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


        public  void GetUserShortList( List<ApplicationUserShort> a,int count)
        {
            if (!db.Entry(this).Collection(x1 => x1.Users).IsLoaded)
                db.Entry(this).Collection(x1 => x1.Users).Load();
            a.AddRange(ApplicationUser.UserListToShort((List<ApplicationUser>)this.Users,null,count));
            return;
        }
        public  void GetAdminShortList( List<ApplicationUserShort> a, int count)
        {
            if (!db.Entry(this).Collection(x1 => x1.Admins).IsLoaded)
                db.Entry(this).Collection(x1 => x1.Admins).Load();
            a.AddRange(ApplicationUser.UserListToShort((List<ApplicationUser>)this.Admins, null, count));
            return;
        }

        public  GroupShort GetGroupShort()
        {
            //картинка
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

            GroupShort res = new GroupShort(this);

            return res;
        }


        public void AddRecordMemeWall(Record record)
        {
            this.WallRecord.Add(record);
            if (!db.Entry(this).Collection(x1 => x1.Users).IsLoaded)
                db.Entry(this).Collection(x1 => x1.Users).Load();
            ((List<Models.ApplicationUser>)record.UsersNews).AddRange(this.Users);
            //foreach(var i in group.Users)
            //{
            //    i.News.Add(record);
            //}
            db.SaveChanges();
        }



        public List<Record> GetWallRecords(int start, int count)
        {
            if (!db.Entry(this).Collection(x1 => x1.WallRecord).IsLoaded)
                db.Entry(this).Collection(x1 => x1.WallRecord).Load();
            
            List<Record> res = new List<Record>();//System.Collections.Generic.
           
            res.AddRange(this.WallRecord.Reverse().Skip(start > 0 ? start - 1 : 0).Take(count));
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
                    if (!db.Entry(this).Collection(x1 => x1.Users).IsLoaded)
                        db.Entry(this).Collection(x1 => x1.Users).Load();
                    var u = this.Users.FirstOrDefault(x1 => x1.Id == user_id);
                    if (u == null)
                        res = false;
                }
                else
                    res = true;


            }
            else
            {
                if (!db.Entry(this).Collection(x1 => x1.Admins).IsLoaded)
                    db.Entry(this).Collection(x1 => x1.Admins).Load();
                var ch_adm = this.Admins.FirstOrDefault(x1 => x1.Id == user_id);
                if (ch_adm != null)
                    res = true;
            }
            
            return res;
        }
        public bool? CanFollow(string user_id)
        {
            bool? res = true;
            if (!db.Entry(this).Collection(x1 => x1.Users).IsLoaded)
                db.Entry(this).Collection(x1 => x1.Users).Load();
            var ch_can_foll = this.Users.FirstOrDefault(x1 => x1.Id == user_id);
            if (ch_can_foll != null)
                res = false;
            //TODO тут еще искать по списку не одобренных заявок и если найдено то отправлять null


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