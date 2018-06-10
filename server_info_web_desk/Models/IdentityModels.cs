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

namespace server_info_web_desk.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser, Iuser
    {
        
        public string Name { get; set; }
        public string Status { get; set; }
        public DateTime? Birthday { get; set; }
        public bool Sex { get; set; }
        public int? Age { get; set; }
        public string Surname { get; set; }
        public string Country { get; set; }
        public string Town { get; set; }
        public string Street { get; set; }
        public bool Online { get; set; }
        public string Description { get; set; }
        public bool? WallOpenWrite { get; set; }//кто может писать на стену false только владелец страницы true-все null-друзья
        public bool PrivatePage { get; set; }//кто может просмотреть всю страницу false только владелец страницы true-все null-друзья
        public int New_message_count { get; set; }


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
        }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
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

            if (!db.Entry(a).Collection(x2 => x2.Albums).IsLoaded)
            {
                db.Entry(a).Collection(x2 => x2.Albums).Load();
            }
            if (!db.Entry(a.Albums.First()).Collection(x2 => x2.Images).IsLoaded)
            {
                db.Entry(a.Albums.First()).Collection(x2 => x2.Images).Load();
            }
            

            Image = a.Albums.First().Images.FirstOrDefault()?.Image;
        }
    }


    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
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
            //modelBuilder.Entity<Meme>().HasMany(c => c.UsersLikes)
            //    .WithMany(s => s.UsersLikesMeme)
            //    .Map(t => t.MapLeftKey("MemeId")
            //    .MapRightKey("ApplicationUserId")
            //    .ToTable("ApplicationUserMeme"));
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

            base.OnModelCreating(modelBuilder);
        }
    }
}