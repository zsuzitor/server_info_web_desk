using server_info_web_desk.Models.SocialNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace server_info_web_desk.Models.Interfaces
{
    public interface IDomain<T>
    {
        T Id { get; set; }
    }
    public interface IPageR
    {
        bool LoadDataForShort();
    }
    public interface IHaveAlbum
    {
        List<Album> Albums { get; set; }
        List<Album> GetAlbums(int? id, int start, int? count, bool get_start, bool end);

    }
    public interface IHaveUsersList
    {

    }


    //если сущность может содержать 1 поле которое может быть удалено 
    //например запись которую репостнули(создали таким образом новую)
    //и затем 1 запись удаляют и 2 помечается этим флагом=>можно отобразить сообщение об удалении контента и сохранить запись

        //ТЕ сущность в которой установлен этот флаг ссылается на удаленный элемент и нужно отобразать "контент удален у удаленного элемента"
    public interface IHaveNoCascadeDelContend
    {
        bool DeleteContent { get; set; }
    }

    //реализация удаления из бд
    public interface IDeleteDb<T>
    {

        T DeleteFull(out bool success);
    }

    public abstract class AImage
    {
        //int Id { get; set; }
        [HiddenInput(DisplayValue = false)]
         public int Id { get; set; }
        public DateTime Birthday { get; set; }
        public byte[] Data { get; set; }

        public string UserId { get; set; }//тот кто загрузил??
        public ApplicationUser User { get; set; }

        public AImage()
        {
            Id = 0;
            Data = null;
            Birthday = DateTime.Now;
            UserId = null;
            User = null;
        }
    }

    public interface Iuser
    {

    }


    public class Interfaces
    {
    }
}