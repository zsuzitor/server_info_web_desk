using System;
using System.Collections.Generic;

using System.Linq;
//using MoreLinq;

using System.Web;
using System.Web.Mvc;

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
        [HttpGet]
        public ActionResult Index()
        {
            var check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            

            return View();
        }
        [HttpGet]
        public ActionResult Info_page(string person_id)
        {
            var check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            person_id=person_id ?? check_id;
            IndexInfoView res = new IndexInfoView();
            Section first_sec = null;
            try
            {
                //var first_sec_id = db.Sections.AsNoTracking().Where(x1 => x1.UserId == person_id).Min(x1 => x1.Id);
                //first_sec = db.Sections.AsNoTracking().First(x1 => x1.Id == first_sec_id);
                first_sec = db.Sections.AsNoTracking().Where(x1 => x1.UserId == person_id).First(x1=>x1.Section_parrentId==null);
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
            GetSectionInside(first_sec.Id, res.Sections, res.Articles);
            //res.Sections.AddRange();
            res.Sections.Add(first_sec);




            return View();
        }
        
        //TODO
        [HttpGet]
        public JsonResult Load_inside_section(int id)
        {

            //TODO проверить есть ли доступ
            var check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();


            var section = db.Sections.AsNoTracking().FirstOrDefault(x1 => x1.Id == id);
            db.Entry(section).Reference(x1=>x1.User).Load();
            if(!section.User.Open_data_info&& section.User.Id != check_id)
            {
                //TODO обработать ошибку доступа к данным
                //return new HttpStatusCodeResult(423);//Locked
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            db.Entry(section).Reference(x1=>x1.Sections).Load();
            db.Entry(section).Reference(x1=>x1.Articles).Load();
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
            Section parrent_sec = CheckAccessSection(check_id, parrent_sec_id,out success);
            if (!success)
            {
                //TODO обработать ошибку
                //return new HttpStatusCodeResult(423);//Locked
                return Json(false, JsonRequestBehavior.AllowGet);
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
            Section parrent_sec = CheckAccessSection(check_id, parrent_sec_id, out success);
            if (!success)
            {
                //TODO обработать ошибку
                //return new HttpStatusCodeResult(423);//Locked
                return Json(false, JsonRequestBehavior.AllowGet);
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
            CheckAccessSection(check_id, section.Section_parrent.Id, out success);
            if (!success)
            {
                //TODO обработать ошибку
                //return new HttpStatusCodeResult(423);//Locked
                return Json(false, JsonRequestBehavior.AllowGet);
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
            CheckAccessSection(check_id, article.Section_parrent.Id, out success);
            if (!success)
            {
                //TODO обработать ошибку
                //return new HttpStatusCodeResult(423);//Locked
                return Json(false, JsonRequestBehavior.AllowGet);
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