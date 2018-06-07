using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace server_info_web_desk.Models.SocialNetwork
{
    public class Image : server_info_web_desk.Models.Interfaces.AImage
    {
        //public bool MainImages { get; set; }


        //public string GroupId { get; set; }
        //public Group Group { get; set; }

        public int? MemeId { get; set; }
        public Meme Meme { get; set; }//картинка в меме

        public int? CommentId { get; set; }
        public Comment Comment { get; set; }//если картинка в комментарии



        public ICollection<Comment> Comments { get; set; }//комменты которые "под" это картинкой

        public ICollection<Message> Messages { get; set; }

        public Image() : base()
        {
            

            //Base_url_img = "/Content/images/camera_200.png";
        }
        public Image(Image a, bool with_out_reference = false)
        {
            Id = a.Id;
            Data = a.Data;
           
            UserId = a.UserId;

            if (!with_out_reference)
            {
                
            }
            else
            {
                
            }
        }
    }
}