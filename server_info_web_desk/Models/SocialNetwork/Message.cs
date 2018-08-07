﻿using server_info_web_desk.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using server_info_web_desk.Models;

namespace server_info_web_desk.Models.SocialNetwork
{
    public class Message: IDomain<int>, IHaveNoCascadeDelContend, IDeleteDb<Message>
    {
        [Key]
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }
        public DateTime Birthday { get; set; }
        public bool DeleteContent { get; set; }//для  Meme
        [UIHint("MultilineText")]
        [Display(Name = "Текст")]
        public string Text { get; set; }


        public string CreatorId { get; set; }
        public ApplicationUser Creator { get; set; }//создатель

        public int? RecordId { get; set; }
        public Record Record { get; set; }

        public int? ChatId { get; set; }
        public Chat Chat { get; set; }

        public ICollection<Image> Images { get; set; }
        //public ICollection<ApplicationUser> Users { get; set; }

        public ICollection<ApplicationUser> UserNeedRead { get; set; }

        //public static int? GetCountNewMessages(ApplicationUser user)
        //{
        //    if (user == null)
        //        return null;
                
        //        using (ApplicationDbContext db = new ApplicationDbContext())
        //        {
        //            db.Set<ApplicationUser>().Attach(user);
        //            if (!db.Entry(user).Collection(x1 => x1.MessageNeedRead).IsLoaded)
        //                db.Entry(user).Collection(x1 => x1.MessageNeedRead).Load();
        //        }

        //        return user.MessageNeedRead.Count;
            
        //}



        public Message()
        {
            Id = 0;
            Birthday = DateTime.Now;
            Text = null;
            CreatorId = null;
            Creator = null;
            RecordId = null;
            Record = null;
            Chat = null;
            ChatId = null;
            Images = new List<Image>();
            UserNeedRead = new List<ApplicationUser>();

        }


        public bool CanDelete()
        {
            var check_id = ApplicationUser.GetUserId();
            if (this.CreatorId == check_id)
                return true;
            return false;
        }

        public Message DeleteFull(out bool success, ApplicationDbContext db)
        {
            success = false;
            db.Set<Message>().Attach(this);
            if (!db.Entry(this).Collection(x1 => x1.UserNeedRead).IsLoaded)
                db.Entry(this).Collection(x1 => x1.UserNeedRead).Load();
            if (!db.Entry(this).Collection(x1 => x1.Images).IsLoaded)
                db.Entry(this).Collection(x1 => x1.Images).Load();
            foreach (var i in this.Images)
            {
                bool suc;
                i.DeleteFull(out suc, db);
            }
            db.SaveChanges();

            db.Messages.Remove(this);
            db.SaveChanges();
            success = true;
            return this;
        }


        public Message TryDeleteFull(out bool success, ApplicationDbContext db)
        {
            success = false;
            if (this.CanDelete())
            {
                this.DeleteFull(out success);
            }
            return this;

        }
        public Message TryDeleteFull(out bool success)
        {
            success = false;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                this.TryDeleteFull(out success, db);
            }
            return this;
        }


        public Message DeleteFull(out bool success)
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