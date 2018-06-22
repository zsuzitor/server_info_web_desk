using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using server_info_web_desk.Models;
using server_info_web_desk.Models.Info;
using System.Collections.Generic;
using server_info_web_desk.Models.SocialNetwork;
using System;
using server_info_web_desk.Models.Interfaces;
using static server_info_web_desk.Models.DataBase.DataBase;
using System.Linq;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace server_info_web_desk.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser, Iuser
    {
        [Required(ErrorMessage="Обязательный параметр")]
        [Display(Name = "Имя")]
        public string Name { get; set; }
        [Display(Name = "Статус")]
        public string Status { get; set; }
        [Required(ErrorMessage = "Обязательный параметр")]
        [Display(Name = "День Рождения")]
        [DataType(DataType.Date)]
        public DateTime? Birthday { get; set; }
        [Display(Name = "Пол")]
        public bool Sex { get; set; }
        public int? Age { get; set; }
        [Required(ErrorMessage = "Обязательный параметр")]
        [Display(Name = "Фамилия")]
        public string Surname { get; set; }
        [Display(Name = "Страна")]
        public string Country { get; set; }
        [Display(Name = "Город")]
        public string Town { get; set; }
        [Display(Name = "Улица")]
        public string Street { get; set; }
        public bool Online { get; set; }
        [Display(Name = "О себе")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        [Display(Name = "Открытая стена")]
        public bool? WallOpenWrite { get; set; }//кто может писать на стену false только владелец страницы true-все null-друзья
        [Display(Name = "Открытая страница")]
        public bool PrivatePage { get; set; }//кто может просмотреть всю страницу false только владелец страницы true-все null-друзья
        public int New_message_count { get; set; }
        public DateTime DateRegistration { get; set; }
        [Display(Name = "Открытые секции и статьи")]
        public bool Open_data_info { get; set; }



        public List<Section> Sections { get; set; }
        public List<Article> Articles { get; set; }
        public List<Info.ImageInfo> ImagesInfo { get; set; }



        //public ICollection<SocialNetwork.Image> ImagesSocial { get; set; }

        public List<Meme> MemeCreated { get; set; }
        public List<Record> WallRecord { get; set; }
        public List<Record> UsersLikesRecord { get; set; }
        public List<Record> UsersRipostes { get; set; }
        public List<Record> News { get; set; }

        public List<Album> Albums { get; set; }

        public List<Comment> Comments { get; set; }//которые пользователь оставил где либо
        public List<Comment> UsersLikesComment { get; set; }//комменты которые он лайкнул

        public List<Message> MessagesCreated { get; set; }

        public List<Chat> ChatsCreated { get; set; }
        public List<Chat> Chats { get; set; }

        public List<Group> GroupCreared { get; set; }
        public List<Group> Group { get; set; }
        public List<Group> GroupAdmin { get; set; }


        public List<Message> MessageNeedRead { get; set; }
        //TODO
        public List<ApplicationUser> Friends { get; set; }//друзья пользователя
        public List<ApplicationUser> FriendUser { get; set; }//тоже друзья(для связи), сюда не обращаться
        public List<ApplicationUser> Followers { get; set; }//фолловеры пользователя
        public List<ApplicationUser> FollowUser { get; set; }//на кого зафоловлен

        public ApplicationUser() : base()
        {

            Age = null;
           
            Online = false;
            Surname = null;
            Country = null;
            Town = null;
            Street = null;
            Status = null;
            Birthday = null;
            Sex = true;
            New_message_count = 0;
            //о себе 
            Description = null;
            WallOpenWrite = true;
            Open_data_info = false;
            PrivatePage = false;
            DateRegistration = DateTime.Now;

            MessageNeedRead = new List<Message>();
            Sections =new List<Section>();
            Articles = new List<Article>();
            ImagesInfo = new List<ImageInfo>();
            MemeCreated = new List<Meme>();
            WallRecord = new List<Record>();
            UsersLikesRecord = new List<Record>();
            UsersRipostes = new List<Record>();
            News = new List<Record>();
            Albums = new List<Album>();
            Comments = new List<Comment>();
            UsersLikesComment = new List<Comment>();
            MessagesCreated = new List<Message>();
            ChatsCreated = new List<Chat>();
            Chats = new List<Chat>();
            GroupCreared = new List<Group>();
            Group = new List<Group>();
            GroupAdmin = new List<Group>();
            Followers = new List<ApplicationUser>();
            FollowUser = new List<ApplicationUser>();
            Friends = new List<ApplicationUser>();
            FriendUser= new List<ApplicationUser>();
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }


        //---------------------------------METHODS------------------------------------------------------------------
        public static string GetUserId()
        {
            return System.Web.HttpContext.Current.User.Identity.GetUserId();
        }

        public static ApplicationUser GetUser(string id)
        {
            //string check_id = ApplicationUser.GetUserId();
            ApplicationUser res = null;
            //if (string.IsNullOrWhiteSpace(id))
            //    return res;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                res = ApplicationUser.GetUser(id,db);
            }
                
            return res;
        }
        public static ApplicationUser GetUser(string id, ApplicationDbContext db)
        {
            //string check_id = ApplicationUser.GetUserId();
            ApplicationUser res = null;
            if (string.IsNullOrWhiteSpace(id))
                return res;
            res = db.Users.FirstOrDefault(x1 => x1.Id == id);
            
            return res;
        }

        public Chat GetChat(int? id_chat)
        {
            //тут возможно не загружать диалоги юзера а искать диалог по id и загружать его пользователей мб так лучше
            if (id_chat == null)
            
                return null;
            else
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    db.Set<ApplicationUser>().Attach(this);
                    if (!db.Entry(this).Collection(x1 => x1.Chats).IsLoaded)
                        db.Entry(this).Collection(x1 => x1.Chats).Load();
                }
                    
                
                var dialog = this.Chats.FirstOrDefault(x1 => x1.Id == id_chat);
              
                return dialog;
            }

        }
        public Chat GetChat(string id_user)
        {
            if (id_user == null)
                return null;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Set<ApplicationUser>().Attach(this);
                if (!db.Entry(this).Collection(x1 => x1.Chats).IsLoaded)
                    db.Entry(this).Collection(x1 => x1.Chats).Load();

                foreach (var i in this.Chats)
                {
                    if (!db.Entry(i).Collection(x2 => x2.Users).IsLoaded)
                        db.Entry(i).Collection(x2 => x2.Users).Load();
                    if (i.Users.FirstOrDefault(x2 => x2.Id == id_user) != null && i.Users.Count == 2)
                        return i;
                }
            }
            return null;
        }


        //если указан старт то count должен быть>0
        public static List<ApplicationUserShort> UserListToShort(List<ApplicationUser> a,int? start, int count)
        {
            List<ApplicationUserShort> res = new List<ApplicationUserShort>();
            if (start == null)
            {

           
            if (count > 0)
                res.AddRange(a.Take((count <= a.Count ? count : a.Count)).Select(x1 =>
                       new Models.ApplicationUserShort(x1)));
            else
                res.AddRange(a.Skip((a.Count - Math.Abs(count) > 0 ? a.Count - Math.Abs(count) : 0)).Select(x1 =>
                         new Models.ApplicationUserShort(x1)));
            }
            else
                res.AddRange(a.Skip(((int)start > 1 ? (int)start -1 : (int)start)).Take(count).Select(x1 =>
                             new Models.ApplicationUserShort(x1)));
            
            return res;
        }

        //редактирует и сохраняем данные пользователя
        public bool ChageUserData(ApplicationUser a)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Set<ApplicationUser>().Attach(a);

                if (!string.IsNullOrWhiteSpace(a.Name))
                this.Name = a.Name;
            if (!string.IsNullOrWhiteSpace(a.Surname))
                this.Surname = a.Surname;

            this.Status = a.Status;
            this.Birthday = a.Birthday;
            this.Sex = a.Sex;
            this.Country = a.Country;
            this.Town = a.Town;
            this.Street = a.Street;
            this.Description = a.Description;
            this.WallOpenWrite = a.WallOpenWrite;
            this.PrivatePage = a.PrivatePage;
            this.Open_data_info = a.Open_data_info;
            
                db.SaveChanges();
            }
            return true;

        }


        //добавить запись на стену пользователя
        public void AddRecordWall(Record record)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Set<ApplicationUser>().Attach(this);
                db.Set<Record>().Attach(record);
                if (!db.Entry(this).Collection(x1 => x1.Friends).IsLoaded)
                    db.Entry(this).Collection(x1 => x1.Friends).Load();
                if (!db.Entry(this).Collection(x1 => x1.Followers).IsLoaded)
                    db.Entry(this).Collection(x1 => x1.Followers).Load();
                if (!db.Entry(this).Collection(x1 => x1.News).IsLoaded)
                    db.Entry(this).Collection(x1 => x1.News).Load();
                this.News.Add(record);
                //user.WallRecord.Add(record);


                ((List<Models.ApplicationUser>)record.UsersNews).AddRange(this.Friends);
                ((List<Models.ApplicationUser>)record.UsersNews).AddRange(this.Followers);
                
                db.SaveChanges();
            }
        }

        //добавить запись в новости пользователя
        public void AddRecordNews( Record record)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Set<ApplicationUser>().Attach(this);
                db.Set<Record>().Attach(record);
                this.News.Add(record);
                
                db.SaveChanges();
            }
        }

        //вернуть список усеченных гурупп пользователя
        public  List<GroupShort> UserGroupToShort( int? start, int count)
        {
            List<GroupShort> res = new List<GroupShort>();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Set<ApplicationUser>().Attach(this);
                if (!db.Entry(this).Collection(x1 => x1.Group).IsLoaded)
                    db.Entry(this).Collection(x1 => x1.Group).Load();
            }
            
                

            res.AddRange(this.Group.Select(x1 => {
                x1.LoadDataForShort();
                return new GroupShort(x1);
                }));
            
            return res;
        }
        //получить список записей на стене
        public List<Record> GetWallRecords(int start, int count)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Set<ApplicationUser>().Attach(this);
                if (!db.Entry(this).Collection(x1 => x1.WallRecord).IsLoaded)
                    db.Entry(this).Collection(x1 => x1.WallRecord).Load();
            }
            
               
           
            List<Record> res = new List<Record>();//System.Collections.Generic.
            //this.WallRecord.Reverse();
            start = start > 0 ? start - 1 : 0;
            start = this.WallRecord.Count - start - count;
            res.AddRange(this.WallRecord.Skip(start).Take(count));
            foreach (var i in res)
            {
                i.RecordLoadForView();
                
            }
            
            return res;
        }

        //получить список записей в новостях
        public List<Record> GetNewsRecords(int start, int count)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Set<ApplicationUser>().Attach(this);
                if (!db.Entry(this).Collection(x1 => x1.News).IsLoaded)
                    db.Entry(this).Collection(x1 => x1.News).Load();
            }
            
                
           
            List<Record> res = new List<Record>();//System.Collections.Generic.
            //this.News.Reverse();
            start = start > 0 ? start - 1 : 0;
            start = this.News.Count - start - count;
            res.AddRange(this.News.Skip(start).Take(count));
            foreach (var i in res)
            {
                i.RecordLoadForView();

            }

            return res;
        }

        //получить альбом по номеру или id
        public List<Album> GetAlbums(int? id,int start=0, int count=1)
        {
            List<Album> res = new List<Album>();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Set<ApplicationUser>().Attach(this);
                if (!db.Entry(this).Collection(x1 => x1.Albums).IsLoaded)
                    db.Entry(this).Collection(x1 => x1.Albums).Load();
            }
            
                
            if (id != null)
            {
                var al=this.Albums.FirstOrDefault(x1=>x1.Id==id);
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

        //проверка на то можно ли писать на стене
        public bool CanAddRecordWall(string user_id_action)
        {
            bool res = false;
            if (user_id_action != this.Id)
            {
                if (this.WallOpenWrite == true)
                    res = true;
                else
                    if (this.WallOpenWrite == null)
                {
                    using (ApplicationDbContext db = new ApplicationDbContext())
                    {
                        db.Set<ApplicationUser>().Attach(this);
                        if (!db.Entry(this).Collection(x1 => x1.Friends).IsLoaded)
                            db.Entry(this).Collection(x1 => x1.Friends).Load();
                    }

                        
                    var ch_acc = this.Friends.FirstOrDefault(x1 => x1.Id == user_id_action);
                    if (ch_acc != null)
                        res = true;
                }
            }
            else
                res = true;
            return res;
        }


        //проверка на то что отобразить (добавить отписаться удалить)
        //1 можно добавить в списках нет, 2уже в друзьях 3 user_id  подписан н this 4 this подписан на user_id 0 ошибка или просто не отображать
        public int CanFollow(string user_id)
        {
            if (string.IsNullOrWhiteSpace(user_id))
                return 0;
            //int res = 0;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Set<ApplicationUser>().Attach(this);
                if (!db.Entry(this).Collection(x1 => x1.Friends).IsLoaded)
                    db.Entry(this).Collection(x1 => x1.Friends).Load();

                if (this.Friends.Any(x1 => x1.Id == user_id))
                    return 2;

                if (!db.Entry(this).Collection(x1 => x1.Followers).IsLoaded)
                    db.Entry(this).Collection(x1 => x1.Followers).Load();
                if (this.Followers.Any(x1 => x1.Id == user_id))
                    return 3;
                if (!db.Entry(this).Collection(x1 => x1.FollowUser).IsLoaded)
                    db.Entry(this).Collection(x1 => x1.FollowUser).Load();
                if (this.FollowUser.Any(x1 => x1.Id == user_id))
                    return 4;
                return 1;
            }
            //TODO тут еще искать по списку не одобренных заявок и если найдено то отправлять null


            //return res;
        }
        public Chat GetListMessages(int? id,bool? new_m,int start, int count)
        {
            Chat res = new Chat();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Set<ApplicationUser>().Attach(this);
                List<Message> not_res = new List<Message>();
                if (!db.Entry(this).Collection(x1 => x1.Chats).IsLoaded)
                    db.Entry(this).Collection(x1 => x1.Chats).Load();

                var dialog = this.Chats.FirstOrDefault(x1 => x1.Id == id);
                if (dialog == null)
                    return null;
                res = new Chat() { Id = dialog.Id };
                if (!db.Entry(dialog).Collection(x1 => x1.Messages).IsLoaded)
                    db.Entry(dialog).Collection(x1 => x1.Messages).Load();

                //dialog.Messages= dialog.Messages.Reverse().ToList();
                start = start > 0 ? start - 1 : 0;
                start = dialog.Messages.Count - start - count;
                not_res.AddRange(dialog.Messages.Skip(start).Take(count));

                foreach (var i in not_res)
                {
                    if (!db.Entry(i).Collection(x1 => x1.UserNeedRead).IsLoaded)
                        db.Entry(i).Collection(x1 => x1.UserNeedRead).Load();
                    var us = i.UserNeedRead.FirstOrDefault(x1 => x1.Id == this.Id);
                    if (us != null)
                    {
                        i.UserNeedRead.Remove(us);
                    }
                    if (new_m == true)
                    {
                        if (us != null)
                        {
                            if (!db.Entry(i).Reference(x1 => x1.Creator).IsLoaded)
                                db.Entry(i).Reference(x1 => x1.Creator).Load();
                            res.Messages.Add(i);
                        }
                    }
                    else
                    {
                        if (!db.Entry(i).Reference(x1 => x1.Creator).IsLoaded)
                            db.Entry(i).Reference(x1 => x1.Creator).Load();
                        res.Messages.Add(i);
                    }
                       
                }
                db.SaveChanges();
            }
            foreach (var i in res.Messages)
                {
                i.Creator.LoadDataForShort();
                }
           
            return res;
        }


        public Message SendNewMessage(int dialog,List<byte[]>images,string text)
        {
            Message res = null;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Set<ApplicationUser>().Attach(this);
                if (!db.Entry(this).Collection(x1 => x1.Chats).IsLoaded)
                    db.Entry(this).Collection(x1 => x1.Chats).Load();
                var chat = this.Chats.FirstOrDefault(x1 => x1.Id == dialog);
                if (chat == null)
                    return null;


                res = new Message() { Text = text, CreatorId = this.Id, ChatId = dialog };

                db.Messages.Add(res);
                db.SaveChanges();

                List<Image> image_list = new List<Models.SocialNetwork.Image>();
                foreach (var i in images)
                {
                    //кратинки еще и в бд и тдтд

                    var img = new Image() { Data = i, UserId = this.Id, MessageId = res.Id };
                    db.ImagesSocial.Add(img);
                    db.SaveChanges();
                    image_list.Add(img);

                }

                if (!db.Entry(chat).Collection(x1 => x1.Users).IsLoaded)
                    db.Entry(chat).Collection(x1 => x1.Users).Load();
                foreach (var i in chat.Users)
                    res.UserNeedRead.Add(i);
                db.SaveChanges();


            }
            return res;
        }
        public bool LoadDataForShort()
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Set<ApplicationUser>().Attach(this);
                if (!db.Entry(this).Collection(x2 => x2.Albums).IsLoaded)
                {
                    db.Entry(this).Collection(x2 => x2.Albums).Load();
                }
                if (!db.Entry(this.Albums.First()).Collection(x2 => x2.Images).IsLoaded)
                {
                    db.Entry(this.Albums.First()).Collection(x2 => x2.Images).Load();
                }
                var check = this.Albums.First().Images.FirstOrDefault();
                if(check!=null)
                if (!db.Entry(check).Reference(x2 => x2.Image).IsLoaded)
                {
                    db.Entry(check).Reference(x2 => x2.Image).Load();
                }
            }
            return true;
        }
    }

    public class ApplicationUserShort
    {
        public string PageId { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string Surname { get; set; }
        public bool Online { get; set; }
        public Image Image{ get; set; }

        public ApplicationUserShort()
        {
            Name = null;
            PageId = null;
                Status = null;
            Surname = null;
            Surname = null;
            Online = false;
            Image = null;
        }

        public ApplicationUserShort(ApplicationUser a)
        {
            Name = a.Name;
            Status = a.Status;
            Surname = a.Surname;
            Online = a.Online;
            PageId = a.Id;

            a.LoadDataForShort();
            Image = a.Albums.First().Images.FirstOrDefault()?.Image;
        }
    }


    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>//,IDisposable
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public DbSet<Article> Articles { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Info.ImageInfo> ImagesInfo { get; set; }
        public DbSet<SocialNetwork.Image> ImagesSocial { get; set; }
        public DbSet<Meme> Memes { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Record> Record { get; set; }

        public static ApplicationDbContext Create()
        {
            
            return new ApplicationDbContext();
        }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Record>().HasMany(c => c.UsersLikes)
                .WithMany(s => s.UsersLikesRecord)
                .Map(t => t.MapLeftKey("RecordId")
                .MapRightKey("ApplicationUserId")
                .ToTable("ApplicationUserRecordLike"));

            modelBuilder.Entity<Record>().HasMany(c => c.UsersRipostes)
                .WithMany(s => s.UsersRipostes)
                .Map(t => t.MapLeftKey("RecordId")
                .MapRightKey("ApplicationUserId")
                .ToTable("ApplicationUserRecordRiposte"));

            modelBuilder.Entity<Comment>().HasMany(c => c.UsersLikes)
                .WithMany(s => s.UsersLikesComment)
                .Map(t => t.MapLeftKey("CommentId")
                .MapRightKey("ApplicationUserId")
                .ToTable("ApplicationUserComment"));

            modelBuilder.Entity<Group>().HasMany(c => c.Users)
                .WithMany(s => s.Group)
                .Map(t => t.MapLeftKey("GroupId")
                .MapRightKey("ApplicationUserId")
                .ToTable("ApplicationUserGroupUsers"));

            modelBuilder.Entity<Group>().HasMany(c => c.Admins)
                .WithMany(s => s.GroupAdmin)
                .Map(t => t.MapLeftKey("GroupId")
                .MapRightKey("ApplicationUserId")
                .ToTable("ApplicationUserGroupAdmins"));

            modelBuilder.Entity<Chat>().HasMany(c => c.Users)
               .WithMany(s => s.Chats)
               .Map(t => t.MapLeftKey("ChatId")
               .MapRightKey("ApplicationUserId")
               .ToTable("ApplicationUserChat"));

            modelBuilder.Entity<Record>().HasMany(c => c.UsersNews)
               .WithMany(s => s.News)
               .Map(t => t.MapLeftKey("RecordId")
               .MapRightKey("ApplicationUserId")
               .ToTable("ApplicationUserRecordNews"));

            modelBuilder.Entity<Record>().HasMany(c => c.GroupWall)
               .WithMany(s => s.WallRecord)
               .Map(t => t.MapLeftKey("RecordId")
               .MapRightKey("GroupId")
               .ToTable("GroupRecordWall"));

            modelBuilder.Entity<ApplicationUser>().HasMany(c => c.Followers)
               .WithMany(s => s.FollowUser)
               .Map(t => t.MapLeftKey("ApplicationUserFollowersId")
               .MapRightKey("ApplicationUserFollowId")
               .ToTable("ApplicationUserFollowers"));

            modelBuilder.Entity<ApplicationUser>().HasMany(c => c.Friends)
              .WithMany(s => s.FriendUser)
              .Map(t => t.MapLeftKey("ApplicationUserFriendsId")
              .MapRightKey("ApplicationUserFriendUserId")
              .ToTable("ApplicationUserFriends"));

            modelBuilder.Entity<Message>().HasMany(c => c.UserNeedRead)
              .WithMany(s => s.MessageNeedRead)
              .Map(t => t.MapLeftKey("MessageId")
              .MapRightKey("ApplicationUserId")
              .ToTable("MessageApplicationUser"));

            base.OnModelCreating(modelBuilder);
        }
    }
}