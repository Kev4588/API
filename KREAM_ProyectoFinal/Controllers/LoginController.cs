using KREAM_ProyectoFinal.Models;
using KREAM_ProyectoFinal.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace KREAM_ProyectoFinal.Controllers
{
    [VerificarSesion]
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return RedirectToAction("Login");
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }



        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            if (username.Length != 9)
            {
                ViewBag.ErrorMessage = "La cédula debe contener exactamente 9 dígitos.";
                return View("Login");
            }

            using (TRAVEL2Entities db = new TRAVEL2Entities())
            {
                var user = db.Database.SqlQuery<UsersViewModel>("EXEC VerifyUser @var_cedula, @var_contrasena",
                    new SqlParameter("var_cedula", username),
                    new SqlParameter("var_contrasena", password)
                ).SingleOrDefault();


                SqlParameter outputParam = new SqlParameter
                {
                    ParameterName = "@Autenticado",
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.Output
                };
                var user2 = db.Database.SqlQuery<UsersViewModel>("EXEC VerifyPassword @Cedula, @Contrasena, @Autenticado OUTPUT",
                    new SqlParameter("Cedula", username),
                    new SqlParameter("Contrasena", password),
                    outputParam
                ).SingleOrDefault();

                bool resultadoAutenticado = (bool)outputParam.Value;

                if (user != null)
                {
                    if (user.TipoDeUsuario == 1) //admin
                    {
                        Session["UsuarioActual"] = user;
                        return RedirectToAction("Index", "Home");
                    }
                    else if (user.TipoDeUsuario == 2) //user / Vendedor   
                    {
                        Session["UsuarioActual"] = user;
                        return RedirectToAction("Index", "Home");
                    }
                    else if (user.TipoDeUsuario == 3) //user / Comprador
                    {
                        Session["UsuarioActual"] = user;
                        return RedirectToAction("Index", "Home");
                    }
                }
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





        [HttpPost]
        public ActionResult usuarioExiste(string email)
        {


            using (TRAVEL2Entities db = new TRAVEL2Entities())
            {
                var user = db.Database.SqlQuery<UsersViewModel>("EXEC VerifyUserGmail @var_Correo",
                    new SqlParameter("var_Correo", email)
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
                    return Content("1");
                }
                else
                {
                    return Content("0");
                }
            }
        }


        [HttpPost]
        public ActionResult usuarioExiste2(string email)
        {


            using (TRAVEL2Entities db = new TRAVEL2Entities())
            {
                var user = db.Database.SqlQuery<UsersViewModel>("EXEC VerifyUserGmail @var_Correo",
                    new SqlParameter("var_Correo", email)
                ).SingleOrDefault();


                SqlParameter outputParam = new SqlParameter
                {
                    ParameterName = "@Autenticado",
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.Output
                };


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

        [HttpPost]
        public ActionResult usuarioExisteSocialMedia(string socialMediaID)
        {


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
                    return Content("1");
                }
                else
                {
                    return Content("0");
                }
            }

        }


        [HttpPost]
        public ActionResult CreateAPI(string nombre, string primerapellido, string userEmail)
        {
            try
            {
                using (var db = new TRAVEL2Entities())
                {
                    Users userTO = new Users();
                    userTO.Cedula = 0;
                    userTO.Nombre = nombre;
                    userTO.PrimerApellido = primerapellido;
                    userTO.SegundoApellido = "API";
                    userTO.Edad = 0;
                    userTO.Telefono = 0;
                    userTO.Correo = userEmail;

                    userTO.Sexo = "X";
                    userTO.Direccion = "API";
                    userTO.TipoDeUsuario = 3;
                    userTO.Contrasena = "API";
                    userTO.socialMediaID = "API";
                    userTO.PreguntaSeguridad = "API";
                    userTO.RespuestaPreguntaSeguridad = "API";

                    db.Users.Add(userTO);
                    db.SaveChanges();
                }

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

        [HttpPost]
        public ActionResult CreateAPI2(string nombre, string primerapellido, string socialMediaID)
        {
            try
            {
                using (var db = new TRAVEL2Entities())
                {
                    Users userTO = new Users();
                    userTO.Cedula = 0;
                    userTO.Nombre = nombre;
                    userTO.PrimerApellido = primerapellido;
                    userTO.SegundoApellido = "API";
                    userTO.Edad = 0;
                    userTO.Telefono = 0;
                    userTO.Correo = "API";

                    userTO.Sexo = "X";
                    userTO.Direccion = "API";
                    userTO.TipoDeUsuario = 3;
                    userTO.Contrasena = "API";
                    userTO.socialMediaID = socialMediaID;
                    userTO.PreguntaSeguridad = "API";
                    userTO.RespuestaPreguntaSeguridad = "API";

                    db.Users.Add(userTO);
                    db.SaveChanges();
                }

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

        [HttpPost]
        public ActionResult devuelvePreguntaSeguridad(string email)
        {
            using (TRAVEL2Entities db = new TRAVEL2Entities())
            {
                var user = db.Database.SqlQuery<UsersViewModel>("EXEC VerifyUserGmail @var_Correo",
                    new SqlParameter("var_Correo", email)
                ).SingleOrDefault();


                SqlParameter outputParam = new SqlParameter
                {
                    ParameterName = "@Autenticado",
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.Output
                };


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

        [HttpPost]
        public ActionResult devuelveRespuestaPreguntaSeguridad(string email)
        {
            using (TRAVEL2Entities db = new TRAVEL2Entities())
            {
                var user = db.Database.SqlQuery<UsersViewModel>("EXEC VerifyUserGmail @var_Correo",
                    new SqlParameter("var_Correo", email)
                ).SingleOrDefault();


                SqlParameter outputParam = new SqlParameter
                {
                    ParameterName = "@Autenticado",
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.Output
                };


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

        public ActionResult recuperaContrasena(string email)
        {
            UserRecuperaContrasenaViewModel model = new UserRecuperaContrasenaViewModel();


            using (TRAVEL2Entities db = new TRAVEL2Entities())
            {
                var user = db.Database.SqlQuery<UsersViewModel>("EXEC VerifyUserGmail @var_Correo",
                    new SqlParameter("var_Correo", email)
                ).SingleOrDefault();


                SqlParameter outputParam = new SqlParameter
                {
                    ParameterName = "@Autenticado",
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.Output
                };


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
                    // Manejar el caso en que el usuario no sea encontrado
                    ModelState.AddModelError(string.Empty, "Usuario no encontrado");
                    return View(model);
                }

            }
            return View(model);

        }


        [HttpPost]
        public ActionResult recuperaContrasena(UserRecuperaContrasenaViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }

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

            return Redirect(Url.Content("/Login/Login"));

        }







        public ActionResult Logout()
        {
            Session["UsuarioActual"] = null;
            return RedirectToAction("Index", "Login");
        }





    }
}
