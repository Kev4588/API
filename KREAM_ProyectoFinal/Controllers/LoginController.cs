using KREAM_ProyectoFinal.Models;
using PURIS_FLASH.Models;
using PURIS_FLASH.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace PURIS_FLASH.Controllers
{
    // Atributo que verifica si hay una sesión activa antes de permitir el acceso al controlador.
    [VerificarSesion]
    public class LoginController : Controller
    {
        // Acción que redirige a la página de login.
        public ActionResult Index()
        {
            return RedirectToAction("Login");
        }

        // Acción GET para mostrar la vista de login.
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        // Acción POST para manejar la lógica de autenticación de usuarios.
        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            // Verifica que la cédula tenga exactamente 9 dígitos.
            if (username.Length != 9)
            {
                ViewBag.ErrorMessage = "La cédula debe contener exactamente 9 dígitos.";
                return View("Login");
            }

            // Usa la base de datos para verificar al usuario.
            using (TRAVEL2Entities db = new TRAVEL2Entities())
            {
                // Ejecuta un procedimiento almacenado para verificar al usuario.
                var user = db.Database.SqlQuery<UsersViewModel>("EXEC VerifyUser @var_cedula, @var_contrasena",
                    new SqlParameter("var_cedula", username),
                    new SqlParameter("var_contrasena", password)
                ).SingleOrDefault();

                // Parámetro de salida para la autenticación.
                SqlParameter outputParam = new SqlParameter
                {
                    ParameterName = "@Autenticado",
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.Output
                };

                // Verifica la contraseña del usuario.
                var user2 = db.Database.SqlQuery<UsersViewModel>("EXEC VerifyPassword @Cedula, @Contrasena, @Autenticado OUTPUT",
                    new SqlParameter("Cedula", username),
                    new SqlParameter("Contrasena", password),
                    outputParam
                ).SingleOrDefault();

                bool resultadoAutenticado = (bool)outputParam.Value;

                // Verifica el tipo de usuario y redirige según corresponda.
                if (user != null)
                {
                    if (user.TipoDeUsuario == 1) // Administrador
                    {
                        Session["UsuarioActual"] = user;
                        return RedirectToAction("Index", "Home");
                    }
                    else if (user.TipoDeUsuario == 2) // Vendedor
                    {
                        Session["UsuarioActual"] = user;
                        return RedirectToAction("Index", "Home");
                    }
                    else if (user.TipoDeUsuario == 3) // Comprador
                    {
                        Session["UsuarioActual"] = user;
                        return RedirectToAction("Index", "Home");
                    }
                }
                // Si el usuario no es autenticado, muestra un mensaje de error.
                else if (outputParam.Value != DBNull.Value)
                {
                    if (!resultadoAutenticado)
                    {
                        ViewBag.ErrorMessage = "La contraseña es incorrecta";
                        return View("Login");
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "Usuario no encontrado, favor registrarse";
                    return RedirectToAction("Index", "Register");
                }
            }
            return View("Login");
        }

        // Acción POST para verificar si un usuario existe por su email.
        [HttpPost]
        public ActionResult usuarioExiste(string email)
        {
            using (TRAVEL2Entities db = new TRAVEL2Entities())
            {
                // Ejecuta un procedimiento almacenado para verificar si el usuario existe.
                var user = db.Database.SqlQuery<UsersViewModel>("EXEC VerifyUserGmail @var_Correo",
                    new SqlParameter("var_Correo", email)
                ).SingleOrDefault();

                // Parámetro de salida para la autenticación.
                SqlParameter outputParam = new SqlParameter
                {
                    ParameterName = "@Autenticado",
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.Output
                };

                // Si el usuario existe, lo guarda en la sesión y retorna "1".
                if (user != null)
                {
                    Session["UsuarioActual"] = user;
                    return Content("1");
                }
                else
                {
                    return Content("0");
                }
            }
        }

        // Acción POST similar a `usuarioExiste` para verificar si un usuario existe por su email (parece duplicado).
        [HttpPost]
        public ActionResult usuarioExiste2(string email)
        {
            using (TRAVEL2Entities db = new TRAVEL2Entities())
            {
                // Ejecuta un procedimiento almacenado para verificar si el usuario existe.
                var user = db.Database.SqlQuery<UsersViewModel>("EXEC VerifyUserGmail @var_Correo",
                    new SqlParameter("var_Correo", email)
                ).SingleOrDefault();

                // Parámetro de salida para la autenticación.
                SqlParameter outputParam = new SqlParameter
                {
                    ParameterName = "@Autenticado",
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.Output
                };

                // Retorna "1" si el usuario existe, de lo contrario "0".
                if (user != null)
                {
                    return Content("1");
                }
                else
                {
                    return Content("0");
                }
            }
        }

        // Acción POST para verificar si un usuario existe por su ID de redes sociales.
        [HttpPost]
        public ActionResult usuarioExisteSocialMedia(string socialMediaID)
        {
            using (TRAVEL2Entities db = new TRAVEL2Entities())
            {
                // Ejecuta un procedimiento almacenado para verificar si el usuario existe.
                var user = db.Database.SqlQuery<UsersViewModel>("EXEC VerifyUserSocialMedia @var_socialMediaID",
                    new SqlParameter("var_socialMediaID", socialMediaID)
                ).SingleOrDefault();

                // Parámetro de salida para la autenticación.
                SqlParameter outputParam = new SqlParameter
                {
                    ParameterName = "@Autenticado",
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.Output
                };

                // Si el usuario existe, lo guarda en la sesión y retorna "1".
                if (user != null)
                {
                    Session["UsuarioActual"] = user;
                    return Content("1");
                }
                else
                {
                    return Content("0");
                }
            }
        }

        // Acción POST para crear un usuario mediante la API usando nombre, primer apellido y email.
        [HttpPost]
        public ActionResult CreateAPI(string nombre, string primerapellido, string userEmail)
        {
            try
            {
                // Crea un nuevo usuario en la base de datos.
                using (var db = new TRAVEL2Entities())
                {
                    Users userTO = new Users
                    {
                        Cedula = 0,
                        Nombre = nombre,
                        PrimerApellido = primerapellido,
                        SegundoApellido = "API",
                        Edad = 0,
                        Telefono = 0,
                        Correo = userEmail,
                        Sexo = "X",
                        Direccion = "API",
                        TipoDeUsuario = 3,
                        Contrasena = "API",
                        socialMediaID = "API",
                        PreguntaSeguridad = "API",
                        RespuestaPreguntaSeguridad = "API"
                    };

                    db.Users.Add(userTO);
                    db.SaveChanges();
                }

                // Verifica si el usuario fue creado correctamente.
                using (TRAVEL2Entities db = new TRAVEL2Entities())
                {
                    var user = db.Database.SqlQuery<UsersViewModel>("EXEC VerifyUserGmail @var_Correo",
                        new SqlParameter("var_Correo", userEmail)
                    ).SingleOrDefault();

                    SqlParameter outputParam = new SqlParameter
                    {
                        ParameterName = "@Autenticado",
                        SqlDbType = SqlDbType.Bit,
                        Direction = ParameterDirection.Output
                    };

                    if (user != null)
                    {
                        Session["UsuarioActual"] = user;
                        return Content("0");
                    }
                    else
                    {
                        return Content("1");
                    }
                }
            }
            catch (Exception ex)
            {
                return Content("Error al procesar la solicitud: " + ex.Message);
            }
        }

        // Acción POST para crear un usuario mediante la API usando nombre, primer apellido y ID de redes sociales.
        [HttpPost]
        public ActionResult CreateAPI2(string nombre, string primerapellido, string socialMediaID)
        {
            try
            {
                // Crea un nuevo usuario en la base de datos.
                using (var db = new TRAVEL2Entities())
                {
                    Users userTO = new Users
                    {
                        Cedula = 0,
                        Nombre = nombre,
                        PrimerApellido = primerapellido,
                        SegundoApellido = "API",
                        Edad = 0,
                        Telefono = 0,
                        Correo = "API",
                        Sexo = "X",
                        Direccion = "API",
                        TipoDeUsuario = 3,
                        Contrasena = "API",
                        socialMediaID = socialMediaID,
                        PreguntaSeguridad = "API",
                        RespuestaPreguntaSeguridad = "API"
                    };

                    db.Users.Add(userTO);
                    db.SaveChanges();
                }

                // Verifica si el usuario fue creado correctamente.
                using (TRAVEL2Entities db = new TRAVEL2Entities())
                {
                    var user = db.Database.SqlQuery<UsersViewModel>("EXEC VerifyUserSocialMedia @var_socialMediaID",
                        new SqlParameter("var_socialMediaID", socialMediaID)
                    ).SingleOrDefault();

                    SqlParameter outputParam = new SqlParameter
                    {
                        ParameterName = "@Autenticado",
                        SqlDbType = SqlDbType.Bit,
                        Direction = ParameterDirection.Output
                    };

                    if (user != null)
                    {
                        Session["UsuarioActual"] = user;
                        return Content("0");
                    }
                    else
                    {
                        return Content("1");
                    }
                }
            }
            catch (Exception ex)
            {
                return Content("Error al procesar la solicitud: " + ex.Message);
            }
        }

        // Acción POST para devolver la pregunta de seguridad de un usuario por email.
        [HttpPost]
        public ActionResult devuelvePreguntaSeguridad(string email)
        {
            using (TRAVEL2Entities db = new TRAVEL2Entities())
            {
                // Ejecuta un procedimiento almacenado para obtener la pregunta de seguridad.
                var user = db.Database.SqlQuery<UsersViewModel>("EXEC VerifyUserGmail @var_Correo",
                    new SqlParameter("var_Correo", email)
                ).SingleOrDefault();

                // Parámetro de salida para la autenticación.
                SqlParameter outputParam = new SqlParameter
                {
                    ParameterName = "@Autenticado",
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.Output
                };

                // Si el usuario existe, retorna la pregunta de seguridad, de lo contrario, retorna "0".
                if (user != null)
                {
                    return Content(user.PreguntaSeguridad.ToString());
                }
                else
                {
                    return Content("0");
                }
            }
        }

        // Acción POST para devolver la respuesta de la pregunta de seguridad de un usuario por email.
        [HttpPost]
        public ActionResult devuelveRespuestaPreguntaSeguridad(string email)
        {
            using (TRAVEL2Entities db = new TRAVEL2Entities())
            {
                // Ejecuta un procedimiento almacenado para obtener la respuesta de seguridad.
                var user = db.Database.SqlQuery<UsersViewModel>("EXEC VerifyUserGmail @var_Correo",
                    new SqlParameter("var_Correo", email)
                ).SingleOrDefault();

                // Parámetro de salida para la autenticación.
                SqlParameter outputParam = new SqlParameter
                {
                    ParameterName = "@Autenticado",
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.Output
                };

                // Si el usuario existe, retorna la respuesta de seguridad, de lo contrario, retorna "0".
                if (user != null)
                {
                    return Content(user.RespuestaPreguntaSeguridad.ToString());
                }
                else
                {
                    return Content("0");
                }
            }
        }

        // Acción GET para mostrar la vista de recuperación de contraseña usando el email.
        public ActionResult recuperaContrasena(string email)
        {
            UserRecuperaContrasenaViewModel model = new UserRecuperaContrasenaViewModel();

            using (TRAVEL2Entities db = new TRAVEL2Entities())
            {
                // Ejecuta un procedimiento almacenado para verificar el usuario por email.
                var user = db.Database.SqlQuery<UsersViewModel>("EXEC VerifyUserGmail @var_Correo",
                    new SqlParameter("var_Correo", email)
                ).SingleOrDefault();

                SqlParameter outputParam = new SqlParameter
                {
                    ParameterName = "@Autenticado",
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.Output
                };

                // Si el usuario existe, mapea los datos al modelo.
                if (user != null)
                {
                    model.Id = user.Id;
                    model.Correo = user.Correo;
                    model.Contrasena = user.Contrasena;
                    System.Diagnostics.Debug.WriteLine(model.Correo);
                    System.Diagnostics.Debug.WriteLine(user.Correo);
                }
                else
                {
                    // Si el usuario no se encuentra, muestra un error en el modelo.
                    ModelState.AddModelError(string.Empty, "Usuario no encontrado");
                    return View(model);
                }
            }
            return View(model);
        }

        // Acción POST para actualizar la contraseña del usuario en la base de datos.
        [HttpPost]
        public ActionResult recuperaContrasena(UserRecuperaContrasenaViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Usa la base de datos para actualizar la contraseña del usuario.
            using (TRAVEL2Entities db = new TRAVEL2Entities())
            {
                var ouser = db.Users.Find(model.Id);
                ouser.Contrasena = model.Contrasena;

                if (model.Contrasena != null && model.Contrasena.Trim() != "")
                {
                    ouser.Contrasena = model.Contrasena;
                }

                db.Entry(ouser).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            // Redirige al usuario a la página de login después de actualizar la contraseña.
            return Redirect(Url.Content("/Login/Login"));
        }

        // Acción para cerrar la sesión del usuario.
        public ActionResult Logout()
        {
            Session["UsuarioActual"] = null;
            return RedirectToAction("Index", "Login");
        }
    }
}
