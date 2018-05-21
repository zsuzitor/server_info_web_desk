using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using static server_info_web_desk.Models.DataBase.DataBase;
using server_info_web_desk.Models.Info;

namespace server_info_web_desk.Models.functions
{
    public static class FunctionsInfo
    {
        //массивы только дополнять
        public static bool GetSectionInside(int id_section,List<Section> section, List<Article> article)
        {
            //ListData res = null;
            section.AddRange( db.Sections.AsNoTracking().Where(x1 => x1.Section_parrentId == id_section));
            article.AddRange(db.Articles.AsNoTracking().Where(x1 => x1.Section_parrentId == id_section).Select(x1=>new Article() { Id=x1.Id, Head=x1.Head }));


            return true;
        }

        public static Section CheckAccessSection(string check_id, int? parrent_sec_id, out bool success)
        {
            success = true;
            Section parrent_sec = null;
            if (check_id == null)
            {
                //TODO ошибку обработать
                success = false;
                return null;
            }
            if (parrent_sec_id == null)
            {
                //TODO ошибку обработать
                success = false;
                return null;
            }
             parrent_sec = db.Sections.FirstOrDefault(x1 => x1.Id == parrent_sec_id);
            if (parrent_sec == null)
            {
                //TODO ошибку обработать
                success = false;
                return null;
            }
            db.Entry(parrent_sec).Reference(x1 => x1.User).Load();
            if (parrent_sec.User.Id != check_id)
            {
                //TODO ошибку обработать
                success = false;
                return null;
            }
            //mm:
            //if (!success)
            //    return null;
            
            return parrent_sec;
        }

    }
}