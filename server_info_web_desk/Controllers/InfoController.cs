using System;
using System.Collections.Generic;

using System.Linq;
//using MoreLinq;

using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

using server_info_web_desk.Models;
using server_info_web_desk.Models.Info;
using server_info_web_desk.Models.ViewModel;

using System.IO;
using Newtonsoft.Json;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;

//
using static server_info_web_desk.Models.functions.FunctionsProject;
using static server_info_web_desk.Models.functions.FunctionsInfo;
using static server_info_web_desk.Models.DataBase.DataBase;


namespace server_info_web_desk.Controllers
{

    //var check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();
    //[Authorize(Roles="admin")] [Authorize]
    public class InfoController : Controller
    {
        // GET: Info
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Index()
        {
            var check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            

            return View();
        }


        //тут убрать .AsNoTracking() проверить как работает
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Info_page(string person_id)
        {
            var check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            person_id=person_id ?? check_id;
            if (person_id == null)
                return RedirectToAction("Login", "Account");
            IndexInfoView res = new IndexInfoView();
            Section first_sec = null;
            using (ApplicationDbContext db = new ApplicationDbContext())
            { 
                try
                {
                    //var first_sec_id = db.Sections.AsNoTracking().Where(x1 => x1.UserId == person_id).Min(x1 => x1.Id);
                    //first_sec = db.Sections.AsNoTracking().First(x1 => x1.Id == first_sec_id);
                    first_sec = db.Sections.Where(x1 => x1.UserId == person_id).First(x1 => x1.SectionParrentId == null);
                    if (person_id != check_id)
                    {
                        db.Entry(first_sec).Reference(x1 => x1.User).Load();
                        if (!first_sec.User.Open_data_info)
                            return new HttpStatusCodeResult(423);
                    }

                }
                catch (Exception ex)
                {
                    if (!(ex is ArgumentNullException) && !(ex is InvalidOperationException))
                    {
                        throw ex;
                    }


                    ApplicationUser pers = db.Users.FirstOrDefault(x1 => x1.Id == check_id);
                    first_sec = new Section() { Head = "ALL", User = pers };
                    db.Sections.Add(first_sec);
                    db.SaveChanges();


                }
            }
            res.Sections.Add(first_sec);//главная секция должна быть первой в списке
            GetSectionInside(first_sec.Id, res.Sections, res.Articles);
            //res.Sections.AddRange();
            




            return View(res);
        }
        //TODO
        [AllowAnonymous]
        // [HttpGet][HttpPost]
        public JsonResult Load_article_body(int? id)
        {
            if (string.IsNullOrWhiteSpace(id.ToString()))
                return Json(false, JsonRequestBehavior.AllowGet);
            var check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            Article res = null;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                 res = db.Articles.FirstOrDefault(x1 => x1.Id == id);
                if (res.UserId != check_id)
                {
                    db.Entry(res).Reference(x1 => x1.User).Load();
                    if (!res.User.Open_data_info)
                        return Json(false, JsonRequestBehavior.AllowGet);
                }
            }
            
