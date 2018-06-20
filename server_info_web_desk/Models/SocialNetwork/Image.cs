using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using static server_info_web_desk.Models.DataBase.DataBase;


namespace server_info_web_desk.Models.SocialNetwork
{
    [Table("ImageSocial")]
    public class Image : server_info_web_desk.Models.Interfaces.AImage
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

        public int? RecordId { get; set; }//если картинка самостоятельная и должна быть обернута в запись
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

            //ищет в бд по id  если id нет то создает record только для представления(бд не трогает)
        public void GetRecordForShow()
        {
            
            if (this.RecordId != null)
            {
                this.Record_NM = db.Record.FirstOrDefault(x1 => x1.Id == this.RecordId);
                this.Record_NM.Image = this;
                if (!db.Entry(this.Record_NM).Collection(x1 => x1.UsersLikes).IsLoaded)
                    db.Entry(this.Record_NM).Collection(x1 => x1.UsersLikes).Load();
            }
            else
            {
                this.Record_NM = new Record();
                this.Record_NM.Image = this;
                //if (!db.Entry(this.Record_NM).Collection(x1 => x1.UsersLikes).IsLoaded)
                //    db.Entry(this.Record_NM).Collection(x1 => x1.UsersLikes).Load();
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