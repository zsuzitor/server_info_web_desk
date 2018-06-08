using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using static server_info_web_desk.Models.DataBase.DataBase;
using server_info_web_desk.Models.Info;
using System.Text.RegularExpressions;

namespace server_info_web_desk.Models.functions
{
    public static class FunctionsInfo
    {
        //массивы только дополнять
        //только внешний уровень возвращается
        public static bool GetSectionInside(int id_section,List<Section> section, List<Article> article)
        {
            //ListData res = null;
            if (section == null)
                section = new List<Section>();
            section.AddRange( db.Sections.AsNoTracking().Where(x1 => x1.SectionParrentId == id_section));
            //article.AddRange(db.Articles.AsNoTracking().Where(x1 => x1.Section_parrentId == id_section).Select(x1=>new{ Id=x1.Id, Head=x1.Head }).ToList().Select(x1=>new Article() { Id = x1.Id, Head = x1.Head }));
            article.AddRange(db.Articles.AsNoTracking().Where(x1 => x1.SectionParrentId == id_section).ToList().Select(x1=> new Article() { Id=x1.Id, Head=x1.Head }).ToList());


            return true;
        }

        //ВСЕ вложенные
        public static void Get_inside_id(int id,List<int>sections_id, List<int> articles_id)
        {
            var sec_list=db.Sections.Where(x1 => x1.SectionParrentId == id).Select(x1=>x1.Id).ToList();
            if (articles_id != null)
            {
                var art_list = db.Articles.Where(x1 => x1.SectionParrentId == id).Select(x1=>x1.Id).ToList();
                articles_id.AddRange(art_list);
            }
            if (sections_id == null)
                sections_id = new List<int>();
            sections_id.AddRange(sec_list);
            foreach (var i in sec_list)
            {
                Get_inside_id(i,  sections_id,  articles_id);
            }
            return;
        }
        //Проверка на то принадлежит ли секция пользователю(пользователь может ее редактировать)
        public static Section CheckAccessSection(string check_id, int? section_id, out bool success)
        {
            success = false;
            Section sec = null;
            if (check_id == null)
            {
                return null;
            }
            if (section_id == null)
            {
                return null;
            }
            sec = db.Sections.FirstOrDefault(x1 => x1.Id == section_id);
            if (sec == null)
            {
                return null;
            }
            //db.Entry(parrent_sec).Reference(x1 => x1.User).Load();
            if (sec.UserId != check_id)
            {
                return null;
            }
            success = true;
            return sec;
        }


        public static double GetMarkArticle(Article art,params string[] str)
        {
            double res = 1;
            List<string> mass_tag = new List<string>();
            List<string> mass_not_tag = new List<string>();

            //for(int i=0;i<str.Count();++i)
            foreach (var i in str)
            {

                if (i[0] == '#')
                {
                    mass_tag.Add(i);
                    mass_not_tag.Add(i.Substring(1));
                }
                else
                {
                    mass_not_tag.Add(i);
                }


            }

            for (var num_count = 1; num_count <= mass_not_tag.Count;++num_count)
                for (var i = 0; i+ num_count <= mass_not_tag.Count; ++i)
                {
                    string tmp = "";
                    //int count_word = 1;
                    for (var i2 = i; i2 < num_count + i; tmp += mass_not_tag[i2++]+" ") ;

                    string pattern = tmp.Trim(' ');
                    int count = new Regex(pattern, RegexOptions.IgnoreCase).Matches(art.Head).Count; //
                    res += count * num_count;

                    count = new Regex(pattern).Matches(art.Body).Count;
                    res += count * num_count * 0.7;
                }

            for(var i = 0; i < mass_tag.Count; ++i)
            {
                int count = new Regex(mass_tag[i]).Matches(art.Head).Count;
                res += count * 5;
                 count = new Regex(mass_tag[i]).Matches(art.Body).Count;
                res += count * 4;
            }
            



            return res;
        }
        }
}