            return Json(new Article(res, true), JsonRequestBehavior.AllowGet);
        }
        //TODO
        [AllowAnonymous]
       // [HttpGet][HttpPost]
        public JsonResult Load_inside_section(int? id)
        {
            if(string.IsNullOrWhiteSpace(id.ToString()))
                return Json(false, JsonRequestBehavior.AllowGet);
            //TODO проверить есть ли доступ
            var check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            Section section = null;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                 section = db.Sections.FirstOrDefault(x1 => x1.Id == id);//AsNoTracking()
                if (section == null)
                    return Json(false, JsonRequestBehavior.AllowGet);
                db.Entry(section).Reference(x1 => x1.User).Load();
                if (!section.User.Open_data_info && section.User.Id != check_id)
                {

                    return Json(false, JsonRequestBehavior.AllowGet);
                }
                db.Entry(section).Collection(x1 => x1.Sections).Load();
                db.Entry(section).Collection(x1 => x1.Articles).Load();
                section = new Section(section) { User = null, SectionParrentId = null };
                section.Articles = section.Articles.Select(x1 => x1 = new Article() { Id = x1.Id, Head = x1.Head, Body = null, SectionParrentId = x1.SectionParrentId }).ToList();
                section.Sections = section.Sections.Select(x1 => x1 = new Section() { Id = x1.Id, Head = x1.Head, SectionParrentId = x1.SectionParrentId }).ToList();
                //return JsonConvert.SerializeObject(section);
            }
            return Json(section, JsonRequestBehavior.AllowGet);
            /*
             function OnSuccess(data) {
        var results = $('#results'); // получаем нужный элемент
        results.empty(); //очищаем элемент
        for (var i = 0; i < data.length; i++) {
            results.append('<li>' + data[i].Name + '</li>'); // добавляем данные в список
        }
    }
             */
        }



        ////TODO
        [Authorize]
        [HttpPost]
        public JsonResult Add_section(int? parrent_sec_id, [Bind(Include = "Head")] Section a)
        {
            var check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();

            bool success = true;
            Section parrent_sec = CheckAccessSection(check_id, parrent_sec_id,out success);
            if (!success)
            {
                //TODO обработать ошибку
                //return new HttpStatusCodeResult(423);//Locked
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            using (ApplicationDbContext db = new ApplicationDbContext())
            {

                db.Sections.Add(a);
                a.SectionParrentId = parrent_sec.Id;
                a.UserId = check_id;
                db.SaveChanges();
            }
            //parrent_sec.Sections.Add(a);
            
           // parrent_sec.User.Sections.Add(a);
            //db.SaveChanges();


            //var pers = db.Users.FirstOrDefault(x1 => x1.Id == check_id);

            
           
            return Json(new Section(a,true), JsonRequestBehavior.AllowGet);
        }

        //TODO
        [Authorize]
        [HttpPost]
        public JsonResult Add_article(int? parrent_sec_id, [Bind(Include = "Head, Body")]Article a)
        {
            var check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            bool success = true;
            Section parrent_sec = CheckAccessSection(check_id, parrent_sec_id, out success);
            if (!success)
            {
                //TODO обработать ошибку
                //return new HttpStatusCodeResult(423);//Locked
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            a.SectionParrentId = (int)parrent_sec_id;
            a.UserId = check_id;


            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Articles.Add(a);
                db.SaveChanges();
            }

            return Json(new Article(a,true) , JsonRequestBehavior.AllowGet);
        }

        //TODO
        [Authorize]
        [HttpPost]
        public JsonResult Edit_section([Bind(Include = "Id, Head")]Section a)
        {
            var check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            //bool success = true;
            
            Section section =null;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                section = db.Sections.FirstOrDefault(x1 => x1.Id == a.Id && x1.UserId == check_id);
                //db.Entry(section).Reference(x1 => x1.Section_parrent).Load();
                //CheckAccessSection(check_id, section.Section_parrent.Id, out success);
                if (section == null)
                {
                    //TODO обработать ошибку
                    //return new HttpStatusCodeResult(423);//Locked
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
                section.Head = a.Head;
                db.SaveChanges();
            }
            return Json(new Section(section,true) , JsonRequestBehavior.AllowGet);
        }

        //TODO
        [Authorize]
        [HttpPost]
        public JsonResult Edit_article([Bind(Include = "Id, Head, Body")]Article a)
        {
            var check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            //bool success = true;
            Article article = null;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                 article = db.Articles.FirstOrDefault(x1 => x1.Id == a.Id && x1.UserId == check_id);
                //db.Entry(article).Reference(x1 => x1.Section_parrent).Load();
                //CheckAccessSection(check_id, article.Section_parrent.Id, out success);
                if (article == null)
                {
                    //TODO обработать ошибку
                    //return new HttpStatusCodeResult(423);//Locked
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
                article.Head = a.Head;
                article.Body = a.Body;
                db.SaveChanges();
            }


            return Json(new Article(article,true), JsonRequestBehavior.AllowGet);
        }

        //TODO
        [Authorize]
        [HttpPost]
        public JsonResult Delete_section(int? id)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();
                var sec = db.Sections.FirstOrDefault(x1 => x1.Id == id && x1.UserId == check_id);
                if (sec == null)
                    return Json(false);
                List<int> sec_list = new List<int>();
                //List<int> art_list = new List<int>();
                if (sec.SectionParrentId != null)
                    sec_list.Add(sec.Id);
                else
                {
                    var lst = db.Articles.Where(x1 => x1.SectionParrentId == sec.Id).ToList();
                    if (lst.Count > 0)
                        db.Articles.RemoveRange(lst);
                }
                Get_inside_id((int)id, sec_list, null);
                var section_for_delete = db.Sections.Join(sec_list, p => p.Id, c => c, (p, c) => p);
                int? parrent_id = sec.SectionParrentId;
                db.Sections.RemoveRange(section_for_delete);
                db.SaveChanges();

                //var gg = db.Articles.ToList();
                //var gg2 = db.Sections.ToList();
                //return Json("inside_"+id);
                return Json(new { main_id = id, parrent_id_main = parrent_id, sec_list = sec_list });
            }
        }

        //TODO
        [Authorize]
        [HttpPost]
        public JsonResult Delete_article(int? id)
        {
            var check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var art = db.Articles.FirstOrDefault(x1 => x1.Id == id && x1.UserId == check_id);
                if (art == null)
                    return Json(false);
                db.Articles.Remove(art);
                db.SaveChanges();
            }

            return Json(id);
        }



        //TODO

            //загрузка и обновление всей db из файла\текста пользователя
        //[Authorize]
        //[HttpPost]
        //public ActionResult Load_all_data_in_db(string a)
        //{
        //    var check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();
        //    ListData data = null;
        //    try
        //    {
        //        data = JsonConvert.DeserializeObject<ListData>(a);
        //    }
        //    catch {
                
        //        return new HttpStatusCodeResult(400);//bad request
        //    }

        //    db.Articles.RemoveRange(db.Articles.Where(x1 => x1.UserId == check_id));
        //    db.Sections.RemoveRange(db.Sections.Where(x1 => x1.UserId == check_id));


        //    return View();
        //}






        //TODO
        [AllowAnonymous]
        [HttpPost]
        public JsonResult Start_search(int? id_for_search,string src,string user_id)
        {
            //OnComplete_search  start_search
            var check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            if (user_id == null)
            {
                //return Json(false);
                user_id = check_id;
            }
            if (user_id == null)
            {
                return Json(false);
               // user_id = check_id;
            }
            if (check_id != user_id)
            {
                bool? sc = false;
                using (ApplicationDbContext db = new ApplicationDbContext())
                    sc = db.Users.FirstOrDefault(x1 => x1.Id == user_id)?.Open_data_info;
                if(sc!=true)
                    return Json(false);
            }
           


            var mass_words = src.Split(' ');
            List<dynamic> Mark = new List<dynamic>();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var art = db.Articles.AsNoTracking().Where(x1 => x1.UserId == user_id).ToList();
                //Dictionary<int, double> Mark = new Dictionary<int, double>();

                List<int> atr_list_id = new List<int>();
                if (id_for_search != null)
                {
                    //Get_inside_id
                    //добавить список вложенных статей к результату для отрисовки на клиенте доп поля
                    Get_inside_id((int)id_for_search, null, atr_list_id);
                }

                
                foreach (var i in art)
                {
                    int tmp = atr_list_id.FirstOrDefault(x1 => x1 == i.Id);
                    Mark.Add(new
                    {
                        Id = i.Id,
                        Mark = GetMarkArticle(i, mass_words),
                        Head = i.Head,
                        inside = tmp == 0 ? false : true
                    });

                }
            }



            //foreach i.Key, i.Value
            return Json( Mark );//,art_id_list_inside= atr_list_id
        }

        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult Main_header(string action_name)
        {
            //var check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            ViewBag.action_name = action_name;

            return PartialView();
        }

        [Authorize]
        
        public JsonResult Download_data_file()
        {
            var check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();

            AllData res = new AllData();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                res.Articles.AddRange(db.Articles.Where(x1 => x1.UserId == check_id).ToList().Select(x1 => new Article(x1, true) { UserId = null }));
                res.Sections.AddRange(db.Sections.Where(x1 => x1.UserId == check_id).ToList().Select(x1 => new Section(x1, true) { UserId = null }));
                foreach (var i in res.Articles)
                {
                    try
                    {
                        res.Images.AddRange(db.ImagesInfo.Where(x1 => x1.Article_parrentId == i.Id).ToList().Select(x1 => new ImageInfo(x1, true)));
                    }
                    catch { }

                }
            }

            return Json(res);
        }


        [Authorize]
        [HttpPost]
        public JsonResult Upload_data_file_text(string upload_text)
        {

            var check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            AllData data = null;
            try
            {
                data = JsonConvert.DeserializeObject<AllData>(upload_text);
               
            }
            catch
            {
                return Json(false); //bad request
            }




            //db.Articles.RemoveRange(db.Articles.Where(x1 => x1.UserId == check_id));
            //db.Sections.RemoveRange(db.Sections.Where(x1 => x1.UserId == check_id));


            //TODO  тут данные не удалять а изменять??? 
            //идти по бд и сравнивать со строкой и удалять менять 
            //идти по файлу и сравнивать с бд менять
            //если в файле есть блок которого нет в бд то добавлять

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var lst_sec = db.Sections.Where(x1 => x1.UserId == check_id).ToList();
                foreach (var i in lst_sec)
                {
                    var tmp_sec = data.Sections.FirstOrDefault(x1 => x1.Id == i.Id);
                    if (tmp_sec == null)
                        db.Sections.Remove(i);
                    else
                    {
                        if (!i.Head.Equals(tmp_sec.Head))
                        {
                            tmp_sec.Head = tmp_sec.Head;
                        }
                        i.Order = tmp_sec.Order;
                        bool owner_sec = true;
                        if (i.SectionParrentId != tmp_sec.SectionParrentId)
                        {
                            CheckAccessSection(check_id, tmp_sec.SectionParrentId, out owner_sec);
                        }
                        if (owner_sec)
                            i.SectionParrentId = tmp_sec.SectionParrentId;
                        data.Sections.Remove(tmp_sec);
                    }
                }
                foreach (var i in data.Sections)
                {
                    bool owner_sec = true;
                    //тут мб при ошибке проставлять Section_parrentId id главной секции?
                    CheckAccessSection(check_id, i.SectionParrentId, out owner_sec);
                    if (i.SectionParrentId == null || owner_sec)
                    {
                        i.Id = 0;
                        i.UserId = check_id;
                        db.Sections.Add(i);
                    }

                }
                var lst_art = db.Articles.Where(x1 => x1.UserId == check_id).ToList();
                foreach (var i in lst_art)
                {
                    var tmp_art = data.Articles.FirstOrDefault(x1 => x1.Id == i.Id);
                    if (tmp_art == null)
                        db.Articles.Remove(i);
                    else
                    {
                        if (!i.Head.Equals(tmp_art.Head))
                        {
                            tmp_art.Head = tmp_art.Head;
                        }
                        if (!i.Body.Equals(tmp_art.Body))
                        {
                            tmp_art.Body = tmp_art.Body;
                        }
                        i.Order = tmp_art.Order;
                        bool owner_sec = true;
                        if (i.SectionParrentId != tmp_art.SectionParrentId)
                        {
                            CheckAccessSection(check_id, tmp_art.SectionParrentId, out owner_sec);
                        }
                        if (owner_sec)
                            i.SectionParrentId = tmp_art.SectionParrentId;
                        data.Articles.Remove(tmp_art);
                    }
                }
                foreach (var i in data.Articles)
                {
                    //i.Section_parrentId ==0   НЕ УВЕРЕН ЧТО НУЖНО не знаю как json парсит null для типов INT //i.Section_parrentId ==0||
                    bool owner_sec = true;
                    //тут мб при ошибке проставлять Section_parrentId id главной секции?
                    CheckAccessSection(check_id, i.SectionParrentId, out owner_sec);
                    if (owner_sec)
                    {

                        i.Id = 0;
                        i.UserId = check_id;
                        db.Articles.Add(i);
                    }
                }
                var lst_img = db.ImagesInfo.Where(x1 => x1.UserId == check_id).ToList();
                foreach (var i in lst_img)
                {
                    var tmp_img = data.Images.FirstOrDefault(x1 => x1.Id == i.Id);
                    if (tmp_img == null)
                        db.ImagesInfo.Remove(i);
                    else
                    {

                        i.Data = tmp_img.Data;
                        bool owner_sec = true;
                        if (i.Article_parrentId != tmp_img.Article_parrentId)
                        {
                            CheckAccessSection(check_id, db.Articles.FirstOrDefault(x1 => x1.Id == tmp_img.Article_parrentId)?.SectionParrentId, out owner_sec);
                        }
                        if (owner_sec)
                            i.Article_parrentId = tmp_img.Article_parrentId;
                        data.Images.Remove(tmp_img);
                    }
                }
                foreach (var i in data.Images)
                {
                    if (i.Article_parrentId == null || db.Articles.FirstOrDefault(x1 => x1.Id == i.Article_parrentId && x1.UserId == check_id) != null)
                    {
                        i.Id = 0;
                        i.UserId = check_id;
                        db.ImagesInfo.Add(i);
                    }
                }

                db.SaveChanges();
            }
            return Json(true);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Upload_data_file(HttpPostedFileBase upload)
        {
            if (upload != null)
            {
                Stream documentConverted = upload.InputStream;
                byte[] mass_data = null;
                using (MemoryStream ms = new MemoryStream())
                {
                    documentConverted.CopyTo(ms);
                    mass_data = ms.ToArray();
                }
                /*using (FileStream st=new FileStream(mass_data))
                {

                }*/
                string fileName = System.IO.Path.GetFileName(upload.FileName);
                try
                {
                    using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                    {
                        fs.Write(mass_data, 0, mass_data.Length);

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception caught in process: {0}", ex);

                }
                // получаем имя файла

                // сохраняем файл в папку Files в проекте

            }
            return RedirectToAction("Index");
        }
    }
}