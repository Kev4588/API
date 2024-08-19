using PURIS_FLASH.Models.TableViewModel;
using PURIS_FLASH.Models.ViewModel;
using PURIS_FLASH.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using KREAM_ProyectoFinal.Models;

namespace PURIS_FLASH.Controllers
{
    // Atributo que verifica si hay una sesión activa antes de permitir el acceso al controlador.
    [VerificarSesion]
    public class UserController : Controller
    {
        // Acción GET que muestra la vista para editar la información de un usuario específico.
        [HttpGet]
        public ActionResult Edit(int Id)
        {
            // Inicializa un modelo para pasar los datos del usuario a la vista.
            UsersViewModel model = new UsersViewModel();

            // Usa la base de datos para buscar el usuario por su ID.
            using (var db = new TRAVEL2Entities())
            {
                var userTO = db.Users.Find(Id);

                // Mapea los datos del usuario al modelo.
                model.Cedula = userTO.Cedula;
                model.Nombre = userTO.Nombre;
                model.PrimerApellido = userTO.PrimerApellido;
                model.SegundoApellido = userTO.SegundoApellido;
                model.Edad = (int)userTO.Edad;
                model.Telefono = (int)userTO.Telefono;
                model.Correo = userTO.Correo;
                model.Sexo = userTO.Sexo;
                model.Direccion = userTO.Direccion;

                // Guarda el nombre y apellido del usuario en un ViewBag para usarlo en la vista de edición.
                ViewBag.Nombre = userTO.Nombre + " " + userTO.PrimerApellido;
            }

            // Devuelve la vista con el modelo de usuario.
            return View(model);
        }

        // Acción POST que maneja la lógica para actualizar la información de un usuario.
        [HttpPost]
        public ActionResult Edit(UsersTableViewModel model)
        {
            // Usa la base de datos para actualizar los datos del usuario.
            using (var db = new TRAVEL2Entities())
            {
                // Recupera la información del usuario actual desde la sesión.
                var usuario = Session["UsuarioActual"] as UsersTableViewModel;

                // Ejecuta un procedimiento almacenado para actualizar la información del usuario.
                db.Database.ExecuteSqlCommand(
                    "EXEC UpdateUser @Id, @Cedula, @Nombre, @PrimerApellido, @SegundoApellido, @Edad, @Telefono, @Correo, @Direccion, @Sexo",
                    new SqlParameter("@Id", model.Id),
                    new SqlParameter("@Cedula", model.Cedula),
                    new SqlParameter("@Nombre", model.Nombre),
                    new SqlParameter("@PrimerApellido", model.PrimerApellido),
                    new SqlParameter("@SegundoApellido", model.SegundoApellido),
                    new SqlParameter("@Edad", model.Edad),
                    new SqlParameter("@Telefono", model.Telefono),
                    new SqlParameter("@Correo", model.Correo),
                    new SqlParameter("@Direccion", model.Direccion),
                    new SqlParameter("@Sexo", model.Sexo)
                );

                // Actualiza el nombre del usuario actual en ViewBag para reflejar los cambios.
                ViewBag.UsuarioActual = model.Nombre;

                // Almacena un mensaje de éxito en TempData para mostrarlo después de redirigir.
                TempData["SuccessMessage"] = "Datos actualizados correctamente.";

                // Redirige a la página principal después de actualizar la información del usuario.
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
