using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace server_info_web_desk.Models.DataBase
{
    public class AppDbInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            // создаем две роли
            var role1 = new IdentityRole { Name = "admin" };
            var role2 = new IdentityRole { Name = "user" };

            // добавляем роли в бд
            roleManager.Create(role1);
            roleManager.Create(role2);

            // создаем пользователей
           
            var admin = new ApplicationUser { Email = "admin@mail.ru", UserName = "admin@mail.ru", Name="zsuz",Surname="zsuzSUR",Birthday=DateTime.Now };
            string password = "Admin1!";
            var result = userManager.Create(admin, password);
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Albums.Add(new Models.SocialNetwork.Album()
                {
                    Name = "Main",
                    Description = "Сюда добавляются главные фотографии с вашей страницы",
                    //User = admin
                    UserId = admin.Id
                });
                db.Albums.Add(new Models.SocialNetwork.Album()
                {
                    Name = "NotMain",
                    Description = "Сюда добавляются фотографии с вашей страницы",
                    UserId = admin.Id
                });
                db.SaveChanges();
            }
            // если создание пользователя прошло успешно
            if (result.Succeeded)
            {
                // добавляем для пользователя роль
                userManager.AddToRole(admin.Id, role1.Name);
                userManager.AddToRole(admin.Id, role2.Name);
            }
            //CONSTRAINT [FK_dbo.Articles_dbo.Sections_Section_parrentId] FOREIGN KEY ([Section_parrentId]) REFERENCES [dbo].[Sections] ([Id]) ON DELETE CASCADE
            //context.Database.ExecuteSqlCommand("ALTER TABLE dbo.Sections ADD CONSTRAINT Delete_section_cascade FOREIGN KEY (SectionParrentId) REFERENCES dbo.Sections (Id) ON DELETE CASCADE");

            base.Seed(context);
        }
    }
}
