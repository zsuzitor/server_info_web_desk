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


namespace server_info_web_desk.Models.SocialNetwork
{

    //при репосте создается новая запись и новая ссылается на ту которую репостили
    public class Record: IDomain<int>
    {
        [Key]
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        
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



        public ICollection<ApplicationUser> UsersLikes { get; set; }
        public ICollection<ApplicationUser> UsersRipostes { get; set; }
        public ICollection<ApplicationUser> UsersNews { get; set; }
        public ICollection<Group> GroupWall { get; set; }

        public ICollection<Comment> Comments { get; set; }


        public Record()
        {
            Id = 0;
               // MemeId = null;
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
            RecordRiposters = new List<Record>();
            UsersLikes = new List<ApplicationUser>();
            UsersRipostes = new List<ApplicationUser>();
            UsersNews = new List<ApplicationUser>();
            GroupWall = new List<Group>();
            Comments = new List<Comment>();
        }

        //возвращает true если лайк поставлен(красное сердце) и false если снят(серое)
        public bool? LikeAction(string id_user)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                if (id_user == null)
                    return null;
                db.Set<Record>().Attach(this);
                if (!db.Entry(this).Collection(x1 => x1.UsersLikes).IsLoaded)
                    db.Entry(this).Collection(x1 => x1.UsersLikes).Load();
                var like = this.UsersLikes.FirstOrDefault(x1 => x1.Id == id_user);
                bool red_heart = false;
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

                return red_heart;
            }
        }

        public static Record AddRecordMem(string creator_id , string user_id, int? group_id,List<byte[]>images,string text)
        {
            

            Record record = null;
            if (group_id != null&& group_id > 0)
            
                record = new Record() { GroupId = group_id };
                
            else
                record=new Record() {CreatorId= creator_id, UserId = user_id };
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Record.Add(record);
                db.SaveChanges();
                //if (group_id != null && group_id > 0)
                //    record.GroupWall.Add(Group.GetGroup(group_id));

                Meme mem = new Meme() { Id = record.Id, Description = text, CreatorId = creator_id };
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
                    record = new Record() { AlbumId = album.Id, UserId = user.Id, Description = text };
                else
                    record = new Record() { AlbumId = album.Id, GroupId = group.Id, Description = text };
                db.Record.Add(record);
                db.SaveChanges();
                img.RecordId = record.Id;
                record.ImageId = img.Id;

                db.SaveChanges();
            }
            return record;
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

        //удаление со стены из новостей и тд
        public  Record DeleteWall(int id)
        {
            //Record res = null;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                //res = db.Record.FirstOrDefault(x1=>x1.Id==id);
                db.Set<Record>().Attach(this);
                if (!db.Entry(this).Collection(x1 => x1.GroupWall).IsLoaded)
                    db.Entry(this).Collection(x1 => x1.GroupWall).Load();
                this.GroupWall.Clear();
                //TODO уже удалять у пользователя со стены

                db.SaveChanges();
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