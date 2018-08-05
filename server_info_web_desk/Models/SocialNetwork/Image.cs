using server_info_web_desk.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using static server_info_web_desk.Models.DataBase.DataBase;


namespace server_info_web_desk.Models.SocialNetwork
{
    [Table("ImageSocial")]
    public class Image : server_info_web_desk.Models.Interfaces.AImage, IDomain<int>, IDeleteDb<Image>
    {
        //public bool MainImages { get; set; }


        //public string GroupId { get; set; }
        //public Group Group { get; set; }

        public int? MemeId { get; set; }
        public Meme Meme { get; set; }//картинка в меме

        public int? CommentId { get; set; }
        public Comment Comment { get; set; }//если картинка в комментарии

        //public int? AlbumId { get; set; }
        //public Album Album { get; set; }//картинка в альбоме

        //public ICollection<Comment> Comments { get; set; }//комменты которые "под" это картинкой

        public int? MessageId { get; set; }
        public Message Message { get; set; }//если картинка в сообщении


        //если картинка самостоятельная и должна быть обернута в запись, при удалении это не трогать, тк удаление начинается 
        //с записи=> это дургой уровень
        public int? RecordId { get; set; }
        
        [NotMapped]
        public Record Record_NM { get; set; }


        public Image() : base()
        {
            MemeId = null;
            Meme = null;
            CommentId = null;
            Comment = null;
            //Comments = new List<Comment>();
            MessageId = null;
            Message = null;
            //Album = null;
            //AlbumId = null;
            //Base_url_img = "/Content/images/camera_200.png";
        }
        //public Image(Image a, bool with_out_reference = false)
        //{
        //    Id = a.Id;
        //    Data = a.Data;

        //    UserId = a.UserId;

        //    if (!with_out_reference)
        //    {

        //    }
        //    else
        //    {

        //    }
        //}

        public static Image GetImage(int id)
        {
            Image img = null;
            using (ApplicationDbContext db = new ApplicationDbContext())
                img = db.ImagesSocial.FirstOrDefault(x1 => x1.Id == id);
            return img; 
        }

        public Image DeleteFull(out bool success)
        {
            success = false;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.ImagesSocial.Remove(this);
                db.SaveChanges();

            }
            success = true;
                return this;
        }


        //ищет в бд по id  если id нет то создает record только для представления(бд не трогает)
        public void GetRecordForShow()
        {
            
            if (this.RecordId != null)
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    db.Set<Image>().Attach(this);
                    this.Record_NM = db.Record.FirstOrDefault(x1 => x1.Id == this.RecordId);
                    this.Record_NM.Image = this;
                    
                    if(this.Record_NM.UserId!=null)
                    if (!db.Entry(this.Record_NM).Reference(x1 => x1.User).IsLoaded)
                        db.Entry(this.Record_NM).Reference(x1 => x1.User).Load();
                    if (this.Record_NM.GroupId != null)
                        if (!db.Entry(this.Record_NM).Reference(x1 => x1.Group).IsLoaded)
                            db.Entry(this.Record_NM).Reference(x1 => x1.Group).Load();
                    if (!db.Entry(this.Record_NM).Collection(x1 => x1.UsersLikes).IsLoaded)
                        db.Entry(this.Record_NM).Collection(x1 => x1.UsersLikes).Load();
                }
                if (this.Record_NM.UserId != null)
                    this.Record_NM.User.LoadDataForShort();
                if (this.Record_NM.GroupId != null)
                    this.Record_NM.Group.LoadDataForShort();
            }
            else
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    db.Set<Image>().Attach(this);
                    if (!db.Entry(this).Reference(x1 => x1.User).IsLoaded)
                        db.Entry(this).Reference(x1 => x1.User).Load();

                    this.Record_NM = new Record();
                    this.Record_NM.Image = this;
                    //if (!db.Entry(this.Record_NM).Collection(x1 => x1.UsersLikes).IsLoaded)
                    //    db.Entry(this.Record_NM).Collection(x1 => x1.UsersLikes).Load();
                }
                this.User.LoadDataForShort();
            }

        }




        }

    public class ImageShort
    {
        public int Id { get; set; }
        public byte[] Data { get; set; }
        public int? PreviewId { get; set; }//id image
        public int? NextId { get; set; }//id image
        public ImageShort()
        {
            Id = 0;
            Data = null;
            PreviewId = null;
            NextId = null;
        }
    }
}