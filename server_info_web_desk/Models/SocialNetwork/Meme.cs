using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace server_info_web_desk.Models.SocialNetwork
{
    public class Meme
    {
        [Key]
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }
        [UIHint("MultilineText")]
        [Display(Name = "Описание")]
        public string Description { get; set; }
        public DateTime Birthday { get; set; }

        
        public string СreatorId { get; set; }
        public ApplicationUser Сreator { get; set; }//создатель

        //public int? GroupId { get; set; }
        //public Group Group { get; set; }
        //public string UserId { get; set; }//чья страница на которой мем
        //public ApplicationUser User { get; set; }

        //public string MessageId { get; set; }
        //public Message Message { get; set; }

        
        public int? RecordId { get; set; }
        [NotMapped]
        public Record Record_NM { get; set; }


        public ICollection<Image> Images { get; set; }
        //public ICollection<ApplicationUser> UsersLikes { get; set; }
        //public ICollection<ApplicationUser> UsersRipostes { get; set; }
        //public ICollection<ApplicationUser> UsersNews { get; set; }
        public ICollection<Message> Messages { get; set; }//мем отправляется в сообщении
        //public ICollection<Comment> Comments { get; set; }



        public Meme()
        {
            Id = 0;
            Description = null;
                СreatorId= null;
            Сreator = null;
            Record_NM = null;
            //GroupId = null;
            //Group = null;
            //UserId = null;
            //User = null;
            Images = new List<Image>();
            Messages= new List<Message>();
            Birthday = DateTime.Now;
        }
    }
}