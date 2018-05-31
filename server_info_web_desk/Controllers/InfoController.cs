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
            var ff = db.Users.ToList();

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
            try
            {
                //var first_sec_id = db.Sections.AsNoTracking().Where(x1 => x1.UserId == person_id).Min(x1 => x1.Id);
                //first_sec = db.Sections.AsNoTracking().First(x1 => x1.Id == first_sec_id);
                first_sec = db.Sections.Where(x1 => x1.UserId == person_id).First(x1=>x1.Section_parrentId==null);
                if (person_id != check_id)
                {
                    db.Entry(first_sec).Reference(x1 => x1.User).Load();
                    if(!first_sec.User.Open_data_info)
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
                    first_sec=new Section() { Head="ALL",User= pers };
                    db.Sections.Add(first_sec);
                    db.SaveChanges();
                

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
            Article res = db.Articles.FirstOrDefault(x1 => x1.Id == id);
            if (res.UserId != check_id)
            {
                db.Entry(res).Reference(x1 => x1.User).Load();
                if(!res.User.Open_data_info)
                    return Json(false, JsonRequestBehavior.AllowGet);
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


            var section = db.Sections.FirstOrDefault(x1 => x1.Id == id);//AsNoTracking()
            db.Entry(section).Reference(x1 => x1.User).Load();
            if (!section.User.Open_data_info&& section.User.Id != check_id)
            {
                
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            db.Entry(section).Collection(x1=>x1.Sections).Load();
            db.Entry(section).Collection(x1=>x1.Articles).Load();
            section = new Section(section) { User = null, Section_parrent = null };
            section.Articles=section.Articles.Select(x1=>x1=new Article() { Id=x1.Id, Head=x1.Head, Body=null,Section_parrentId=x1.Section_parrentId }).ToList();
            section.Sections=section.Sections.Select(x1 => x1 = new Section() { Id = x1.Id, Head = x1.Head, Section_parrentId = x1.Section_parrentId }).ToList();
            //return JsonConvert.SerializeObject(section);
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

            db.Sections.Add(a);
            a.Section_parrentId = parrent_sec.Id;
            a.UserId = check_id;
            db.SaveChanges();
            
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
            a.Section_parrentId = (int)parrent_sec_id;
            a.UserId = check_id;

           
            
            db.Articles.Add(a);
            db.SaveChanges();


            return Json(new Article(a,true) , JsonRequestBehavior.AllowGet);
        }

        //TODO
        [Authorize]
        [HttpPost]
        public JsonResult Edit_section([Bind(Include = "Id, Head")]Section a)
        {
            var check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            //bool success = true;
            
            Section section = db.Sections.FirstOrDefault(x1 => x1.Id == a.Id&&x1.UserId== check_id);
            //db.Entry(section).Reference(x1 => x1.Section_parrent).Load();
            //CheckAccessSection(check_id, section.Section_parrent.Id, out success);
            if (section==null)
            {
                //TODO обработать ошибку
                //return new HttpStatusCodeResult(423);//Locked
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            section.Head = a.Head;
            db.SaveChanges();

            return Json(new Section(section,true) , JsonRequestBehavior.AllowGet);
        }

        //TODO
        [Authorize]
        [HttpPost]
        public JsonResult Edit_article([Bind(Include = "Id, Head, Body")]Article a)
        {
            var check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            //bool success = true;

            Article article = db.Articles.FirstOrDefault(x1 => x1.Id == a.Id&&x1.UserId==check_id);
            //db.Entry(article).Reference(x1 => x1.Section_parrent).Load();
            //CheckAccessSection(check_id, article.Section_parrent.Id, out success);
            if (article == null)
            {
                //TODO обработать ошибку
                //return new HttpStatusCodeResult(423);//Locked
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            article.Head = a.Head;
            article.Body= a.Body;
            db.SaveChanges();



            return Json(new Article(article,true), JsonRequestBehavior.AllowGet);
        }

        //TODO
        [Authorize]
        [HttpPost]
        public JsonResult Delete_section(int? id)
        {
            var check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var sec = db.Sections.FirstOrDefault(x1 => x1.Id == id && x1.UserId == check_id);
            if(sec==null)
                return Json(false);
            List<int> sec_list = new List<int>();
            List<int> art_list = new List<int>();
            if (sec.Section_parrentId != null)
                sec_list.Add(sec.Id);
            else
            {
                var lst = db.Articles.Where(x1 => x1.Section_parrentId == sec.Id).ToList();
                if(lst.Count>0)
                db.Articles.RemoveRange(lst);
            }
            Get_inside_id((int)id, sec_list, art_list);
            var section_for_delete = db.Sections.Join(sec_list, p => p.Id, c => c, (p, c) => p);
            db.Sections.RemoveRange(section_for_delete);
            db.SaveChanges();

            //var gg = db.Articles.ToList();
            //var gg2 = db.Sections.ToList();
            //return Json("inside_"+id);
            return Json(new {main_id=id,parrent_id_main= sec.Section_parrentId, sec_list= sec_list });
        }

        //TODO
        [Authorize]
        [HttpPost]
        public JsonResult Delete_article(int? id)
        {
            var check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var art = db.Articles.FirstOrDefault(x1 => x1.Id == id && x1.UserId == check_id);
            if (art == null)
                return Json(false);
            db.Articles.Remove(art);
            db.SaveChanges();


            return Json(id);
        }



        //TODO

            //загрузка и обновление всей db из файла\текста пользователя
        [Authorize]
        [HttpPost]
        public ActionResult Load_all_data_in_db(string a)
        {
            var check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            ListData data = null;
            try
            {
                data = JsonConvert.DeserializeObject<ListData>(a);
            }
            catch {
                //TODO обработать ошибку
                //return RedirectToAction("Send_error","Home",new { ret=2, code="Произошла ошибка" });
                //return StatusCode(418);
                return new HttpStatusCodeResult(400);//bad request
            }

            db.Articles.RemoveRange(db.Articles.Where(x1 => x1.UserId == check_id));
            db.Sections.RemoveRange(db.Sections.Where(x1 => x1.UserId == check_id));


            return View();
        }






        //TODO
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Start_search(string src)
        {
            var check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();


            return View();
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