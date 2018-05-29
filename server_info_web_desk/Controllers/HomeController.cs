using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using server_info_web_desk.Models;
using System.IO;
using Newtonsoft.Json;

using Microsoft.AspNet.Identity.Owin;
using System.Runtime.Serialization.Formatters.Binary;

using Microsoft.AspNet.Identity;

//
using static server_info_web_desk.Models.functions.FunctionsProject;
using static server_info_web_desk.Models.DataBase.DataBase;




namespace server_info_web_desk.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var check_id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            return View();
        }

        //TODO подумать как сделать
        //ret==0- просто на другое представление  / 1-частичное/2-json
        public ActionResult Send_error(int ret,string code)
        {
            switch (ret)
            {
                case 0:
                    
                    return View();
                    break;
                case 1:
                    ViewBag.PartialView = true;
                    return PartialView();
                    break;
                case 2:
                    return Json(false, JsonRequestBehavior.AllowGet);
                    break;
            }
            return View();

        }
    }
}