using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using server_info_web_desk.Models;
using System.IO;
using Newtonsoft.Json;

//
using static server_info_web_desk.Models.functions.Functions_project;
using static server_info_web_desk.Models.DataBase.DataBase;
using Microsoft.AspNet.Identity.Owin;
using System.Runtime.Serialization.Formatters.Binary;

namespace server_info_web_desk.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            
            return View();
        }

        
    }
}