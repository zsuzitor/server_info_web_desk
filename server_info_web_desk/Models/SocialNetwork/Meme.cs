using server_info_web_desk.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace server_info_web_desk.Models.SocialNetwork
{
    public class Meme: IDomain<int>, IDeleteDb<Meme>
    {
        [Key]
        [ForeignKey("Record")]
        [HiddenInput(DisplayValue = false)]
        [Required]
        public int Id { get; set; }
        [UIHint("MultilineText")]
        [Display(Name = "Описание")]
        public string Description { get; set; }
        public DateTime Birthday { get; set; }
        

        public string CreatorId { get; set; }
        public ApplicationUser Creator { get; set; }//создатель

        //public int? GroupId { get; set; }
        //public Group Group { get; set; }
        //public string UserId { get; set; }//чья страница на которой мем
        //public ApplicationUser User { get; set; }

        //public string MessageId { get; set; }
        //public Message Message { get; set; }

        
        //public int? RecordId { get; set; }
        //[NotMapped]
        public Record Record { get; set; }


        public ICollection<Image> Images { get; set; }
        //public ICollection<ApplicationUser> UsersLikes { get; set; }
        //public ICollection<ApplicationUser> UsersRipostes { get; set; }
        //public ICollection<ApplicationUser> UsersNews { get; set; }
        //public ICollection<Message> Messages { get; set; }//мем отправляется в сообщении
        //public ICollection<Comment> Comments { get; set; }



        public Meme()
        {
            Id = 0;
            Description = null;
                CreatorId= null;
            Creator = null;
            Record = null;
            //GroupId = null;
            //Group = null;
            //UserId = null;
            //User = null;
            //DeleteContent = false;
            Images = new List<Image>();
            //Messages= new List<Message>();
            Birthday = DateTime.Now;
        }


        public bool CanDelete()
        {
            var check_id = ApplicationUser.GetUserId();
            if (this.CreatorId == check_id)
                return true;
            return false;
        }


        //Meme напрямую удаляться не должн, только через record
        public Meme DeleteFull(out bool success, ApplicationDbContext db)
        {
            success = false;
            db.Set<Meme>().Attach(this);
            if (!db.Entry(this).Collection(x1 => x1.Images).IsLoaded)
                db.Entry(this).Collection(x1 => x1.Images).Load();
            foreach (var i in this.Images)
            {
                bool suc;
                i.DeleteFull(out suc, db);
            }
            db.SaveChanges();

            //if (!db.Entry(this).Collection(x1 => x1.Messages).IsLoaded)
            //    db.Entry(this).Collection(x1 => x1.Messages).Load();


            db.Memes.Remove(this);
            db.SaveChanges();
            success = true;
            return this;
        }

        //Meme напрямую удаляться не должн, только через record
        public Meme TryDeleteFull(out bool success, ApplicationDbContext db)
        {
            success = false;
            if (this.CanDelete())
            {
                this.DeleteFull(out success);
            }
            return this;

        }
        //Meme напрямую удаляться не должн, только через record
        public Meme TryDeleteFull(out bool success)
        {
            success = false;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                this.TryDeleteFull(out success, db);
            }
            return this;
        }

        //Meme напрямую удаляться не должн, только через record
        public Meme DeleteFull(out bool success)
        {
            success = false;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {

                this.DeleteFull(out success, db);
            }
           // success = true;
            return this;
        }



    }
}