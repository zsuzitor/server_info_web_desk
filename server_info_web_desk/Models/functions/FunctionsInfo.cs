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
            //article.AddRange(db.Articles.AsNoTracking().Where(x1 => x1.Section_parrentId == id_section).Select(x1=>new{ Id=x1.Id, Head=x1.Head }).ToList().Select(x1=>new Article() { Id = x1.Id, Head = x1.Head }));
            article.AddRange(db.Articles.AsNoTracking().Where(x1 => x1.Section_parrentId == id_section).ToList().Select(x1=> new Article() { Id=x1.Id, Head=x1.Head }).ToList());


            return true;
        }


        public static void Get_inside_id(int id,List<int>sections_id, List<int> articles_id)
        {
            var sec_list=db.Sections.Where(x1 => x1.Section_parrentId == id).Select(x1=>x1.Id).ToList();
            //var art_list = db.Articles.Where(x1 => x1.Section_parrentId == id).Select(x1=>x1.Id).ToList();
            //articles_id.AddRange(art_list);
            sections_id.AddRange(sec_list);
            foreach (var i in sec_list)
            {
                Get_inside_id(i,  sections_id,  articles_id);
            }
            return;
        }
        public static Section CheckAccessSection(string check_id, int? section_id, out bool success)
        {
            success = true;
            Section parrent_sec = null;
            if (check_id == null)
            {
                
                success = false;
                return null;
            }
            if (section_id == null)
            {
                
                success = false;
                return null;
            }
             parrent_sec = db.Sections.FirstOrDefault(x1 => x1.Id == section_id);
            if (parrent_sec == null)
            {
                
                success = false;
                return null;
            }
            db.Entry(parrent_sec).Reference(x1 => x1.User).Load();
            if (parrent_sec.User.Id != check_id)
            {
              
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