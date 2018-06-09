using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

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
    }
}