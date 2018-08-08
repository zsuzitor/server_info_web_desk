using server_info_web_desk.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static server_info_web_desk.Models.DataBase.DataBase;
using static server_info_web_desk.Models.functions.FunctionsProject;


namespace server_info_web_desk.Models.SocialNetwork
{

    //при репосте создается новая запись и новая ссылается на ту которую репостили
    public class Record: IDomain<int>, IHaveNoCascadeDelContend, IDeleteDb<Record>
    {
        [Key]
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        public bool DeleteContent { get; set; }//для   RecordRiposters---RecordRiposte
        //public int? MemeId { get; set; }
        //[NotMapped]
        public Meme Meme { get; set; }//мем который в запись

        public int? ImageId { get; set; }
        public Image Image { get; set; }//запись==картинка 
        public int? AlbumId { get; set; }
        public Album Album { get; set; }// запись==картинка и она в альбоме//для отображение картинок в альбоме тк там тоже есть лайки и тдтд
        public string Description { get; set; }

        public int? GroupId { get; set; }
        public Group Group { get; set; }//группа в которой запись висит первоначально
        public string UserId { get; set; }//чья страница на которой мем
        public ApplicationUser User { get; set; }
        public string CreatorId { get; set; }
        [NotMapped]
        public ApplicationUser Creator_NM { get; set; }


        public int? RecordRiposteId { get; set; }
        public Record RecordRiposte { get; set; }//если запись репостят то ссылка на родителя
        public ICollection<Record> RecordRiposters { get; set; }


        public ICollection<Message> Messages { get; set; }//запись отправляется в сообщении
        public ICollection<ApplicationUser> UsersLikes { get; set; }
        public ICollection<ApplicationUser> UsersRipostes { get; set; }
        public ICollection<ApplicationUser> UsersNews { get; set; }
        //public ICollection<Group> GroupWall { get; set; }
        //public ICollection<ApplicationUser> UserWall { get; set; }

        public ICollection<Comment> Comments { get; set; }


        public Record()
        {
            Id = 0;
            // MemeId = null;
            DeleteContent = false;
            Meme = null;
            ImageId = null;
            Image = null;
            //AlbumId = null;
            //Album = null;
            GroupId = null;
            Group = null;
            UserId = null;
            User = null;
            Creator_NM = null;
            RecordRiposteId = null;
            RecordRiposte = null;
            Description = null;
            CreatorId = null;
            Messages = new List<Message>();
            RecordRiposters = new List<Record>();
            UsersLikes = new List<ApplicationUser>();
            UsersRipostes = new List<ApplicationUser>();
            UsersNews = new List<ApplicationUser>();
            //GroupWall = new List<Group>();

            Comments = new List<Comment>();
        }


       


        //возвращает true если лайк поставлен(красное сердце) и false если снят(серое)
        public bool? LikeAction(string id_user)
        {
            bool red_heart = false;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                if (id_user == null)
                    return null;
                db.Set<Record>().Attach(this);
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
        }

