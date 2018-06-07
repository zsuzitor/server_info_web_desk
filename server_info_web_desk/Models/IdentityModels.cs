using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using server_info_web_desk.Models;
using server_info_web_desk.Models.Info;
using System.Collections.Generic;
using server_info_web_desk.Models.SocialNetwork;

namespace server_info_web_desk.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public bool Open_data_info { get; set; }







        public ICollection<Section> Sections { get; set; }
        public ICollection<Article> Articles { get; set; }
        public ICollection<Info.Image> ImagesInfo { get; set; }



        //public ICollection<SocialNetwork.Image> ImagesSocial { get; set; }

        public ICollection<Meme> MemeCreated { get; set; }
        public ICollection<Meme> WallMeme { get; set; }
        public ICollection<Meme> UsersLikesMeme { get; set; }
        public ICollection<Meme> UsersRipostes { get; set; }


        public ICollection<Album> Albums { get; set; }

        public ICollection<Comment> Comments { get; set; }
        public ICollection<Comment> UsersLikesComment { get; set; }

        public ICollection<Message> MessagesCreated { get; set; }

        public ICollection<Chat> ChatsCreated { get; set; }
        public ICollection<Chat> Chats { get; set; }

        public ICollection<Group> GroupCreared { get; set; }
        public ICollection<Group> Group { get; set; }
        public ICollection<Group> GroupAdmin { get; set; }



        public ApplicationUser() : base()
        {
            Open_data_info = false;
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
        public DbSet<Info.Image> ImagesInfo { get; set; }
        public DbSet<SocialNetwork.Image> ImagesSocial { get; set; }
        public DbSet<Meme> Memes { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Group> Groups { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}