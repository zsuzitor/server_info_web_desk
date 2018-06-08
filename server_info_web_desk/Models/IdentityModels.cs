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


        public int New_message_count { get; set; }


        public bool Open_data_info { get; set; }



        public ICollection<Section> Sections { get; set; }
        public ICollection<Article> Articles { get; set; }
        public ICollection<Info.ImageInfo> ImagesInfo { get; set; }



        //public ICollection<SocialNetwork.Image> ImagesSocial { get; set; }

        public ICollection<Meme> MemeCreated { get; set; }
        public ICollection<Meme> WallMeme { get; set; }
        public ICollection<Meme> UsersLikesMeme { get; set; }
        public ICollection<Meme> UsersRipostes { get; set; }
        public ICollection<Meme> News { get; set; }

        public ICollection<Album> Albums { get; set; }

        public ICollection<Comment> Comments { get; set; }//которые пользователь оставил где либо
        public ICollection<Comment> UsersLikesComment { get; set; }//комменты которые он лайкнул

        public ICollection<Message> MessagesCreated { get; set; }

        public ICollection<Chat> ChatsCreated { get; set; }
        public ICollection<Chat> Chats { get; set; }

        public ICollection<Group> GroupCreared { get; set; }
        public ICollection<Group> Group { get; set; }
        public ICollection<Group> GroupAdmin { get; set; }


        //TODO
        public ICollection<ApplicationUser> Friends { get; set; }



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

            Open_data_info = false;

            Sections=new List<Section>();
            Articles = new List<Article>();
            ImagesInfo = new List<ImageInfo>();
            MemeCreated = new List<Meme>();
            WallMeme = new List<Meme>();
            UsersLikesMeme = new List<Meme>();
            UsersRipostes = new List<Meme>();
            News = new List<Meme>();
            Albums = new List<Album>();
            Comments = new List<Comment>();
            UsersLikesComment = new List<Comment>();
            MessagesCreated = new List<Message>();
            ChatsCreated = new List<Chat>();
            Chats = new List<Chat>();
            GroupCreared = new List<Group>();
            Group = new List<Group>();
            GroupAdmin = new List<Group>();
        }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
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
            modelBuilder.Entity<Meme>().HasMany(c => c.UsersLikes)
                .WithMany(s => s.UsersLikesMeme)
                .Map(t => t.MapLeftKey("MemeId")
                .MapRightKey("ApplicationUserId")
                .ToTable("ApplicationUserMemeLike"));

            modelBuilder.Entity<Meme>().HasMany(c => c.UsersRipostes)
                .WithMany(s => s.UsersRipostes)
                .Map(t => t.MapLeftKey("MemeId")
                .MapRightKey("ApplicationUserId")
                .ToTable("ApplicationUserMemeRiposte"));

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

            modelBuilder.Entity<Meme>().HasMany(c => c.UsersNews)
               .WithMany(s => s.News)
               .Map(t => t.MapLeftKey("MemeId")
               .MapRightKey("ApplicationUserId")
               .ToTable("ApplicationUserMemeNews"));



            base.OnModelCreating(modelBuilder);
        }
    }
}