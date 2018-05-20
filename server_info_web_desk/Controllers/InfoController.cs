using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using server_info_web_desk.Models;
using server_info_web_desk.Models.Info;
using System.IO;
using Newtonsoft.Json;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;

//
using static server_info_web_desk.Models.functions.Functions_project;
using static server_info_web_desk.Models.functions.Functions_info;
using static server_info_web_desk.Models.DataBase.DataBase;
namespace server_info_web_desk.Controllers
{

    //var check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();
    //[Authorize(Roles="admin")] [Authorize]
    public class InfoController : Controller
    {
        // GET: Info
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        //TODO
        [HttpGet]
        public JsonResult Load_inside_section(int id)
        {

            //TODO проверить есть ли доступ
            var check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();


            var section = db.Sections.AsNoTracking().FirstOrDefault(x1 => x1.Id == id);
            db.Entry(section).Reference("User").Load();
            if(!section.User.Open_data_info&& section.User.Id != check_id)
            {
                //TODO обработать ошибку доступа к данным
            }
            db.Entry(section).Reference("Sections").Load();
            db.Entry(section).Reference("Articles").Load();
            section.User = null;
            section.Articles.Select(x1=>x1=new Article() { Id=x1.Id, Head=x1.Head, Body=null });

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



        //TODO
        [HttpPost]
        public JsonResult Add_section(int? parrent_sec_id, [Bind(Include = "Id, Head")] Section a)
        {
            var check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();

            bool success = true;
            Section parrent_sec = Check_access_section(check_id, parrent_sec_id,out success);
            if (!success)
            {
                //TODO обработать ошибку
            }

            db.Sections.Add(a);
            db.SaveChanges();
            parrent_sec.Sections.Add(a);
            parrent_sec.User.Sections.Add(a);
            db.SaveChanges();


            //var pers = db.Users.FirstOrDefault(x1 => x1.Id == check_id);



            return Json(true, JsonRequestBehavior.AllowGet);
        }

        //TODO
        [HttpPost]
        public JsonResult Add_article(int? parrent_sec_id, [Bind(Include = "Id, Head, Body")]Article a)
        {
            var check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            bool success = true;
            Section parrent_sec = Check_access_section(check_id, parrent_sec_id, out success);
            if (!success)
            {
                //TODO обработать ошибку
            }
            db.Articles.Add(a);
            db.SaveChanges();
            parrent_sec.Articles.Add(a);
            parrent_sec.User.Articles.Add(a);
            db.SaveChanges();


            return Json(true, JsonRequestBehavior.AllowGet);
        }

        //TODO
        [HttpPost]
        public JsonResult Edit_section([Bind(Include = "Id, Head")]Section a)
        {
            var check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            bool success = true;
            
            Section section = db.Sections.FirstOrDefault(x1 => x1.Id == a.Id);
            db.Entry(section).Reference(x1 => x1.Section_parrent).Load();
             Check_access_section(check_id, section.Section_parrent.Id, out success);
            if (!success)
            {
                //TODO обработать ошибку
            }
            section.Head = a.Head;
            db.SaveChanges();

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        //TODO
        [HttpPost]
        public JsonResult Edit_article([Bind(Include = "Id, Head, Body")]Article a)
        {
            var check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            bool success = true;

            Article article = db.Articles.FirstOrDefault(x1 => x1.Id == a.Id);
            db.Entry(article).Reference(x1 => x1.Section_parrent).Load();
            Check_access_section(check_id, article.Section_parrent.Id, out success);
            if (!success)
            {
                //TODO обработать ошибку
            }
            article.Head = a.Head;
            article.Body= a.Body;
            db.SaveChanges();



            return Json(true, JsonRequestBehavior.AllowGet);
        }

        //TODO
        [HttpPost]
        public JsonResult Delete_section(Section a)
        {
            var check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();




            return Json(true, JsonRequestBehavior.AllowGet);
        }

        //TODO
        [HttpPost]
        public JsonResult Delete_article(Article a)
        {
            var check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();



            return Json(true, JsonRequestBehavior.AllowGet);
        }




        //TODO
        [HttpPost]
        public ActionResult Start_search(string src)
        {
            var check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();


            return View();
        }






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