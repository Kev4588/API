using KREAM_ProyectoFinal.Models.TableViewModel;
using KREAM_ProyectoFinal.Models.ViewModel;
using KREAM_ProyectoFinal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;

namespace KREAM_ProyectoFinal.Controllers
{
    [VerificarSesion]
    public class UserController : Controller
    {
        // GET: Admin
     

        [HttpGet]
        public ActionResult Edit(int Id)
        {
            UsersViewModel model = new UsersViewModel();
            using (var db = new TRAVEL2Entities())
            {
                var userTO = db.Users.Find(Id);

                model.Cedula = userTO.Cedula;
                model.Nombre = userTO.Nombre;
                model.PrimerApellido = userTO.PrimerApellido;
                model.SegundoApellido = userTO.SegundoApellido;
                model.Edad = (int)userTO.Edad;
                model.Telefono = (int)userTO.Telefono;
                model.Correo = userTO.Correo;
                model.Direccion = userTO.Direccion;


                // Esta linea es totalmente necesaria para que en la parte de EDIT , muestre en el titulo a quien esta editando
                // estos viewbags estan dentro de este catch up de arreglo para tomar los datos de la persona que coinciden con 
                // el ID , una vez eso ya tenemos el nombre , apellido etc de esa persona del ID de quien se le dio click en Edit
                ViewBag.Nombre = userTO.Nombre + " " + userTO.PrimerApellido;

            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(UsersTableViewModel model)
        {
            //if (!ModelState.IsValid) return View(model);

            using (var db = new TRAVEL2Entities())
            {
                var usuario = Session["UsuarioActual"] as UsersTableViewModel;


                //ViewBag.UsuarioActual = usuario.Nombre;

                db.Database.ExecuteSqlCommand(
                    "EXEC UpdateUser @Id, @Cedula, @Nombre, @PrimerApellido, @SegundoApellido, @Edad, @Telefono, @Correo, @Direccion",
                    new SqlParameter("@Id", model.Id),
                    new SqlParameter("@Cedula", model.Cedula),
                    new SqlParameter("@Nombre", model.Nombre),
                    new SqlParameter("@PrimerApellido", model.PrimerApellido),
                    new SqlParameter("@SegundoApellido", model.SegundoApellido),
                    new SqlParameter("@Edad", model.Edad),
                    new SqlParameter("@Telefono", model.Telefono),
                    new SqlParameter("@Correo", model.Correo),
                    new SqlParameter("@Direccion", model.Direccion)

                );

                ViewBag.UsuarioActual = model.Nombre;


                return RedirectToAction("Index", "Home");
            }
        }

     










    }
}

