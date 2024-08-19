using PURIS_FLASH.Models.TableViewModel;
using PURIS_FLASH.Models;
using PURIS_FLASH.Models.ViewModel;
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
    public class RegisterController : Controller
    {
        // Acción GET que prepara la vista para agregar un nuevo usuario.
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        // Método POST para agregar un nuevo usuario a la base de datos.
        [HttpPost]
        public ActionResult Create(UsersViewModel nuevaCuenta)
        {
            // Verifica si el modelo es válido.
            if (!ModelState.IsValid)
            {
                // Verifica si la cédula ya está registrada.
                if (usuarioExisteCedula(nuevaCuenta.Cedula) == 1)
                {
                    TempData["Mensaje"] = "Cédula ya está registrada en el sistema, por favor intente de nuevo";
                }
                // Verifica si el correo ya está registrado.
                else if (usuarioExisteEmail(nuevaCuenta.Correo) == 1)
                {
                    TempData["Mensaje"] = "Correo ya está registrado en el sistema, por favor intente de nuevo";
                }
                // Si no hay conflictos, procede a registrar el nuevo usuario.
                else
                {
                    using (var db = new TRAVEL2Entities())
                    {
                        // Crea una nueva entidad de usuario y asigna los valores del modelo.
                        Users userTO = new Users
                        {
                            Cedula = nuevaCuenta.Cedula,
                            Contrasena = nuevaCuenta.Contrasena,
                            Correo = nuevaCuenta.Correo,
                            Nombre = nuevaCuenta.Nombre,
                            PrimerApellido = nuevaCuenta.PrimerApellido,
                            SegundoApellido = nuevaCuenta.SegundoApellido,
                            Edad = nuevaCuenta.Edad,
                            Telefono = nuevaCuenta.Telefono,
                            Sexo = nuevaCuenta.Sexo,
                            Direccion = nuevaCuenta.Direccion,
                            TipoDeUsuario = 2, // Tipo de usuario estándar (no administrador).
                            PreguntaSeguridad = nuevaCuenta.PreguntaSeguridad,
                            RespuestaPreguntaSeguridad = nuevaCuenta.RespuestaPreguntaSeguridad
                        };

                        // Agrega el nuevo usuario a la base de datos y guarda los cambios.
                        db.Users.Add(userTO);
                        db.SaveChanges();
                    }
                    // Redirige al usuario a la página de login después de registrarse.
                    return Redirect("/Login/Login");
                }
            }
            // Si el modelo no es válido, retorna la vista con los datos ingresados.
            return View(nuevaCuenta);
        }

        // Método para verificar si la cédula ya existe en la base de datos.
        public int usuarioExisteCedula(int cedula)
        {
            using (TRAVEL2Entities db = new TRAVEL2Entities())
            {
                // Ejecuta un procedimiento almacenado para verificar la existencia de la cédula.
                var user = db.Database.SqlQuery<UsersViewModel>("EXEC VerifyUserCedula @var_cedula",
                    new SqlParameter("var_cedula", cedula)
                ).SingleOrDefault();

                // Retorna 1 si el usuario existe, de lo contrario, retorna 0.
                return user != null ? 1 : 0;
            }
        }

        // Método para verificar si el correo ya existe en la base de datos.
        public int usuarioExisteEmail(string email)
        {
            using (TRAVEL2Entities db = new TRAVEL2Entities())
            {
                // Ejecuta un procedimiento almacenado para verificar la existencia del correo.
                var user = db.Database.SqlQuery<UsersViewModel>("EXEC VerifyUserGmail @var_Correo",
                    new SqlParameter("var_Correo", email)
                ).SingleOrDefault();

                // Retorna 1 si el usuario existe, de lo contrario, retorna 0.
                return user != null ? 1 : 0;
            }
        }
    }
}
