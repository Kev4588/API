using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PURIS_FLASH.Controllers
{
    [VerificarSesion]
    public class CompradorController : Controller
    {
        // GET: Users
        public ActionResult Index()
        {
            return View();
        }
    }
}