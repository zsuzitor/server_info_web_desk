using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using server_info_web_desk.Models;
using server_info_web_desk.Models.Info;
using System.IO;
using Newtonsoft.Json;

//
using static server_info_web_desk.Models.functions.Functions_project;
using static server_info_web_desk.Models.DataBase.DataBase;
namespace server_info_web_desk.Controllers
{
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
        public ActionResult Load_inside_section()
        {
            return View();
        }



        //TODO
        [HttpPost]
        public ActionResult Add_section( Section a)
        {
            return View();
        }

        //TODO
        [HttpPost]
        public ActionResult Add_article(Article a)
        {
            return View();
        }

        //TODO
        [HttpPost]
        public ActionResult Edit_section(Section a)
        {
            return View();
        }

        //TODO
        [HttpPost]
        public ActionResult Edit_article(Article a)
        {
            return View();
        }

        //TODO
        [HttpPost]
        public ActionResult Delete_section(Section a)
        {
            return View();
        }

        //TODO
        [HttpPost]
        public ActionResult Delete_article(Article a)
        {
            return View();
        }




        //TODO
        [HttpPost]
        public ActionResult Start_search(string src)
        {



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