        public static Record AddRecordMem(string creator_id , string user_id, int? group_id,List<byte[]>images,string text)
        {
            

            Record record = null;
            if (group_id != null&& group_id > 0)
            
                record = new Record() { CreatorId = creator_id, GroupId = group_id };
                
            else
                record=new Record() {CreatorId= creator_id, UserId = user_id };
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Record.Add(record);
                db.SaveChanges();
                //if (group_id != null && group_id > 0)
                //    record.GroupWall.Add(Group.GetGroup(group_id));

                Meme mem = new Meme() { Id = record.Id, Description = text, CreatorId = creator_id , GroupCreatorId = group_id };
                db.Memes.Add(mem);
                db.SaveChanges();
                var list_img = images.Select(x1 => new Image() { MemeId = mem.Id, Data = x1, UserId = creator_id });
                db.ImagesSocial.AddRange(list_img);
                db.SaveChanges();
            }
            return record;
        }
        public static Record AddRecordImage(Album album,ApplicationUser user, Group group, List<byte[]> images, string text)
        {
            //var album = user.GetAlbums(album_id).FirstOrDefault();
            if (album == null)
                return null;
            if(user==null)
                return null;
            if(images==null|| images.Count<1)
                return null;
            var img = new Image() { Data = images?.ElementAt(0), UserId = user.Id };
            Record record = null;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.ImagesSocial.Add(img);
                db.SaveChanges();
               
                if (group == null)
                    record = new Record() { CreatorId = user.Id, AlbumId = album.Id, UserId = user.Id, Description = text };
                else
                    record = new Record() { CreatorId = user.Id, AlbumId = album.Id, GroupId = group.Id, Description = text };
                db.Record.Add(record);
                db.SaveChanges();
                img.RecordId = record.Id;
                record.ImageId = img.Id;

                db.SaveChanges();
            }
            return record;
        }


        public List<Comment> GetComments(int start=0, int? count=null)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Set<Record>().Attach(this);
                if (!db.Entry(this).Collection(x1 => x1.Comments).IsLoaded)
                    db.Entry(this).Collection(x1 => x1.Comments).Load();
            }
            var lst = (List<Comment>)GetPartialList<Comment>(this.Comments, start, count, true);
            foreach(var i in lst)
            {
                i.LoadForView();
            }
            return lst;
        }

        public Comment AddComment(string text,int? AnswerCommentId)
        {
            var us_id = ApplicationUser.GetUserId();
            var comm = new Comment() {Text= text, CreatorId= us_id,AnswerCommentId= AnswerCommentId, RecordId=Id };
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Comments.Add(comm);
                db.SaveChanges();

            }


                return comm;
        }

        public static Record GetRecord(int? id)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                if (id == null)
                    return null;
                var record = db.Record.FirstOrDefault(x1 => x1.Id == id);

                return record;
            }
        }

        public  void RecordLoadForView()
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Set<Record>().Attach(this);
                if (this.ImageId != null && !db.Entry(this).Reference(x1 => x1.Image).IsLoaded)
                    db.Entry(this).Reference(x1 => x1.Image).Load();

                ////TODO тут может быть ошибка тк запись == картинка
                //if (!db.Entry(this.Meme).Collection(x1 => x1.Images).IsLoaded)
                //    db.Entry(this.Meme).Collection(x1 => x1.Images).Load();

                //можно загружать усеченную версию
                if (!db.Entry(this).Collection(x1 => x1.UsersLikes).IsLoaded)
                    db.Entry(this).Collection(x1 => x1.UsersLikes).Load();

                if (!db.Entry(this).Collection(x1 => x1.Comments).IsLoaded)
                    db.Entry(this).Collection(x1 => x1.Comments).Load();

                //колличество нужно +5 записей последних
                //if (!db.Entry(i).Collection(x1 => x1.Comments).IsLoaded)
                //    db.Entry(i).Collection(x1 => x1.Comments).Load();
                //колличество нужно +5 записей последних
                if (!db.Entry(this).Collection(x1 => x1.RecordRiposters).IsLoaded)
                    db.Entry(this).Collection(x1 => x1.RecordRiposters).Load();

                if (!db.Entry(this).Reference(x1 => x1.Meme).IsLoaded)
                    db.Entry(this).Reference(x1 => x1.Meme).Load();
                if (this.Meme != null)
                    if (!db.Entry(this.Meme).Collection(x1 => x1.Images).IsLoaded)
                        db.Entry(this.Meme).Collection(x1 => x1.Images).Load();
                if (this.ImageId != null)
                    if (!db.Entry(this).Reference(x1 => x1.Image).IsLoaded)
                        db.Entry(this).Reference(x1 => x1.Image).Load();

                if (this.GroupId != null)
                {
                    if (!db.Entry(this).Reference(x1 => x1.Group).IsLoaded)
                        db.Entry(this).Reference(x1 => x1.Group).Load();
                   
                }

                //if (this.UserId != null)
                //{
                //    if (!db.Entry(this).Reference(x1 => x1.User).IsLoaded)
                //        db.Entry(this).Reference(x1 => x1.User).Load();

                //}
                //пока что так мб не загружать если например по GroupId уже будет загрузка
                if (this.CreatorId != null)
                {
                    this.Creator_NM = ApplicationUser.GetUser(this.CreatorId);
                    this.Creator_NM.LoadDataForShort();

                }

            }
            if (this.GroupId != null)
            {
                
                this.Group.LoadDataForShort();
            }

            if (this.UserId != null)
            {
                
                this.User.LoadDataForShort();
            }
            //i.Meme_NM.Record_NM=
        }


        public bool CanDelete()
        {
            var check_id = ApplicationUser.GetUserId();
            if (this.CreatorId == check_id)
                return true;
            var gr = Group.GetGroup(this.GroupId);

            if (gr.HaveAccessAdminGroup(check_id))
                return true;
            return false;
        }


        public Record TryDeleteFull(out bool success, ApplicationDbContext db)
        {
            success = false;
            if (this.CanDelete())
            {
                this.DeleteFull(out success);
            }
            return this;

        }
        public Record TryDeleteFull(out bool success)
        {
            success = false;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                this.TryDeleteFull(out success, db);
            }
            return this;
        }


        public Record DeleteFull(out bool success, ApplicationDbContext db)
        {
            success = false;
            db.Set<Record>().Attach(this);


            if (this.ImageId != null || this.AlbumId != null)
            {
                    if (!db.Entry(this).Reference(x1 => x1.Image).IsLoaded)
                        db.Entry(this).Reference(x1 => x1.Image).Load();
                    bool sc;
                    this.Image.DeleteFull(out sc, db);
                    //var img = SocialNetwork.Image.GetImage((int)this.ImageId);
                    //var img = db.ImagesSocial.FirstOrDefault(x1 => x1.Id == this.ImageId);
                    //db.ImagesSocial.Remove(img);
                    this.ImageId = null;
                    this.AlbumId = null;
                    db.SaveChanges();
                
            }

            
            //удаление полностью

            if (!db.Entry(this).Collection(x1 => x1.RecordRiposters).IsLoaded)
                db.Entry(this).Collection(x1 => x1.RecordRiposters).Load();
            foreach (var i in this.RecordRiposters)
            {
                i.DeleteContent = true;
                i.RecordRiposteId = null;
            }
            this.RecordRiposters.Clear();
            db.SaveChanges();

            if (!db.Entry(this).Reference(x1 => x1.Meme).IsLoaded)
                db.Entry(this).Reference(x1 => x1.Meme).Load();
            if (this.Meme != null)
            {
                //удаляем картинки в меме
                //if (!db.Entry(this.Meme).Collection(x1 => x1.Images).IsLoaded)
                //    db.Entry(this.Meme).Collection(x1 => x1.Images).Load();
                bool sc = false;
                this.Meme.DeleteFull(out sc, db);

                //db.ImagesSocial.RemoveRange(this.Meme.Images);

            }
            //удаление лайков
            if (!db.Entry(this).Collection(x1 => x1.UsersLikes).IsLoaded)
                db.Entry(this).Collection(x1 => x1.UsersLikes).Load();

            if (!db.Entry(this).Collection(x1 => x1.UsersNews).IsLoaded)
                db.Entry(this).Collection(x1 => x1.UsersNews).Load();

            if (!db.Entry(this).Collection(x1 => x1.UsersRipostes).IsLoaded)
                db.Entry(this).Collection(x1 => x1.UsersRipostes).Load();

            if (!db.Entry(this).Collection(x1 => x1.Comments).IsLoaded)
                db.Entry(this).Collection(x1 => x1.Comments).Load();
            foreach (var i in this.Comments.ToList())//
            {
                bool suc;
                i.DeleteFull(out suc,db);
            }
            db.SaveChanges();

            if (!db.Entry(this).Collection(x1 => x1.Messages).IsLoaded)
                db.Entry(this).Collection(x1 => x1.Messages).Load();
            foreach (var i in this.Messages.ToList())
            {
                i.DeleteContent = true;
                i.RecordId = null;
            }
            db.SaveChanges();
            //если запись картинка то удалять только со стены

            db.Record.Remove(this);
            db.SaveChanges();

            success = true;
            return this;
        }


        //удаление со стены(если является фотографией) из новостей и тд и полное удаление если обычная запись(не фото)
        //с валидацией
        public  Record DeleteWall(out bool success)
        {
            //Record res = null;
            var check_id = ApplicationUser.GetUserId();
            success = false;
            this.TryDeleteFull(out success);
            if (!success)
                return this;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Set<Record>().Attach(this);
                //удаление только со стены
                if (this.ImageId != null || this.AlbumId != null)
                {
                    this.UserId = null;
                    this.GroupId = null;
                    db.SaveChanges();
                    success = true;
                    return this;
                }
                this.DeleteFull(out success, db);
            }
            //success = true;
            return this;

            }
        public Record DeleteFull(out bool success)
        {
           
            success = false;
           // Record res = null;
            
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                //res=
                this.DeleteFull(out success, db);

            }
            return this; 
        }
            //public Record DeleteInside(int id)
            //{

            //    //TODO загружать сначала
            //    this.Meme.DeleteInside();
            //    this.Image.DeleteInside();
            //    RecordRiposters
            //        UsersLikes
            //        UsersNews
            //        GroupWall
            //        Comments
            //}
        }
    }