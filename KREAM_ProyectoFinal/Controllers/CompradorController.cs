using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KREAM_ProyectoFinal.Controllers
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