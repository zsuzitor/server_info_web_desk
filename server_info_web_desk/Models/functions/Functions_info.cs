using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using static server_info_web_desk.Models.DataBase.DataBase;
using server_info_web_desk.Models.Info;

namespace server_info_web_desk.Models.functions
{
    public static class Functions_info
    {
        public static Section Check_access_section(string check_id, int? parrent_sec_id, out bool success)
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