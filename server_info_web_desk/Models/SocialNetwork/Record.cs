using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace server_info_web_desk.Models.SocialNetwork
{

    //при репосте создается новая запись и новая ссылается на ту которую репостили
    public class Record
    {
        [Key]
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        
        public int? MemeId { get; set; }
        [NotMapped]
        public Meme Meme_NM { get; set; }//мем который в запись

        public int? ImageId { get; set; }
        public Image Image { get; set; }//запись==картинка 
        public int? AlbumId { get; set; }
        public Album Album { get; set; }// запись==картинка и она в альбоме//для отображение картинок в альбоме тк там тоже есть лайки и тдтд


        public int? GroupId { get; set; }
        public Group Group { get; set; }//группа в которой запись висит первоначально
        public string UserId { get; set; }//чья страница на которой мем
        public ApplicationUser User { get; set; }

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
            Meme_NM = null;
            ImageId = null;
            Image = null;
            //AlbumId = null;
            //Album = null;
            GroupId = null;
            Group = null;
            UserId = null;
            User = null;
            RecordRiposteId = null;
            RecordRiposte = null;
            RecordRiposters = new List<Record>();
            UsersLikes = new List<ApplicationUser>();
            UsersRipostes = new List<ApplicationUser>();
            UsersNews = new List<ApplicationUser>();
            GroupWall = new List<Group>();
            Comments = new List<Comment>();
        }
    }
}