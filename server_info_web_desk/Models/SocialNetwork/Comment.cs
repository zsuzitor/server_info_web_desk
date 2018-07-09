using server_info_web_desk.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace server_info_web_desk.Models.SocialNetwork
{
    public class Comment: IDomain<int>
    {
        [Key]
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }
        public DateTime Birthday { get; set; }

        [UIHint("MultilineText")]
        [Display(Name = "Текст")]
        public string Text { get; set; }


        public string CreatorId { get; set; }
        public ApplicationUser Creator { get; set; }//создатель

        public int? AnswerCommentId { get; set; }
        public Comment AnswerComment { get; set; }//комментарийна который отвечают этим
        public ICollection<Comment> AnswerComments { get; set; }//комментарии которые ответили на этот коммент


        public int? RecordId { get; set; }
        public Record Record { get; set; }//запись к которой написали коммент

        //public int? ImageId { get; set; }
        //public Image Image { get; set; }//картинка к которой написали коммент

        public ICollection<Image> Images { get; set; }//список картинок в комменте
        public ICollection<ApplicationUser> UsersLikes { get; set; }//список лайкнувших


        public Comment()
        {
            Id = 0;
            Birthday = DateTime.Now;
            CreatorId = null;
            Creator = null;
            AnswerCommentId = null;
            AnswerComment = null;
            Record = null;
            Record = null;
            //ImageId = null;
            //Image = null;
            Images = new List<Image>();
            UsersLikes = new List<ApplicationUser>();


        }
        public static Comment GetComment(int? id)
        {
           
            
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                if (id == null)
                    return null;
                var comm = db.Comments.FirstOrDefault(x1 => x1.Id == id);

                return comm;
            }
        }

        public bool? LikeAction(string id_user)
        {
            if (id_user == null)
                return null;
            bool red_heart = false;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Set<Comment>().Attach(this);
                if (!db.Entry(this).Collection(x1 => x1.UsersLikes).IsLoaded)
                    db.Entry(this).Collection(x1 => x1.UsersLikes).Load();
                var like = this.UsersLikes.FirstOrDefault(x1 => x1.Id == id_user);
                
                if (like == null)
                {
                    this.UsersLikes.Add(db.Users.First(x1 => x1.Id == id_user));
                    red_heart = true;
                }

                else
                {
                    this.UsersLikes.Remove(db.Users.First(x1 => x1.Id == id_user));
                    red_heart = false;
                }

                db.SaveChanges();
                
            }
            return red_heart;
            

            //var pers = ApplicationUser.GetUser(ApplicationUser.GetUserId());
            //using (ApplicationDbContext db = new ApplicationDbContext())
            //{
            //    db.Set<Comment>().Attach(this);
            //    db.Set<ApplicationUser>().Attach(pers);
            //    this.UsersLikes.Add(pers);
            //    db.SaveChanges();

            //}
        }


        public void LoadForView()
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Set<Comment>().Attach(this);
                if (!db.Entry(this).Reference(x1 => x1.Creator).IsLoaded)
                    db.Entry(this).Reference(x1 => x1.Creator).Load();
                if (!db.Entry(this).Collection(x1 => x1.UsersLikes).IsLoaded)
                    db.Entry(this).Collection(x1 => x1.UsersLikes).Load();

            }
            this.Creator.LoadDataForShort();
        }
            }
}